using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("CommissionRealization")]
    public class CommissionRealization
    {     
        public int Id { get; set; }
        public int ProceedRealizationMasId { get; set; }
        [Display(Name = "Exchange Rate")]
        public decimal ExchangeRate { get; set; }
        [Display(Name = "AIT (10%) Tk.")]
        public decimal AITAmount { get; set; }
        [Display(Name = "Bank Charge Tk.")]
        public decimal BankCharge { get; set; }
        [Display(Name = "Realization Date")]
        public DateTime RealizationDate { get; set; }
        public virtual ProceedRealizationMas ProceedRealizationMas { get; set; }

    }
}