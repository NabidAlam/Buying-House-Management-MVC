namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExFactoryShipDet")]
    public partial class ExFactoryShipDet
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ExFactoryShipDet()
        //{
        //    InvoiceCommFactDet = new HashSet<InvoiceCommFactDet>();
        //}

        public int Id { get; set; }

        public int ExFactoryDetId { get; set; }

        public int BuyerOrderDetId { get; set; }

        public int ShipmentSummDetId { get; set; }

        public int ShipQuantity { get; set; }

        [StringLength(50)]
        public string Remarks { get; set; }

        public bool IsShipClosed { get; set; }

        public int? ShipmentMode { get; set; }
        public virtual BuyerOrderDet BuyerOrderDet { get; set; }

        public virtual ExFactoryDet ExFactoryDet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<InvoiceCommFactDet> InvoiceCommFactDet { get; set; }

        public virtual ShipmentSummDet ShipmentSummDet { get; set; }
    }
}
