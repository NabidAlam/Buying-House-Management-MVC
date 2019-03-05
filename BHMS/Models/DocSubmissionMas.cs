namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DocSubmissionMas
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public DocSubmissionMas()
        //{
        //    DocSubmissionDet = new HashSet<DocSubmissionDet>();
        //}

        public int Id { get; set; }

        [Display(Name = "Proceed Type")]
        public int PaymentTypeId { get; set; }

        [Display(Name = "Buyer Name")]
        public int BuyerInfoId { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Sub Date")]
        public DateTime SubmissionDate { get; set; }

        [StringLength(50)]
        [Display(Name = "FDBC No.")]
        public string FDBCNo { get; set; }

        [Display(Name = "FDBC/TT Value($)")]
        public decimal? FDBCValue { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "FDBC Date")]
        public DateTime? FDBCDate { get; set; }

        [Display(Name = "LC No.")]
        //public int InvoiceCommMasId { get; set; }
        public int? MasterLCInfoMasId { get; set; }

        [StringLength(50)]
        [Display(Name = "AWB No.")]
        public string AWBNo { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "AWB Date")]
        public DateTime? AWBDate { get; set; }

        [Display(Name = "Courier")]
        public int? CourierInfoId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }

        public virtual CourierInfo CourierInfo { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocSubmissionDet> DocSubmissionDet { get; set; }

        //public virtual InvoiceCommMas InvoiceCommMas { get; set; }
        public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }
    }
}
