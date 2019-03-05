namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCTransferDet")]
    public partial class LCTransferDet
    {
        public int Id { get; set; }

        public int LCTransferMasId { get; set; }

        public int FactoryOrderMasId { get; set; }

        [Column(TypeName = "date")]
        public DateTime TransferDate { get; set; }

        public virtual FactoryOrderMas FactoryOrderMas { get; set; }

        public virtual LCTransferMas LCTransferMas { get; set; }
    }
}
