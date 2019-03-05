using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMBuyerOrderDetCottonOn
    {

        public int TempOrderDetId { get; set; }

        public int Id { get; set; }

        public int BuyerOrderMasId { get; set; }

        public string ProdCatId { get; set; }
        //public string ProdCatName { get; set; }

        public int? ProdCatTypeId { get; set; }
        //public string ProdCatTypeName { get; set; }

        [StringLength(50)]
        public string StyleNo { get; set; }

        //public int? ProdSizeId { get; set; }
        public string ProdSizeName { get; set; }

        //public int? FabricItemId { get; set; }
        public string FabricItemId { get; set; }

        public int? FabricSupplierId { get; set; }
        //public string FabricItemName { get; set; }

        //public int? ProdColorId { get; set; }
        public string ProdColorName { get; set; }
        //public string ProdColorName { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public int SupplierId { get; set; }

        public decimal? RDLTotal { get; set; }
    }
}