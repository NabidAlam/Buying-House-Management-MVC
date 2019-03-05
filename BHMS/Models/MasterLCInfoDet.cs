namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MasterLCInfoDet")]
    public partial class MasterLCInfoDet
    {        
        public int Id { get; set; }

        public int MasterLCInfoMasId { get; set; }

        public int BuyerOrderMasId { get; set; }

        [StringLength(50)]
        public string PINo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PIDate { get; set; }

        public virtual BuyerOrderMas BuyerOrderMas { get; set; }

        public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }
                
        public virtual ICollection<MasterLCInfoOrderDet> MasterLCInfoOrderDet { get; set; }
    }
}
