using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMFactoryOrderDelivDet
    {
        public int DelivOrderDetTempId { get; set; }

        public int Id { get; set; }

        public int FactoryOrderDetId { get; set; }

        public DateTime? ExFactoryDate { get; set; }

        public decimal? FactFOB { get; set; }
        public decimal? FactTransferPrice { get; set; }

        //public decimal? DiscountFOB { get; set; }

        public int? ShipmentSummDetId { get; set; }

        public string Remarks { get; set; }

    }
}