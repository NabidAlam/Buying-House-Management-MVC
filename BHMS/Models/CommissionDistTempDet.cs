namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommissionDistTempDet")]
    public partial class CommissionDistTempDet
    {
        public int Id { get; set; }

        public int CommissionDistTempMasId { get; set; }

        public double MinRange { get; set; }

        public double MaxRange { get; set; }

        public double OverseasComm { get; set; }

        public double OthersComm { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public virtual CommissionDistTempMas CommissionDistTempMas { get; set; }
    }
}
