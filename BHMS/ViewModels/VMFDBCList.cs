using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMFDBCList
    {
        public int ProceedRealizationDetId { get; set; }
        public int ProceedRealizationMasId { get; set; }
        public int ProceedTypeId { get; set; }
        public string ProceedType { get; set; }
        public string FDBCNo { get; set; }
        public string ProceedDate { get; set; }
        public decimal ProceedValue { get; set; }
        public decimal? RDLInvoiceValue { get; set; }
        public decimal? RDLInvoiceValueWithDiscount { get; set; }
        public int Pending { get; set; }
        public int Paid { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalFDDAmount { get; set; }
        public int CheckCommission { get; set; }

    }
}