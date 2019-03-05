using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDiscountAdjFact
    {
        public int FactOrderDelivDetId { get; set; }
        public int BuyerMasId { get; set; }
        public int BuyerDetId { get; set; }
        public string StyleNo { get; set; }
        public string PONo { get; set; }
        public int DelivNo { get; set; }
        public string DestPort { get; set; }
        public int? OrderQty { get; set; }
        public int ShipQty { get; set; }
        public decimal? FactFOB { get; set; }
        public decimal? FactValue { get; set; }
        public decimal? Adjustment { get; set; }
        public decimal? FactoryTransFOB { get; set; }
        public decimal? FactoryInvVal { get; set; }
        public int DiscountAdjId { get; set; }
        public int DiscountFactDetId { get; set; }
        public bool flag { get; set; }
    }
}