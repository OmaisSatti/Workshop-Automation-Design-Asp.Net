using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehical_Workshop_Automation.Models
{
    public class Appointment
    {
        public int Appointment_No { get; set; }
        public int Bay_No { get; set; }
        public string Vehicle_Reg_No { get; set; }
        public string Customer_Mobile_No { get; set; }
        public string Date { get; set; } 
        public string Slot { get; set; }
        public string Miles { get; set; }
        public string CurrentMiles { get; set; }
        public string Status { get; set; }
        public string SCheck { get; set; }
        public string MCheck { get; set; }
        public string Total_Bill { get; set; }
    }
}