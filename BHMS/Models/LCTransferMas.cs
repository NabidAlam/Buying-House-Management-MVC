namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCTransferMas
    {
        
        public int Id { get; set; }

        public int MasterLCInfoMasId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }
                
        public virtual ICollection<LCTransferDet> LCTransferDet { get; set; }

        public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }
    }
}
