namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceCommDet")]
    public partial class InvoiceCommDet
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public InvoiceCommDet()
        //{
        //    DocSubmissionFactDet = new HashSet<DocSubmissionFactDet>();
        //}

        public int Id { get; set; }

        public int InvoiceCommMasId { get; set; }

        public int InvoiceCommFactMasId { get; set; }
        public virtual InvoiceCommFactMas InvoiceCommFactMas { get; set; }

        public decimal? InvoiceRDLTotalAmt { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocSubmissionFactDet> DocSubmissionFactDet { get; set; }

        public virtual InvoiceCommMas InvoiceCommMas { get; set; }

        //public int BuyerOrderMasId { get; set; }
        //public virtual BuyerOrderMas BuyerOrderMas { get; set; }



    }
}
