namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProdDepartment")]
    public partial class ProdDepartment
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ProdDepartment()
        //{
        //    BuyerOrderMas = new HashSet<BuyerOrderMas>();
        //    CommissionDistTempMas = new HashSet<CommissionDistTempMas>();
        //}

        public int Id { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Department")]
        public string Name { get; set; }

        public virtual Brand Brand { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderMas> BuyerOrderMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommissionDistTempMas> CommissionDistTempMas { get; set; }
    }
}
