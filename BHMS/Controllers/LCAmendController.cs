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
    public class LCAmendController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: LCAmend
        public ActionResult Index()
        {
            var lCAmendInfo = db.LCAmendInfo.Include(l => l.MasterLCInfoMas);
            return View(lCAmendInfo.ToList());
        }

        // GET: LCAmend/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LCAmendInfo lCAmendInfo = db.LCAmendInfo.Find(id);
            if (lCAmendInfo == null)
            {
                return HttpNotFound();
            }
            return View(lCAmendInfo);
        }

        // GET: LCAmend/Create
        public ActionResult Create()
        {
            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo");

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text");

            ViewBag.AmendPaymentTerm = paymentTerm;
            return View();
        }

        // POST: LCAmend/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MasterLCInfoMasId,AmendDate,AmendLCNo,AmendLCRecvDate,AmendLatestShipDate,AmendQuantity,AmendTotalValue,AmendLCExpiryDate,AmendPaymentTerm,AmendTenor")] LCAmendInfo lCAmendInfo)
        {
            if (ModelState.IsValid)
            {
                db.LCAmendInfo.Add(lCAmendInfo);
                db.SaveChanges();
                Success("Saved Successfully", true);
                return RedirectToAction("Index");
            }

            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCAmendInfo.MasterLCInfoMasId);

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text", lCAmendInfo.AmendPaymentTerm);
            ViewBag.AmendPaymentTerm = paymentTerm;

            return View(lCAmendInfo);
        }

        // GET: LCAmend/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LCAmendInfo lCAmendInfo = db.LCAmendInfo.Find(id);
            if (lCAmendInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCAmendInfo.MasterLCInfoMasId);

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text", lCAmendInfo.AmendPaymentTerm);
            ViewBag.AmendPaymentTerm = paymentTerm;

            return View(lCAmendInfo);
        }

        // POST: LCAmend/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MasterLCInfoMasId,AmendDate,AmendLCNo,AmendLCRecvDate,AmendLatestShipDate,AmendQuantity,AmendTotalValue,AmendLCExpiryDate,AmendPaymentTerm,AmendTenor")] LCAmendInfo lCAmendInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lCAmendInfo).State = EntityState.Modified;
                db.SaveChanges();
                Success("Saved Successfully", true);
                return RedirectToAction("Index");
            }
            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCAmendInfo.MasterLCInfoMasId);

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text", lCAmendInfo.AmendPaymentTerm);
            ViewBag.AmendPaymentTerm = paymentTerm;


            return View(lCAmendInfo);
        }

        //// GET: LCAmend/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LCAmendInfo lCAmendInfo = db.LCAmendInfo.Find(id);
        //    if (lCAmendInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(lCAmendInfo);
        //}

        //// POST: LCAmend/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    LCAmendInfo lCAmendInfo = db.LCAmendInfo.Find(id);
        //    db.LCAmendInfo.Remove(lCAmendInfo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}



        public JsonResult DeleteAmend(int id)
        {
            //string result = "";

            bool flag = false;
            try
            {

                var itemToDelete = db.LCAmendInfo.Find(id);

                if (itemToDelete == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nAmend info not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                db.LCAmendInfo.Remove(itemToDelete);

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
