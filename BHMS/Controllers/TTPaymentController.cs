using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHMS.Models;

namespace BHMS.Controllers
{
    public class TTPaymentController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: TTPayment
        public ActionResult Index(int? ProceedRealizationDetId)
        {
            var tTPayment = db.TTPayment.Include(t => t.ProceedRealizationDet).Include(t => t.Supplier);

            if (ProceedRealizationDetId != null)
            {
                tTPayment = tTPayment.Where(x => x.ProceedRealizationDetId == ProceedRealizationDetId);
            }
            return View(tTPayment.ToList());
        }

        // GET: TTPayment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TTPayment tTPayment = db.TTPayment.Find(id);
            if (tTPayment == null)
            {
                return HttpNotFound();
            }
            return View(tTPayment);
        }

        // GET: TTPayment/Create
        //public ActionResult Create()
        //{
        //    ViewBag.DocSubmissionDetId = new SelectList(db.DocSubmissionDet, "Id", "Id");
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
        //    ViewBag.BuyerRefNo = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
        //    return View();
        //}

        public ActionResult Create(int? ProceedTypeId, DateTime? ProceedDate, int? ProceedRealizationDetId, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue)
        {
            //ViewBag.DocSubmissionFactDetId = new SelectList(db.DocSubmissionFactDet, "Id", "FactFDBCNo");
            ViewBag.SupplierId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");

            var refData = (from docSubmissionDet in db.DocSubmissionDet
                           join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                           where docSubmissionDet.DocSubmissionMas.FDBCNo.Trim() == FDBCNO.Trim()
                           select buyerMas).ToList();

            ViewBag.BuyerRefNo = new SelectList(refData, "Id", "OrderRefNo");
            ViewBag.DocSubmissionFactDetId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "FactFDBCNo");

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

        // POST: TTPayment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SupplierId,ProceedRealizationDetId,FDDNo,FDDDate,FDDAmount,DocSubmissionFactDetId")] TTPayment tTPayment)
        {

            if (ModelState.IsValid)
            {
                db.TTPayment.Add(tTPayment);
                db.SaveChanges();
                return RedirectToAction("PendingFDD","FDDPayment",null);
            }

            //ViewBag.DocSubmissionDetId = new SelectList(db.DocSubmissionDet, "Id", "Id", tTPayment.ProceedRealizationDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", tTPayment.SupplierId);
            return View(tTPayment);
        }

        // GET: TTPayment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TTPayment tTPayment = db.TTPayment.Find(id);
            if (tTPayment == null)
            {
                return HttpNotFound();
            }

            var refData = (from docSubmissionDet in db.DocSubmissionDet
                           join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                           where docSubmissionDet.Id== tTPayment.ProceedRealizationDet.DocSubmissionDetId
                           select buyerMas).ToList();

            ViewBag.BuyerRefNo = new SelectList(refData, "Id", "OrderRefNo", tTPayment.ProceedRealizationDet.DocSubmissionDet.BuyerOrderMasId);
            ViewBag.ProceedRealizationDetId = new SelectList(db.DocSubmissionDet, "Id", "Id", tTPayment.ProceedRealizationDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", tTPayment.SupplierId);
            return View(tTPayment);
        }

        // POST: TTPayment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierId,ProceedRealizationDetId,FDDNo,FDDDate,FDDAmount")] TTPayment tTPayment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tTPayment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProceedRealizationDetId = new SelectList(db.DocSubmissionDet, "Id", "Id", tTPayment.ProceedRealizationDetId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", tTPayment.SupplierId);
            return View(tTPayment);
        }

        // GET: TTPayment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TTPayment tTPayment = db.TTPayment.Find(id);
            if (tTPayment == null)
            {
                return HttpNotFound();
            }
            return View(tTPayment);
        }

        // POST: TTPayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TTPayment tTPayment = db.TTPayment.Find(id);
            db.TTPayment.Remove(tTPayment);
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
        public JsonResult GetSupplier(int BuyerRefNo)
        {

            var suppliers = (from docSubmissionDet in db.DocSubmissionDet
                             join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                             join buyerDet in db.BuyerOrderDet on buyerMas.Id equals buyerDet.BuyerOrderMasId
                             join supplier in db.Supplier on buyerDet.SupplierId equals supplier.Id
                             where buyerMas.Id == BuyerRefNo
                             select supplier).Distinct().ToList();

            List<SelectListItem> supplierList = new List<SelectListItem>();

            foreach (var i in suppliers)
            {
                supplierList.Add(new SelectListItem { Text = i.Name, Value = i.Id.ToString() });

            }


            var result = new
            {

                Suppliers = supplierList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult GetFactFDBC(int BuyerRefNo,int SupplierId)
        {
            var suppliers = (from docSubmissionDet in db.DocSubmissionDet
                             join docSubmissionFactDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubmissionFactDet.DocSubmissionDetId
                             join buyerMas in db.BuyerOrderMas on docSubmissionDet.BuyerOrderMasId equals buyerMas.Id
                             join buyerDet in db.BuyerOrderDet on buyerMas.Id equals buyerDet.BuyerOrderMasId
                             join supplier in db.Supplier on buyerDet.SupplierId equals supplier.Id
                             where buyerMas.Id == BuyerRefNo && buyerDet.SupplierId == SupplierId
                             select docSubmissionFactDet).Distinct().ToList();

            List<SelectListItem> FactFDBC = new List<SelectListItem>();

            foreach (var i in suppliers)
            {
                FactFDBC.Add(new SelectListItem { Text = i.FactFDBCNo, Value = i.Id.ToString() });

            }


            var result = new
            {

                FactFDBC = FactFDBC
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetFactoryInvoice(int SupplierId, int realizatiomDetId)
        {
            var FactinvoicData = (from realizatiomDet in db.ProceedRealizationDet
                                  join docSubmissionDet in db.DocSubmissionDet on realizatiomDet.DocSubmissionDetId equals docSubmissionDet.Id
                                  join docSubDetailDet in db.DocSubmissionFactDet on docSubmissionDet.Id equals docSubDetailDet.DocSubmissionDetId
                                  join InvoiceFactDet in db.InvoiceCommFactDet on docSubDetailDet.InvoiceCommFactDetId equals InvoiceFactDet.Id
                                  join exfactoryDet in db.ExFactoryDet on InvoiceFactDet.ExFactoryDetId equals exfactoryDet.Id
                                  join exFactoryMas in db.ExFactoryMas on exfactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                  join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                  join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id } equals new { ColA = buyerOrderDet.BuyerOrderMasId }
                                  join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                  join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id } equals new { ColA = shipDet.BuyerOrderDetId }
                                  join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                  from pJoin in joinDest.DefaultIfEmpty()
                                  where realizatiomDet.Id == realizatiomDetId
                                  select new
                                  {
                                      FactoryId = InvoiceFactDet.InvoiceCommFactMas.SupplierId,
                                      FactoryName = InvoiceFactDet.InvoiceCommFactMas.Supplier.Name,
                                      FactoryInvoiceId = InvoiceFactDet.InvoiceCommFactMas.Id,
                                      FactoryInvoiceNo = InvoiceFactDet.InvoiceCommFactMas.InvoiceNoFact,
                                      //FactoryInvoiceValue = (from invoiceFactMas1 in db.InvoiceCommFactMas
                                      //                       join invoiceFactDet1 in db.InvoiceCommFactDet on invoiceFactMas1.Id equals invoiceFactDet1.InvoiceCommFactMasId
                                      //                       join exFactoryDet1 in db.ExFactoryDet on invoiceFactDet1.ExFactoryDetId equals exFactoryDet1.Id
                                      //                       join exShipDet1 in db.ExFactoryShipDet on exFactoryDet1.Id equals exShipDet1.ExFactoryDetId
                                      //                       join factDet in db.FactoryOrderDet on exShipDet1.BuyerOrderDetId equals factDet.BuyerOrderDetId
                                      //                       //where invoiceMas.Id == RDLInvoiceMas.Id
                                      //                       where exFactoryDet1.ExFactoryMasId == exFactoryMas.Id
                                      //                       select new { FactoryInvoice = exShipDet1.ShipQuantity * factDet.FOBUnitPrice }).Sum(x => x.FactoryInvoice)
                                      FactoryInvoiceValue= InvoiceFactDet.InvoiceTotalAmt

                                  }).Distinct().ToList();


            return Json(FactinvoicData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTTPayment(int id)
        {
            bool flag = false;
            try
            {
                var itemToDelete = db.TTPayment.Find(id);

                if (itemToDelete == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nTT Payment Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                db.TTPayment.Remove(itemToDelete);

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
