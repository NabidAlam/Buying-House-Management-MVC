namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SeasonInfo")]
    public partial class SeasonInfo
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public SeasonInfo()
        //{
        //    BuyerOrderMas = new HashSet<BuyerOrderMas>();
        //}

        public int Id { get; set; }

        [Display(Name = "Buyer")]
        public int BuyerInfoId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Season")]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuyerOrderMas> BuyerOrderMas { get; set; }

        public virtual BuyerInfo BuyerInfo { get; set; }
    }
}
