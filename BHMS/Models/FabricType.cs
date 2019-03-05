using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("FabricType")]
    public class FabricType
    {
        public int Id { get; set; }
        [Display(Name = "Fabric Type")]
        public string Name { get; set; }

        [Display(Name = "Product Type")]
        public int ProdCategoryId { get; set; }
        public virtual ProdCategory ProdCategory { get; set; }


    }
}