namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FactoryOrderMas
    {
        public int Id { get; set; }

        public int BuyerOrderMasId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Sales Contract No")]
        public string SalesContractNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]
        public DateTime SalesContractDate { get; set; }

        public int SupplierId { get; set; }

        public bool IsAuth { get; set; }

        public int OpBy { get; set; }

        public DateTime OpOn { get; set; }

        public int? AuthBy { get; set; }

        public DateTime? AuthOn { get; set; }

        public bool IsLocked { get; set; }

        public virtual BuyerOrderMas BuyerOrderMas { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<FactoryOrderDet> FactoryOrderDet { get; set; }
    }
}
