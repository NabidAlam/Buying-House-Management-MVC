using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDiscountAdjustmentBuyerAdj
    {
        public int Id { get; set; }

        public int DiscountAdjustmentBuyerMasId { get; set; }

        public int BuyerOrderMasId { get; set; }

        public int TempOrderIdAdj { get; set; }
    }
}