using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountMas")]
    public class DiscountMas
    {
        
        public int Id { get; set; }
        //public int BuyerInfoId { get; set; }
        //public int SupplierId { get; set; }

        public int BuyerOrderDetId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Discount Date")]
        public DateTime? DiscountDate { get; set; }
        public bool IsAuth { get; set; }
        public int OpBy { get; set; }
        public DateTime OpOn { get; set; }
        public int? AuthBy { get; set; }
        public DateTime? AuthOn { get; set; }

        public bool? DiscountFlag { get; set; }
        //public virtual BuyerInfo BuyerInfo { get; set; }         
        public virtual BuyerOrderDet BuyerOrderDet { get; set; }
        //public virtual Supplier Supplier { get; set; }
       

    }
}



