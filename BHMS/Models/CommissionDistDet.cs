using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BHMS.Models
{
    [Table("CommissionDistDet")]
    public partial class CommissionDistDet
    {
        public int Id { get; set; }

        public int CommissionDistMasId { get; set; }

        public int FactoryOrderDetId { get; set; }

        public double OverseasCommPer { get; set; }

        public decimal OverseasCommValue { get; set; }

        public double OthersCommPer { get; set; }

        public decimal OthersCommValue { get; set; }

        public decimal CompCommValue { get; set; }

        public virtual CommissionDistMas CommissionDistMas { get; set; }

        public virtual FactoryOrderDet FactoryOrderDet { get; set; }

        public bool? CheckFlag { get; set; }
    }
}