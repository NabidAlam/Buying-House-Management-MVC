using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHMS.Models;
using BHMS.ViewModels;
using BHMS.Helpers;

namespace BHMS.Controllers
{
    public class TimeActionMasController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: TimeActionMas
        public ActionResult Index()
        {
            var timeActionMas = db.TimeActionMas.Include(t => t.BuyerInfo).Include(t => t.CompanyResource).Include(t => t.UserDept);
            return View(timeActionMas.ToList());
        }

        // GET: TimeActionMas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeActionMas timeActionMas = db.TimeActionMas.Find(id);
            if (timeActionMas == null)
            {
                return HttpNotFound();
            }
            return View(timeActionMas);
        }

        // GET: TimeActionMas/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.CompanyResourceId = new SelectList(db.CompanyResources, "Id", "Name");
            ViewBag.UserDeptId = new SelectList(db.UserDepts, "Id", "DeptName");
            return View();
        }


        public JsonResult SaveTimeAction(IEnumerable<TimeActionDet> ActionDetails, TimeActionMas ActionMas, int[] DelItems)
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

                        var OrderM = new TimeActionMas()
                        {
                            Id = ActionMas.Id,
                            TemplateName = ActionMas.TemplateName,
                            BuyerInfoId = ActionMas.BuyerInfoId,
                            UserDeptId = ActionMas.UserDeptId,
                            CompanyResourceId = ActionMas.CompanyResourceId,
                        };

                        db.Entry(OrderM).State = OrderM.Id == 0 ? EntityState.Added : EntityState.Modified;
                        //db.TimeActionMas.Add(OrderM);
                        db.SaveChanges();


                        foreach (var item in ActionDetails)
                        {
                            var OrderD = new TimeActionDet()
                            {
                                Id = item.Id,
                                TimeActionMasId= OrderM.Id,
                                ActivityName = item.ActivityName,
                                ActivityDays = item.ActivityDays,
                                Source = item.Source ==0 ? null : item.Source
                            };

                            db.Entry(OrderD).State = OrderD.Id == 0 ? EntityState.Added : EntityState.Modified;
                            //db.TimeActionDet.Add(OrderD);
                            db.SaveChanges();

                        }


                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                var delOrder = db.TimeActionDet.Find(item);
                                db.TimeActionDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = OrderM.Id
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


        public JsonResult GetSourceForm()
        {
            var data = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Order Confirmation Date", Value = "1" }, new SelectListItem { Text = "Exfactory Date", Value = "2" }, }, "Value", "Text");

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetTimeActionList(int Id,int BuyerMasId)
        {
            List<VMActionActivityList> list = new List<VMActionActivityList>();

            var data = (from timeActionDet in db.TimeActionDet
                        where timeActionDet.TimeActionMasId == Id
                        select new
                        {
                            timeActionDetId = timeActionDet.Id,
                            ActivityName = timeActionDet.ActivityName,
                            ActivityDays = timeActionDet.ActivityDays,
                            Source = timeActionDet.Source
                        }).ToList();


            foreach(var item in data)
            {
                VMActionActivityList vm = new VMActionActivityList();
                vm.timeActionDetId = item.timeActionDetId;
                vm.ActivityName = item.ActivityName;
                vm.ActivityDays = item.ActivityDays;
               
                if(item.Source == 1)
                {
                    vm.PlanDate = NullHelpers.DateToString(db.BuyerOrderMas.SingleOrDefault(x => x.Id == BuyerMasId).OrderDate);
                    vm.Source = "Order Confirmation Date";
                }
                else if(item.Source == 2)
                {
                    //Exfactory maybe will come from FactoryOrderDelivDet
                    //vm.PlanDate = NullHelpers.DateToString(db.ExFactoryDet.FirstOrDefault(x=>x.BuyerOrderMasId == BuyerMasId).ExFactoryMas.ExFactoryDate);
                    vm.PlanDate = NullHelpers.DateToString(db.ExFactoryDet.FirstOrDefault(x => x.BuyerOrderMasId == BuyerMasId).ExFactoryMas.ExFactoryDate);
                    vm.Source = "Exfactory Date";
                }
                else
                {
                    vm.PlanDate = "";
                    vm.Source = "";
                }

                list.Add(vm);
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        

        public JsonResult GetSavedData(int Id)
        {
            var data = (from timeActionMas in db.TimeActionMas
                        join timeActionDet in db.TimeActionDet on timeActionMas.Id equals timeActionDet.TimeActionMasId
                        where timeActionMas.Id == Id
                        select new
                        {
                            TimeActionDetId = timeActionDet.Id,
                            ActivityName = timeActionDet.ActivityName,
                            ActivityDays = timeActionDet.ActivityDays,
                            Source = timeActionDet.Source
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,TemplateName,BuyerInfoId,UserDeptId,CompanyResourceId")] TimeActionMas timeActionMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TimeActionMas.Add(timeActionMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", timeActionMas.BuyerInfoId);
        //    ViewBag.CompanyResourceId = new SelectList(db.CompanyResources, "Id", "Name", timeActionMas.CompanyResourceId);
        //    ViewBag.UserDeptId = new SelectList(db.UserDepts, "Id", "DeptName", timeActionMas.UserDeptId);
        //    return View(timeActionMas);
        //}

        // GET: TimeActionMas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeActionMas timeActionMas = db.TimeActionMas.Find(id);
            if (timeActionMas == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", timeActionMas.BuyerInfoId);
            ViewBag.CompanyResourceId = new SelectList(db.CompanyResources, "Id", "Name", timeActionMas.CompanyResourceId);
            ViewBag.UserDeptId = new SelectList(db.UserDepts, "Id", "DeptName", timeActionMas.UserDeptId);
            return View(timeActionMas);
        }

        // POST: TimeActionMas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TemplateName,BuyerInfoId,UserDeptId,CompanyResourceId")] TimeActionMas timeActionMas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeActionMas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", timeActionMas.BuyerInfoId);
            ViewBag.CompanyResourceId = new SelectList(db.CompanyResources, "Id", "Name", timeActionMas.CompanyResourceId);
            ViewBag.UserDeptId = new SelectList(db.UserDepts, "Id", "DeptName", timeActionMas.UserDeptId);
            return View(timeActionMas);
        }

        //// GET: TimeActionMas/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeActionMas timeActionMas = db.TimeActionMas.Find(id);
        //    if (timeActionMas == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(timeActionMas);
        //}

        //// POST: TimeActionMas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TimeActionMas timeActionMas = db.TimeActionMas.Find(id);
        //    db.TimeActionMas.Remove(timeActionMas);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}



        public JsonResult DeleteTimeAction(int id)
        {
            bool flag = false;
            try
            {
                var itemToDeleteMas = db.TimeActionMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nTime & Action Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var itemsToDeleteDet = db.TimeActionDet.Where(x => x.TimeActionMasId == id);

                db.TimeActionDet.RemoveRange(itemsToDeleteDet);
                db.TimeActionMas.Remove(itemToDeleteMas);

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
