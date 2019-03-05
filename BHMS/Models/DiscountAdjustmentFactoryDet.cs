using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentFactoryDet")]

    public class DiscountAdjustmentFactoryDet
    {
        public int Id { get; set; }
        public decimal? AdjustmentAmt { get; set; }
        public int DiscountAdjustmentFactoryAdjId { get; set; }
        public int FactoryOrderDelivDetId { get; set; }

        public virtual DiscountAdjustmentFactoryAdj DiscountAdjustmentFactoryAdj { get; set; }
        public virtual FactoryOrderDelivDet FactoryOrderDelivDet { get; set; }

    }
}