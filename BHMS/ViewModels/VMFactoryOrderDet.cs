using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMFactoryOrderDet
    {
        public int TempOrderDetId { get; set; }

        public int Id { get; set; }

        public int BuyerOrderDetId { get; set; }

        public decimal? FOBUnitPrice { get; set; }

        public decimal? TransferPrice { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        public bool IsLocked { get; set; }

        public int FactoryOrderMasId { get; set; }
    }
}