namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceCommMas
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public InvoiceCommMas()
        //{
        //    DocSubmissionDet = new HashSet<DocSubmissionDet>();
        //    DocSubmissionMas = new HashSet<DocSubmissionMas>();
        //    InvoiceCommDet = new HashSet<InvoiceCommDet>();
        //}

        public int Id { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Buyer Name")]
        public int BuyerInfoId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name ="RDL Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Payment Type")]
        public int PaymentTypeId { get; set; }

        [Display(Name = "Master LC No.")]
        public int? MasterLCInfoMasId { get; set; }

        [StringLength(50)]
        public string TTNo { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocSubmissionDet> DocSubmissionDet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<DocSubmissionMas> DocSubmissionMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommDet> InvoiceCommDet { get; set; }

        public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }

        //16/10/2018(tazbirul)
        [Display(Name ="Invoice Type")]
        public int? SplitFlag { get; set; }
    }
}
