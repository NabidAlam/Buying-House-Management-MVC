using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMShipmentSummDet
    {
        public int DelivOrderDetTempId { get; set; }

        public int Id { get; set; }

        public int BuyerOrderDetId { get; set; }

        public int DelivSlno { get; set; }

        public int DelivQuantity { get; set; }

        public decimal? RdlFobDetailDet { get; set; }
        public string BuyerSlNo { get; set; }



        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExFactoryDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? HandoverDate { get; set; }

        public int? DestinationPortId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ETD { get; set; }

        [StringLength(50)]
        public string BuyersPONo { get; set; }

        public int? ShipmentMode { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        public bool IsLocked { get; set; }

    }
}