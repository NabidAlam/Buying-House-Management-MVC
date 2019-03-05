using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("ActionActivityMas")]
    public class ActionActivityMas
    {

        public int Id { get; set; }
        public int FactoryOrderDelivDetId { get; set; }
        public int TimeActionMasId { get; set; }
        public bool PlanFlag { get; set; }
        public bool RevisedFlag { get; set; }

        public virtual FactoryOrderDelivDet FactoryOrderDelivDet { get; set; }
        public virtual TimeActionMas TimeActionMas { get; set; }

    }
}