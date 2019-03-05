using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("FDDPaymentDet")]
    public class FDDPaymentDet
    {
        public int Id { get; set; }
        public int FDDPaymentId { get; set; }
        public virtual FDDPayment FDDPayment { get; set; }

        public int DocSubmissionFactDetId { get; set; }
        public virtual DocSubmissionFactDet DocSubmissionFactDet { get; set; }
    }
}