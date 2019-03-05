namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactoryOrderDet")]
    public partial class FactoryOrderDet
    {
        //public FactoryOrderDet()
        //{
        //    CommissionDistDet = new HashSet<CommissionDistDet>();
        //}

        public int Id { get; set; }

        public int BuyerOrderDetId { get; set; }

        public decimal? FOBUnitPrice { get; set; }

        public decimal? TransferPrice { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        public bool IsLocked { get; set; }

        public int FactoryOrderMasId { get; set; }

        public virtual BuyerOrderDet BuyerOrderDet { get; set; }

        public virtual ICollection<CommissionDistDet> CommissionDistDet { get; set; }

        public virtual FactoryOrderMas FactoryOrderMas { get; set; }
    }
}
