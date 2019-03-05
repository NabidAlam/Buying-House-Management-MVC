using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("AIT")]
    public class AIT
    {
        public int Id { get; set; }
        public decimal AITPercent { get; set; }
    }
}