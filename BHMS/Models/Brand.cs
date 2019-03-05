using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Brand")]
        public string Name { get; set; }

        public int BuyerInfoId { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }
    }
}