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
using System.Globalization;
using BHMS.ViewModels;

namespace BHMS.Controllers
{
    public class FDDPaymentController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        public ActionResult PendingFDD()
        {
            var searchType = new SelectList(new List<SelectListItem>
            {
              new SelectListItem { Text = "FDBC", Value = "0" },
              new SelectListItem { Text = "TT No", Value = "1" },
              new SelectListItem { Text = "Master LC", Value = "2" },
              new SelectListItem { Text = "Buyer", Value = "3" },
              new SelectListItem { Text = "Factory", Value = "4" },
            }, "Value", "Text");
            ViewBag.searchType = searchType;

            return View();
        }



        public JsonResult FDBCList()
        {
            List<VMFDBCList> list = new List<VMFDBCList>();

            var realizationList = db.ProceedRealizationMas.ToList();
            foreach (var item in realizationList)
            {
                var data = (from docSubmissionMas in db.DocSubmissionMas
                            join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                            join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                            join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                            //where docSubmissionMas.FDBCNo.Trim() == value.Trim() && docSubmissionMas.PaymentTypeId == 0
                            //where docSubmissionMas.FDBCNo.Trim() == item.DocSubmissionMas.FDBCNo.Trim()
                            where proceedRealizationDet.ProceedRealizationMas.Id == item.Id
                            select new
                            {
                                //ProceedRealizationDetId = proceedRealizationDet.Id,
                                ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                FDBCNo = docSubmissionMas.FDBCNo,
                                ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                RDLInvoiceValue = docSubmissionMas.PaymentTypeId == 0 ? (from realizeMas in db.ProceedRealizationMas
                                                                                         join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                                                         join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                                                         join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                                                         join RDLInvoiceDet in db.InvoiceCommDet on docSubFactDet.InvoiceCommDetId equals RDLInvoiceDet.Id
                                                                                         join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                                                                         join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                                                         join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                                                         join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                                                         join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                                                         //where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                                                         where realizeMas.Id == item.Id
                                                                                         select new
                                                                                         {
                                                                                             //RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                                                             RDLValue = RDLInvoiceDet.InvoiceRDLTotalAmt
                                                                                         }).Select(m => m.RDLValue).Sum()
                                                   :
                                                   (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                                    join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                    join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                    join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                    join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                    where buyerDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId
                                                    select new
                                                    {
                                                        RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                    }).Select(m => m.RDLValue).Sum(),

                                RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == docSubmissionDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt),

                                //Pending = (from realizeMas in db.ProceedRealizationMas
                                //           join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                //           join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                //           join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                //           where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                //           select docSubFactDet).ToList().Count - (docSubmissionMas.PaymentTypeId == 0 ?
                                //      (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()) :
                                //      (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count())),

                                //17/10/2018
                                Pending = (from realizeMas in db.ProceedRealizationMas
                                           join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                           join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                           join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                           where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                           select docSubFactDet.FactFDBCNo).Distinct().ToList().Count - (docSubmissionMas.PaymentTypeId == 0 ?
                                      (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()) :
                                      (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count())),

                                Paid = docSubmissionMas.PaymentTypeId == 0 ?
                                db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() :
                                db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count(),

                                Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                        (docSubmissionMas.PaymentTypeId == 0 ?
                                        ((db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum())) :
                                     ((db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count() == 0 ?
                                        0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Select(x => x.FDDAmount).Sum())))
                                        ,

                                TotalFDDAmount = (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum()),

                                CheckCommission = db.CommissionRealization.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).ToList().Count()
                            }).Distinct().ToList().Select(x => new
                            {
                                //ProceedRealizationDetId = x.ProceedRealizationDetId,
                                ProceedRealizationMasId = x.ProceedRealizationMasId,
                                ProceedTypeId = x.ProceedTypeId,
                                ProceedType = x.ProceedType,
                                FDBCNo = x.FDBCNo,
                                ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                ProceedValue = x.ProceedValue,
                                RDLInvoiceValue = x.RDLInvoiceValue,
                                //RDLInvoiceValueWithDiscount = x.RDLInvoiceValueWithDiscount,
                                Pending = x.Pending,
                                Paid = x.Paid,
                                Amount = x.Amount,
                                TotalFDDAmount = x.TotalFDDAmount,
                                CheckCommission = x.CheckCommission
                            }).Distinct();

