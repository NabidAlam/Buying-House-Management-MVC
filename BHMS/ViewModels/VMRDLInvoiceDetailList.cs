using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMRDLInvoiceDetailList
    {
        public int InvCommDetId { get; set; }
        public int FactoryId { get; set; }
        public int InvCommFactMasId { get; set; }
        public decimal TotalQty { get; set; }
        public decimal FactoryInvValue { get; set; }
        public decimal RDLValue { get; set; }
    }
}