using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public class Secu_User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public string Salt { get; set; }

        [Display(Name = "Full Name")]
        public string UserFullName { get; set; }

        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "User Status")]
        public string UserStatus { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }
        public int? InvalidAttempt { get; set; }

        [Display(Name = "Team Name")]
        public int? TeamId { get; set; }

        [Display(Name = "Designation Name")]
        public int? DesignationId { get; set; }

        [Display(Name = "Role Name")]
        public int? RoleId { get; set; }

    }
}