using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMExFactoryShipDet
    {
        public int Id { get; set; }
        public int ExFactoryDetDetId { get; set; }
        public int ExFactoryDetId { get; set; }

        public int BuyerOrderDetId { get; set; }

        public int ShipmentSummDetId { get; set; }

        public int ShipQuantity { get; set; }

        [StringLength(50)]
        public string Remarks { get; set; }

        public bool IsShipClosed { get; set; }

        public int? CurrentShipmentMode { get; set; }



    }
}