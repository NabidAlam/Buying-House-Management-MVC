namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BankBranch")]
    public partial class BankBranch
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public BankBranch()
        //{
        //    Supplier = new HashSet<Supplier>();
        //    BuyerInfo = new HashSet<BuyerInfo>();
        //}

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Bank Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [StringLength(50)]
        [Display(Name = "Swift Code")]
        public string SwiftCode { get; set; }

        [Display(Name = "Foreign Bank")]
        public bool IsForeign { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Supplier> Supplier { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerInfo> BuyerInfo { get; set; }
    }
}
