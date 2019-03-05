using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public class Secu_UsersBuyer
    {
        public int Id { get; set; }
    
        [Display(Name ="User Name")]
        [Index("UK_Secu_UsersBuyer", 1, IsUnique = true)]
        public int UserId { get; set; }

        [Index("UK_Secu_UsersBuyer", 2, IsUnique = true)]
        [Display(Name = "Buyer Name")]
        public int BuyerInfoId { get; set; }

        public virtual BuyerInfo BuyInfo { get; set; }


    }
}


