using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHMS.Models
{
    [Table("BuyerCashAdjustment")]
    public class BuyerCashAdjustment
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Entry Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime EntryDate { get; set; }

        [Required]
        public int BuyerInfoId { get; set; }

        [Required]
        [Display(Name = "Adustment Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BuyerAdjustDate { get; set; }

        [Required]
        [StringLength(100)]
        public string BuyerReciptNo { get; set; }


        [Required]
        public decimal BuyerAdjustAmount { get; set; }

        [StringLength(500)]
        public string BuyerAdjustRemarks { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }


        public virtual BuyerInfo BuyerInfo { get; set; }


    }
}