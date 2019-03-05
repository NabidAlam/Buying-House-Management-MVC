using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHMS.Helpers;
using System.Data.Entity;
using BHMS.ViewModels;

namespace BHMS.Controllers
{
    public class InvoiceCommercialRDLController : Controller
    {
        private ModelBHMS db = new ModelBHMS();



        public ActionResult Index()
        {
            var invoiceCommMas = db.InvoiceCommMas;
            return View(invoiceCommMas.ToList());
        }


        //--------------------------------------------------EDIT-------------------------------------------------------------------------------------
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceCommMas invoiceCommMas = db.InvoiceCommMas.Find(id);
            if (invoiceCommMas == null)
            {
                return HttpNotFound();
            }

            ViewBag.InvoiceNo = db.InvoiceCommMas.SingleOrDefault(x => x.Id == id).InvoiceNo;

            var date = db.InvoiceCommMas.SingleOrDefault(x => x.Id == id).IssueDate;
            ViewBag.IssueDate = NullHelpers.DateToString(date);
            var getBuyer = (from invComMas in db.InvoiceCommMas
                            join buyInfo in db.BuyerInfo on invComMas.BuyerInfoId equals buyInfo.Id
                            where invComMas.Id == id
                            select buyInfo).Distinct().ToList();

            ViewBag.BuyerInfoId = new SelectList(getBuyer, "Id", "Name", invoiceCommMas.BuyerInfoId);

            var MasterLCdata = db.MasterLCInfoMas.Where(x => x.BuyerInfoId == invoiceCommMas.BuyerInfoId).OrderBy(x => x.LCNo).Distinct().ToList();
            ViewBag.MasterLCInfoMasId = new SelectList(MasterLCdata, "Id", "LCNo", invoiceCommMas.MasterLCInfoMasId);
            ViewBag.LCNo = db.InvoiceCommMas.SingleOrDefault(x => x.Id == id).MasterLCInfoMas.LCNo;
            var PaymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text", 0);
            ViewBag.PaymentTypeId = PaymentType;

