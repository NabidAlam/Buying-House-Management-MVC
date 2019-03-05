using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMFactoryOrderMas
    {
        public int Id { get; set; }

        public int BuyerOrderMasId { get; set; }

        public string BuyerOrderRefNo { get; set; }

        
        public string BuyerOrderRefDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Sales Contract No")]
        public string SalesContractNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]
        public DateTime SalesContractDate { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        //public bool IsAuth { get; set; }

        //public int OpBy { get; set; }

        //public DateTime OpOn { get; set; }

        //public int? AuthBy { get; set; }

        //public DateTime? AuthOn { get; set; }

        //public bool IsLocked { get; set; }

       
    }
}