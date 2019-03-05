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

namespace BHMS.Controllers
{
    public class ActionActivityController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ActionActivity
        public ActionResult Index()
        {
            var actionActivityMas = db.ActionActivityMas.Include(a => a.FactoryOrderDelivDet).Include(a => a.TimeActionMas);
            return View(actionActivityMas.ToList());
        }

        // GET: ActionActivity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionActivityMas actionActivityMas = db.ActionActivityMas.Find(id);
            if (actionActivityMas == null)
            {
                return HttpNotFound();
            }
            return View(actionActivityMas);
        }

        // GET: ActionActivity/Create
        public ActionResult Create(int factoryOrderDetId)
        {
            var factoryDet = db.FactoryOrderDet.SingleOrDefault(x => x.Id == factoryOrderDetId);

            ViewBag.BuyerMasId = factoryDet.BuyerOrderDet.BuyerOrderMasId;
            ViewBag.RDLRefNo = factoryDet.BuyerOrderDet.BuyerOrderMas.OrderRefNo;
            ViewBag.OrderDate = NullHelpers.DateToString(factoryDet.BuyerOrderDet.BuyerOrderMas.OrderDate);
            ViewBag.Style = factoryDet.BuyerOrderDet.StyleNo;
            ViewBag.OrderQty = factoryDet.BuyerOrderDet.Quantity;

            ViewBag.TimeActionMasId = new SelectList(db.TimeActionMas.Where(x=>x.BuyerInfoId == factoryDet.BuyerOrderDet.BuyerOrderMas.BuyerInfoId), "Id", "TemplateName");
            //ViewBag.ShipmentSummDetId = new SelectList(db.ShipmentSummDet.Where(x=>x.BuyerOrderDetId == factoryDet.BuyerOrderDetId), "Id", "BuyerSlNo");
            ViewBag.ShipmentSummDetId = new SelectList(db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == factoryDet.BuyerOrderDetId), "Id", "DelivSlno");

            return View();
        }


        public JsonResult SaveActivity(IEnumerable<ActionActivityDet> ActivityDetails, ActionActivityMas ActivityMas, int[] DelItems)
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

                        var factoryDelivDet = db.FactoryOrderDelivDet.SingleOrDefault(x => x.ShipmentSummDetId == ActivityMas.FactoryOrderDelivDetId);

                        var OrderM = new ActionActivityMas()
                        {
                            Id = ActivityMas.Id,
                            TimeActionMasId = ActivityMas.TimeActionMasId,
                            FactoryOrderDelivDetId = factoryDelivDet.Id,
                            PlanFlag = true,
                            RevisedFlag = true
                        };

                        db.Entry(OrderM).State = OrderM.Id == 0 ? EntityState.Added : EntityState.Modified;
                        //db.TimeActionMas.Add(OrderM);
                        db.SaveChanges();


                        foreach (var item in ActivityDetails)
                        {
                            var OrderD = new ActionActivityDet()
                            {
                                Id = item.Id,
                                ActionActivityMasId = OrderM.Id,
                                TimeActionDetId = item.TimeActionDetId,
                                PlanDate = item.PlanDate,
                                RevisedDate = item.RevisedDate,
                                ActualDate = item.ActualDate,
                                Remarks = item.Remarks
                            };

                            db.Entry(OrderD).State = OrderD.Id == 0 ? EntityState.Added : EntityState.Modified;
                            //db.TimeActionDet.Add(OrderD);
                            db.SaveChanges();

                        }


                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                var delOrder = db.ActionActivityDet.Find(item);
                                db.ActionActivityDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = OrderM.FactoryOrderDelivDet.FactoryOrderDet.FactoryOrderMasId
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
        //public ActionResult Create([Bind(Include = "Id,FactoryOrderDelivDetId,TimeActionMasId,PlanFlag,RevisedFlag")] ActionActivityMas actionActivityMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ActionActivityMas.Add(actionActivityMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.FactoryOrderDelivDetId = new SelectList(db.FactoryOrderDelivDet, "Id", "Id", actionActivityMas.FactoryOrderDelivDetId);
        //    ViewBag.TimeActionMasId = new SelectList(db.TimeActionMas, "Id", "TemplateName", actionActivityMas.TimeActionMasId);
        //    return View(actionActivityMas);
        //}

        // GET: ActionActivity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionActivityMas actionActivityMas = db.ActionActivityMas.Find(id);
            if (actionActivityMas == null)
            {
                return HttpNotFound();
            }

            var factoryDet = db.FactoryOrderDet.SingleOrDefault(x => x.Id == actionActivityMas.FactoryOrderDelivDet.FactoryOrderDetId);

            ViewBag.RDLRefNo = factoryDet.BuyerOrderDet.BuyerOrderMas.OrderRefNo;
            ViewBag.OrderDate = NullHelpers.DateToString(factoryDet.BuyerOrderDet.BuyerOrderMas.OrderDate);
            ViewBag.Style = factoryDet.BuyerOrderDet.StyleNo;
            ViewBag.OrderQty = factoryDet.BuyerOrderDet.Quantity;

            ViewBag.TimeActionMasId = new SelectList(db.TimeActionMas.Where(x => x.BuyerInfoId == factoryDet.BuyerOrderDet.BuyerOrderMas.BuyerInfoId), "Id", "TemplateName", actionActivityMas.TimeActionMasId);
            //ViewBag.ShipmentSummDetId = new SelectList(db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == factoryDet.BuyerOrderDetId), "Id", "BuyerSlNo");
            ViewBag.ShipmentSummDetId = new SelectList(db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == factoryDet.BuyerOrderDetId), "Id", "DelivSlno", actionActivityMas.FactoryOrderDelivDet.ShipmentSummDetId);
            //ViewBag.FactoryOrderDelivDetId = new SelectList(db.FactoryOrderDelivDet, "Id", "Id", actionActivityMas.FactoryOrderDelivDetId);
            //ViewBag.TimeActionMasId = new SelectList(db.TimeActionMas, "Id", "TemplateName", actionActivityMas.TimeActionMasId);
            return View(actionActivityMas);
        }



        public JsonResult GetSavedData(int Id)
        {
            var data = (from activityMas in db.ActionActivityMas
                        join activityDet in db.ActionActivityDet on activityMas.Id equals activityDet.ActionActivityMasId
                        where activityMas.Id == Id
                        select new
                        {
                            ActionActivityDetId = activityDet.Id,
                            timeActionDetId = activityDet.TimeActionDetId,
                            ActivityName = activityDet.TimeActionDet.ActivityName,
                            ActivityDays = activityDet.TimeActionDet.ActivityDays,
                            PlanDate = activityDet.PlanDate,
                            RevisedDate = activityDet.RevisedDate,
                            ActualDate = activityDet.ActualDate,
                            Remarks = activityDet.Remarks,
                            Source = activityDet.TimeActionDet.Source
                        }).ToList();



            var refinedata = data.AsEnumerable().Select(x => new
            {
                ActionActivityDetId = x.ActionActivityDetId,
                timeActionDetId = x.timeActionDetId,
                ActivityName = x.ActivityName,
                ActivityDays = x.ActivityDays,
                PlanDate = NullHelpers.DateToString(x.PlanDate),
                RevisedDate = x.RevisedDate==null ? "" : NullHelpers.DateToString(x.RevisedDate),
                ActualDate = x.ActualDate == null ? "" : NullHelpers.DateToString(x.ActualDate),
                Remarks = x.Remarks == null ? "" : x.Remarks,
                Source = x.Source
            }).Distinct();

            return Json(refinedata, JsonRequestBehavior.AllowGet);

        }

        // POST: ActionActivity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FactoryOrderDelivDetId,TimeActionMasId,PlanFlag,RevisedFlag")] ActionActivityMas actionActivityMas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actionActivityMas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FactoryOrderDelivDetId = new SelectList(db.FactoryOrderDelivDet, "Id", "Id", actionActivityMas.FactoryOrderDelivDetId);
            ViewBag.TimeActionMasId = new SelectList(db.TimeActionMas, "Id", "TemplateName", actionActivityMas.TimeActionMasId);
            return View(actionActivityMas);
        }

        // GET: ActionActivity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionActivityMas actionActivityMas = db.ActionActivityMas.Find(id);
            if (actionActivityMas == null)
            {
                return HttpNotFound();
            }
            return View(actionActivityMas);
        }

        // POST: ActionActivity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActionActivityMas actionActivityMas = db.ActionActivityMas.Find(id);
            db.ActionActivityMas.Remove(actionActivityMas);
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
    }
}
