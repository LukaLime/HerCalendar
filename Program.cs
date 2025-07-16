using HerCalendar.Data;
using HerCalendar.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace HerCalendar
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // User Secrets
            builder.Configuration.AddUserSecrets<Program>();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure( //  EF Core retry failed DB calls (e.g., 5 times with 10s max delay)
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity services configuration
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false; // Require email confirmation for account activation
                    options.User.RequireUniqueEmail = true; // Ensure unique email addresses for users
                })
                .AddRoles<IdentityRole>() // Roles support (eg. Admin, User)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Custom email sender for development (console output test)
            //builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();

            // Login path configuration
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Optional: where to redirect if user lacks access
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
               .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                try
                { 
                    // Admin Login Setup
                    string adminEmail = builder.Configuration["AdminUser:Email"]!;
                    string adminPassword = builder.Configuration["AdminUser:Password"]!;

                    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                    {
                        throw new Exception("⚠️ Admin email or password not configured.");
                    }

                    // Create Admin role if it doesn't exist
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        Console.WriteLine("Creating 'Admin' role...");
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    var adminUser = await userManager.FindByEmailAsync(adminEmail);

                    /*
                     * Using this commented code, you can delete the admin user if it already exists (if admin login is lost, need to delete and recreate).
                    if (adminUser != null)
                    {
                        Console.WriteLine("Admin user already exists. Deleting to recreate...");
                        await userManager.DeleteAsync(adminUser);
                    }
                    */

                    if (adminUser == null)
                    {
                        Console.WriteLine("Admin user not found. Creating...");
                        adminUser = new IdentityUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(adminUser, adminPassword);
                        if (result.Succeeded)
                        {
                            Console.WriteLine("✅ Admin user created.");
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                            Console.WriteLine("✅ Admin role assigned.");
                        }
                        else
                        {
                            Console.WriteLine("❌ Failed to create admin user:");
                            foreach (var error in result.Errors)
                            {
                                Console.WriteLine($" - {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Admin user already exists.");
                        // Ensure user has Admin role
                        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                            Console.WriteLine("✅ Admin role assigned to existing user.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error during admin user setup:");
                    Console.WriteLine($"   {ex.Message}");
                }
            }
            // EMAIL SENDER TESTING (uncomment to test email sending)
            //using var test = app.Services.CreateScope();
            //var emailSender = test.ServiceProvider.GetRequiredService<IEmailSender>();
            //await emailSender.SendEmailAsync("test@example.com", "Test Subject", "Hello from test!");

            app.Run();
        }
    }
}

