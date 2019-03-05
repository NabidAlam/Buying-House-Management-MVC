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
    public class BuyerCashAdjustmentsController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: BuyerCashAdjustments
        public ActionResult Index()
        {
            //var buyerCashAdjustment = db.BuyerCashAdjustment.Include(b => b.BuyerInfo);
            //return View(buyerCashAdjustment.ToList());

            var buyerCash = db.BuyerCashAdjustment;
            return View(buyerCash.ToList());

        }

        // GET: BuyerCashAdjustments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerCashAdjustment buyerCashAdjustment = db.BuyerCashAdjustment.Find(id);
            if (buyerCashAdjustment == null)
            {
                return HttpNotFound();
            }
            return View(buyerCashAdjustment);
        }

        // GET: BuyerCashAdjustments/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            return View();
        }

        // POST: BuyerCashAdjustments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult Create([Bind(Include = "Id,BuyerInfoId,BuyerAdjustDate,BuyerReciptNo,BuyerAdjustAmount,BuyerAdjustRemarks,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] BuyerCashAdjustment buyerCashAdjustment)
        public ActionResult Create([Bind(Include = "Id,BuyerInfoId,BuyerAdjustDate,BuyerReciptNo,BuyerAdjustAmount,BuyerAdjustRemarks")] BuyerCashAdjustment buyerCashAdjustment)
        {
            if (ModelState.IsValid)
            {
                //db.BuyerCashAdjustment.Add(buyerCashAdjustment);
                //db.SaveChanges();
                //return RedirectToAction("Index");

                buyerCashAdjustment.OpBy = 1;
                buyerCashAdjustment.OpOn = DateTime.Now;
                buyerCashAdjustment.IsAuth = false;
                db.BuyerCashAdjustment.Add(buyerCashAdjustment);
                db.SaveChanges();
                //Success("Saved successfully !!", true);
                return RedirectToAction("Index");

            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", buyerCashAdjustment.BuyerInfoId);
            return View(buyerCashAdjustment);
        }

        // GET: BuyerCashAdjustments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerCashAdjustment buyerCashAdjustment = db.BuyerCashAdjustment.Find(id);
            if (buyerCashAdjustment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", buyerCashAdjustment.BuyerInfoId);
            return View(buyerCashAdjustment);
        }

        // POST: BuyerCashAdjustments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,BuyerAdjustDate,BuyerReciptNo,BuyerAdjustAmount,BuyerAdjustRemarks,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] BuyerCashAdjustment buyerCashAdjustment)
        public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,BuyerAdjustDate,BuyerReciptNo,BuyerAdjustAmount,BuyerAdjustRemarks,EntryDate")] BuyerCashAdjustment buyerCashAdjustment)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(buyerCashAdjustment).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");

                buyerCashAdjustment.OpBy = 1;
                buyerCashAdjustment.OpOn = DateTime.Now;
                buyerCashAdjustment.IsAuth = false;
                db.Entry(buyerCashAdjustment).State = EntityState.Modified;
                db.SaveChanges();
                //Success("Saved successfully !!", true);
                return RedirectToAction("Index");

            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", buyerCashAdjustment.BuyerInfoId);
            return View(buyerCashAdjustment);
        }

        // GET: BuyerCashAdjustments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerCashAdjustment buyerCashAdjustment = db.BuyerCashAdjustment.Find(id);
            if (buyerCashAdjustment == null)
            {
                return HttpNotFound();
            }
            return View(buyerCashAdjustment);
        }

        // POST: BuyerCashAdjustments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BuyerCashAdjustment buyerCashAdjustment = db.BuyerCashAdjustment.Find(id);
            db.BuyerCashAdjustment.Remove(buyerCashAdjustment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //------------------------------------------------------SAVE-----------------------------------------------
        public JsonResult SaveBuyerCashAdjustment(IEnumerable<BuyerCashAdjustment> buyerCashAdjustment, DateTime EntryDate)
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


                        foreach (var item in buyerCashAdjustment)
                        {
                            var OrderD = new BuyerCashAdjustment()
                            {
                                Id = 0,
                                EntryDate = EntryDate,
                                BuyerInfoId = item.BuyerInfoId,
                                BuyerAdjustDate = item.BuyerAdjustDate,
                                BuyerReciptNo = item.BuyerReciptNo,
                                BuyerAdjustAmount = item.BuyerAdjustAmount,
                                BuyerAdjustRemarks = item.BuyerAdjustRemarks,
                                IsAuth = false,
                                OpBy = 1,
                                OpOn = DateTime.Now
                                // AuthBy = ,
                                // AuthOn = ,

                            };

                            db.BuyerCashAdjustment.Add(OrderD);
                            db.SaveChanges();



                        }

                        //---- shipment data


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!",
                            Id = 0
                        };

                        Success("Record saved successfully.", true);


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
