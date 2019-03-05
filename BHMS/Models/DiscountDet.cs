using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountDet")]
    public class DiscountDet
    {
        public int Id { get; set; }
        public int DiscountMasId { get; set; }
        public int FactoryOrderDelivDetId { get; set; }
        public decimal BuyerDiscount { get; set; }
        public bool AdjustBuyerNow { get; set; }
        public decimal FactoryDiscount { get; set; }
        public bool AdjustFactoryNow { get; set; }
        public virtual DiscountMas DiscountMas { get; set; }
        public virtual FactoryOrderDelivDet FactoryOrderDelivDet { get; set; }

    }
}