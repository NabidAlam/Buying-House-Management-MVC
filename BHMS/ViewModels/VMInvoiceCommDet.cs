using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMInvoiceCommDet
    {
        public int TempOrderDetId { get; set; }

        public int Id { get; set; }

        public int InvoiceCommMasId { get; set; }

        public int InvoiceCommFactMasId { get; set; }

        public decimal? InvoiceRDLTotalAmt { get; set; }

    }
}