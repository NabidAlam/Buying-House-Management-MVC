namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FabricItem")]
    public partial class FabricItem
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public FabricItem()
        //{
        //    BuyerOrderDet = new HashSet<BuyerOrderDet>();
        //}

        public int Id { get; set; }

        //[Required]
        //[StringLength(50)]
        public string Name { get; set; }

        //[StringLength(256)]
        public string Description { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public int? FabricTypeId { get; set; }
        public virtual FabricType FabricType { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderDet> BuyerOrderDet { get; set; }
    }
}
