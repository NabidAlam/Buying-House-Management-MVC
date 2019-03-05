namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShipmentSummDet")]
    public partial class ShipmentSummDet
    {
        public int Id { get; set; }

        public int BuyerOrderDetId { get; set; }

        public int DelivSlno { get; set; }

        public int DelivQuantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExFactoryDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? HandoverDate { get; set; }

        public int? DestinationPortId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ETD { get; set; }

        [StringLength(50)]
        public string BuyersPONo { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        public bool IsLocked { get; set; }

        public virtual BuyerOrderDet BuyerOrderDet { get; set; }

        public virtual DestinationPort DestinationPort { get; set; }

        //7 Aug 18
        public decimal? RdlFOB { get; set; }
        public int? ShipmentMode { get; set; }
        public string BuyerSlNo { get; set; }
    }
}
