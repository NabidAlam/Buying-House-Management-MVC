using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentBuyerAdj")]
    public class DiscountAdjustmentBuyerAdj
    {
        public int Id { get; set; }
        public int DiscountAdjustmentBuyerMasId { get; set; }
        public int BuyerOrderMasId { get; set; }
        public virtual DiscountAdjustmentBuyerMas DiscountAdjustmentBuyerMas { get; set; }
        public virtual BuyerOrderMas BuyerOrderMas { get; set; }
    }
}