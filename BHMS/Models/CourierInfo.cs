namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CourierInfo")]
    public partial class CourierInfo
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public CourierInfo()
        //{
        //    DocSubmissionMas = new HashSet<DocSubmissionMas>();
        //}

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocSubmissionMas> DocSubmissionMas { get; set; }
    }
}
