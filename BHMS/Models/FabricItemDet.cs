using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("FabricItemDet")]
    public class FabricItemDet
    {
        public int Id { get; set; }
        public int FabricItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual FabricItem FabricItem { get; set; }
    }
}