using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("BankCharge")]
    public class BankCharge
    {
        public int Id { get; set; }
        public decimal Charge { get; set; }

        //new
        [Display(Name = "Proceed Type")]
        public int PaymentTypeId { get; set; }

    }
}