                if (data.Count() > 0)
                {

                    foreach (var realizeData in data)
                    {
                        VMFDBCList vm = new VMFDBCList();

                        //vm.ProceedRealizationDetId = realizeData.ProceedRealizationDetId;
                        vm.ProceedRealizationMasId = realizeData.ProceedRealizationMasId;
                        vm.ProceedTypeId = realizeData.ProceedTypeId;
                        vm.ProceedType = realizeData.ProceedType;
                        vm.FDBCNo = realizeData.FDBCNo;
                        vm.ProceedDate = realizeData.ProceedDate;
                        vm.ProceedValue = realizeData.ProceedValue;
                        vm.RDLInvoiceValue = realizeData.RDLInvoiceValue;
                        //vm.RDLInvoiceValueWithDiscount = realizeData.RDLInvoiceValueWithDiscount;
                        vm.Pending = realizeData.Pending;
                        vm.Paid = realizeData.Paid;
                        vm.Amount = realizeData.Amount;
                        vm.TotalFDDAmount = realizeData.TotalFDDAmount;
                        vm.CheckCommission = realizeData.CheckCommission;

                        list.Add(vm);
                    }

                }


            }

            if (list.Count() > 0)
            {
                var result = new
                {
                    FDBCdata = list,
                    flag = true
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new
                {
                    data = "Not Found!",
                    flag = false
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetSearchValues(int typeId, string value)
        {
            if (typeId == 0) //FDBC LC 
            {
                var data = (from docSubmissionMas in db.DocSubmissionMas
                            join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                            join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                            join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                            //where docSubmissionMas.FDBCNo.Trim() == value.Trim() && docSubmissionMas.PaymentTypeId == 0
                            where docSubmissionMas.FDBCNo.Trim() == value.Trim()
                            select new
                            {
                                ProceedRealizationDetId = proceedRealizationDet.Id,
                                ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                FDBCNo = docSubmissionMas.FDBCNo,
                                ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                RDLInvoiceValue = docSubmissionMas.PaymentTypeId == 0 ? (from realizeMas in db.ProceedRealizationMas
                                                                                         join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                                                         join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                                                         join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                                                         join RDLInvoiceDet in db.InvoiceCommDet on docSubFactDet.InvoiceCommDetId equals RDLInvoiceDet.Id
                                                                                         join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                                                                         join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                                                         join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                                                         join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                                                         join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                                                         where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                                                         select new
                                                                                         {
                                                                                             RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                                                         }).Select(m => m.RDLValue).Sum()
                                                   :
                                                   (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                                    join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                    join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                    join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                    join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                    where buyerDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId
                                                    select new
                                                    {
                                                        RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                    }).Select(m => m.RDLValue).Sum(),

                                RDLInvoiceValueWithDiscount = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == docSubmissionDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt),

                                Pending = (from realizeMas in db.ProceedRealizationMas
                                           join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                           join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                           join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                           where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                           select docSubFactDet).ToList().Count - (docSubmissionMas.PaymentTypeId == 0 ?
                                      (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()) :
                                      (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count())),
                                //Pending = (from realizeMas in db.ProceedRealizationMas
                                //           join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                //           join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                //           join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                //           where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                //           select docSubFactDet).ToList().Count,

                                Paid = docSubmissionMas.PaymentTypeId == 0 ?
                                db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() :
                                db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count(),

                                Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                        (docSubmissionMas.PaymentTypeId == 0 ?
                                        ((db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum())) :
                                     ((db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count() == 0 ?
                                        0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Select(x => x.FDDAmount).Sum())))
                                        ,

                                TotalFDDAmount = (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum()),

                                CheckCommission = db.CommissionRealization.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).ToList().Count()
                            }).Distinct().ToList().Select(x => new
                            {
                                ProceedRealizationDetId = x.ProceedRealizationDetId,
                                ProceedRealizationMasId = x.ProceedRealizationMasId,
                                ProceedTypeId = x.ProceedTypeId,
                                ProceedType = x.ProceedType,
                                FDBCNo = x.FDBCNo,
                                ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                ProceedValue = x.ProceedValue,
                                RDLInvoiceValue = x.RDLInvoiceValue,
                                RDLInvoiceValueWithDiscount = x.RDLInvoiceValueWithDiscount,
                                Pending = x.Pending,
                                Paid = x.Paid,
                                Amount = x.Amount,
                                TotalFDDAmount = x.TotalFDDAmount,
                                CheckCommission = x.CheckCommission
                            }).Distinct();

                if (data.Count() > 0)
                {
                    var result = new
                    {
                        FDBCdata = data,
                        flag = true
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        data = "Not Found!",
                        flag = false
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            //-----------------------------FDBC (TT)----------------------------------------------
            else if (typeId == 1)
            {
                var TTdata = (from docSubmissionMas in db.DocSubmissionMas
                              join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                              join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                              join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                              join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                              where docSubmissionMas.FDBCNo.Trim() == value.Trim() && docSubmissionMas.PaymentTypeId == 1
                              select new
                              {
                                  ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                  ProceedRealizationDetId = proceedRealizationDet.Id,
                                  ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                  ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                  FDBCNo = docSubmissionMas.FDBCNo,
                                  ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                  ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                  RDLInvoiceValue = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                                     join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                     join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                     join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                     join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                     //where FactoryInvoiceMas.Id == docSubmissionFactDet.InvoiceCommFactDet.InvoiceCommFactMasId
                                                     where buyerDet.BuyerOrderMasId == buyerMas.Id
                                                     select new
                                                     {
                                                         RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                     }).Select(m => m.RDLValue).Sum(),
                                  //Pending = (db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()) -
                                  //            (db.TTPayment.Where(x => x.ProceedRealizationDetId == proceedRealizationDet.Id).Count()),
                                  Pending = (from realizeMas in db.ProceedRealizationMas
                                             join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                             join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                             join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                             where docSubDet.BuyerOrderMasId == buyerMas.Id
                                             select docSubFactDet).ToList().Count -
                                      (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count()),
                                  Paid = db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count(),
                                  Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                          (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count() == 0 ?
                                        0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Select(x => x.FDDAmount).Sum()),
                                  TotalFDDAmount = (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count() == 0 ?
                                        0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Select(x => x.FDDAmount).Sum()),
                                  CheckCommission = db.CommissionRealization.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).ToList().Count()
                              }).Distinct().ToList().Select(x => new
                              {
                                  ProceedRealizationDetId = x.ProceedRealizationDetId,
                                  ProceedRealizationMasId = x.ProceedRealizationMasId,
                                  ProceedTypeId = x.ProceedTypeId,
                                  ProceedType = x.ProceedType,
                                  FDBCNo = x.FDBCNo,
                                  ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                  ProceedValue = x.ProceedValue,
                                  RDLInvoiceValue = x.RDLInvoiceValue,
                                  Pending = x.Pending,
                                  Paid = x.Paid,
                                  Amount = x.Amount,
                                  TotalFDDAmount = x.TotalFDDAmount,
                                  CheckCommission = x.CheckCommission
                              });

                if (TTdata.Count() > 0)
                {
                    var result = new
                    {
                        FDBCdata = TTdata,
                        flag = true
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        data = "Not Found!",
                        flag = false
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            //-----------------------------Master LC----------------------------------------
            else if (typeId == 2)
            {
                var MasterLCdata = (from docSubmissionMas in db.DocSubmissionMas
                                    join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                                    join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                                    join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                                    where docSubmissionMas.MasterLCInfoMas.LCNo.Trim().ToLower() == value.Trim().ToLower()
                                    select new
                                    {
                                        ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                        ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                        ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                        FDBCNo = docSubmissionMas.FDBCNo,
                                        ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,

                                        ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                        RDLInvoiceValue = (from RDlInvoiceMas in db.InvoiceCommMas
                                                           join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                                                           join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                                           join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                           join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                           join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                           join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                           where RDlInvoiceMas.Id == docSubmissionFactDet.InvoiceCommDet.InvoiceCommMasId
                                                           select new
                                                           {
                                                               RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                           }).Select(m => m.RDLValue).Sum(),
                                        Pending = (from realizeMas in db.ProceedRealizationMas
                                                   join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                   join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                   join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                   where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                   select docSubFactDet).ToList().Count -
                                      (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()),

                                        Paid = db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count(),

                                        Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                        (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum()),
                                        TotalFDDAmount = (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                      0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum())
                                    }).Distinct().ToList().Select(x => new
                                    {
                                        ProceedRealizationMasId = x.ProceedRealizationMasId,
                                        ProceedTypeId = x.ProceedTypeId,
                                        ProceedType = x.ProceedType,
                                        FDBCNo = x.FDBCNo,
                                        ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                        ProceedValue = x.ProceedValue,
                                        RDLInvoiceValue = x.RDLInvoiceValue,
                                        Pending = x.Pending,
                                        Paid = x.Paid,
                                        Amount = x.Amount,
                                        TotalFDDAmount = x.TotalFDDAmount
                                    }).Distinct();


                if (MasterLCdata.Count() > 0)
                {
                    var result = new
                    {
                        FDBCdata = MasterLCdata,
                        flag = true
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        data = "Not Found!",
                        flag = false
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            //----------------------------Buyer-----------------------------------
            else if (typeId == 3)
            {
                var buyerPaymentId = db.BuyerInfo.SingleOrDefault(x => x.Name.Trim().ToLower() == value.Trim().ToLower()).PaymentTypeId;

                if (buyerPaymentId == 0)
                {
                    var Buyerdata = (from docSubmissionMas in db.DocSubmissionMas
                                     join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                                     join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                                     join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                                     where docSubmissionMas.BuyerInfo.Name.Trim().ToLower() == value.Trim().ToLower()
                                     select new
                                     {
                                         ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                         ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                         ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                         FDBCNo = docSubmissionMas.FDBCNo,
                                         ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                         ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                         RDLInvoiceValue = (from RDlInvoiceMas in db.InvoiceCommMas
                                                            join RDLInvoiceDet in db.InvoiceCommDet on RDlInvoiceMas.Id equals RDLInvoiceDet.InvoiceCommMasId
                                                            join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                                            join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                            join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                            join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                            where RDlInvoiceMas.Id == docSubmissionFactDet.InvoiceCommDet.InvoiceCommMasId
                                                            select new
                                                            {
                                                                RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                            }).Select(m => m.RDLValue).Sum(),

                                         Pending = (from realizeMas in db.ProceedRealizationMas
                                                    join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                    join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                    join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                    where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                    select docSubFactDet).ToList().Count -
                                           (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()),

                                         Paid = db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count(),

                                         Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                             (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                           0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum()),
                                         TotalFDDAmount = (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                           0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum())
                                     }).Distinct().ToList().Select(x => new
                                     {
                                         ProceedRealizationMasId = x.ProceedRealizationMasId,
                                         ProceedTypeId = x.ProceedTypeId,
                                         ProceedType = x.ProceedType,
                                         FDBCNo = x.FDBCNo,
                                         ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                         ProceedValue = x.ProceedValue,
                                         RDLInvoiceValue = x.RDLInvoiceValue,
                                         Pending = x.Pending,
                                         Paid = x.Paid,
                                         Amount = x.Amount,
                                         TotalFDDAmount = x.TotalFDDAmount
                                     }).Distinct();

                    if (Buyerdata.Count() > 0)
                    {

                        var result = new
                        {
                            FDBCdata = Buyerdata,
                            flag = true
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        var result = new
                        {
                            data = "Not Found!",
                            flag = false
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var Buyerdata = (from docSubmissionMas in db.DocSubmissionMas
                                     join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                                     join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                                     join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                                     join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                                     where docSubmissionMas.BuyerInfo.Name.Trim().ToLower() == value.Trim().ToLower()
                                     select new
                                     {
                                         ProceedRealizationDetId = proceedRealizationDet.Id,
                                         ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                         ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                         FDBCNo = docSubmissionMas.FDBCNo,
                                         ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                         ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                         RDLInvoiceValue = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                                            join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                            join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                            join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                            where buyerDet.BuyerOrderMasId == buyerMas.Id
                                                            select new
                                                            {
                                                                RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                            }).Select(m => m.RDLValue).Sum(),
                                         Pending = (from realizeMas in db.ProceedRealizationMas
                                                    join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                    join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                    join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                    where docSubDet.BuyerOrderMasId == buyerMas.Id
                                                    select docSubFactDet).ToList().Count -
                                             (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count()),
                                         Paid = db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count(),
                                         Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                                 (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count() == 0 ?
                                               0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Select(x => x.FDDAmount).Sum()),
                                         TotalFDDAmount = (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Count() == 0 ?
                                           0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == buyerMas.Id).Select(x => x.FDDAmount).Sum())
                                     }).Distinct().ToList().Select(x => new
                                     {
                                         ProceedRealizationDetId = x.ProceedRealizationDetId,
                                         ProceedTypeId = x.ProceedTypeId,
                                         ProceedType = x.ProceedType,
                                         FDBCNo = x.FDBCNo,
                                         ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                         ProceedValue = x.ProceedValue,
                                         RDLInvoiceValue = x.RDLInvoiceValue,
                                         Pending = x.Pending,
                                         Paid = x.Paid,
                                         Amount = x.Amount,
                                         TotalFDDAmount = x.TotalFDDAmount
                                     });

                    if (Buyerdata.Count() > 0)
                    {
                        var result = new
                        {
                            FDBCdata = Buyerdata,
                            flag = true
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var result = new
                        {
                            data = "Not Found!",
                            flag = false
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }


            }
            //----------------------------Factory-----------------------------------
            else if (typeId == 4)
            {
                var Factorydata = (from docSubmissionMas in db.DocSubmissionMas
                                   join docSubmissionDet in db.DocSubmissionDet on docSubmissionMas.Id equals docSubmissionDet.DocSubmissionMasId
                                   join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                                   join proceedRealizationDet in db.ProceedRealizationDet on docSubmissionDet.Id equals proceedRealizationDet.DocSubmissionDetId
                                   join invoiceDet in db.InvoiceCommDet on docSubmissionFactDet.InvoiceCommDetId equals invoiceDet.Id
                                   where invoiceDet.InvoiceCommFactMas.Supplier.Name.Trim().ToLower() == value.Trim().ToLower()
                                   select new
                                   {
                                       //ProceedRealizationDetId = proceedRealizationDet.Id,
                                       ProceedRealizationMasId = proceedRealizationDet.ProceedRealizationMasId,
                                       ProceedTypeId = docSubmissionMas.PaymentTypeId,
                                       ProceedType = docSubmissionMas.PaymentTypeId == 0 ? "LC" : "TT",
                                       FDBCNo = docSubmissionMas.FDBCNo,
                                       ProceedDate = proceedRealizationDet.ProceedRealizationMas.ProceedDate,
                                       ProceedValue = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum(),
                                       RDLInvoiceValue = docSubmissionMas.PaymentTypeId == 0 ? (from realizeMas in db.ProceedRealizationMas
                                                                                                join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                                                                join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                                                                join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                                                                join RDLInvoiceDet in db.InvoiceCommDet on docSubFactDet.InvoiceCommDetId equals RDLInvoiceDet.Id
                                                                                                join FactoryInvoiceMas in db.InvoiceCommFactMas on RDLInvoiceDet.InvoiceCommFactMasId equals FactoryInvoiceMas.Id
                                                                                                join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                                                                join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                                                                join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                                                                join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                                                                where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                                                                select new
                                                                                                {
                                                                                                    RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                                                                }).Select(m => m.RDLValue).Sum()
                                                          :
                                                          (from FactoryInvoiceMas in db.InvoiceCommFactMas
                                                           join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                                                           join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                                                           join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                           join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                                           where buyerDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId
                                                           select new
                                                           {
                                                               RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                                                           }).Select(m => m.RDLValue).Sum(),

                                       Pending = (from realizeMas in db.ProceedRealizationMas
                                                  join realizeDet in db.ProceedRealizationDet on realizeMas.Id equals realizeDet.ProceedRealizationMasId
                                                  join docSubDet in db.DocSubmissionDet on realizeDet.DocSubmissionDetId equals docSubDet.Id
                                                  join docSubFactDet in db.DocSubmissionFactDet on docSubDet.Id equals docSubFactDet.DocSubmissionDetId
                                                  where realizeMas.Id == proceedRealizationDet.ProceedRealizationMasId
                                                  select docSubFactDet).ToList().Count - (docSubmissionMas.PaymentTypeId == 0 ?
                                             (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count()) :
                                             (db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count())),


                                       Paid = docSubmissionMas.PaymentTypeId == 0 ?
                                       db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() :
                                       db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count(),

                                       Amount = db.ProceedRealizationDet.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.ProceedQty).Sum() -
                                               (docSubmissionMas.PaymentTypeId == 0 ?
                                               ((db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                             0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum())) :
                                            ((db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Count() == 0 ?
                                               0 : db.TTPayment.Where(x => x.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId == docSubmissionDet.BuyerOrderMasId).Select(x => x.FDDAmount).Sum())))
                                               ,

                                       TotalFDDAmount = (db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Count() == 0 ?
                                             0 : db.FDDPayment.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).Select(x => x.FDDAmount).Sum()),

                                       CheckCommission = db.CommissionRealization.Where(x => x.ProceedRealizationMasId == proceedRealizationDet.ProceedRealizationMasId).ToList().Count()
                                   }).Distinct().ToList().Select(x => new
                                   {
                                       ProceedRealizationMasId = x.ProceedRealizationMasId,
                                       ProceedTypeId = x.ProceedTypeId,
                                       ProceedType = x.ProceedType,
                                       FDBCNo = x.FDBCNo,
                                       ProceedDate = NullHelpers.DateToString(x.ProceedDate),
                                       ProceedValue = x.ProceedValue,
                                       RDLInvoiceValue = x.RDLInvoiceValue,
                                       Pending = x.Pending,
                                       Paid = x.Paid,
                                       Amount = x.Amount,
                                       TotalFDDAmount = x.TotalFDDAmount,
                                       CheckCommission = x.CheckCommission
                                   }).Distinct();


                if (Factorydata.Count() > 0)
                {

                    var result = new
                    {
                        FDBCdata = Factorydata,
                        flag = true
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var result = new
                    {
                        data = "Not Found!",
                        flag = false
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var result = new
                {
                    data = "Not Found!",
                    flag = false
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: FDDPayment
        public ActionResult Index(int? ProceedRealizationMasId)
        {
            var fDDPayment = db.FDDPayment.Include(f => f.ProceedRealizationMas).Include(f => f.Supplier);
            if (ProceedRealizationMasId != null)
            {
                fDDPayment = fDDPayment.Where(x => x.ProceedRealizationMasId == ProceedRealizationMasId);
            }
            return View(fDDPayment.ToList());
        }

        // GET: FDDPayment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDDPayment fDDPayment = db.FDDPayment.Find(id);
            if (fDDPayment == null)
            {
                return HttpNotFound();
            }
            return View(fDDPayment);
        }

        // GET: FDDPayment/Create
        //public ActionResult Create(int? ProceedTypeId, DateTime? ProceedDate, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue)
        //{
        //    ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo");
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
        //    ViewBag.FactoryInvoice = new SelectList(db.InvoiceCommFactMas, "Id", "InvoiceNoFact");

        //    return View();
        //}


        //public ActionResult CreateFDDPayment(int? ProceedTypeId, DateTime? ProceedDate, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue)
        //{
        //    //return Json(null);
        //    return RedirectToAction("Create", new { ProceedTypeId = ProceedTypeId });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////public ActionResult Create([Bind(Include = "Id,SupplierId,DocSubmissionFactDetId,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        //public ActionResult Create([Bind(Include = "Id,SupplierId,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        fDDPayment.DocSubmissionFactDetId = 23;
        //        db.FDDPayment.Add(fDDPayment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo", fDDPayment.DocSubmissionFactDetId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", fDDPayment.SupplierId);
        //    return View(fDDPayment);
        //}



        // GET: FDDPayment/Create
        public ActionResult Create(int? ProceedTypeId, int? ProceedRealizationMasId, DateTime? ProceedDate, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue, int? FDBCId)
        {

            ViewBag.DocSubmissionFactNO = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo");

            var factData = (from docSubmissionDet in db.DocSubmissionDet
                            join invoiceMas in db.InvoiceCommMas on docSubmissionDet.InvoiceCommMasId equals invoiceMas.Id
                            join invoiceDet in db.InvoiceCommDet on invoiceMas.Id equals invoiceDet.InvoiceCommMasId
                            where docSubmissionDet.DocSubmissionMas.FDBCNo.Trim() == FDBCNO.Trim()
                            select invoiceDet.InvoiceCommFactMas.Supplier).Distinct().ToList();

            ViewBag.SupplierId = new SelectList(factData, "Id", "Name");
            ViewBag.FactoryInvoice = new SelectList(db.InvoiceCommFactMas, "Id", "InvoiceNoFact");

            if (ProceedTypeId == 0)
            {
                var lc = "LC";
                ViewBag.ProceedTypeId = lc;

            }
            else
            {
                var tt = "TT";
                ViewBag.ProceedTypeId = tt;

            }


            return View();
        }


        //public ActionResult CreateFDDPayment(int? ProceedTypeId, DateTime? ProceedDate, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue)
        //{
        //    //return Json(null);
        //    return RedirectToAction("Create", new { ProceedTypeId = ProceedTypeId });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////public ActionResult Create([Bind(Include = "Id,SupplierId,DocSubmissionFactDetId,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        //public ActionResult Create([Bind(Include = "Id,SupplierId,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        fDDPayment.DocSubmissionFactDetId = 23;
        //        db.FDDPayment.Add(fDDPayment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo", fDDPayment.DocSubmissionFactDetId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", fDDPayment.SupplierId);
        //    return View(fDDPayment);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SupplierId,ProceedRealizationMasId,DocSubmissionFactNO,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        {
            if (ModelState.IsValid)
            {
                fDDPayment.ProceedRealizationMasId = fDDPayment.ProceedRealizationMasId;
                db.FDDPayment.Add(fDDPayment);
                db.SaveChanges();

                var FDBCData = db.DocSubmissionFactDet.Where(x => x.FactFDBCNo.Trim() == fDDPayment.DocSubmissionFactNO.Trim()).ToList();

                foreach (var item in FDBCData)
                {


                    var OrderD = new FDDPaymentDet()
                    {
                        Id = 0,
                        DocSubmissionFactDetId = item.Id,
                        FDDPaymentId = fDDPayment.Id
                    };

                    db.FDDPaymentDet.Add(OrderD);
                    db.SaveChanges();
                }

                return RedirectToAction("PendingFDD");
            }

            //ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo", fDDPayment.DocSubmissionFactDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", fDDPayment.SupplierId);
            return View(fDDPayment);
        }

        // GET: FDDPayment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDDPayment fDDPayment = db.FDDPayment.Find(id);
            if (fDDPayment == null)
            {
                return HttpNotFound();
            }
            //ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo", fDDPayment.ProceedRealizationDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", fDDPayment.SupplierId);
            return View(fDDPayment);
        }

        // POST: FDDPayment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierId,DocSubmissionFactDetId,FDDNo,FDDDate,FDDAmount")] FDDPayment fDDPayment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fDDPayment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo", fDDPayment.DocSubmissionFactDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", fDDPayment.SupplierId);
            return View(fDDPayment);
        }

        // GET: FDDPayment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDDPayment fDDPayment = db.FDDPayment.Find(id);
            if (fDDPayment == null)
            {
                return HttpNotFound();
            }
            return View(fDDPayment);
        }

        // POST: FDDPayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FDDPayment fDDPayment = db.FDDPayment.Find(id);
            db.FDDPayment.Remove(fDDPayment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        public JsonResult GetInvoice(int SupplierId, string FDBC)
        {

            var factInvoice = (from DocSubFact in db.DocSubmissionFactDet
                               join invoiceComDet in db.InvoiceCommDet on DocSubFact.InvoiceCommDetId equals invoiceComDet.Id
                               join InvoiceFactMas in db.InvoiceCommFactMas on invoiceComDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
                               where InvoiceFactMas.SupplierId == SupplierId && DocSubFact.DocSubmissionDet.DocSubmissionMas.FDBCNo.Trim() == FDBC.Trim()
                               select InvoiceFactMas).Distinct().ToList();

            List<SelectListItem> siteList = new List<SelectListItem>();

            foreach (var i in factInvoice)
            {
                var site = db.InvoiceCommFactMas.FirstOrDefault(x => x.Id == i.Id);
                siteList.Add(new SelectListItem { Text = site.InvoiceNoFact, Value = site.Id.ToString() });

            }


            var result = new
            {

                Sites = siteList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSelectedInvoiceValue(int InvoiceCommFactMasId)
        {
            //var FactoryInvoiceValue = (from invoiceFactMas in db.InvoiceCommFactMas
            //                           join invoiceFactDet in db.InvoiceCommFactDet on invoiceFactMas.Id equals invoiceFactDet.InvoiceCommFactMasId
            //                           join exFactoryDet in db.ExFactoryDet on invoiceFactDet.ExFactoryDetId equals exFactoryDet.Id
            //                           join exShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exShipDet.ExFactoryDetId
            //                           join factDet in db.FactoryOrderDet on exShipDet.BuyerOrderDetId equals factDet.BuyerOrderDetId
            //                           //where invoiceMas.Id == RDLInvoiceMas.Id
            //                           where invoiceFactMas.Id == InvoiceCommFactMasId
            //                           select new { FactoryInvoice = exShipDet.ShipQuantity * factDet.FOBUnitPrice }).Sum(x => x.FactoryInvoice);


            ////17/10/2018
            //var factInvoice = (from DocSubFact in db.DocSubmissionFactDet
            //                   join invoiceComDet in db.InvoiceCommDet on DocSubFact.InvoiceCommDetId equals invoiceComDet.Id
            //                   join InvoiceFactMas in db.InvoiceCommFactMas on invoiceComDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
            //                   where InvoiceFactMas.Id == InvoiceCommFactMasId
            //                   select DocSubFact).Distinct().ToList();

            //List<SelectListItem> FDBCList = new List<SelectListItem>();

            //foreach (var i in factInvoice)
            //{
            //    FDBCList.Add(new SelectListItem { Text = i.FactFDBCNo, Value = i.Id.ToString() });

            //}

            var factInvoice = (from DocSubFact in db.DocSubmissionFactDet
                               join invoiceComDet in db.InvoiceCommDet on DocSubFact.InvoiceCommDetId equals invoiceComDet.Id
                               join InvoiceFactMas in db.InvoiceCommFactMas on invoiceComDet.InvoiceCommFactMasId equals InvoiceFactMas.Id
                               where InvoiceFactMas.Id == InvoiceCommFactMasId
                               select DocSubFact.FactFDBCNo).Distinct().ToList();

            List<SelectListItem> FDBCList = new List<SelectListItem>();

            foreach (var i in factInvoice)
            {
                FDBCList.Add(new SelectListItem { Text = i, Value = i.ToString() });

            }


            var result = new
            {
                //FactoryInvoice = FactoryInvoiceValue,
                factFDBCList = FDBCList
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }



        //[HttpPost]
        //public JsonResult GetFactInvoiceValue(int DocSubmissionFactDetId)
        //{
        //    var FactoryInvoiceValue = db.DocSubmissionFactDet.SingleOrDefault(x => x.Id == DocSubmissionFactDetId);

        //    if (FactoryInvoiceValue != null)
        //    {
        //        if (FactoryInvoiceValue.DocSubmissionDet.DocSubmissionMas.PaymentTypeId == 0) //LC
        //        {
        //            var result = new
        //            {
        //                FactoryInvoice = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == FactoryInvoiceValue.InvoiceCommDet.InvoiceCommFactMasId).Sum(x => x.InvoiceTotalAmt)
        //            };

        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var result = new
        //            {
        //                FactoryInvoice = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == FactoryInvoiceValue.InvoiceCommDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt)
        //            };

        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    else
        //    {
        //        var result = new
        //        {
        //            FactoryInvoice = 0
        //        };

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public JsonResult GetFactInvoiceValue(string DocSubmissionFactDetId)
        {
            var FactoryInvoiceValue = db.DocSubmissionFactDet.FirstOrDefault(x => x.FactFDBCNo.Trim() == DocSubmissionFactDetId.Trim());

            if (FactoryInvoiceValue != null)
            {
                if (FactoryInvoiceValue.DocSubmissionDet.DocSubmissionMas.PaymentTypeId == 0) //LC
                {
                    var result = new
                    {
                        FactoryInvoice = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == FactoryInvoiceValue.InvoiceCommDet.InvoiceCommFactMasId).Sum(x => x.InvoiceTotalAmt)
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        FactoryInvoice = db.InvoiceCommDet.Where(x => x.InvoiceCommMasId == FactoryInvoiceValue.InvoiceCommDet.InvoiceCommMasId).Sum(x => x.InvoiceRDLTotalAmt)
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var result = new
                {
                    FactoryInvoice = 0
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }





        public JsonResult DeleteFDDPayment(int id)
        {
            bool flag = false;
            try
            {
                var itemToDelete = db.FDDPayment.Find(id);

                if (itemToDelete == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nFDD Payment Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                var check = db.CommissionRealization.Where(x => x.ProceedRealizationMasId == itemToDelete.ProceedRealizationMasId);
                if (check.Count() > 0)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed! Delete comision realization first!."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var fddDet = db.FDDPaymentDet.Where(x => x.FDDPaymentId == itemToDelete.Id).ToList();

                    if(fddDet !=null)
                    {
                        db.FDDPaymentDet.RemoveRange(fddDet);
                    }

                    db.FDDPayment.Remove(itemToDelete);
                }


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

    }
}
