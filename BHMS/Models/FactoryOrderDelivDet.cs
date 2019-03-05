using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("FactoryOrderDelivDet")]
    public class FactoryOrderDelivDet
    {
        public int Id { get; set; }

        public int FactoryOrderDetId { get; set; }
        public virtual FactoryOrderDet FactoryOrderDet { get; set; }

        public DateTime? ExFactoryDate { get; set; }

        public decimal? FactFOB { get; set; }

        public decimal? FactTransferPrice { get; set; }

        public decimal? DiscountFOB { get; set; }
        //public bool DiscountFlag { get; set; }

        public int? ShipmentSummDetId { get; set; }
        public virtual ShipmentSummDet ShipmentSummDet { get; set; }

        public string Remarks { get; set; }
    }
}