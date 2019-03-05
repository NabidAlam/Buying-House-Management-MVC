using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMBuyerOrderMasCottonOnEdit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Order Ref No")]
        public string OrderRefNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        [Display(Name = "Department")]
        public int? ProdDepartmentId { get; set; }

        [Display(Name = "Season")]
        public int? SeasonInfoId { get; set; }

        [Display(Name = "Fabric Supplier")]
        public int? FabSupplierId { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        public int? FOBType { get; set; }
        public int? DeliveryBased { get; set; }

    }
}