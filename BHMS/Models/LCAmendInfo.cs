namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCAmendInfo")]
    public partial class LCAmendInfo
    {
        public int Id { get; set; }

        [Display(Name = "LC No")]
        public int MasterLCInfoMasId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Amendment Date")]
        public DateTime AmendDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "LC Amendment No")]
        public string AmendLCNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Amendment Rcv Date")]
        public DateTime? AmendLCRecvDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Re. Latest Shipment Date")]
        public DateTime? AmendLatestShipDate { get; set; }

        [Display(Name = "Re. Quantity (pcs)")]
        public int? AmendQuantity { get; set; }

        [Display(Name = "Revised Value ($)")]
        public decimal AmendTotalValue { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Re. Expiry Date")]
        public DateTime? AmendLCExpiryDate { get; set; }

        [Display(Name = "Re. Payment Term")]
        public int? AmendPaymentTerm { get; set; }

        [Display(Name = "Re. Tenor")]
        public int? AmendTenor { get; set; }

        public virtual MasterLCInfoMas MasterLCInfoMas { get; set; }
    }
}
