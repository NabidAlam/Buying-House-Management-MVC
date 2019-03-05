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
    public class FactoryCashAdjustmentsController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: FactoryCashAdjustments
        public ActionResult Index()
        {
            //var factoryCashAdjustment = db.FactoryCashAdjustment.Include(f => f.Supplier);
            //return View(factoryCashAdjustment.ToList());

            var factCash = db.FactoryCashAdjustment;
            return View(factCash.ToList());
        }

        // GET: FactoryCashAdjustments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactoryCashAdjustment factoryCashAdjustment = db.FactoryCashAdjustment.Find(id);
            if (factoryCashAdjustment == null)
            {
                return HttpNotFound();
            }
            return View(factoryCashAdjustment);
        }

        // GET: FactoryCashAdjustments/Create
        public ActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }

        // POST: FactoryCashAdjustments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,SupplierId,FacAdjustDate,FacReciptNo,FacAdjustAmount,FacAdjustRemarks,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] FactoryCashAdjustment factoryCashAdjustment)
        public ActionResult Create([Bind(Include = "Id,SupplierId,FacAdjustDate,FacReciptNo,FacAdjustAmount,FacAdjustRemarks")] FactoryCashAdjustment factoryCashAdjustment)
        {
            if (ModelState.IsValid)
            {
                //db.FactoryCashAdjustment.Add(factoryCashAdjustment);
                //db.SaveChanges();
                //return RedirectToAction("Index");

                factoryCashAdjustment.OpBy = 1;
                factoryCashAdjustment.OpOn = DateTime.Now;
                factoryCashAdjustment.IsAuth = false;
                db.FactoryCashAdjustment.Add(factoryCashAdjustment);
                db.SaveChanges();
                //Success("Saved successfully !!", true);
                return RedirectToAction("Index");

            }

            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryCashAdjustment.SupplierId);
            return View(factoryCashAdjustment);
        }


        // GET: FactoryCashAdjustments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactoryCashAdjustment factoryCashAdjustment = db.FactoryCashAdjustment.Find(id);
            if (factoryCashAdjustment == null)
            {
                return HttpNotFound();
            }
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryCashAdjustment.SupplierId);
            return View(factoryCashAdjustment);
        }

        // POST: FactoryCashAdjustments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierId,FacAdjustDate,FacReciptNo,FacAdjustAmount,FacAdjustRemarks,EntryDate")] FactoryCashAdjustment factoryCashAdjustment)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(factoryCashAdjustment).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");

                factoryCashAdjustment.OpBy = 1;
                factoryCashAdjustment.OpOn = DateTime.Now;
                factoryCashAdjustment.IsAuth = false;
                db.Entry(factoryCashAdjustment).State = EntityState.Modified;
                db.SaveChanges();
                //Success("Saved successfully !!", true);                
                return RedirectToAction("Index");

            }


            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryCashAdjustment.SupplierId);
            return View(factoryCashAdjustment);
        }

        // GET: FactoryCashAdjustments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactoryCashAdjustment factoryCashAdjustment = db.FactoryCashAdjustment.Find(id);
            if (factoryCashAdjustment == null)
            {
                return HttpNotFound();
            }
            return View(factoryCashAdjustment);
        }

        // POST: FactoryCashAdjustments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FactoryCashAdjustment factoryCashAdjustment = db.FactoryCashAdjustment.Find(id);
            db.FactoryCashAdjustment.Remove(factoryCashAdjustment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
        //------------------------------------------------------SAVE-----------------------------------------------
        public JsonResult SaveFactoryCashAdjustment(IEnumerable<FactoryCashAdjustment> factoryCashAdjustment, DateTime EntryDate)
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
                                         

                        foreach (var item in factoryCashAdjustment)
                        {
                            var OrderD = new FactoryCashAdjustment()
                            {
                                Id = 0,
                                EntryDate = EntryDate,
                                SupplierId = item.SupplierId,
                                FacAdjustDate = item.FacAdjustDate,
                                FacReciptNo = item.FacReciptNo,
                                FacAdjustAmount = item.FacAdjustAmount,
                                FacAdjustRemarks = item.FacAdjustRemarks,
                                IsAuth = false,
                                OpBy = 1,
                                OpOn = DateTime.Now
                               // AuthBy = ,
                               // AuthOn = ,
                             
                            };

                            db.FactoryCashAdjustment.Add(OrderD);
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
