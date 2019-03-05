using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentBuyerMas")]

    public class DiscountAdjustmentBuyerMas
    {
      

      
            public int Id { get; set; }
            [Display(Name = "Buyer")]
            public int BuyerInfoId { get; set; }
            [Display(Name = "Supplier")]
            public int SupplierId { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(Name = "Date")]
            public DateTime? DateAdj { get; set; }


            public bool IsAuth { get; set; }
            public int OpBy { get; set; }
            public DateTime OpOn { get; set; }
            public int? AuthBy { get; set; }
            public DateTime? AuthOn { get; set; }


           public virtual BuyerInfo BuyerInfo { get; set; }
           public virtual Supplier Supplier { get; set; }


    }

  }