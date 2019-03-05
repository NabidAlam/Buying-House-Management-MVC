using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHMS.Helpers;
using System.Net;
using System.Data.Entity;

namespace BHMS.Controllers
{
    public class ProceedRealizationController : Controller
    {
        private ModelBHMS db = new ModelBHMS();
        // GET: ProceedRealization
        public ActionResult Index()
        {
            var proceedMasList = db.ProceedRealizationMas;
            return View(proceedMasList.ToList());
        }

        //--------------------------------------------------------------CREATE-------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------------------------


        public ActionResult Create()
        {
            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text");
            ViewBag.PaymentTypeId = paymentType;
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.DocSubmissionMasId = new SelectList(db.DocSubmissionMas, "Id", "FDBCNo");
            ViewBag.MasterLCInfoId = "MasterLC";
            return View();
        }


        public JsonResult GetBuyerByPayment(int Id)
        {
            var data = (from buyer in db.BuyerInfo
                        where buyer.PaymentTypeId == Id
                        select buyer).Distinct().Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetFDBCByBuyerId(int Id)
        {
            var data = (from docMas in db.DocSubmissionMas
                        where docMas.BuyerInfoId == Id
                        select docMas).Distinct().Select(y => new { Name = y.FDBCNo, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDocSubDataByFDBCNo(int DocSubmissionMasId, int BuyerInfoId, int paymentType)
        {

            /*select * from [dbo].[DocSubmissionMas] docMas 
                    inner join [dbo].[MasterLCInfoMas] lcMas on docMas.MasterLCInfoMasId = lcMas.Id and docMas.BuyerInfoId = lcMas.BuyerInfoId
                    inner join [dbo].[MasterLCInfoDet] lcDet on lcMas.Id = lcDet.MasterLCInfoMasId 
                    inner join [dbo].[DocSubmissionDet] docDet on docMas.Id = docDet.DocSubmissionMasId
                    inner join [dbo].[DocSubmissionFactDet] docDetDetail on docDet.Id = docDetDetail.DocSubmissionDetId
                    inner join [dbo].[InvoiceCommDet] invCommDet on docDetDetail.InvoiceCommDetId = invCommDet.Id
                    inner join [dbo].[InvoiceCommFactDet] invCommFactDet on invCommDet.InvoiceCommFactMasId = invCommFactDet.InvoiceCommFactMasId
                    inner join [dbo].[ExFactoryDet] exFactDet on invCommFactDet.ExFactoryDetId = exFactDet.Id and lcDet.BuyerOrderMasId = exFactDet.BuyerOrderMasId*/

            if (paymentType == 0)
            {
                var data = (from docMas in db.DocSubmissionMas
                            join lcMas in db.MasterLCInfoMas on new { a = docMas.MasterLCInfoMasId ?? 0, b = docMas.BuyerInfoId } equals new { a = lcMas.Id, b = lcMas.BuyerInfoId ?? 0 } into joinLeft
                            from pJoin in joinLeft.DefaultIfEmpty()
                            join lcDet in db.MasterLCInfoDet on pJoin.Id equals lcDet.MasterLCInfoMasId
                            join docDet in db.DocSubmissionDet on docMas.Id equals docDet.DocSubmissionMasId
                            join docDetDetail in db.DocSubmissionFactDet on docDet.Id equals docDetDetail.DocSubmissionDetId
                            join invCommDet in db.InvoiceCommDet on docDetDetail.InvoiceCommDetId equals invCommDet.Id
                            join invCommDetDet in db.InvoiceCommDetDet on invCommDet.Id equals invCommDetDet.InvoiceCommDetId
                            //join exFactDet in db.ExFactoryDet on invCommFactDet.ExFactoryDetId equals exFactDet.Id
                            where docMas.BuyerInfoId == BuyerInfoId && docMas.Id == DocSubmissionMasId
                            select new
                            {
                                //exFactDet,
                                docMas.MasterLCInfoMas,
                                invCommDet.InvoiceCommMas,
                                docDet
                            }).Distinct().ToList();


                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //OrderMasId = x.exFactDet.BuyerOrderMasId,
                    OrderRefNo = x.docDet.InvoiceCommMas.InvoiceNo,
                    //RDlInvoiceData = (from RDlInvoiceMas in db.InvoiceCommMas
                    //                  join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                    //                  join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                    //                  join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                    //                  join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                    //                  join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                    //                  join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                    //                  where RDlInvoiceMas.Id == x.InvoiceCommMas.Id
                    //                  select new
                    //                  {
                    //                      Quantity = exFactoryShipDet.ShipQuantity,
                    //                      RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                    //                  }

                    //   ).Distinct().Sum(m => m.RDLValue),

                    //19/09/2018
                    RDlInvoiceData = (from RDlInvoiceMas in db.InvoiceCommMas
                                      join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                                      join RDLInvoiceDetDet in db.InvoiceCommDetDet on RDLInvoiceDet.Id equals RDLInvoiceDetDet.InvoiceCommDetId
                                      join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                      join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                      join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                      join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                      join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                      where RDlInvoiceMas.Id == x.docDet.InvoiceCommMasId
                                      select new
                                      {
                                          //Quantity = exFactoryShipDet.ShipQuantity,
                                          //RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                          RDLValue = RDLInvoiceDet.InvoiceRDLTotalAmt
                                      }).Distinct().Sum(m => m.RDLValue),

                    DocDetId = x.docDet.Id

                }).Distinct();
                return Json(refinedata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = (from docMas in db.DocSubmissionMas
                            join docDet in db.DocSubmissionDet on docMas.Id equals docDet.DocSubmissionMasId
                            join docDetDetail in db.DocSubmissionFactDet on docDet.Id equals docDetDetail.DocSubmissionDetId
                            //join invCommDet in db.InvoiceCommDet on docDetDetail.InvoiceCommDetId equals invCommDet.Id
                            join invCommFactDet in db.InvoiceCommFactDet on docDetDetail.InvoiceCommFactDet.InvoiceCommFactMasId equals invCommFactDet.InvoiceCommFactMasId
                            join exFactDet in db.ExFactoryDet on invCommFactDet.ExFactoryDetId equals exFactDet.Id
                            where docMas.BuyerInfoId == BuyerInfoId && docMas.Id == DocSubmissionMasId
                            select new
                            {
                                //invCommDet.InvoiceCommMas,
                                invCommFactDet.ExFactoryDet.BuyerOrderMas,
                                docDet
                            }).Distinct().ToList();


                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //OrderMasId = x.exFactDet.BuyerOrderMasId,
                    OrderRefNo = x.BuyerOrderMas.OrderRefNo,
                    RDlInvoiceData = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                      join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                      join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                      join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                      join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                      where buyerDet.BuyerOrderMasId == x.BuyerOrderMas.Id
                                      select new
                                      {
                                          Quantity = exFactoryShipDet.ShipQuantity,
                                          RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                      }

                       ).Distinct().Sum(m => m.RDLValue),
                    DocDetId = x.docDet.Id

                }).Distinct();
                return Json(refinedata, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult GetMasterLCByFDBCNo(int DocSubmissionMasId, int BuyerInfoId)
        {
            var lcNo = (from docMas in db.DocSubmissionMas
                        where docMas.BuyerInfoId == BuyerInfoId && docMas.Id == DocSubmissionMasId
                        select docMas.MasterLCInfoMas.LCNo).Distinct();
            var data = new
            {
                MasterLCNo = lcNo,
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadFactoryInvoiceDetails(int Id, int paymentType)
        {
            if (paymentType == 0)
            {
                var RDLinvoicData = (from docSubDetailDet in db.DocSubmissionFactDet
                                     join docSubmissionDet in db.DocSubmissionDet on docSubDetailDet.DocSubmissionDetId equals docSubmissionDet.Id
                                     //join RDLInvoiceMas in db.InvoiceCommMas on docSubmissionDet.InvoiceCommMasId equals RDLInvoiceMas.Id
                                     join RDLInvoiceDet in db.InvoiceCommDet on docSubDetailDet.InvoiceCommDetId equals RDLInvoiceDet.Id                               
                                     join RDLInvoiceDetDet in db.InvoiceCommDetDet on RDLInvoiceDet.Id equals RDLInvoiceDetDet.InvoiceCommDetId
                                     join InvoiceFactMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
                                     join InvoiceFactDet in db.InvoiceCommFactDet on InvoiceFactMas.Id equals InvoiceFactDet.InvoiceCommFactMasId

                                     // join ex_factoryShipDet in db.ExFactoryShipDet on InvoiceFactDet. equals ex_factoryShipDet.Id
                                     join exfactoryDet in db.ExFactoryDet on InvoiceFactDet.ExFactoryDetId equals exfactoryDet.Id
                                     join exFactoryMas in db.ExFactoryMas on exfactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                     join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                     //join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = ex_factoryShipDet.BuyerOrderDetId } equals new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                     join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id } equals new { ColA = buyerOrderDet.BuyerOrderMasId }
                                     join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                     join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                                     //join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = ex_factoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                     join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                     from pJoin in joinDest.DefaultIfEmpty()
                                     where docSubmissionDet.Id == Id
                                     select new
                                     {
                                         DocSubDetailDetId = docSubDetailDet.Id,
                                         DocSubDetId = docSubmissionDet.Id,
                                         InvoiceCommDetId = RDLInvoiceDet.Id,
                                         FactoryId = InvoiceFactMas.SupplierId,
                                         FactoryName = InvoiceFactMas.Supplier.Name,
                                         FactoryInvoiceId = InvoiceFactMas.Id,
                                         FactoryInvoiceNo = InvoiceFactMas.InvoiceNoFact,
                                         FDBCNo = docSubDetailDet.FactFDBCNo ?? "",
                                         //ShipQty = (from invoiceMas in db.InvoiceCommMas
                                         //           join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                                         //           join invoiceFactMas in db.InvoiceCommFactMas on invoiceDet.InvoiceCommFactMasId equals invoiceFactMas.Id
                                         //           join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                         //           join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                         //           join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                         //           //where invoiceMas.Id == RDLInvoiceMas.Id
                                         //           where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                         //           select exShipDet.ShipQuantity).Sum(),
                                         ShipQty = db.InvoiceCommDetDet.Where(x => x.InvoiceCommDetId == RDLInvoiceDet.Id).Sum(x => x.ShipQty),

                                         //FactoryInvoiceValue = (from invoiceMas1 in db.InvoiceCommMas
                                         //                       join invoiceDet1 in db.InvoiceCommDet on invoiceMas1.Id equals invoiceDet1.InvoiceCommMasId
                                         //                       join invoiceFactMas1 in db.InvoiceCommFactMas on invoiceDet1.InvoiceCommFactMasId equals invoiceFactMas1.Id
                                         //                       join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                         //                       join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                         //                       join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                         //                       join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                         //                       //where invoiceMas.Id == RDLInvoiceMas.Id
                                         //                       where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                         //                       select new { FactoryInvoice = exShipDet1.ShipQuantity * factDet.FOBUnitPrice }).Sum(x => x.FactoryInvoice),

                                         //// 17/9/2018
                                         //FactoryInvoiceValue = (from invoiceMas1 in db.InvoiceCommMas
                                         //                       join invoiceDet1 in db.InvoiceCommDet on invoiceMas1.Id equals invoiceDet1.InvoiceCommMasId
                                         //                       join invoiceFactMas1 in db.InvoiceCommFactMas on invoiceDet1.InvoiceCommFactMasId equals invoiceFactMas1.Id
                                         //                       join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                         //                       join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                         //                       join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                         //                       join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                         //                       join factoryDelivDet in db.FactoryOrderDelivDet on new { ColA = factDet.Id } equals new { ColA = factoryDelivDet.FactoryOrderDetId }
                                         //                       //where invoiceMas.Id == RDLInvoiceMas.Id
                                         //                       where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                         //                       select new { FactoryInvoice = exShipDet1.ShipQuantity * factoryDelivDet.FactFOB }).Sum(x => x.FactoryInvoice),

                                         //FactoryInvoiceValue = InvoiceFactDet.InvoiceTotalAmt,
                                         FactoryInvoiceValue = db.InvoiceCommFactDet.Where(x=>x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x=>x.InvoiceTotalAmt),

                                         //RDLPrice = (from invoiceMas in db.InvoiceCommMas
                                         //            join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                                         //            join invoiceFactMas in db.InvoiceCommFactMas on invoiceDet.InvoiceCommFactMasId equals invoiceFactMas.Id
                                         //            join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                         //            join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                         //            join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                         //            join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                         //            join buyerDet in db.BuyerOrderDet on factDet.BuyerOrderDetId equals buyerDet.Id
                                         //            where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                         //            select new { FactoryInvoice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Sum(x => x.FactoryInvoice),

                                         RDLPrice = RDLInvoiceDet.InvoiceRDLTotalAmt,
  
                                         FDBCDetNo = docSubDetailDet.FactFDBCNo ?? ""

                                     }).Distinct().ToList();


                return Json(RDLinvoicData, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var FactinvoicData = (from docSubDetailDet in db.DocSubmissionFactDet
                                      join docSubmissionDet in db.DocSubmissionDet on docSubDetailDet.DocSubmissionDetId equals docSubmissionDet.Id
                                      //join RDLInvoiceMas in db.InvoiceCommMas on docSubmissionDet.InvoiceCommMasId equals RDLInvoiceMas.Id
                                      //join RDLInvoiceDet in db.InvoiceCommDet on docSubDetailDet.InvoiceCommDetId equals RDLInvoiceDet.Id
                                      //join InvoiceFactMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
                                      join InvoiceFactDet in db.InvoiceCommFactDet on docSubDetailDet.InvoiceCommFactDetId equals InvoiceFactDet.Id
                                      // join ex_factoryShipDet in db.ExFactoryShipDet on InvoiceFactDet. equals ex_factoryShipDet.Id
                                      join exfactoryDet in db.ExFactoryDet on InvoiceFactDet.ExFactoryDetId equals exfactoryDet.Id
                                      join exFactoryMas in db.ExFactoryMas on exfactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                      join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                      //join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = ex_factoryShipDet.BuyerOrderDetId } equals new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                      join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id } equals new { ColA = buyerOrderDet.BuyerOrderMasId }
                                      join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                      join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                                      //join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = ex_factoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                      join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                      from pJoin in joinDest.DefaultIfEmpty()

                                      where docSubmissionDet.Id == Id
                                      select new
                                      {
                                          DocSubDetailDetId = docSubDetailDet.Id,
                                          DocSubDetId = docSubmissionDet.Id,
                                          FactoryId = InvoiceFactDet.InvoiceCommFactMas.SupplierId,
                                          FactoryName = InvoiceFactDet.InvoiceCommFactMas.Supplier.Name,
                                          FactoryInvoiceId = InvoiceFactDet.InvoiceCommFactMas.Id,
                                          FactoryInvoiceNo = InvoiceFactDet.InvoiceCommFactMas.InvoiceNoFact,
                                          FDBCNo = docSubDetailDet.FactFDBCNo ?? "",
                                          ShipQty = (from invoiceFactMas in db.InvoiceCommFactMas
                                                     join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                                     join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                                     join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                                     //where invoiceMas.Id == RDLInvoiceMas.Id
                                                     where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                                     select exShipDet.ShipQuantity).Sum(),

                                          FactoryInvoiceValue = InvoiceFactDet.InvoiceTotalAmt,
                                          //FactoryInvoiceValue = (from invoiceFactMas1 in db.InvoiceCommFactMas
                                          //                       join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                          //                       join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                          //                       join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                          //                       join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                          //                       //where invoiceMas.Id == RDLInvoiceMas.Id
                                          //                       where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                          //                       select new { FactoryInvoice = exShipDet1.ShipQuantity * factDet.FOBUnitPrice }).Sum(x => x.FactoryInvoice),

                                          RDLPrice = (from invoiceFactMas in db.InvoiceCommFactMas
                                                      join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                                      join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                                      join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                                      join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                      join buyerDet in db.BuyerOrderDet on factDet.BuyerOrderDetId equals buyerDet.Id
                                                      where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                                      select new { FactoryInvoice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Sum(x => x.FactoryInvoice),
                                          FDBCDetNo = docSubDetailDet.FactFDBCNo

                                      }).Distinct().ToList();


                return Json(FactinvoicData, JsonRequestBehavior.AllowGet);
            }


        }


        public JsonResult SaveData(IEnumerable<ProceedRealizationDet> InvoiceDetails, ProceedRealizationMas InvoiceMas)
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


                        db.ProceedRealizationMas.Add(InvoiceMas);
                        db.SaveChanges();

                        foreach (var item in InvoiceDetails)
                        {
                            item.ProceedRealizationMasId = InvoiceMas.Id;
                            item.ProceedQty = item.ProceedQty;
                            item.DocSubmissionDetId = item.DocSubmissionDetId;

                            db.ProceedRealizationDet.Add(item);
                            db.SaveChanges();

                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!",
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



        //----------------------------------------------------------------END--------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------------------------





        //-------------------------------------------------------------------EDIT-----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------------


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProceedRealizationMas procRelMas = db.ProceedRealizationMas.Find(id);
            if (procRelMas == null)
            {
                return HttpNotFound();
            }

            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text", procRelMas.PaymentTypeId);
            ViewBag.PaymentTypeId = paymentType;
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.Where(x => x.PaymentTypeId == procRelMas.PaymentTypeId), "Id", "Name", procRelMas.BuyerInfoId);
            ViewBag.DocSubmissionMasId = new SelectList(db.DocSubmissionMas.Where(x => x.BuyerInfoId == procRelMas.BuyerInfoId), "Id", "FDBCNo", procRelMas.DocSubmissionMasId);

            ViewBag.ProceedDate = NullHelpers.DateToString(db.ProceedRealizationMas.SingleOrDefault(x => x.Id == id).ProceedDate);
            var lcNo = (from docMas in db.DocSubmissionMas
                        join proceedRelMas in db.ProceedRealizationMas on docMas.Id equals proceedRelMas.DocSubmissionMasId
                        where proceedRelMas.Id == id
                        select docMas.MasterLCInfoMas.LCNo).Distinct();
            ViewBag.MasterLCNo = lcNo.SingleOrDefault();

            return View(procRelMas);
        }




        public JsonResult GetSelectedData(int Id, int paymentType)
        {


            if (paymentType == 0)
            {
                var data = (from docMas in db.DocSubmissionMas
                            join lcMas in db.MasterLCInfoMas on new { a = docMas.MasterLCInfoMasId ?? 0, b = docMas.BuyerInfoId } equals new { a = lcMas.Id, b = lcMas.BuyerInfoId ?? 0 } into joinLeft
                            from pJoin in joinLeft.DefaultIfEmpty()
                            join lcDet in db.MasterLCInfoDet on pJoin.Id equals lcDet.MasterLCInfoMasId
                            join docDet in db.DocSubmissionDet on docMas.Id equals docDet.DocSubmissionMasId
                            join docDetDetail in db.DocSubmissionFactDet on docDet.Id equals docDetDetail.DocSubmissionDetId
                            join invCommDet in db.InvoiceCommDet on docDetDetail.InvoiceCommDetId equals invCommDet.Id
                            join invCommFactDet in db.InvoiceCommFactDet on invCommDet.InvoiceCommFactMasId equals invCommFactDet.InvoiceCommFactMasId
                            join exFactDet in db.ExFactoryDet on invCommFactDet.ExFactoryDetId equals exFactDet.Id
                            //where docMas.BuyerInfoId == BuyerInfoId && docMas.Id == DocSubmissionMasId
                            join procRelMas in db.ProceedRealizationMas on new { a = docMas.Id, b = docMas.BuyerInfoId }
                            equals new { a = procRelMas.DocSubmissionMasId, b = procRelMas.BuyerInfoId }
                            join procRelDet in db.ProceedRealizationDet on new { a = docDet.Id, b = procRelMas.Id }
                            equals new { a = procRelDet.DocSubmissionDetId, b = procRelDet.ProceedRealizationMasId }
                            where procRelMas.Id == Id
                            select new
                            {
                                //exFactDet,
                                docMas.MasterLCInfoMas,
                                invCommDet.InvoiceCommMas,
                                docDet,
                                procRelDet
                            }).Distinct().ToList();


                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //OrderMasId = x.exFactDet.BuyerOrderMasId,
                    OrderRefNo = x.InvoiceCommMas.InvoiceNo,
                    RDlInvoiceData = (from RDlInvoiceMas in db.InvoiceCommMas
                                      join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                                      join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                      join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                      join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                      join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                      join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                      where RDlInvoiceMas.Id == x.InvoiceCommMas.Id
                                      select new
                                      {
                                          //Quantity = exFactoryShipDet.ShipQuantity,
                                          //RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)

                                          //19/9/2018
                                          RDLValue = RDLInvoiceDet.InvoiceRDLTotalAmt
                                      }
                       ).Distinct().Sum(m => m.RDLValue),
                    ProceedAmt = x.procRelDet.ProceedQty,
                    DocDetId = x.docDet.Id,
                    ProceedDetId = x.procRelDet.Id
                }).Distinct();
                return Json(refinedata, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var data = (from docMas in db.DocSubmissionMas
                            join docDet in db.DocSubmissionDet on docMas.Id equals docDet.DocSubmissionMasId
                            join docDetDetail in db.DocSubmissionFactDet on docDet.Id equals docDetDetail.DocSubmissionDetId
                            join invCommFactDet in db.InvoiceCommFactDet on docDetDetail.InvoiceCommFactDet.InvoiceCommFactMasId equals invCommFactDet.InvoiceCommFactMasId
                            join exFactDet in db.ExFactoryDet on invCommFactDet.ExFactoryDetId equals exFactDet.Id
                            join procRelMas in db.ProceedRealizationMas on new { a = docMas.Id, b = docMas.BuyerInfoId }
                            equals new { a = procRelMas.DocSubmissionMasId, b = procRelMas.BuyerInfoId }
                            join procRelDet in db.ProceedRealizationDet on new { a = docDet.Id, b = procRelMas.Id }
                            equals new { a = procRelDet.DocSubmissionDetId, b = procRelDet.ProceedRealizationMasId }
                            where procRelMas.Id == Id
                            select new
                            {
                                exFactDet.BuyerOrderMas,
                                //exFactDet,
                                //docMas.MasterLCInfoMas,
                                //invCommDet.InvoiceCommMas,
                                docDet,
                                procRelDet
                            }).Distinct().ToList();


                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //OrderMasId = x.exFactDet.BuyerOrderMasId,
                    OrderRefNo = x.BuyerOrderMas.OrderRefNo,
                    RDlInvoiceData = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                      join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                      join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                      join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                      join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                      where buyerDet.BuyerOrderMasId == x.BuyerOrderMas.Id
                                      select new
                                      {
                                          Quantity = exFactoryShipDet.ShipQuantity,
                                          RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                      }

                       ).Distinct().Sum(m => m.RDLValue),
                    ProceedAmt = x.procRelDet.ProceedQty,
                    DocDetId = x.docDet.Id,
                    ProceedDetId = x.procRelDet.Id
                }).Distinct();
                return Json(refinedata, JsonRequestBehavior.AllowGet);
            }



        }


        public JsonResult UpdateData(IEnumerable<ProceedRealizationDet> InvoiceDetails, ProceedRealizationMas InvoiceMas)
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
                        var relMas = db.ProceedRealizationMas.Find(InvoiceMas.Id);

                        if (relMas == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invoice not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        relMas.ProceedDate = InvoiceMas.ProceedDate;

                        db.Entry(relMas).State = EntityState.Modified;

                        db.SaveChanges();


                        foreach (var item in InvoiceDetails)
                        {

                            if (item.Id == 0) //insert
                            {
                                item.ProceedRealizationMasId = relMas.Id;
                                //item.ExFactoryShipDetId = item.ExFactoryShipDetId;

                                db.ProceedRealizationDet.Add(item);

                            }
                            else //update
                            {
                                var oItem = db.ProceedRealizationDet.Find(item.Id);

                                oItem.ProceedQty = item.ProceedQty;

                                db.Entry(oItem).State = EntityState.Modified;
                            }

                            db.SaveChanges();


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




        //-------------------------------------------------------------------END------------------------------------------------------------------------------------ 
        //---------------------------------------------------------------------------------------------------------------------------------------------------------- 




        public JsonResult DeleteOrder(int id)
        {


            bool flag = false;
            try
            {

                var itemToDeleteMas = db.ProceedRealizationMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nProceed Realization Not found"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }




                var itemsToDeleteDet = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == id);



                db.ProceedRealizationDet.RemoveRange(itemsToDeleteDet);

                db.ProceedRealizationMas.Remove(itemToDeleteMas);

                flag = db.SaveChanges() > 0;

            }
            catch (Exception)
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


    }
}