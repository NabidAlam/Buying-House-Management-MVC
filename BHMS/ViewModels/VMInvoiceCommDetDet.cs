using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMInvoiceCommDetDet
    {
        public int DelivOrderDetTempId { get; set; }

        public int Id { get; set; }

        public int InvoiceCommDetId { get; set; }

        public int ExFactoryShipDetId { get; set; }

        public int? ShipQty { get; set; }

        public bool? DiscountFlag { get; set; }

        public decimal CurrentValue { get; set; }
    }
}