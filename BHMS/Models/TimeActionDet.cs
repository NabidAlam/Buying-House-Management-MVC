using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("TimeActionDet")]
    public class TimeActionDet
    {
        public int Id { get; set; }
        public int TimeActionMasId { get; set; }

        public string ActivityName { get; set; }
        public int? ActivityDays { get; set; }
        public int? Source { get; set; }

        public virtual TimeActionMas TimeActionMas { get; set; }
    }
}