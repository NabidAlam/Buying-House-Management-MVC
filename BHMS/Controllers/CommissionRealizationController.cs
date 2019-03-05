using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHMS.Helpers;
using System.Net;

namespace BHMS.Controllers
{
    public class CommissionRealizationController : Controller
    {


        private ModelBHMS db = new ModelBHMS();

        public ActionResult PendingRealization()
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

        // GET: CommissionRealization
        public ActionResult Index(int? ProceedRealizationMasId)
        {
            var list = db.CommissionRealization.ToList();
            if (ProceedRealizationMasId != null)
            {
                list = list.Where(x => x.ProceedRealizationMasId == ProceedRealizationMasId).ToList();
            }
            return View(list);
        }

        public ActionResult Create(int? ProceedTypeId, DateTime? ProceedDate, string FDBCNO, decimal? RDLInvoiceValue, decimal? ProceedValue, decimal? TotalFDDAmount, int? ProceedRealizationMasId)
        {

            if (ProceedTypeId == 0)
            {
                ViewBag.ProceedTypeId = "LC";

            }
            else
            {
                ViewBag.ProceedTypeId = "TT";
            }

            var diff = ProceedValue - TotalFDDAmount;
            //var commInTk= diff * 

            //ViewBag.ProceedTypeId = "ProceedTypeId";
            ViewBag.ProceedDate = ProceedDate;
            ViewBag.FDBCNO = FDBCNO;
            ViewBag.RDLInvoiceValue = RDLInvoiceValue;
            ViewBag.ProceedValue = ProceedValue;
            ViewBag.TotalFDDAmount = TotalFDDAmount;
            ViewBag.CommissionInUSD = diff;

            var ait = db.AIT.FirstOrDefault().AITPercent;

            ViewBag.AitHidden = ait;

            ViewBag.BankChargeTk = db.BankCharge.FirstOrDefault(x => x.PaymentTypeId == ProceedTypeId).Charge;

            ViewBag.ProceedRealizationMasId = ProceedRealizationMasId;

            return View();
        }





        public JsonResult SaveData(CommissionRealization InvoiceDetails)
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

                        db.CommissionRealization.Add(InvoiceDetails);
                        db.SaveChanges();


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!",
                            Id = InvoiceDetails.Id
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



        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionRealization realization = db.CommissionRealization.Find(id);
            if (realization == null)
            {
                return HttpNotFound();
            }
            return View(realization);
        }


        // GET: FDDPayment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionRealization realization = db.CommissionRealization.Find(id);
            if (realization == null)
            {
                return HttpNotFound();
            }
            return View(realization);
        }

        // POST: FDDPayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommissionRealization realization = db.CommissionRealization.Find(id);
            db.CommissionRealization.Remove(realization);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult DeleteRealization(int id)
        {
            bool flag = false;
            try
            {
                var itemToDelete = db.CommissionRealization.Find(id);

                if (itemToDelete == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nCommission Realization Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                db.CommissionRealization.Remove(itemToDelete);

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