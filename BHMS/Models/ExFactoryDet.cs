namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExFactoryDet")]
    public partial class ExFactoryDet
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ExFactoryDet()
        //{
        //    ExFactoryShipDet = new HashSet<ExFactoryShipDet>();
        //}

        public int Id { get; set; }

        public int ExFactoryMasId { get; set; }

        public int BuyerOrderMasId { get; set; }

        public int ShipQuantity { get; set; }

        public virtual BuyerOrderMas BuyerOrderMas { get; set; }

        public virtual ExFactoryMas ExFactoryMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExFactoryShipDet> ExFactoryShipDet { get; set; }
    }
}
