namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Supplier")]
    public partial class Supplier
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Supplier()
        //{
        //    BuyerOrderDet = new HashSet<BuyerOrderDet>();
        //    BuyerOrderMas = new HashSet<BuyerOrderMas>();
        //    FactoryOrderMas = new HashSet<FactoryOrderMas>();
        //}

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Factory")]
        public string Name { get; set; }

        [StringLength(10)]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Bank Branch")]
        public int? BankBranchId { get; set; }

        [StringLength(50)]
        [Display(Name = "Account No")]
        public string AccNo { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BankBranch BankBranch { get; set; }

        [Display(Name = "Opening Balance")]
        public decimal? OpeningBalance { get; set; }

        [Display(Name = "Balance Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? BalanceDate { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderDet> BuyerOrderDet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderMas> BuyerOrderMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactoryOrderMas> FactoryOrderMas { get; set; }
    }
}
