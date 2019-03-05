namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MasterLCInfoOrderDet")]
    public partial class MasterLCInfoOrderDet
    {
        public int Id { get; set; }

        public int MasterLCInfoDetId { get; set; }

        public int BuyerOrderDetId { get; set; }

        public virtual BuyerOrderDet BuyerOrderDet { get; set; }

        public virtual MasterLCInfoDet MasterLCInfoDet { get; set; }
    }
}
