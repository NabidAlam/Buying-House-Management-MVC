using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDiscountAdjustmentBuyerDet
    {
       
        public int Id { get; set; }

        public decimal AdjustmentAmt { get; set; }

        public int DiscountAdjustmentBuyerAdjId { get; set; }
        public int DelivOrderDetTempIdAdj { get; set; }
        public int ExFactoryShipDetId { get; set; }
    }
}