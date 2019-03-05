using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMInvoiceDetailList
    {
        public int InvoiceDetailId { get; set; }
        public int ExFactoryDetId { get; set; }
        public int OrderMasId { get; set; }
        public string OrderRefNo { get; set; }
        public int ShipQuantity { get; set; }
        public string ExFactoryDate { get; set; }
        public int MasterLCId { get; set; }
        public string MasterLCnO { get; set; }
        public bool Ischecked { get; set; }
    }
}