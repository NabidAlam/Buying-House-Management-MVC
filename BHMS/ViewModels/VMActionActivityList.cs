using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMActionActivityList
    {
        public int timeActionDetId { get; set; }
        public string ActivityName { get; set; }
        public int? ActivityDays { get; set; }
        public string PlanDate { get; set; }
        public string Source { get; set; }
    }
}