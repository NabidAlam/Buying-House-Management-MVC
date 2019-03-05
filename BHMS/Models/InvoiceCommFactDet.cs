namespace BHMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceCommFactDet")]
    public partial class InvoiceCommFactDet
    {
        public int Id { get; set; }

        public int InvoiceCommFactMasId { get; set; }

        public int ExFactoryDetId { get; set; }


        public decimal? InvoiceTotalAmt { get; set; }

        //public virtual ExFactoryShipDet ExFactoryShipDet { get; set; }
        public virtual ExFactoryDet ExFactoryDet { get; set; }

        public virtual InvoiceCommFactMas InvoiceCommFactMas { get; set; }
    }
}
