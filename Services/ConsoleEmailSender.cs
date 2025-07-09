using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;

// Simple console email sender for development purposes for testing email authentication

namespace HerCalendar.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulate sending an email by writing to the console
            Console.WriteLine($"Sending email to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {htmlMessage}");
            return Task.CompletedTask; // Simulate async operation
        }
    }
}
