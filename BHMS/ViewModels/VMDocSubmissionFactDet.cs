using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDocSubmissionFactDet
    {
        public int FactInvoicerDetTempId { get; set; }

        public int Id { get; set; }

        public int DocSubmissionDetId { get; set; }

        public int? InvoiceCommDetId { get; set; }


        public int? InvoiceFactDetId { get; set; }

        public string FactFDBCNo { get; set; }

    }
}