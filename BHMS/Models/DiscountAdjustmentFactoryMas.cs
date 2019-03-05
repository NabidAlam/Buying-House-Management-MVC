using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("DiscountAdjustmentFactoryMas")]

    public class DiscountAdjustmentFactoryMas
    {
        public int Id { get; set; }
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        //[ForeignKey("BuyerOrderMasIdPrev"), Column(Order = 0)]
        //public int BuyerOrderMasPrevId { get; set; }
        //[ForeignKey("BuyerOrderMasIdAdj"), Column(Order = 1)]
        //public int BuyerOrderMasAdjId { get; set; }

        [Display(Name = "Date")]
        public DateTime DateAdj { get; set; }


        public bool IsAuth { get; set; }
        public int OpBy { get; set; }
        public DateTime OpOn { get; set; }
        public int? AuthBy { get; set; }
        public DateTime? AuthOn { get; set; }


        public virtual Supplier Supplier { get; set; }
        public virtual BuyerInfo BuyerInfo { get; set; }  
                          
        //public virtual BuyerOrderMas BuyerOrderMasIdPrev { get; set; }
        //public virtual BuyerOrderMas BuyerOrderMasIdAdj { get; set; }


       
       


    }
}