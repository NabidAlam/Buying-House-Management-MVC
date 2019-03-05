using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("InvoiceCommDetDet")]
    public class InvoiceCommDetDet
    {
        public int Id { get; set; }

        public int InvoiceCommDetId { get; set; }

        public int ExFactoryShipDetId { get; set; }

        public int? ShipQty { get; set; }

        public bool? DiscountFlag { get; set; }

        public virtual InvoiceCommDet InvoiceCommDet { get; set; }

        public virtual ExFactoryShipDet ExFactoryShipDet { get; set; }
    }
}