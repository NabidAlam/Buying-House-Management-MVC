using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("UserDept")]
    public class UserDept
    {

        public int Id { get; set; }

        [Display(Name = "Department")]
        public string DeptName { get; set; }
    }
}