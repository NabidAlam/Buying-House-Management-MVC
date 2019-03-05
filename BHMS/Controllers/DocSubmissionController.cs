using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHMS.Models;
using BHMS.Helpers;
using BHMS.ViewModels;

namespace BHMS.Controllers
{
    public class DocSubmissionController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: DocSubmission
        public ActionResult Index()
        {
            var docSubmissionMas = db.DocSubmissionMas.Include(d => d.BuyerInfo).Include(d => d.CourierInfo).Include(d => d.MasterLCInfoMas);
            return View(docSubmissionMas.ToList());
        }

        // GET: DocSubmission/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSubmissionMas docSubmissionMas = db.DocSubmissionMas.Find(id);
            if (docSubmissionMas == null)
            {
                return HttpNotFound();
            }
            return View(docSubmissionMas);
        }

        // GET: DocSubmission/Create
        public ActionResult Create()
        {
            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text");
            ViewBag.PaymentTypeId = paymentType;
            ViewBag.BuyerInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.CourierInfoId = new SelectList(db.CourierInfo, "Id", "Name");
            //ViewBag.MasterLCInfoId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo");
            ViewBag.MasterLCInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "LCNo");

            return View();
        }


        //If Lc Selected
        public JsonResult LoadLCDetailData(int Id)
        {
            var data = db.DocSubmissionDet.Where(x => x.DocSubmissionMasId == Id)
                .Select(x => new
                {
                    Id = x.Id,
                    DocSubmissionMasId = x.DocSubmissionMasId,
                    InvoiceCommMasId = x.InvoiceCommMasId,
                    BuyerOrderMasId = x.BuyerOrderMasId
                }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        //If Lc Selected
        public JsonResult GetInvoiceNo(int Id)
        {
            var data = db.InvoiceCommMas.Where(x => x.MasterLCInfoMasId == Id).OrderBy(x => x.InvoiceNo).Select(y => new { Name = y.InvoiceNo, Id = y.Id }).ToList();
            var getMasterLC = db.MasterLCInfoMas.SingleOrDefault(x => x.Id == Id);

            var result = new
            {
                RDlInvoiceList = data,
                LCDate = NullHelpers.DateToString(getMasterLC.LCDate),
                BankName = getMasterLC.BankBranch.Name
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //If TT selected
        public JsonResult GetReferenceNo(int Id)
        {
            var data = db.BuyerOrderMas.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.OrderRefNo).Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();
            //var getMasterLC = db.MasterLCInfoMas.SingleOrDefault(x => x.Id == Id);

            var result = new
            {
                RDlRefList = data
                //BankName = getMasterLC.BankBranch.Name
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetRDLInvoiceDetails(int Id)
        {
            var data = db.InvoiceCommMas.SingleOrDefault(x => x.Id == Id);


            var RDlInvoiceData = (from RDlInvoiceMas in db.InvoiceCommMas
                                  join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                                  join RDLInvoiceDetDet in db.InvoiceCommDetDet on RDLInvoiceDet.Id equals RDLInvoiceDetDet.InvoiceCommDetId
                                  join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                  join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                  join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                  join exFactoryShipDet in db.ExFactoryShipDet on new { ColA = exFactoryDet.Id, ColB = RDLInvoiceDetDet.ExFactoryShipDetId }
                                                equals new { ColA = exFactoryShipDet.ExFactoryDetId, ColB = exFactoryShipDet.Id }
                                  join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                  where RDlInvoiceMas.Id == Id
                                  select new
                                  {
                                      Quantity = RDLInvoiceDetDet.ShipQty ?? 0,
                                      //Quantity = exFactoryShipDet.ShipQuantity
                                      RDLValue = (RDLInvoiceDetDet.ShipQty ?? 0) * exFactoryShipDet.ShipmentSummDet.RdlFOB

                                      //19/9/2018
                                      //RDLValue = RDLInvoiceDet.InvoiceRDLTotalAmt
                                  }
                      ).AsEnumerable().ToList();


            var TotalShipQty = (decimal)0;
            var TotalRDLQty = (decimal)0;

            foreach (var item in RDlInvoiceData)
            {
                TotalShipQty = TotalShipQty + item.Quantity;
                TotalRDLQty = TotalRDLQty + item.RDLValue ?? 0;
            }

            var result = new
            {
                InvoiceDate = NullHelpers.DateToString(data.IssueDate),
                ShipQty = TotalShipQty,
                RDLQty = TotalRDLQty
                //RDLQty = db.InvoiceCommDet.Where(x=>x.InvoiceCommMasId==Id).Sum(x=>x.InvoiceRDLTotalAmt)
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }



        public JsonResult LoadFactoryInvoiceDetails(int Id, int MasterLcID)
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
                                     FactoryInvoiceValue = (from invoiceMas1 in db.InvoiceCommMas
                                                            join invoiceDet1 in db.InvoiceCommDet on invoiceMas1.Id equals invoiceDet1.InvoiceCommMasId
                                                            join invoiceFactMas1 in db.InvoiceCommFactMas on invoiceDet1.InvoiceCommFactMasId equals invoiceFactMas1.Id
                                                            join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                                            join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                                            join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                                            join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                            join factoryDelivDet in db.FactoryOrderDelivDet on new { ColA = factDet.Id } equals new { ColA = factoryDelivDet.FactoryOrderDetId }
                                                            //where invoiceMas.Id == RDLInvoiceMas.Id
                                                            where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                                            select new { FactoryInvoice = exShipDet1.ShipQuantity * factoryDelivDet.FactFOB }).Sum(x => x.FactoryInvoice),

                                     FactoryInvoiceValueWithDiscount = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),

                                     RDLPrice = (from invoiceMas in db.InvoiceCommMas
                                                 join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                                                 join invoiceFactMas in db.InvoiceCommFactMas on invoiceDet.InvoiceCommFactMasId equals invoiceFactMas.Id
                                                 join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                                 join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                                 join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                                 join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                 join buyerDet in db.BuyerOrderDet on factDet.BuyerOrderDetId equals buyerDet.Id
                                                 where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                                 select new { FactoryInvoice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Sum(x => x.FactoryInvoice),

                                     //RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == RDLInvoiceDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt)
                                     RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.Id == RDLInvoiceDet.Id).Sum(x => x.InvoiceRDLTotalAmt)

                                 }).Distinct().ToList();


            if (RDLinvoicData.Count() == 0)
            {
                var docSubmissionDet = db.DocSubmissionDet.SingleOrDefault(x => x.Id == Id);

                var DocSubmissionDetData = (from RDLInvoiceMas in db.InvoiceCommMas
                                            join RDLInvoiceDet in db.InvoiceCommDet on RDLInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
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
                                            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                            from pJoin in joinDest.DefaultIfEmpty()
                                            where RDLInvoiceMas.Id == docSubmissionDet.InvoiceCommMasId
                                            select new
                                            {
                                                DocSubDetailDetId = 0,
                                                DocSubDetId = docSubmissionDet.Id,
                                                InvoiceCommDetId = RDLInvoiceDet.Id,
                                                FactoryId = InvoiceFactMas.SupplierId,
                                                FactoryName = InvoiceFactMas.Supplier.Name,
                                                FactoryInvoiceId = InvoiceFactMas.Id,
                                                FactoryInvoiceNo = InvoiceFactMas.InvoiceNoFact,
                                                ShipQty = db.InvoiceCommDetDet.Where(x => x.InvoiceCommDetId == RDLInvoiceDet.Id).Sum(x => x.ShipQty),
                                                FDBCNo =  "",
                                                FactoryInvoiceValue = (from invoiceMas1 in db.InvoiceCommMas
                                                                       join invoiceDet1 in db.InvoiceCommDet on invoiceMas1.Id equals invoiceDet1.InvoiceCommMasId
                                                                       join invoiceFactMas1 in db.InvoiceCommFactMas on invoiceDet1.InvoiceCommFactMasId equals invoiceFactMas1.Id
                                                                       join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                                                       join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                                                       join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                                                       join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                                       join factoryDelivDet in db.FactoryOrderDelivDet on new { ColA = factDet.Id } equals new { ColA = factoryDelivDet.FactoryOrderDetId }
                                                                       //where invoiceMas.Id == RDLInvoiceMas.Id
                                                                       where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                                                       select new { FactoryInvoice = exShipDet1.ShipQuantity * factoryDelivDet.FactFOB }).Sum(x => x.FactoryInvoice),

                                                FactoryInvoiceValueWithDiscount = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),

                                                RDLPrice = (from invoiceMas in db.InvoiceCommMas
                                                            join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                                                            join invoiceFactMas in db.InvoiceCommFactMas on invoiceDet.InvoiceCommFactMasId equals invoiceFactMas.Id
                                                            join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                                            join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                                            join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                                            join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                            join buyerDet in db.BuyerOrderDet on factDet.BuyerOrderDetId equals buyerDet.Id
                                                            where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                                            select new { FactoryInvoice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Sum(x => x.FactoryInvoice),

                                                //RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == RDLInvoiceDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt),
                                                RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.Id == RDLInvoiceDet.Id).Sum(x => x.InvoiceRDLTotalAmt)

                                            }).Distinct().ToList();

                return Json(DocSubmissionDetData, JsonRequestBehavior.AllowGet);


            }


            return Json(RDLinvoicData, JsonRequestBehavior.AllowGet);

        }




        public JsonResult GetFactoryInvoiceDetails(int Id)
        {
            var RDLinvoicData = (from RDLInvoiceMas in db.InvoiceCommMas
                                 join RDLInvoiceDet in db.InvoiceCommDet on RDLInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
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
                                 //join factoryDelivDet in db.FactoryOrderDelivDet on new {ColA= factoryOrderDet.Id,ColB = shipDet.Id } equals new { ColA = factoryDelivDet.FactoryOrderDetId, ColB = factoryDelivDet.ShipmentSummDetId ?? 0 }
                                 ////join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                                 ////join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = ex_factoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                 join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                 from pJoin in joinDest.DefaultIfEmpty()
                                 where RDLInvoiceMas.Id == Id
                                 select new
                                 {
                                     InvoiceCommDetId = RDLInvoiceDet.Id,
                                     FactoryId = InvoiceFactMas.SupplierId,
                                     FactoryName = InvoiceFactMas.Supplier.Name,
                                     FactoryInvoiceId = InvoiceFactMas.Id,
                                     FactoryInvoiceNo = InvoiceFactMas.InvoiceNoFact,
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


                                     FactoryInvoiceValue = (from invoiceMas1 in db.InvoiceCommMas
                                                            join invoiceDet1 in db.InvoiceCommDet on invoiceMas1.Id equals invoiceDet1.InvoiceCommMasId
                                                            join invoiceFactMas1 in db.InvoiceCommFactMas on invoiceDet1.InvoiceCommFactMasId equals invoiceFactMas1.Id
                                                            join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                                            join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                                            join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                                            join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                            join factoryDelivDet in db.FactoryOrderDelivDet on new { ColA = factDet.Id } equals new { ColA = factoryDelivDet.FactoryOrderDetId }
                                                            //where invoiceMas.Id == RDLInvoiceMas.Id
                                                            where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                                            select new { FactoryInvoice = exShipDet1.ShipQuantity * factoryDelivDet.FactFOB }).Sum(x => x.FactoryInvoice),

                                     FactoryInvoiceValueWithDiscount = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),

                                     RDLPrice = (from invoiceMas in db.InvoiceCommMas
                                                 join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                                                 join invoiceFactMas in db.InvoiceCommFactMas on invoiceDet.InvoiceCommFactMasId equals invoiceFactMas.Id
                                                 join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
                                                 join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
                                                 join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
                                                 join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                                 join buyerDet in db.BuyerOrderDet on factDet.BuyerOrderDetId equals buyerDet.Id
                                                 where exFactoryDet.ExFactoryMasId == exFactoryMas.Id
                                                 select new { FactoryInvoice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Sum(x => x.FactoryInvoice),

                                     //RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == RDLInvoiceDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt),
                                     RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.Id == RDLInvoiceDet.Id).Sum(x => x.InvoiceRDLTotalAmt)

                                 }).Distinct().ToList();


            return Json(RDLinvoicData, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetRDLRefDetails(int Id)
        {
            var data = db.BuyerOrderMas.SingleOrDefault(x => x.Id == Id);

            var RDLRefData = (from factoryInvoiceMas in db.InvoiceCommFactMas
                              join factoryInvoiceDet in db.InvoiceCommFactDet on factoryInvoiceMas.Id equals factoryInvoiceDet.InvoiceCommFactMasId
                              join exFactoryDet in db.ExFactoryDet on factoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                              join exFactoryMas in db.ExFactoryMas on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
                              join buyerMas in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerMas.Id
                              join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = factoryInvoiceMas.SupplierId } equals
                                                                     new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.SupplierId }
                              join exFactoryShipDet in db.ExFactoryShipDet on new { ColA = exFactoryDet.Id, ColB = buyerDet.Id } equals
                                                                      new { ColA = exFactoryShipDet.ExFactoryDetId, ColB = exFactoryShipDet.BuyerOrderDetId }
                              where buyerMas.Id == Id
                              select new
                              {
                                  Quantity = exFactoryShipDet.ShipQuantity,
                                  RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                              }
                      ).AsEnumerable().ToList();

            var TotalShipQty = (decimal)0;
            var TotalRDLQty = (decimal)0;

            foreach (var item in RDLRefData)
            {
                TotalShipQty = TotalShipQty + item.Quantity;
                TotalRDLQty = TotalRDLQty + item.RDLValue ?? 0;
            }

            var result = new
            {
                InvoiceDate = NullHelpers.DateToString(data.OrderDate),
                ShipQty = TotalShipQty,
                RDLQty = TotalRDLQty
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public JsonResult LoadRefFactoryDetails(int Id, int BuyerOrderRefId)
        {
            var RDLRefData = (from docSubDetailDet in db.DocSubmissionFactDet
                              join docSubmissionDet in db.DocSubmissionDet on docSubDetailDet.DocSubmissionDetId equals docSubmissionDet.Id
                              join InvoiceFactDet in db.InvoiceCommFactDet on docSubDetailDet.InvoiceCommFactDetId equals InvoiceFactDet.Id
                              join InvoiceFactMas in db.InvoiceCommFactMas on InvoiceFactDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
                              // join ex_factoryShipDet in db.ExFactoryShipDet on InvoiceFactDet. equals ex_factoryShipDet.Id
                              join exfactoryDet in db.ExFactoryDet on InvoiceFactDet.ExFactoryDetId equals exfactoryDet.Id
                              join exFactoryMas in db.ExFactoryMas on exfactoryDet.ExFactoryMasId equals exFactoryMas.Id
                              join exFactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                              join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                              //join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = ex_factoryShipDet.BuyerOrderDetId } equals new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                              join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id } equals new { ColA = buyerOrderDet.BuyerOrderMasId }
                              join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                              join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                              //join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = ex_factoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                              join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                              from pJoin in joinDest.DefaultIfEmpty()
                              where buyerOrderMas.Id == BuyerOrderRefId && docSubmissionDet.Id == Id
                              select new
                              {
                                  DocSubDetailDetId = docSubDetailDet.Id,
                                  DocSubDetId = docSubmissionDet.Id,
                                  //BuyerOrderDetDetId = buyerOrderDet.Id,
                                  InvoiceFactDetId = InvoiceFactDet.Id,
                                  FactoryId = InvoiceFactMas.SupplierId,
                                  FactoryName = InvoiceFactMas.Supplier.Name,
                                  FactoryInvoiceId = InvoiceFactMas.Id,
                                  FactoryInvoiceNo = InvoiceFactMas.InvoiceNoFact,
                                  FDBCNo = docSubDetailDet.FactFDBCNo ?? "",
                                  ShipQty = db.ExFactoryShipDet.Where(x => x.ExFactoryDetId == exfactoryDet.Id).Select(x => x.ShipQuantity).Sum(),
                                  //FactoryPrice = factoryOrderDet.TransferPrice == null ? factoryOrderDet.FOBUnitPrice : factoryOrderDet.TransferPrice
                                  FactoryPrice = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),
                                  //FactoryPrice = (from exFactDet in db.ExFactoryDet
                                  //                join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                  //                join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                  //                join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                  //                join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                  //                //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                  //                where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                  //                select new { FactoryPrice = exShipDet.ShipQuantity * factDet.FOBUnitPrice }).Select(x => x.FactoryPrice).Sum(),
                                  //  //19/09/2018
                                  //FactoryInvoiceValueWithDiscount = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),

                                  RDLPrice = (from exFactDet in db.ExFactoryDet
                                              join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                              join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                              join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                              join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                              //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                              where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                              select new { FactoryPrice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Select(x => x.FactoryPrice).Sum()
                              }).Distinct().ToList();


            return Json(RDLRefData, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetRefFactoryDetails(int Id)
        {
            var RDLRefData = (
                                 from InvoiceFactMas in db.InvoiceCommFactMas
                                 join InvoiceFactDet in db.InvoiceCommFactDet on InvoiceFactMas.Id equals InvoiceFactDet.InvoiceCommFactMasId
                                 // join ex_factoryShipDet in db.ExFactoryShipDet on InvoiceFactDet. equals ex_factoryShipDet.Id
                                 join exfactoryDet in db.ExFactoryDet on InvoiceFactDet.ExFactoryDetId equals exfactoryDet.Id
                                 join exFactoryMas in db.ExFactoryMas on exfactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                 join exFactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                 join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                 //join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = ex_factoryShipDet.BuyerOrderDetId } equals new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                 join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id } equals new { ColA = buyerOrderDet.BuyerOrderMasId }
                                 join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                 join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                                 //join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = ex_factoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                 join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                 from pJoin in joinDest.DefaultIfEmpty()
                                 where buyerOrderMas.Id == Id
                                 select new
                                 {
                                     //BuyerOrderDetDetId = buyerOrderDet.Id,
                                     InvoiceFactDetId = InvoiceFactDet.Id,
                                     FactoryId = InvoiceFactMas.SupplierId,
                                     FactoryName = InvoiceFactMas.Supplier.Name,
                                     FactoryInvoiceId = InvoiceFactMas.Id,
                                     FactoryInvoiceNo = InvoiceFactMas.InvoiceNoFact,
                                     ShipQty = db.ExFactoryShipDet.Where(x => x.ExFactoryDetId == exfactoryDet.Id).Select(x => x.ShipQuantity).Sum(),
                                     //FactoryPrice = factoryOrderDet.TransferPrice == null ? factoryOrderDet.FOBUnitPrice : factoryOrderDet.TransferPrice
                                     //FactoryPrice= (from exFactDet in db.ExFactoryDet
                                     //                join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                     //                join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                     //                join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                     //                join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                     //                //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                     //                where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                     //                select new { FactoryPrice = exShipDet.ShipQuantity * factDet.FOBUnitPrice }).Select(x => x.FactoryPrice).Sum(),
                                     //19/09/2018
                                     //FactoryPrice = (from exFactDet in db.ExFactoryDet
                                     //                join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                     //                join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                     //                join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                     //                join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                     //                join factDelivDet in db.FactoryOrderDelivDet on factDet.Id equals factDelivDet.FactoryOrderDetId
                                     //                //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                     //                where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                     //                select new { FactoryPrice = exShipDet.ShipQuantity * factDelivDet.FactFOB }).Select(x => x.FactoryPrice).Sum(),
                                     FactoryPrice = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),
                                     ////19/09/2018
                                     //FactoryInvoiceValueWithDiscount = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == InvoiceFactMas.Id).Sum(x => x.InvoiceTotalAmt),

                                     //FactoryPriceWithDiscount = (from exFactDet in db.ExFactoryDet
                                     //                            join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                     //                            join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                     //                            join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                     //                            join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                     //                            //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                     //                            where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                     //                            select new { FactoryPrice = exShipDet.ShipQuantity * factDet.FOBUnitPrice }).Select(x => x.FactoryPrice).Sum(),

                                     RDLPrice = (from exFactDet in db.ExFactoryDet
                                                 join exShipDet in db.ExFactoryShipDet on exFactDet.Id equals exShipDet.ExFactoryDetId
                                                 join buyerMas in db.BuyerOrderMas on exFactDet.BuyerOrderMasId equals buyerMas.Id
                                                 join buyerDet in db.BuyerOrderDet on new { ColA = buyerMas.Id, ColB = exShipDet.BuyerOrderDetId } equals new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.Id }
                                                 join factDet in db.FactoryOrderDet on buyerDet.Id equals factDet.BuyerOrderDetId
                                                 //where buyerDet.SupplierId == InvoiceFactMas.SupplierId
                                                 where exFactDet.ExFactoryMasId == exFactoryMas.Id
                                                 select new { FactoryPrice = exShipDet.ShipQuantity * buyerDet.UnitPrice }).Select(x => x.FactoryPrice).Sum()
                                 }).Distinct().ToList();


            return Json(RDLRefData, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SaveDocSubmission(IEnumerable<VMDocSubmissionDet> DocSubDetails, IEnumerable<VMDocSubmissionFactDet> DocDetDetails, VMDocSubmissionMas DocSubMas)
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

                        var DocSubM = new DocSubmissionMas()
                        {
                            Id = 0,
                            PaymentTypeId = DocSubMas.PaymentTypeId,
                            BuyerInfoId = DocSubMas.BuyerInfoId,
                            SubmissionDate = DocSubMas.SubmissionDate,
                            FDBCNo = DocSubMas.FDBCNo,
                            FDBCValue = DocSubMas.FDBCValue,
                            FDBCDate = DocSubMas.FDBCDate,
                            //InvoiceCommMasId = DocSubMas.InvoiceCommMasId,
                            MasterLCInfoMasId = DocSubMas.MasterLCInfoMasId,
                            AWBNo = DocSubMas.AWBNo,
                            AWBDate = DocSubMas.AWBDate,
                            CourierInfoId = DocSubMas.CourierInfoId,
                            OpBy = 1,
                            OpOn = OpDate,
                            IsAuth = true
                        };

                        db.DocSubmissionMas.Add(DocSubM);
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();


                        foreach (var item in DocSubDetails)
                        {
                            if (DocSubMas.PaymentTypeId == 0) //LC
                            {

                                var DocSubD = new DocSubmissionDet()
                                {
                                    Id = 0,
                                    DocSubmissionMasId = DocSubM.Id,
                                    InvoiceCommMasId = item.InvoiceCommMasId
                                };
                                db.DocSubmissionDet.Add(DocSubD);
                                db.SaveChanges();
                                dictionary.Add(item.TempDocSubDetId, DocSubD.Id);
                            }
                            else
                            {

                                var DocSubD = new DocSubmissionDet()
                                {
                                    Id = 0,
                                    DocSubmissionMasId = DocSubM.Id,
                                    BuyerOrderMasId = item.InvoiceCommMasId
                                };
                                db.DocSubmissionDet.Add(DocSubD);
                                db.SaveChanges();
                                dictionary.Add(item.TempDocSubDetId, DocSubD.Id);
                            }

                        }

                        //---- shipment data


                        if (DocDetDetails != null)
                        {
                            foreach (var item in DocDetDetails)
                            {

                                var deliv = new DocSubmissionFactDet()
                                {
                                    Id = item.Id,
                                    DocSubmissionDetId = dictionary[item.FactInvoicerDetTempId],
                                    InvoiceCommDetId = item.InvoiceCommDetId == 0 ? null : item.InvoiceCommDetId,
                                    InvoiceCommFactDetId = item.InvoiceFactDetId == 0 ? null : item.InvoiceFactDetId,
                                    FactFDBCNo = item.FactFDBCNo
                                };

                                //db.Entry(deliv).State = deliv.Id == 0 ?
                                //                            EntityState.Added :
                                //                            EntityState.Modified;

                                db.DocSubmissionFactDet.Add(deliv);
                                db.SaveChanges();

                            }

                        }



                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = DocSubM.Id
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



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,PaymentTypeId,BuyerInfoId,SubmissionDate,FDBCNo,FDBCValue,FDBCDate,InvoiceCommMasId,AWBNo,AWBDate,CourierInfoId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DocSubmissionMas docSubmissionMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.DocSubmissionMas.Add(docSubmissionMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", docSubmissionMas.BuyerInfoId);
        //    ViewBag.CourierInfoId = new SelectList(db.CourierInfo, "Id", "Name", docSubmissionMas.CourierInfoId);
        //    //ViewBag.InvoiceCommMasId = new SelectList(db.InvoiceCommMas, "Id", "InvoiceNo", docSubmissionMas.InvoiceCommMasId);
        //    ViewBag.MasterLCInfoId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", docSubmissionMas.MasterLCInfoMasId);
        //    return View(docSubmissionMas);
        //}

        // GET: DocSubmission/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSubmissionMas docSubmissionMas = db.DocSubmissionMas.Find(id);
            if (docSubmissionMas == null)
            {
                return HttpNotFound();
            }

            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text", docSubmissionMas.PaymentTypeId);
            ViewBag.PaymentTypeId = paymentType;
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.Where(x => x.PaymentTypeId == docSubmissionMas.PaymentTypeId), "Id", "Name", docSubmissionMas.BuyerInfoId);
            ViewBag.CourierInfoId = new SelectList(db.CourierInfo, "Id", "Name", docSubmissionMas.CourierInfoId);
            //ViewBag.InvoiceCommMasId = new SelectList(db.InvoiceCommMas, "Id", "InvoiceNo", docSubmissionMas.InvoiceCommMasId);
            ViewBag.MasterLCInfoId = new SelectList(db.MasterLCInfoMas.Where(x => x.BuyerInfoId == docSubmissionMas.BuyerInfoId), "Id", "LCNo", docSubmissionMas.MasterLCInfoMasId);
            if (docSubmissionMas.PaymentTypeId == 0)
            {
                var getMasterLC = db.MasterLCInfoMas.SingleOrDefault(x => x.Id == docSubmissionMas.MasterLCInfoMasId);
                ViewBag.LCDate = NullHelpers.DateToString(getMasterLC.LCDate);
                ViewBag.BankName = getMasterLC.BankBranch.Name;
            }

            ViewBag.CheckProceedRealizationExists = db.ProceedRealizationMas.Where(x => x.DocSubmissionMasId == docSubmissionMas.Id).Count() > 0 ? true : false;

            return View(docSubmissionMas);
        }


        public JsonResult UpdateDocSubmission(IEnumerable<VMDocSubmissionDet> DocSubDetails, IEnumerable<VMDocSubmissionFactDet> DocDetDetails, VMDocSubmissionMas DocSubMas, int[] DelItems)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var DocSubM = db.DocSubmissionMas.Find(DocSubMas.Id);

                        if (DocSubM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Document Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        DocSubM.PaymentTypeId = DocSubMas.PaymentTypeId;
                        DocSubM.BuyerInfoId = DocSubMas.BuyerInfoId;
                        DocSubM.SubmissionDate = DocSubMas.SubmissionDate;
                        DocSubM.FDBCNo = DocSubMas.FDBCNo;
                        DocSubM.FDBCValue = DocSubMas.FDBCValue;
                        DocSubM.FDBCDate = DocSubMas.FDBCDate;
                        DocSubM.BuyerInfoId = DocSubMas.BuyerInfoId;
                        DocSubM.SubmissionDate = DocSubMas.SubmissionDate;
                        DocSubM.MasterLCInfoMasId = DocSubMas.MasterLCInfoMasId;
                        DocSubM.AWBNo = DocSubMas.AWBNo;
                        DocSubM.AWBDate = DocSubMas.AWBDate;
                        DocSubM.CourierInfoId = DocSubMas.CourierInfoId;
                        DocSubM.AWBDate = DocSubMas.AWBDate;

                        db.Entry(DocSubM).State = EntityState.Modified;
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();


                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                //---- if order detail deleted without manual deletion of shipment
                                // ---- find those shipment and delete first

                                var docDets = db.DocSubmissionFactDet.Where(x => x.DocSubmissionDetId == item);

                                if (docDets != null)
                                {
                                    db.DocSubmissionFactDet.RemoveRange(docDets);
                                }

                                var delOrder = db.DocSubmissionDet.Find(item);
                                db.DocSubmissionDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }


                        foreach (var item in DocSubDetails)
                        {
                            if (DocSubMas.PaymentTypeId == 0) //LC
                            {
                                var DocSubD = new DocSubmissionDet()
                                {
                                    Id = item.Id,
                                    DocSubmissionMasId = DocSubM.Id,
                                    InvoiceCommMasId = item.InvoiceCommMasId
                                };

                                db.Entry(DocSubD).State = DocSubD.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                db.SaveChanges();
                                dictionary.Add(item.TempDocSubDetId, DocSubD.Id);
                            }
                            else
                            {
                                var DocSubD = new DocSubmissionDet()
                                {
                                    Id = item.Id,
                                    DocSubmissionMasId = DocSubM.Id,
                                    BuyerOrderMasId = item.InvoiceCommMasId
                                };

                                db.Entry(DocSubD).State = DocSubD.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                db.SaveChanges();
                                dictionary.Add(item.TempDocSubDetId, DocSubD.Id);

                            }
                        }

                        // var slno = 1;

                        if (DocDetDetails != null)
                        {
                            foreach (var item in DocDetDetails)
                            {
                                var deliv = new DocSubmissionFactDet()
                                {
                                    Id = item.Id,
                                    DocSubmissionDetId = dictionary[item.FactInvoicerDetTempId],
                                    InvoiceCommDetId = item.InvoiceCommDetId == 0 ? null : item.InvoiceCommDetId,
                                    InvoiceCommFactDetId = item.InvoiceFactDetId == 0 ? null : item.InvoiceFactDetId,
                                    FactFDBCNo = item.FactFDBCNo
                                };

                                db.Entry(deliv).State = deliv.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;
                                db.SaveChanges();

                            }
                        }



                        //--- delete shipment detail items
                        //if (DelDelivItems != null)
                        //{
                        //    foreach (var item in DelDelivItems)
                        //    {
                        //        var delOrder = db.ShipmentSummDet.Find(item);
                        //        db.ShipmentSummDet.Remove(delOrder);
                        //        db.SaveChanges();
                        //    }
                        //}

                        //---- delete order detail items




                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Update successful !!"
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


        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,PaymentTypeId,BuyerInfoId,SubmissionDate,FDBCNo,FDBCValue,FDBCDate,InvoiceCommMasId,AWBNo,AWBDate,CourierInfoId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DocSubmissionMas docSubmissionMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(docSubmissionMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", docSubmissionMas.BuyerInfoId);
        //    ViewBag.CourierInfoId = new SelectList(db.CourierInfo, "Id", "Name", docSubmissionMas.CourierInfoId);
        //    //ViewBag.InvoiceCommMasId = new SelectList(db.InvoiceCommMas, "Id", "InvoiceNo", docSubmissionMas.InvoiceCommMasId);
        //    ViewBag.MasterLCInfoId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", docSubmissionMas.MasterLCInfoMasId);
        //    return View(docSubmissionMas);
        //}

        // GET: DocSubmission/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSubmissionMas docSubmissionMas = db.DocSubmissionMas.Find(id);
            if (docSubmissionMas == null)
            {
                return HttpNotFound();
            }
            return View(docSubmissionMas);
        }

        // POST: DocSubmission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocSubmissionMas docSubmissionMas = db.DocSubmissionMas.Find(id);
            db.DocSubmissionMas.Remove(docSubmissionMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult DeleteDocSubmission(int id)
        {
            bool flag = false;
            try
            {
                var itemToDeleteMas = db.DocSubmissionMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nDocument Submission Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //var checkDocSubDet = db.InvoiceCommDet.Where(x => x.InvoiceCommFactMasId == id).ToList();

                //if (checkDocSubDet.Count > 0)
                //{
                //    var result = new
                //    {
                //        flag = false,
                //        message = "Deletion failed!\nInvoice exists. Delete invoice data first."
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);

                //}


                var itemsToDeleteDet = db.DocSubmissionDet.Where(x => x.DocSubmissionMasId == id);

                foreach (var item in itemsToDeleteDet)
                {
                    var itemsToDeleteDeliv = db.DocSubmissionFactDet.Where(x => x.DocSubmissionDetId == item.Id);
                    db.DocSubmissionFactDet.RemoveRange(itemsToDeleteDeliv);
                }

                db.DocSubmissionDet.RemoveRange(itemsToDeleteDet);

                db.DocSubmissionMas.Remove(itemToDeleteMas);

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
