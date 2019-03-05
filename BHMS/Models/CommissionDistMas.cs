using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    public partial class CommissionDistMas
    {
        
        //public CommissionDistMas()
        //{
        //    CommissionDistDet = new HashSet<CommissionDistDet>();
        //}

        public int Id { get; set; }

        public int BuyerOrderMasId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerOrderMas BuyerOrderMas { get; set; }
                
        public virtual ICollection<CommissionDistDet> CommissionDistDet { get; set; }
    }
}