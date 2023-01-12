using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vehical_Workshop_Automation.Models
{
    public class DateWiseSlots
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string Slot1 { get; set; }
        public string Slot2 { get; set; }
        public string Slot3 { get; set; }
        public string Slot4 { get; set; }
        public bool Checked1 { get; set; }
        public bool Checked2 { get; set; }
        public bool Checked3 { get; set; }

        public bool Checked4 { get; set; }
        public string SelectedSlot { get; set; }
    }
}