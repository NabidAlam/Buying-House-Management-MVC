using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("TimeActionMas")]
    public class TimeActionMas
    {
        public int Id { get; set; }

        [Display(Name = "T & A Template Name")]
        public string TemplateName { get; set; }

        [Display(Name = "Buyer Name")]
        public int BuyerInfoId { get; set; }

        [Display(Name = "Department")]
        public int UserDeptId { get; set; }

        [Display(Name = "Responsible Person")]
        public int CompanyResourceId { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }
        public virtual UserDept UserDept { get; set; }
        public virtual CompanyResource CompanyResource { get; set; }
    }
}