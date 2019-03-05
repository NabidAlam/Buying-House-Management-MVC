using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDocSubmissionDet
    {
        public int TempDocSubDetId { get; set; }
        public int Id { get; set; }

        public int DocSubmissionMasId { get; set; }

        public int? InvoiceCommMasId { get; set; }
        public int? BuyerOrderMasId { get; set; }

    }
}