            ViewBag.SplitType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "No Split", Value = "0" }, new SelectListItem { Text = "Split", Value = "1" }, }, "Value", "Text", invoiceCommMas.SplitFlag);

            //var getSupplier = (from invComMas in db.InvoiceCommMas
            //                   join invComDet in db.InvoiceCommDet on invComMas.Id equals invComDet.BuyerOrderMasId
            //                   join invComFactMas in db.InvoiceCommFactMas on new { a = invComMas.BuyerInfoId, b = invComDet.BuyerOrderMasId }
            //                   equals new { a = invComFactMas.BuyerInfoId, b = invComFactMas.Id }
            //                   where invComMas.Id == id
            //                   select invComFactMas.Id).Distinct().ToList();
            //ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", getSupplier);



            ViewBag.CheckDocSubExists = db.DocSubmissionDet.Where(x => x.InvoiceCommMasId == invoiceCommMas.Id).Count() > 0 ? true : false;



            return View(invoiceCommMas);
        }


        public JsonResult GetOrderList(int Id)
        {
            var data = (from invFactMas in db.InvoiceCommFactMas
                        join invCommMas in db.InvoiceCommMas on invFactMas.BuyerInfoId equals invCommMas.BuyerInfoId
                        where invCommMas.BuyerInfoId == Id
                        select invFactMas).Select(y => new { Name = y.Supplier.Name, Id = y.SupplierId })
                        .ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetSelectedData(int Id)
        //{

        //    var list = (from exFactoryMas in db.ExFactoryMas
        //                join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                        new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                from pJoin in joinDest.DefaultIfEmpty()

        //                join invFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                equals new { a = invFactMas.SupplierId, b = invFactMas.BuyerInfoId }

        //                join invCommFactDet in db.InvoiceCommFactDet on new { a = invFactMas.Id, b = exfactoryDet.Id }
        //                equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }

        //                join invComDet in db.InvoiceCommDet on invFactMas.Id equals invComDet.InvoiceCommFactMasId
        //                where invComDet.InvoiceCommMasId == Id
        //                select new { invFactMas, invComDet }).AsEnumerable()
        //            .Select(x => new
        //            {
        //                FactoryName = x.invFactMas.Supplier.Name,
        //                FactoryId = x.invFactMas.Supplier.Id,
        //                InvCommDetId = x.invComDet.Id,
        //                InvCommFactMasId = x.invFactMas.Id,
        //                InvoiceNo = x.invFactMas.InvoiceNoFact,
        //                //BuyerOrderMasId = x.invComDet.BuyerOrderMasId,
        //                TotalQty = (from exFactoryMas in db.ExFactoryMas
        //                            join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                            join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                            join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                            join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                                    new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                            join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                            join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                            from pJoin in joinDest.DefaultIfEmpty()

        //                            join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                            equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }

        //                            join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
        //                            equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }

        //                            where invCommFactMas.Id == x.invFactMas.Id
        //                            select exfactoryShipDet.ShipQuantity
        //                   ).Sum(),

        //                //RDLValue = (from exFactoryMas in db.ExFactoryMas
        //                //            join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                //            join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                //            join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                //            join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                //                                                    new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                //            join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                //            join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                //            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                //            from pJoin in joinDest.DefaultIfEmpty()
        //                //            join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                //            equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
        //                //            join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
        //                //            equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
        //                //            where invCommFactMas.Id == x.invFactMas.Id
        //                //            select new { RDProduct = exfactoryShipDet.ShipQuantity * buyerOrderDet.UnitPrice }).Distinct().Sum(m => m.RDProduct),


        //                RDLValue = (from FactoryInvoiceMas in db.InvoiceCommFactMas
        //                            join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
        //                            join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
        //                            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
        //                            join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
        //                            join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerDet.SupplierId, b = exFactoryDet.BuyerOrderMas.BuyerInfoId }
        //                            equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
        //                            join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exFactoryDet.Id }
        //                            equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
        //                            where invCommFactMas.Id == x.invFactMas.Id
        //                            select new
        //                            {
        //                                Quantity = exFactoryShipDet.ShipQuantity,
        //                                RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
        //                            }
        //                                ).Distinct().Sum(m => m.RDLValue),



        //                FactoryInvValue = (from exFactoryMas in db.ExFactoryMas
        //                                   join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                                   join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                                   join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                                   join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                                           new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                                   join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                                   join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                                   join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                                   from pJoin in joinDest.DefaultIfEmpty()
        //                                   join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                                   equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
        //                                   join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
        //                                   equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
        //                                   where invCommFactMas.Id == x.invFactMas.Id
        //                                   select new
        //                                   {
        //                                       FactoryInv = exfactoryShipDet.ShipQuantity * (factoryOrderDet.TransferPrice == null ? factoryOrderDet.FOBUnitPrice : factoryOrderDet.TransferPrice)
        //                                   }).Distinct()
        //                          .Sum(n => n.FactoryInv)

        //            }).Distinct();

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}


        //Tazbirul(30 Aug , 2018)
        public JsonResult GetSelectedData(int Id)
        {
            List<VMRDLInvoiceDetailList> list = new List<VMRDLInvoiceDetailList>();

            var RDLInvoiceDetails = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == Id).ToList();

            foreach (var inv in RDLInvoiceDetails)
            {

                //var Quantity = (from buyerOrderMas in db.BuyerOrderMas
                //                join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
                //                join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
                //                join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                //                join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
                //                join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id }
                //                        equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
                //                join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
                //                        equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
                //                join invoiceFactDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceFactDet.ExFactoryDetId
                //                //where buyerOrderMas.BuyerInfoId == BuyerInfoId && buyerOrderDet.SupplierId == FactoryId
                //                where invoiceFactMas.Id == inv.InvoiceCommFactMasId
                //                && lcDet.MasterLCInfoMasId == inv.InvoiceCommMas.MasterLCInfoMasId
                //                //select buyerOrderDet.Quantity).Sum();
                //                select exFactShip.ShipQuantity).Sum();

                var Quantity = db.InvoiceCommDetDet.Where(x => x.InvoiceCommDetId == inv.Id).Sum(x => x.ShipQty);

                //var RDLValue = (from buyerOrderMas in db.BuyerOrderMas
                //                join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
                //                join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
                //                join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                //                join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
                //                join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id } equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
                //                join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
                //                        equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
                //                join invoiceFactDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceFactDet.ExFactoryDetId
                //                where buyerOrderMas.BuyerInfoId == inv.InvoiceCommFactMas.BuyerInfoId && buyerOrderDet.SupplierId == inv.InvoiceCommFactMas.SupplierId
                //                && lcDet.MasterLCInfoMasId == inv.InvoiceCommMas.MasterLCInfoMasId && invoiceFactMas.Id == inv.InvoiceCommFactMasId
                //                select new { RDlValue = exFactShip.ShipQuantity * shipDet.RdlFOB }).Sum(x => x.RDlValue);

                var RDLValue = db.InvoiceCommDet.FirstOrDefault(x => x.Id == inv.Id).InvoiceRDLTotalAmt;

                //var FactData = (from exFactoryMas in db.ExFactoryMas
                //                join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                //                join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                //                join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                //                join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                //                                                        new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                //                join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                //                join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                //                join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                //                join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                //                join invoiceFactMas in db.InvoiceCommFactMas on exFactoryMas.ExFactoryDate equals invoiceFactMas.IssueDate
                //                join invoiceFactDet in db.InvoiceCommFactDet on new { ColA = invoiceFactMas.Id, ColB = exfactoryDet.Id }
                //                                equals new { ColA = invoiceFactDet.InvoiceCommFactMasId, ColB = invoiceFactDet.ExFactoryDetId }
                //                join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                //                from pJoin in joinDest.DefaultIfEmpty()
                //                    //where exFactoryMas.ExFactoryDate == invoiceFactMas.IssueDate && exfactoryDet.Id == invoiceFactDet.ExFactoryDetId
                //                where invoiceFactMas.Id == inv.InvoiceCommFactMasId
                //                select new
                //                {
                //                    StyleNo = buyerOrderDet.StyleNo,
                //                    BuyerOrderDetId = shipDet.BuyerOrderDetId,
                //                    BuyersPONo = shipDet.BuyersPONo ?? "",
                //                    DelivSlno = shipDet.DelivSlno,
                //                    ProdSizeId = buyerOrderDet.ProdSizeId == null ? 0 : buyerOrderDet.ProdSizeId,
                //                    ProdSizeName = buyerOrderDet.ProdSize.SizeRange == null ? "" : buyerOrderDet.ProdSize.SizeRange,
                //                    ProdColorId = buyerOrderDet.ProdColorId == null ? 0 : buyerOrderDet.ProdColorId,
                //                    ProdColorName = buyerOrderDet.ProdColor.Name == null ? "" : buyerOrderDet.ProdColor.Name,
                //                    DestinationPortId = shipDet.DestinationPortId == null ? 0 : shipDet.DestinationPortId,
                //                    DestinationPortName = pJoin == null ? "" : pJoin.Name,
                //                    OrderQuantity = shipDet.DelivQuantity,
                //                    ShipQuantity = exfactoryShipDet.ShipQuantity,
                //                    //FactoryFOB = (factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice),
                //                    FactoryFOB = factoryDetDet.FactFOB,
                //                    FactoryOrderDelivDetId = factoryDetDet.Id,
                //                    DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
                //                    //DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * ((factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice) * exfactoryShipDet.ShipQuantity)),
                //                    DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * (factoryDetDet.FactFOB * exfactoryShipDet.ShipQuantity)),
                //                    PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
                //                }).ToList();
                var FactData = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == inv.InvoiceCommFactMasId).Sum(x => x.InvoiceTotalAmt);

                //calculate factory invoice value including Discount now and previous
                var totalFactInvoiceValue = (decimal)0;
                //foreach (var item in FactData)
                //{
                //    var currentFOB = (decimal)0;
                //    var currentValue = (decimal)0;

                //    if (item.DiscountValue != 0 || item.PreviousDiscountAdj != 0)
                //    {
                //        var FactoryValue = item.ShipQuantity * (item.FactoryFOB ?? 0);
                //        var currentDiscount = item.DiscountValue ?? 0;
                //        var previousDiscount = item.PreviousDiscountAdj == 0 ? 0 : item.PreviousDiscountAdj;
                //        currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (decimal)item.ShipQuantity ?? 0);
                //        //currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (decimal)item.ShipQuantity) ; //problem
                //        currentValue = currentFOB * item.ShipQuantity;
                //    }
                //    else
                //    {
                //        currentValue = item.ShipQuantity * (item.FactoryFOB ?? (decimal)0);
                //    }
                //    totalFactInvoiceValue = totalFactInvoiceValue + currentValue;

                //}

                VMRDLInvoiceDetailList vm = new VMRDLInvoiceDetailList();
                vm.InvCommDetId = inv.Id;
                vm.FactoryId = inv.InvoiceCommFactMas.SupplierId;
                vm.InvCommFactMasId = inv.InvoiceCommFactMasId;
                vm.TotalQty = Quantity ?? 0;
                //vm.FactoryInvValue = totalFactInvoiceValue;
                vm.FactoryInvValue = FactData ?? 0;
                vm.RDLValue = RDLValue ?? 0;

                list.Add(vm);

            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //-----------------------------------------------EDIT END.....................................................................................


        public JsonResult InvoiceRDLFactory(int BuyerInfoId, int FactoryId, int LCMasId, int InvoiceFactMasiD)
        {

            var Quantity = (from buyerOrderMas in db.BuyerOrderMas
                            join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
                            join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
                            join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                            join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
                            join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id }
                                    equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
                            join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
                                    equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
                            join invoiceFactDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceFactDet.ExFactoryDetId
                            //where buyerOrderMas.BuyerInfoId == BuyerInfoId && buyerOrderDet.SupplierId == FactoryId
                            where invoiceFactMas.Id == InvoiceFactMasiD
                            && lcDet.MasterLCInfoMasId == LCMasId
                            //select buyerOrderDet.Quantity).Sum();
                            select exFactShip.ShipQuantity).Sum();

            var RDLValue = (from buyerOrderMas in db.BuyerOrderMas
                            join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
                            join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
                            join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                            join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
                            join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id } equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
                            join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
                                    equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
                            join invoiceFactDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceFactDet.ExFactoryDetId
                            where buyerOrderMas.BuyerInfoId == BuyerInfoId && buyerOrderDet.SupplierId == FactoryId
                            && lcDet.MasterLCInfoMasId == LCMasId && invoiceFactMas.Id == InvoiceFactMasiD
                            select new { RDlValue = exFactShip.ShipQuantity * shipDet.RdlFOB }).Sum(x => x.RDlValue);


            /*select* from  ExFactoryMas exFactoryMas
            inner join  ExFactoryDet exfactoryDet on exFactoryMas.Id = exfactoryDet.ExFactoryMasId
            inner join  ExFactoryShipDet exfactoryShipDet on exfactoryDet.Id = exfactoryShipDet.ExFactoryDetId
            inner join  BuyerOrderMas buyerOrderMas on exfactoryDet.BuyerOrderMasId = buyerOrderMas.Id
            inner join  BuyerOrderDet buyerOrderDet on buyerOrderMas.Id = buyerOrderDet.BuyerOrderMasId
                        and exfactoryShipDet.BuyerOrderDetId = buyerOrderDet.Id
            inner join  FactoryOrderDet factoryOrderDet on buyerOrderDet.Id = factoryOrderDet.BuyerOrderDetId
            inner join  ShipmentSummDet shipDet on buyerOrderDet.Id = shipDet.BuyerOrderDetId and exfactoryShipDet.ShipmentSummDetId = shipDet.Id
            inner join  FactoryOrderDelivDet factoryDetDet on factoryOrderDet.Id = factoryDetDet.FactoryOrderDetId and shipDet.Id = factoryDetDet.ShipmentSummDetId
            inner join  MasterLCInfoDet lcDet on buyerOrderMas.Id = lcDet.BuyerOrderMasId
            inner join  InvoiceCommFactMas invoiceFactMas on exFactoryMas.ExFactoryDate = invoiceFactMas.IssueDate
            inner join  InvoiceCommFactDet invoiceFactDet on invoiceFactMas.Id = invoiceFactDet.InvoiceCommFactMasId
            where buyerOrderMas.Id = 1011*/

            ////17/10/2018
            //var FactData = (from exFactoryMas in db.ExFactoryMas
            //                join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
            //                join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
            //                join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
            //                join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
            //                                                        new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
            //                join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
            //                join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
            //                join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
            //                join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
            //                join invoiceFactMas in db.InvoiceCommFactMas on exFactoryMas.ExFactoryDate equals invoiceFactMas.IssueDate
            //                join invoiceFactDet in db.InvoiceCommFactDet on new { ColA = invoiceFactMas.Id, ColB = exfactoryDet.Id }
            //                                equals new { ColA = invoiceFactDet.InvoiceCommFactMasId, ColB = invoiceFactDet.ExFactoryDetId }
            //                join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
            //                from pJoin in joinDest.DefaultIfEmpty()
            //                    //where exFactoryMas.ExFactoryDate == invoiceFactMas.IssueDate && exfactoryDet.Id == invoiceFactDet.ExFactoryDetId
            //                where invoiceFactMas.Id == InvoiceFactMasiD
            //                select new
            //                {
            //                    StyleNo = buyerOrderDet.StyleNo,
            //                    BuyerOrderDetId = shipDet.BuyerOrderDetId,
            //                    BuyersPONo = shipDet.BuyersPONo ?? "",
            //                    DelivSlno = shipDet.DelivSlno,
            //                    ProdSizeId = buyerOrderDet.ProdSizeId == null ? 0 : buyerOrderDet.ProdSizeId,
            //                    ProdSizeName = buyerOrderDet.ProdSize.SizeRange == null ? "" : buyerOrderDet.ProdSize.SizeRange,
            //                    ProdColorId = buyerOrderDet.ProdColorId == null ? 0 : buyerOrderDet.ProdColorId,
            //                    ProdColorName = buyerOrderDet.ProdColor.Name == null ? "" : buyerOrderDet.ProdColor.Name,
            //                    DestinationPortId = shipDet.DestinationPortId == null ? 0 : shipDet.DestinationPortId,
            //                    DestinationPortName = pJoin == null ? "" : pJoin.Name,
            //                    OrderQuantity = shipDet.DelivQuantity,
            //                    ShipQuantity = exfactoryShipDet.ShipQuantity,
            //                    //FactoryFOB = (factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice),
            //                    FactoryFOB = factoryDetDet.FactFOB,
            //                    FactoryOrderDelivDetId = factoryDetDet.Id,
            //                    DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
            //                    //DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * ((factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice) * exfactoryShipDet.ShipQuantity)),
            //                    DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * (factoryDetDet.FactFOB * exfactoryShipDet.ShipQuantity)),
            //                    PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
            //                }).ToList();


            //calculate factory invoice value including Discount now and previous
            //var totalFactInvoiceValue = (decimal)0;
            //foreach (var item in FactData)
            //{
            //    var currentFOB = (decimal)0;
            //    var currentValue = (decimal)0;

            //    if (item.DiscountValue != 0 || item.PreviousDiscountAdj != 0)
            //    {
            //        var FactoryValue = item.ShipQuantity * (item.FactoryFOB ?? 0);
            //        var currentDiscount = item.DiscountValue ?? 0;
            //        var previousDiscount = item.PreviousDiscountAdj == 0 ? 0 : item.PreviousDiscountAdj;
            //        //currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (decimal)item.ShipQuantity) ; //problem
            //        currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (decimal)item.ShipQuantity ?? 0);
            //        currentValue = currentFOB * item.ShipQuantity;
            //    }
            //    else
            //    {
            //        currentValue = item.ShipQuantity * (item.FactoryFOB ?? (decimal)0);
            //    }
            //    totalFactInvoiceValue = totalFactInvoiceValue + currentValue;

            //}



            var FactData = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMasiD).Sum(x => x.InvoiceTotalAmt);

            var data = new
            {
                Quantity = Quantity,
                RDLValue = RDLValue,
                //FactInvoiceVal = totalFactInvoiceValue
                FactInvoiceVal = FactData

            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        //with RDl Dropdown(Tazbirul 30 Aug,2018)
        //public JsonResult InvoiceRDLFactory(int BuyerOrderMasId, int BuyerInfoId, int FactoryId, int LCMasId, int InvoiceFactMasiD)
        //{

        //    var Quantity = (from buyerOrderMas in db.BuyerOrderMas
        //                    join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
        //                    join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
        //                    where buyerOrderMas.Id == BuyerOrderMasId && buyerOrderMas.BuyerInfoId == BuyerInfoId && buyerOrderDet.SupplierId == FactoryId
        //                    && lcDet.MasterLCInfoMasId == LCMasId
        //                    select buyerOrderDet.Quantity).Sum();

        //    var RDLValue = (from buyerOrderMas in db.BuyerOrderMas
        //                    join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
        //                    join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
        //                    join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
        //                    join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
        //                    join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id } equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
        //                    join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
        //                            equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
        //                    join invoiceFactDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceFactDet.ExFactoryDetId
        //                    where buyerOrderMas.Id == BuyerOrderMasId && buyerOrderMas.BuyerInfoId == BuyerInfoId && buyerOrderDet.SupplierId == FactoryId
        //                    && lcDet.MasterLCInfoMasId == LCMasId && invoiceFactMas.Id == InvoiceFactMasiD
        //                    select new { RDlValue = exFactShip.ShipQuantity * shipDet.RdlFOB }).Sum(x => x.RDlValue);


        //    /*select* from  ExFactoryMas exFactoryMas
        //    inner join  ExFactoryDet exfactoryDet on exFactoryMas.Id = exfactoryDet.ExFactoryMasId
        //    inner join  ExFactoryShipDet exfactoryShipDet on exfactoryDet.Id = exfactoryShipDet.ExFactoryDetId
        //    inner join  BuyerOrderMas buyerOrderMas on exfactoryDet.BuyerOrderMasId = buyerOrderMas.Id
        //    inner join  BuyerOrderDet buyerOrderDet on buyerOrderMas.Id = buyerOrderDet.BuyerOrderMasId
        //                and exfactoryShipDet.BuyerOrderDetId = buyerOrderDet.Id
        //    inner join  FactoryOrderDet factoryOrderDet on buyerOrderDet.Id = factoryOrderDet.BuyerOrderDetId
        //    inner join  ShipmentSummDet shipDet on buyerOrderDet.Id = shipDet.BuyerOrderDetId and exfactoryShipDet.ShipmentSummDetId = shipDet.Id
        //    inner join  FactoryOrderDelivDet factoryDetDet on factoryOrderDet.Id = factoryDetDet.FactoryOrderDetId and shipDet.Id = factoryDetDet.ShipmentSummDetId
        //    inner join  MasterLCInfoDet lcDet on buyerOrderMas.Id = lcDet.BuyerOrderMasId
        //    inner join  InvoiceCommFactMas invoiceFactMas on exFactoryMas.ExFactoryDate = invoiceFactMas.IssueDate
        //    inner join  InvoiceCommFactDet invoiceFactDet on invoiceFactMas.Id = invoiceFactDet.InvoiceCommFactMasId
        //    where buyerOrderMas.Id = 1011*/

        //    var FactData = (from exFactoryMas in db.ExFactoryMas
        //                    join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                    join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                    join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                    join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                            new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                    join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                    join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                    join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
        //                    join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
        //                    join invoiceFactMas in db.InvoiceCommFactMas on exFactoryMas.ExFactoryDate equals invoiceFactMas.IssueDate
        //                    join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
        //                    join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                    from pJoin in joinDest.DefaultIfEmpty()
        //                    where buyerOrderMas.Id == BuyerOrderMasId && exFactoryMas.ExFactoryDate == invoiceFactMas.IssueDate && exfactoryDet.Id == invoiceFactDet.ExFactoryDetId
        //                    select new
        //                    {
        //                        StyleNo = buyerOrderDet.StyleNo,
        //                        BuyerOrderDetId = shipDet.BuyerOrderDetId,
        //                        BuyersPONo = shipDet.BuyersPONo ?? "",
        //                        DelivSlno = shipDet.DelivSlno,
        //                        ProdSizeId = buyerOrderDet.ProdSizeId == null ? 0 : buyerOrderDet.ProdSizeId,
        //                        ProdSizeName = buyerOrderDet.ProdSize.SizeRange == null ? "" : buyerOrderDet.ProdSize.SizeRange,
        //                        ProdColorId = buyerOrderDet.ProdColorId == null ? 0 : buyerOrderDet.ProdColorId,
        //                        ProdColorName = buyerOrderDet.ProdColor.Name == null ? "" : buyerOrderDet.ProdColor.Name,
        //                        DestinationPortId = shipDet.DestinationPortId == null ? 0 : shipDet.DestinationPortId,
        //                        DestinationPortName = pJoin == null ? "" : pJoin.Name,
        //                        OrderQuantity = shipDet.DelivQuantity,
        //                        ShipQuantity = exfactoryShipDet.ShipQuantity,
        //                        FactoryFOB = (factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice),
        //                        FactoryOrderDelivDetId = factoryDetDet.Id,
        //                        DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
        //                        DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * ((factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice) * exfactoryShipDet.ShipQuantity)),
        //                        PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
        //                    }).ToList();


        //    //calculate factory invoice value including Discount now and previous
        //    var totalFactInvoiceValue = (decimal)0;
        //    foreach (var item in FactData)
        //    {
        //        var currentFOB = (decimal)0;
        //        var currentValue = (decimal)0;

        //        if (item.DiscountValue != 0 || item.PreviousDiscountAdj != 0)
        //        {
        //            var FactoryValue = item.ShipQuantity * (item.FactoryFOB ?? 0);
        //            var currentDiscount = item.DiscountValue ?? 0;
        //            var previousDiscount = item.PreviousDiscountAdj == 0 ? 0 : item.PreviousDiscountAdj;
        //            currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (decimal)item.ShipQuantity);
        //            currentValue = currentFOB * item.ShipQuantity;
        //        }
        //        else
        //        {
        //            currentValue = item.ShipQuantity * (item.FactoryFOB ?? (decimal)0);
        //        }
        //        totalFactInvoiceValue = totalFactInvoiceValue + currentValue;

        //    }


        //    var data = new
        //    {
        //        Quantity = Quantity,
        //        RDLValue = RDLValue,
        //        FactInvoiceVal = totalFactInvoiceValue

        //    };

        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}




        public JsonResult GetLCFactory(int MasterLCInfoMasId, int BuyerInfoId)
        {
            /*select supplier.Id from [dbo].[BuyerInfo] buyInfo
                inner join [dbo].[MasterLCInfoMas] lcMas on buyInfo.Id = lcMas.BuyerInfoId
                inner join [dbo].[BuyerOrderMas] buyMas on buyInfo.Id = buyMas.BuyerInfoId
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[Supplier] supplier on buyDet.SupplierId = supplier.Id*/


            //Update 21 May, 18 

            /*select * from [dbo].[MasterLCInfoMas] lcMas 
                inner join [dbo].[BuyerOrderMas] buyMas on lcMas.BuyerInfoId = buyMas.BuyerInfoId
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[MasterLCInfoDet] lcDet on lcMas.Id = lcDet.MasterLCInfoMasId and buyMas.Id = lcDet.BuyerOrderMasId
				where lcMas.Id = 5 and lcMas.BuyerInfoId = 1*/

            //var data = (from lcMas in db.MasterLCInfoMas
            //            join buyMas in db.BuyerOrderMas on lcMas.BuyerInfoId equals buyMas.BuyerInfoId
            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //            join lcDet in db.MasterLCInfoDet on new { a = lcMas.Id, b = buyMas.Id } equals new { a = lcDet.MasterLCInfoMasId, b = lcDet.BuyerOrderMasId }
            //            where lcMas.Id == MasterLCInfoMasId && lcMas.BuyerInfoId == BuyerInfoId
            //            select new { Name = buyDet.Supplier.Name, Id = buyDet.SupplierId }).Distinct().ToList();

            var data = (from masterLCdet in db.MasterLCInfoDet
                        join invFactDet in db.InvoiceCommFactDet on masterLCdet.BuyerOrderMasId equals invFactDet.ExFactoryDet.BuyerOrderMasId
                        where masterLCdet.MasterLCInfoMasId == MasterLCInfoMasId && masterLCdet.MasterLCInfoMas.BuyerInfoId == BuyerInfoId
                        select new { Name = invFactDet.ExFactoryDet.ExFactoryMas.Supplier.Name, Id = invFactDet.ExFactoryDet.ExFactoryMas.SupplierId }).Distinct().ToList();





            //var data = new
            //{
            //    TotQnty = totalQty,
            //    TotFactInvoiceVal = FactoryInvValue,
            //    TotRDLValue = RDLValue

            //};

            //return Json(data, JsonRequestBehavior.AllowGet);


            return Json(data, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetLCFactOrder(int MasLCId, int FactId, int Buyer)
        {
            /*select * from [dbo].[BuyerOrderMas] buyMas
                        inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                        inner join [dbo].[MasterLCInfoDet] lcDet on buyMas.Id = lcDet.BuyerOrderMasId
                        inner join [dbo].[InvoiceCommFactMas] invoiceCommFactMas on 
                        buyMas.BuyerInfoId=invoiceCommFactMas.BuyerInfoId and 
                        buyDet.SupplierId= invoiceCommFactMas.SupplierId
						 where buyMas.BuyerInfoId=3
                        and lcDet.MasterLCInfoMasId=1006
                        and buyDet.SupplierId=1*/

            //var data = (from buyInfo in db.BuyerInfo
            //            join lcMas in db.MasterLCInfoMas on buyInfo.Id equals lcMas.BuyerInfoId
            //            join buyMas in db.BuyerOrderMas on buyInfo.Id equals buyMas.BuyerInfoId
            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //            join supplier in db.Supplier on buyDet.SupplierId equals supplier.Id
            //            join invoiceCommFact in db.InvoiceCommFactMas on new { ColA = buyMas.BuyerInfoId, ColB = buyDet.SupplierId }
            //            equals new { ColA = invoiceCommFact.BuyerInfoId, ColB = invoiceCommFact.SupplierId }
            //            where lcMas.Id == MasLCId && supplier.Id == FactId && buyMas.BuyerInfoId == Buyer
            //            select new { Name = invoiceCommFact.InvoiceNoFact, Id = invoiceCommFact.Id }).Distinct().ToList();

            var data = (from buyMas in db.BuyerOrderMas
                        join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                        join lcDet in db.MasterLCInfoDet on buyMas.Id equals lcDet.BuyerOrderMasId
                        join exfactDet in db.ExFactoryDet on buyMas.Id equals exfactDet.BuyerOrderMasId
                        join invoiceDet in db.InvoiceCommFactDet on exfactDet.Id equals invoiceDet.ExFactoryDetId
                        where lcDet.MasterLCInfoMasId == MasLCId && buyDet.SupplierId == FactId && buyMas.BuyerInfoId == Buyer
                        select new { Name = invoiceDet.InvoiceCommFactMas.InvoiceNoFact, Id = invoiceDet.InvoiceCommFactMas.Id }).Distinct().ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetRDLRefNo(int MasLCId, int FactId, int Buyer, int FactoryInvoiceId)
        {
            //var data = (from buyMas in db.BuyerOrderMas
            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //            join lcDet in db.MasterLCInfoDet on buyMas.Id equals lcDet.BuyerOrderMasId
            //            join invoiceCommFact in db.InvoiceCommFactMas on new { ColA = buyMas.BuyerInfoId, ColB = buyDet.SupplierId }
            //            equals new { ColA = invoiceCommFact.BuyerInfoId, ColB = invoiceCommFact.SupplierId }
            //            where lcDet.MasterLCInfoMasId == MasLCId && buyDet.SupplierId == FactId && buyMas.BuyerInfoId == Buyer && invoiceCommFact.Id == FactoryInvoiceId
            //            select new { Name = buyMas.OrderRefNo, Id = buyMas.Id }).Distinct().ToList();

            var data = (from buyerOrderMas in db.BuyerOrderMas
                        join buyerOrderDet in db.BuyerOrderDet on buyerOrderMas.Id equals buyerOrderDet.BuyerOrderMasId
                        join shipDet in db.ShipmentSummDet on buyerOrderDet.Id equals shipDet.BuyerOrderDetId
                        join lcDet in db.MasterLCInfoDet on buyerOrderMas.Id equals lcDet.BuyerOrderMasId
                        join exfactDet in db.ExFactoryDet on buyerOrderMas.Id equals exfactDet.BuyerOrderMasId
                        join exFactShip in db.ExFactoryShipDet on new { ColA = exfactDet.Id, ColB = shipDet.Id } equals new { ColA = exFactShip.ExFactoryDetId, ColB = exFactShip.ShipmentSummDetId }
                        join invoiceFactMas in db.InvoiceCommFactMas on new { ColA = exfactDet.ExFactoryMas.ExFactoryDate, ColB = buyerOrderDet.SupplierId }
                                equals new { ColA = invoiceFactMas.IssueDate, ColB = invoiceFactMas.SupplierId }
                        where lcDet.MasterLCInfoMasId == MasLCId && buyerOrderDet.SupplierId == FactId && buyerOrderMas.BuyerInfoId == Buyer && invoiceFactMas.Id == FactoryInvoiceId
                        select new { Name = buyerOrderMas.OrderRefNo, Id = buyerOrderMas.Id }).Distinct().ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }



        //public JsonResult GetDelivData(int BuyerId, int SupplierId, int InvMasId)

        //{

        //    var list = (from exFactoryMas in db.ExFactoryMas
        //                join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                        new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                from pJoin in joinDest.DefaultIfEmpty()
        //                    //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
        //                join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }

        //                join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
        //                equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
        //                where buyerOrderMas.BuyerInfoId == BuyerId && buyerOrderDet.SupplierId == SupplierId && invCommFactMas.Id == InvMasId
        //                select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet }).AsEnumerable()
        //               .Select(x => new
        //               {
        //                   //Id = x.shipDet.Id,
        //                   //OrderMasId=x.buyerOrderDet.BuyerOrderMasId,
        //                   OrderDetId = x.buyerOrderDet.Id,
        //                   RDLRefNo = x.buyerOrderDet.BuyerOrderMas.OrderRefNo,
        //                   StyleNo = x.buyerOrderDet.StyleNo,
        //                   BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
        //                   BuyersPONo = x.shipDet.BuyersPONo ?? "",
        //                   //OrderQuantity = x.buyerOrderDet.Quantity,
        //                   ShipQuantity = x.exfactoryShipDet.ShipQuantity,
        //                   RDLUnitPrice = x.buyerOrderDet.UnitPrice,
        //                   //FactoryTransferPrice = x.factoryOrderDet.TransferPrice == null ? x.factoryOrderDet.FOBUnitPrice : x.factoryOrderDet.TransferPrice
        //                   FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
        //                   InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
        //                   SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId
        //               });


        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}



        public JsonResult GetDelivData(int BuyerId, int SupplierId, int InvoiceMasId)

        {

            var list = (from exFactoryMas in db.ExFactoryMas
                        join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                        join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                        join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                        join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                        join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                        join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                        join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                        from pJoin in joinDest.DefaultIfEmpty()
                            //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
                        join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                        equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                        join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                        equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                        where invCommFactMas.Id == InvoiceMasId
                        select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet, factoryDetDet }).AsEnumerable()
                       .Select(x => new
                       {
                           //Id = x.shipDet.Id,
                           //OrderMasId=x.buyerOrderDet.BuyerOrderMasId,
                           ExFactoryShipDetId = x.exfactoryShipDet.Id,
                           OrderDetId = x.buyerOrderDet.Id,
                           RDLRefNo = x.buyerOrderDet.BuyerOrderMas.OrderRefNo,
                           StyleNo = x.buyerOrderDet.StyleNo,
                           BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
                           BuyersPONo = x.shipDet.BuyersPONo ?? "",
                           //OrderQuantity = x.buyerOrderDet.Quantity,
                           ShipQuantity = x.exfactoryShipDet.ShipQuantity,
                           PreviousSplitQty = db.InvoiceCommDetDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.InvoiceCommDetDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Sum(m => m.ShipQty)),
                           RDLUnitPrice = x.buyerOrderDet.UnitPrice,
                           //FactoryTransferPrice = x.factoryOrderDet.TransferPrice == null ? x.factoryOrderDet.FOBUnitPrice : x.factoryOrderDet.TransferPrice
                           FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
                           InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
                           SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId,
                           DiscountPerc = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount),
                           DiscountValue = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount) / 100 * ((x.shipDet.RdlFOB) * x.exfactoryShipDet.ShipQuantity)),
                           PreviousDiscountAdj = db.DiscountAdjustmentBuyerDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentBuyerDet.FirstOrDefault(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).AdjustmentAmt),
                           CheckPrevDiscount = db.InvoiceCommDetDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : 1

                       });


            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetDelivDataEdit(int BuyerId, int SupplierId, int InvoiceCommDetId)

        {

            var list = (from exFactoryMas in db.ExFactoryMas
                        join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                        join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                        join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                        join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                        join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                        join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                        join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                        from pJoin in joinDest.DefaultIfEmpty()
                            //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
                        join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                        equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                        join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                        equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                        join invoiceCommDet in db.InvoiceCommDet on invCommFactMas.Id equals invoiceCommDet.InvoiceCommFactMasId
                        join invoiceCommDetDetDet in db.InvoiceCommDetDet on new { a = invoiceCommDet.Id, b = exfactoryShipDet.Id }
                          equals new { a = invoiceCommDetDetDet.InvoiceCommDetId, b = invoiceCommDetDetDet.ExFactoryShipDetId }
                        where invoiceCommDet.Id == InvoiceCommDetId
                        select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet, factoryDetDet, invoiceCommDetDetDet }).AsEnumerable()
                       .Select(x => new
                       {
                           InvoiceCommDetDetId = x.invoiceCommDetDetDet.Id,
                           //OrderMasId=x.buyerOrderDet.BuyerOrderMasId,
                           ExFactoryShipDetId = x.exfactoryShipDet.Id,
                           OrderDetId = x.buyerOrderDet.Id,
                           RDLRefNo = x.buyerOrderDet.BuyerOrderMas.OrderRefNo,
                           StyleNo = x.buyerOrderDet.StyleNo,
                           BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
                           BuyersPONo = x.shipDet.BuyersPONo ?? "",
                           //OrderQuantity = x.buyerOrderDet.Quantity,
                           ShipQuantity = x.exfactoryShipDet.ShipQuantity,
                           PreviousSplitQty = db.InvoiceCommDetDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id && m.Id !=x.invoiceCommDetDetDet.Id).Count() == 0 ? 0 : (db.InvoiceCommDetDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id && m.Id != x.invoiceCommDetDetDet.Id).Sum(m => m.ShipQty)),
                           RDLUnitPrice = x.buyerOrderDet.UnitPrice,
                           SplitQty = db.InvoiceCommDetDet.Where(m => m.Id == x.invoiceCommDetDetDet.Id).Count() == 0 ? 0 : (db.InvoiceCommDetDet.FirstOrDefault(m => m.Id == x.invoiceCommDetDetDet.Id).ShipQty),
                           //FactoryTransferPrice = x.factoryOrderDet.TransferPrice == null ? x.factoryOrderDet.FOBUnitPrice : x.factoryOrderDet.TransferPrice
                           FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
                           InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
                           SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId,
                           DiscountPerc = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount),
                           DiscountValue = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount) / 100 * ((x.shipDet.RdlFOB) * x.exfactoryShipDet.ShipQuantity)),
                           PreviousDiscountAdj = db.DiscountAdjustmentBuyerDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentBuyerDet.FirstOrDefault(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).AdjustmentAmt),
                           DiscountFlag = x.invoiceCommDetDetDet.DiscountFlag
                       }).Distinct().ToList();


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //--------------------with RDL Ref no drop down(30 Aug, 2018)
        //public JsonResult GetDelivData(int BuyerId, int SupplierId, int BuyerOrderMasId,int InvoiceMasId)

        //{

        //    var list = (from exFactoryMas in db.ExFactoryMas
        //                join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
        //                join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
        //                join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
        //                join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
        //                                                        new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
        //                join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
        //                join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
        //                join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id} equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ??0}
        //                join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
        //                from pJoin in joinDest.DefaultIfEmpty()
        //                    //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
        //                join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
        //                equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
        //                join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
        //                equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
        //                where buyerOrderMas.BuyerInfoId == BuyerId && buyerOrderDet.SupplierId == SupplierId && buyerOrderMas.Id == BuyerOrderMasId
        //                select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet, factoryDetDet }).AsEnumerable()
        //               .Select(x => new
        //               {
        //                   //Id = x.shipDet.Id,
        //                   //OrderMasId=x.buyerOrderDet.BuyerOrderMasId,
        //                   OrderDetId = x.buyerOrderDet.Id,
        //                   RDLRefNo = x.buyerOrderDet.BuyerOrderMas.OrderRefNo,
        //                   StyleNo = x.buyerOrderDet.StyleNo,
        //                   BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
        //                   BuyersPONo = x.shipDet.BuyersPONo ?? "",
        //                   //OrderQuantity = x.buyerOrderDet.Quantity,
        //                   ShipQuantity = x.exfactoryShipDet.ShipQuantity,
        //                   RDLUnitPrice = x.buyerOrderDet.UnitPrice,
        //                   //FactoryTransferPrice = x.factoryOrderDet.TransferPrice == null ? x.factoryOrderDet.FOBUnitPrice : x.factoryOrderDet.TransferPrice
        //                   FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
        //                   InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
        //                   SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId,
        //                   DiscountPerc = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount),
        //                   DiscountValue = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount) / 100 * ((x.shipDet.RdlFOB) * x.exfactoryShipDet.ShipQuantity)),
        //                   PreviousDiscountAdj = db.DiscountAdjustmentBuyerDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentBuyerDet.FirstOrDefault(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).AdjustmentAmt)
        //               });


        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}



        // GET: InvoiceCommFact/Create
        public ActionResult Create()
        {
            var buyerUnderLC = (from lc in db.MasterLCInfoMas
                                join buyInfo in db.BuyerInfo on lc.BuyerInfoId equals buyInfo.Id
                                where lc.BuyerInfoId == buyInfo.Id
                                select buyInfo).Distinct().ToList();

            ViewBag.BuyerInfoId = new SelectList(buyerUnderLC, "Id", "Name");
            //ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo");
            ViewBag.MasterLCInfoMasId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "LCNo");
            var PaymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text", 0);
            ViewBag.PaymentTypeId = PaymentType;

            ViewBag.SplitType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "No Split", Value = "0" }, new SelectListItem { Text = "Split", Value = "1" }, }, "Value", "Text", 0);

            return View();
        }


        public JsonResult GetNames(int Id)
        {
            var data = db.MasterLCInfoMas.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.LCNo).Select(y => new { Name = y.LCNo, Id = y.Id }).Distinct().ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }



        public JsonResult SaveRDLInvoice(IEnumerable<VMInvoiceCommDet> InvoiceDetails, IEnumerable<VMInvoiceCommDetDet> DelivDetails, InvoiceCommMas InvoiceMas)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
                Id = 0
            };

            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        InvoiceMas.OpBy = 1;
                        InvoiceMas.OpOn = OpDate;
                        InvoiceMas.IsAuth = true;
                        //InvoiceMas.IsLocked = false;

                        db.InvoiceCommMas.Add(InvoiceMas);
                        db.SaveChanges();


                        Dictionary<int, int> dictionary =
                                  new Dictionary<int, int>();


                        foreach (var item in InvoiceDetails)
                        {


                            //var list = (from exFactoryMas in db.ExFactoryMas
                            //            join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                            //            join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                            //            join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                            //            join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                            //                                                    new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                            //            join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                            //            join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                            //            join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                            //            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                            //            from pJoin in joinDest.DefaultIfEmpty()
                            //                //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
                            //            join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                            //            equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                            //            join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                            //            equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                            //            where invCommFactMas.Id == item.InvoiceCommFactMasId
                            //            select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet, factoryDetDet }).AsEnumerable()
                            //            .Select(x => new
                            //            {
                            //                ShipQuantity = x.exfactoryShipDet.ShipQuantity,
                            //                RDLUnitPrice = x.buyerOrderDet.UnitPrice,
                            //                FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
                            //                InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
                            //                SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId,
                            //                DiscountPerc = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount),
                            //                DiscountValue = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount) / 100 * ((x.shipDet.RdlFOB) * x.exfactoryShipDet.ShipQuantity)),
                            //                PreviousDiscountAdj = db.DiscountAdjustmentBuyerDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentBuyerDet.FirstOrDefault(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).AdjustmentAmt)
                            //            });


                            decimal invoiceDetDiscountval = 0;

                            //foreach (var i in list)
                            //{
                            //    decimal currentFOB = 0;
                            //    decimal currentValue = 0;

                            //    if (i.DiscountValue != 0 || i.PreviousDiscountAdj != 0)
                            //    {
                            //        var FactoryValue = Convert.ToDecimal(i.ShipQuantity * i.RDLUnitPrice);
                            //        var currentDiscount = Convert.ToDecimal(i.DiscountValue);
                            //        var previousDiscount = Convert.ToDecimal(i.PreviousDiscountAdj);
                            //        currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (i.ShipQuantity));
                            //        currentValue = (currentFOB * i.ShipQuantity);
                            //    }
                            //    else
                            //    {
                            //        currentValue = i.ShipQuantity * (i.RDLUnitPrice ?? 0);
                            //    }

                            //    invoiceDetDiscountval = invoiceDetDiscountval + currentValue;
                            //}
                            if (DelivDetails != null)
                            {
                                foreach (var deliv in DelivDetails)
                                {

                                    if (deliv.DelivOrderDetTempId == item.TempOrderDetId)
                                    {
                                        invoiceDetDiscountval = invoiceDetDiscountval + deliv.CurrentValue;
                                    }
                                }

                            }


                            var OrderD = new InvoiceCommDet()
                            {
                                Id = 0,
                                InvoiceCommMasId = InvoiceMas.Id,
                                InvoiceCommFactMasId = item.InvoiceCommFactMasId,
                                InvoiceRDLTotalAmt = invoiceDetDiscountval
                            };

                            db.InvoiceCommDet.Add(OrderD);
                            db.SaveChanges();

                            dictionary.Add(item.TempOrderDetId, OrderD.Id);
                        }

                        //---- shipment data

                        //    var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                var deliv = new InvoiceCommDetDet()
                                {
                                    Id = item.Id,
                                    InvoiceCommDetId = dictionary[item.DelivOrderDetTempId],
                                    ExFactoryShipDetId = item.ExFactoryShipDetId,
                                    ShipQty = item.ShipQty,
                                    DiscountFlag = item.DiscountFlag
                                };

                                //db.Entry(deliv).State = deliv.Id == 0 ?
                                //                            EntityState.Added :
                                //                            EntityState.Modified;

                                db.InvoiceCommDetDet.Add(deliv);
                                db.SaveChanges();

                            }

                        }




                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = InvoiceMas.Id
                        };

                        //Success("Record saved successfully.", true);


                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message,
                            Id = 0
                        };
                    }
                }

            }
            catch (Exception ex)
            {

                result = new
                {
                    flag = false,
                    message = ex.Message,
                    Id = 0
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateInvoiceRDL(IEnumerable<VMInvoiceCommDet> InvoiceDetails, IEnumerable<VMInvoiceCommDetDet> DelivDetails, InvoiceCommMas InvoiceMas, int[] DelItems)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            try
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var InvoiceCommMasRDL = db.InvoiceCommMas.Find(InvoiceMas.Id);

                        if (InvoiceCommMasRDL == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invoice not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        InvoiceCommMasRDL.IssueDate = InvoiceMas.IssueDate;
                        InvoiceCommMasRDL.InvoiceNo = InvoiceMas.InvoiceNo;

                        db.Entry(InvoiceCommMasRDL).State = EntityState.Modified;

                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                    new Dictionary<int, int>();


                        foreach (var item in InvoiceDetails)
                        {

                            //var list = (from exFactoryMas in db.ExFactoryMas
                            //            join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                            //            join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                            //            join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                            //            join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                            //                                                    new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                            //            join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                            //            join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                            //            join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                            //            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                            //            from pJoin in joinDest.DefaultIfEmpty()
                            //                //join invCommFactDet in db.InvoiceCommFactDet on exfactoryDet.Id equals invCommFactDet.ExFactoryDetId
                            //            join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                            //            equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                            //            join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                            //            equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                            //            where invCommFactMas.Id == item.InvoiceCommFactMasId
                            //            select new { exfactoryShipDet, buyerOrderDet, shipDet, invCommFactDet, factoryDetDet }).AsEnumerable()
                            //            .Select(x => new
                            //            {
                            //                ShipQuantity = x.exfactoryShipDet.ShipQuantity,
                            //                RDLUnitPrice = x.buyerOrderDet.UnitPrice,
                            //                FactoryInvoiceValue = x.exfactoryShipDet.ShipQuantity * x.buyerOrderDet.UnitPrice,
                            //                InvoiceCommFactMasId = x.invCommFactDet.InvoiceCommFactMasId,
                            //                SupplierId = x.invCommFactDet.InvoiceCommFactMas.SupplierId,
                            //                DiscountPerc = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount),
                            //                DiscountValue = db.DiscountDet.Where(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(m => m.FactoryOrderDelivDetId == x.factoryDetDet.Id && m.AdjustBuyerNow == true).BuyerDiscount) / 100 * ((x.shipDet.RdlFOB) * x.exfactoryShipDet.ShipQuantity)),
                            //                PreviousDiscountAdj = db.DiscountAdjustmentBuyerDet.Where(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentBuyerDet.FirstOrDefault(m => m.ExFactoryShipDetId == x.exfactoryShipDet.Id).AdjustmentAmt)
                            //            });


                            decimal invoiceDetDiscountval = 0;

                            //foreach (var i in list)
                            //{
                            //    decimal currentFOB = 0;
                            //    decimal currentValue = 0;

                            //    if (i.DiscountValue != 0 || i.PreviousDiscountAdj != 0)
                            //    {
                            //        var FactoryValue = Convert.ToDecimal(i.ShipQuantity * i.RDLUnitPrice);
                            //        var currentDiscount = Convert.ToDecimal(i.DiscountValue);
                            //        var previousDiscount = Convert.ToDecimal(i.PreviousDiscountAdj);
                            //        currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (i.ShipQuantity));
                            //        currentValue = (currentFOB * i.ShipQuantity);
                            //    }
                            //    else
                            //    {
                            //        currentValue = i.ShipQuantity * (i.RDLUnitPrice ?? 0);
                            //    }

                            //    invoiceDetDiscountval = invoiceDetDiscountval + currentValue;
                            //}

                            if (DelivDetails != null)
                            {
                                foreach (var deliv in DelivDetails)
                                {

                                    if (deliv.DelivOrderDetTempId == item.TempOrderDetId)
                                    {
                                        invoiceDetDiscountval = invoiceDetDiscountval + deliv.CurrentValue;
                                    }
                                }

                            }

                            if (item.Id == 0) //insert
                            {
                                //item.InvoiceRDLTotalAmt = invoiceDetDiscountval;
                                //item.InvoiceCommMasId = InvoiceCommMasRDL.Id;

                                var OrderD = new InvoiceCommDet()
                                {
                                    Id = 0,
                                    InvoiceCommMasId = InvoiceMas.Id,
                                    InvoiceCommFactMasId = item.InvoiceCommFactMasId,
                                    InvoiceRDLTotalAmt = invoiceDetDiscountval
                                };

                                db.InvoiceCommDet.Add(OrderD);
                                dictionary.Add(item.TempOrderDetId, OrderD.Id);
                            }
                            else //update
                            {
                                var oItem = db.InvoiceCommDet.Find(item.Id);
                                oItem.InvoiceRDLTotalAmt = invoiceDetDiscountval;
                                //  oItem. = item.ExFactoryDetId;
                                db.Entry(oItem).State = EntityState.Modified;

                                dictionary.Add(item.TempOrderDetId, oItem.Id);
                            }

                            db.SaveChanges();

                        }


                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                if (item.Id == 0)
                                {
                                    var deliv = new InvoiceCommDetDet()
                                    {
                                        Id = item.Id,
                                        InvoiceCommDetId = dictionary[item.DelivOrderDetTempId],
                                        ExFactoryShipDetId = item.ExFactoryShipDetId,
                                        ShipQty = item.ShipQty,
                                        DiscountFlag = item.DiscountFlag
                                    };

                                    db.Entry(deliv).State = deliv.Id == 0 ?
                                            EntityState.Added :
                                            EntityState.Modified;
                                }
                                else
                                {
                                    var delivItem = db.InvoiceCommDetDet.Find(item.Id);

                                    if (delivItem != null)
                                    {
                                        delivItem.ShipQty = item.ShipQty;

                                        db.Entry(delivItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }








                                db.SaveChanges();

                            }
                        }


                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                var delOrder = db.InvoiceCommDet.Find(item);

                                var InvoiceDettDet = db.InvoiceCommDetDet.Where(x => x.InvoiceCommDetId == delOrder.Id).ToList();

                                if(InvoiceDettDet!= null)
                                {
                                    db.InvoiceCommDetDet.RemoveRange(InvoiceDettDet);
                                }

                                db.InvoiceCommDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Updating successful !!"
                        };

                        //Success("Updated successfully.", true);


                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        result = new
                        {
                            flag = false,
                            message = ex.Message
                        };
                    }
                }

            }
            catch (Exception ex)
            {

                result = new
                {
                    flag = false,
                    message = ex.Message
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult DeleteInvoice(int id)
        {
            bool flag = false;
            try
            {
                var itemToDeleteMas = db.InvoiceCommMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nCommercial Invoice Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var checkInvoiceFact = db.DocSubmissionDet.Where(x => x.InvoiceCommMasId == id).ToList();

                if (checkInvoiceFact.Count > 0)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\n Delete Document Invoice data first."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }


                var itemsToDeleteDet = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == id);

                foreach (var item in itemsToDeleteDet)
                {
                    var itemsToDeleteDeliv = db.InvoiceCommDetDet.Where(x => x.InvoiceCommDetId == item.Id).ToList();

                    if (itemsToDeleteDeliv.Count() > 0)
                    {
                        db.InvoiceCommDetDet.RemoveRange(itemsToDeleteDeliv);
                    }

                }

                db.InvoiceCommDet.RemoveRange(itemsToDeleteDet);

                db.InvoiceCommMas.Remove(itemToDeleteMas);

                flag = db.SaveChanges() > 0;

            }
            catch (Exception ex)
            {

            }

            if (flag)
            {
                var result = new
                {
                    flag = true,
                    message = "Deletion successful."
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var result = new
                {
                    flag = false,
                    message = "Deletion failed!\nError Occured."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}