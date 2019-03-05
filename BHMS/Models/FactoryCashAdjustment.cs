using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHMS.Models
{
    [Table("FactoryCashAdjustment")]
    public class FactoryCashAdjustment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Entry Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EntryDate { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        [Display(Name = "Adustment Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FacAdjustDate { get; set; }

        [Required]
        [StringLength(100)]
        public string FacReciptNo { get; set; }
  

        [Required]
        public decimal FacAdjustAmount { get; set; }

        [StringLength(500)]
        public string FacAdjustRemarks { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }


        public virtual Supplier Supplier { get; set; }

    }
}