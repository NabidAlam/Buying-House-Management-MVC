using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMMasterLCInfoMas
    {
        public int Id { get; set; }
                
        [StringLength(50)]
        [Display(Name = "LC No")]
        public string LCNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Opening Date")]
        public DateTime? LCDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Expiry Date")]
        public DateTime? LCExpiryDate { get; set; }

        [Display(Name = "Payment Term")]
        public int? PaymentTerm { get; set; }

        public int? Tenor { get; set; }

        [Display(Name = "Receive Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LCReceiveDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Latest Shipment Date")]
        public DateTime? LatestShipmentDate { get; set; }

        public int? Quantity { get; set; }

        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [Display(Name = "Buyer")]
        public int? BuyerInfoId { get; set; }

        
        public string BuyerName { get; set; }

        [Display(Name = "Buyer Bank")]
        public int? BuyerBankId { get; set; }

        //public bool IsAuth { get; set; }

        //public int OpBy { get; set; }

        //public DateTime OpOn { get; set; }

        //public int? AuthBy { get; set; }

        //public DateTime? AuthOn { get; set; }

        
    }

    public class VMMasterLCInfoOrderDet
    {
        public int TempMasterLCInfoDetId { get; set; }

        public int Id { get; set; }

        public int MasterLCInfoDetId { get; set; }

        public int BuyerOrderDetId { get; set; }

    }
}