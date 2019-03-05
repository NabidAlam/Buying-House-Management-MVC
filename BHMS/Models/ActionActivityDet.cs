using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("ActionActivityDet")]
    public class ActionActivityDet
    {
        public int Id { get; set; }
        public int ActionActivityMasId { get; set; }
        public int TimeActionDetId { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime? RevisedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; }


        public virtual ActionActivityMas ActionActivityMas { get; set; }
        public virtual TimeActionDet TimeActionDet { get; set; }

    }
}