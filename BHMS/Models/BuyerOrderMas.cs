namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BuyerOrderMas
    {

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public BuyerOrderMas()
        //{
        //    BuyerOrderDet = new HashSet<BuyerOrderDet>();
        //    CommissionDistMas = new HashSet<CommissionDistMas>();
        //    FactoryOrderMas = new HashSet<FactoryOrderMas>();
        //}

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Order Ref No")]
        public string OrderRefNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        [Display(Name = "Department")]
        public int? ProdDepartmentId { get; set; }

        [Display(Name = "Season")]
        public int? SeasonInfoId { get; set; }

        [Display(Name = "Fabric Supplier")]
        public int? FabSupplierId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public bool IsLocked { get; set; }


        //7 Aug 18
        public int? FobType { get; set; }
        public int? DeliveryOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }
        public virtual Brand Brand { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderDet> BuyerOrderDet { get; set; }

        public virtual ICollection<CommissionDistMas> CommissionDistMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactoryOrderMas> FactoryOrderMas { get; set; }

        public virtual ProdDepartment ProdDepartment { get; set; }

        public virtual SeasonInfo SeasonInfo { get; set; }

        [ForeignKey("FabSupplierId")]
        public virtual Supplier Supplier { get; set; }
    }
}
