using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMInvoiceCommFactMas
    {

        public int Id { get; set; }

        public int SupplierId { get; set; }

        public int BuyerInfoId { get; set; }

        public int ExfactoryMasId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Factory Invoice No.")]
        public string InvoiceNoFact { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ex-Factory Date")]
        public DateTime IssueDate { get; set; }

    }
}