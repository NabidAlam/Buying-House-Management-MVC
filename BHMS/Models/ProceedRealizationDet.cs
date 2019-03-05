using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("ProceedRealizationDet")]
    public class ProceedRealizationDet
    {
        public int Id { get; set; }
        //[Display(Name = "Buyer")]
        public int ProceedRealizationMasId { get; set; }
        public int DocSubmissionDetId { get; set; }

        [Required]
        public decimal ProceedQty { get; set; }
        public virtual ProceedRealizationMas ProceedRealizationMas { get; set; }
        public virtual DocSubmissionDet DocSubmissionDet { get; set; }
    }
}