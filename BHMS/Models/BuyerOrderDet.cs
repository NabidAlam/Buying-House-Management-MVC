namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BuyerOrderDet")]
    public partial class BuyerOrderDet
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public BuyerOrderDet()
        //{
        //    FactoryOrderDet = new HashSet<FactoryOrderDet>();
        //    ShipmentSummDet = new HashSet<ShipmentSummDet>();
        //}

        public int Id { get; set; }

        public int BuyerOrderMasId { get; set; }

        public int? ProdCatTypeId { get; set; }

        //28 April, 2018 Added by Nabid
        public bool? IsShipClosed { get; set; }


        [StringLength(50)]
        [Display(Name = "Style No.")]
        public string StyleNo { get; set; }

        public int? ProdSizeId { get; set; }

        public int? FabricItemId { get; set; }

        public int? ProdColorId { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public int SupplierId { get; set; }

        [StringLength(256)]
        public string PictureFilename { get; set; }

        public bool IsLocked { get; set; }

        //7 Aug 18
        public int? FabricSupplierId { get; set; }
        public virtual FabricSupplier FabricSupplier { get; set; }


        public decimal? RdlTotal { get; set; }


        //[Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ex-Factory Date")]
        public DateTime? ExFactoryDate { get; set; }

        public virtual BuyerOrderMas BuyerOrderMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactoryOrderDet> FactoryOrderDet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShipmentSummDet> ShipmentSummDet { get; set; }

        public virtual ProdCatType ProdCatType { get; set; }

        public virtual ProdSize ProdSize { get; set; }


        public virtual FabricItem FabricItem { get; set; }


        public int? FabricItemDetId { get; set; }
        public virtual FabricItemDet FabricItemDet { get; set; }

        public virtual ProdColor ProdColor { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
