namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DocSubmissionFactDet")]
    public partial class DocSubmissionFactDet
    {
        public int Id { get; set; }

        public int DocSubmissionDetId { get; set; }

        public int? InvoiceCommDetId { get; set; }
        public int? InvoiceCommFactDetId { get; set; }
        
        [StringLength(50)]
        public string FactFDBCNo { get; set; }

        public virtual DocSubmissionDet DocSubmissionDet { get; set; }

        public virtual InvoiceCommDet InvoiceCommDet { get; set; }
        public virtual InvoiceCommFactDet InvoiceCommFactDet { get; set; }
    }
}
