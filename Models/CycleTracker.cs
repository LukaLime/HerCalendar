using System;
using System.ComponentModel.DataAnnotations;

namespace HerCalendar.Models
{
    public class CycleTracker
    {
        public int Id { get; set; }

        [Display(Name = "Cycle Length (Days)")]
        public int CycleLength { get; set; } // Length of the cycle in days

        [Display(Name = "Last Period Start Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime LastPeriodStartDate { get; set; } // Start date of the last period

        [Display(Name = "Next Period Start Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime NextPeriodStartDate { get; set; } // Start date of the next period

    }
}
