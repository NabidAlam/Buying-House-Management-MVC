namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ExFactoryMas
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ExFactoryMas()
        //{
        //    ExFactoryDet = new HashSet<ExFactoryDet>();
        //}

        public int Id { get; set; }

        [Column(TypeName = "date")]
        [Display(Name ="Ex-Factory Date")]
        public DateTime ExFactoryDate { get; set; }

        public int SupplierId { get; set; }

        public int BuyerInfoId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExFactoryDet> ExFactoryDet { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
