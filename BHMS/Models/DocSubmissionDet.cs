namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DocSubmissionDet")]
    public partial class DocSubmissionDet
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public DocSubmissionDet()
        //{
        //    DocSubmissionFactDet = new HashSet<DocSubmissionFactDet>();
        //}

        public int Id { get; set; }

        public int DocSubmissionMasId { get; set; }

        public int? InvoiceCommMasId { get; set; }

        public int? BuyerOrderMasId { get; set; }

        public virtual DocSubmissionMas DocSubmissionMas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocSubmissionFactDet> DocSubmissionFactDet { get; set; }

        public virtual InvoiceCommMas InvoiceCommMas { get; set; }
        public virtual BuyerOrderMas BuyerOrderMas { get; set; }
    }
}
