using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMExFactoryMasEdit
    {
        public int Id { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Ex-Factory Date")]
        public DateTime ExFactoryDate { get; set; }

        public int SupplierId { get; set; }

        public int BuyerInfoId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        
        
    }
}