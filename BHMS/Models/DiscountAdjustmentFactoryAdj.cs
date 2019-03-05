using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentFactoryAdj")]
    public class DiscountAdjustmentFactoryAdj
    {
        public int Id { get; set; }
        public int DiscountAdjustmentFactoryMasId { get; set; }
       
        public virtual DiscountAdjustmentFactoryMas DiscountAdjustmentFactoryMas { get; set; }
      
        public int BuyerOrderMasId { get; set; }
        public virtual BuyerOrderMas BuyerOrderMas { get; set; }

    }
}