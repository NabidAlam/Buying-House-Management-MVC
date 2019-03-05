namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceCommFactMas
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public InvoiceCommFactMas()
        //{
        //    InvoiceCommDet = new HashSet<InvoiceCommDet>();
        //    InvoiceCommFactDet = new HashSet<InvoiceCommFactDet>();
        //}

        public int Id { get; set; }

        [Display(Name = "Factory")]
        public int SupplierId { get; set; }

        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Factory Invoice No.")]
        public string InvoiceNoFact { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name ="Ex-Factory Date")]
        public DateTime IssueDate { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommDet> InvoiceCommDet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommFactDet> InvoiceCommFactDet { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
