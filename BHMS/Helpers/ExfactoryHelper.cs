using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.Helpers
{
    public class ExfactoryHelper
    {
        public int BuyerOrderMasId { get; set; }
        public int BuyOrderDetId { get; set; }
        public int BuyOrderDetDetId { get; set; }
        public string StyleNo { get; set; }
        public string PONo { get; set; }
        public string SizeNo { get; set; }
        public string Color { get; set; }
        public string DestPort { get; set; }
        public decimal OrderQty { get; set; }
        public bool? IsStyleIncluded { get; set; }
        public int DelivSlNo { get; set; }
        public int BuyerSlNo { get; set; }
        public int CountCheck { get; set; }
        public decimal PrevShipQty { get; set; }
        public string ShipmentModePrev { get; set; }

        public decimal TotalQty { get; set; }
    }
}