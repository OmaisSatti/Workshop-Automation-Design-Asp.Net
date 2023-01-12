using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehical_Workshop_Automation.Models
{
    public class App_Service
    {
        public int Service_Id { get; set; }
        public int App_No { get; set; }
        public string Status { get; set; }
        public string Customer_Suggestion { get; set; }
        public string Surveyor_Suggestion { get; set; }
    }
}