using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMDocSubmissionMas
    {

        public int Id { get; set; }

        public int PaymentTypeId { get; set; }

        public int BuyerInfoId { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string FDBCNo { get; set; }

        public decimal? FDBCValue { get; set; }

        public DateTime? FDBCDate { get; set; }

        public int? MasterLCInfoMasId { get; set; }

        public string AWBNo { get; set; }

        public DateTime? AWBDate { get; set; }

        public int? CourierInfoId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

    }
}