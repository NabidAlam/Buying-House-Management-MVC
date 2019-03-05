using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public class ProceedRealizationMas
    {
        public int Id { get; set; }

        [Display(Name = "Proceed Type")]
        public int PaymentTypeId { get; set; }

        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Proceed Date")]
        public DateTime ProceedDate { get; set; }

        //[Display(Name = "Master LC")]
        //public int? MasterLCInfoMasId { get; set; }

        [Display(Name = "FDBC/TT No.")]
        public int DocSubmissionMasId { get; set; }

        //[StringLength(50)]
        //public string MasterLCInfoMas { get; set; }  

        public virtual BuyerInfo BuyerInfo { get; set; }
        public virtual DocSubmissionMas DocSubmissionMas { get; set; }

        //public virtual ICollection<DocSubmissionDet> DocSubmissionDet { get; set; }     
        //public virtual ICollection<InvoiceCommDet> InvoiceCommDet { get; set; }
        //public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }
    }
}