using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentBuyerDet")]

    public class DiscountAdjustmentBuyerDet
    {

        public int Id { get; set; }
        public decimal? AdjustmentAmt { get; set; }
        public int DiscountAdjustmentBuyerAdjId { get; set; }
        public int ExFactoryShipDetId { get; set; }

        public virtual DiscountAdjustmentBuyerAdj DiscountAdjustmentBuyerAdj { get; set; }
        public virtual ExFactoryShipDet ExFactoryShipDet { get; set; }
    }
}