using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDiscountAdjustFactoryDet
    {
        ////Prev
        //public int BuyerMasId { get; set; }
        //public int DiscountAdjustmentFactoryMasId { get; set; }
        //public int DelivOrderDetTempId { get; set; }
        //public int PrevId { get; set; }

        ////Adj
        //public int BuyerMasIdAdj { get; set; }
        //public int DelivOrderDetTempIdAdj { get; set; }
        //public int TempOrderIdAdj { get; set; }


        //DetailDetailAdjustment
        public int Id { get; set; }
        public int DiscountFactAdjId { get; set; }
        public decimal Adjustment { get; set; }
        public int FactOrderDelivDetIdAdj { get; set; }
        public int DelivOrderDetTempIdAdj { get; set; }
    }

    public class VMDiscountAdjustFactoryPrev
    {
        //Prev
        public int Id { get; set; }
        public int BuyerMasId { get; set; }
        public int DiscountAdjustmentFactoryMasId { get; set; }
        public int DelivOrderDetTempId { get; set; }

    }

    public class VMDiscountAdjustFactoryAdj
    {
        //Adj
        public int Id { get; set; }
        public int DiscountAdjustmentFactoryMasId { get; set; }
        public int BuyerMasIdAdj { get; set; }
        public int TempOrderIdAdj { get; set; }

    }
}