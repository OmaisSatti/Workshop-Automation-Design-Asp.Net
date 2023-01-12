using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehical_Workshop_Automation.Models
{
    public class Services
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Estimate { get; set; }
        public string Type { get; set; }
        public bool Selected { get; set; }
        public string Duration { get; set; }
    }
}