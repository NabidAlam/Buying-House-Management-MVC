using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public class TTPayment
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        //[Display(Name = "DocSubmissionFactDet")]
        //public int DocSubmissionDetId { get; set; }
        public int DocSubmissionFactDetId { get; set; }
        public int ProceedRealizationDetId { get; set; }
        

        public string FDDNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? FDDDate { get; set; }

        public decimal FDDAmount { get; set; }

        //public bool IsAuth { get; set; }

        //public int OpBy { get; set; }

        //public DateTime OpOn { get; set; }

        //public int? AuthBy { get; set; }

        //public DateTime? AuthOn { get; set; }

        public virtual DocSubmissionFactDet DocSubmissionFactDet { get; set; }

        public virtual ProceedRealizationDet ProceedRealizationDet { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}