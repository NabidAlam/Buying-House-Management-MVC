using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.ViewModels
{
    public class VMFactoryOrderDelivList
    {
        public int Id { get; set; }
        public int BuyerOrderDetId { get; set; }
        public int FactoryOrderDetId { get; set; }
        public int DelivSlno { get; set; }
        public string ExFactoryDate { get; set; }
        public string HandoverDate { get; set; }
        public string ETD { get; set; }
        public int? DestinationPortId { get; set; }
        public int DelivQuantity { get; set; }
        public decimal? RDLFOB { get; set; }
        public decimal? RDLValue { get; set; }
        public decimal FactFOB { get; set; }
        public decimal FactTransferValue { get; set; }
        public string BuyersPONo { get; set; }
        public bool IsLocked { get; set; }
        public string DestinationPortName { get; set; }
        public string ShipmentMode { get; set; }
        public int ShipDetId { get; set; }
        public string Remarks { get; set; }
    }
}