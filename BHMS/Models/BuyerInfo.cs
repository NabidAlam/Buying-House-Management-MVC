namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BuyerInfo")]
    public partial class BuyerInfo
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public BuyerInfo()
        //{
        //    BuyerOrderMas = new HashSet<BuyerOrderMas>();
        //    CommissionDistTempMas = new HashSet<CommissionDistTempMas>();
        //    ProdDepartment = new HashSet<ProdDepartment>();
        //}

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Buyer")]
        public string Name { get; set; }

        [StringLength(20)]
        [Display(Name = "Short Name")]
        public string NameShort { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        [StringLength(50)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [StringLength(50)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        public int? CountryInfoId { get; set; }

        public int? MerchandiserId { get; set; }

        public int? MiddlePartyId { get; set; }

        public int? BankBranchId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? Tenor { get; set; }

        [Display(Name = "Payment Term")]
        public int? PaymentTerm { get; set; }

        [StringLength(20)]
        [Display(Name = "Buyer Group")]
        public string BuyerGroup { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        [Display(Name = "Opening Balance")]
        public decimal? OpeningBalance { get; set; }

        //[DataType(DataType.Date)]
        [Display(Name = "Balance Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? BalanceDate { get; set; }

        public virtual BankBranch BankBranch { get; set; }

        public virtual CountryInfo CountryInfo { get; set; }


        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderMas> BuyerOrderMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommissionDistTempMas> CommissionDistTempMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ProdDepartment> ProdDepartment { get; set; }

        [ForeignKey("MerchandiserId")]
        public virtual CompanyResource CompanyResource { get; set; }

        public virtual MiddleParty MiddleParty { get; set; }
    }
}
