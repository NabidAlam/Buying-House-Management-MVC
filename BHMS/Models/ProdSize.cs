namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProdSize")]
    public partial class ProdSize
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ProdSize()
        //{
        //    BuyerOrderDet = new HashSet<BuyerOrderDet>();
        //}

        public int Id { get; set; }

        [Display(Name = "Department")]
        public int ProdDepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Size Range")]
        public string SizeRange { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderDet> BuyerOrderDet { get; set; }


        public virtual ProdDepartment ProdDepartment { get; set; }
    }
}
