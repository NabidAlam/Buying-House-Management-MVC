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
    public class FabricItemController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: FabricItem
        public ActionResult Index()
        {
            return View(db.FabricItem.ToList());
        }


        public JsonResult GetDesc(int Id)
        {
            //var data = db.FabricItem.Where(x => x.FabricTypeId == Id).OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();
            var data = db.FabricItemDet.Where(x => x.FabricItem.FabricTypeId == Id).OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        // GET: FabricItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricItem fabricItem = db.FabricItem.Find(id);
            if (fabricItem == null)
            {
                return HttpNotFound();
            }
            return View(fabricItem);
        }

        // GET: FabricItem/Create
        public ActionResult Create()
        {
            ViewBag.FabricTypeId = new SelectList(db.FabricType, "Id", "Name");
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.ProdCategoryId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");

            return View();
        }

        // POST: FabricItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn,FabricTypeId")] FabricItem fabricItem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //if (db.FabricItem.Where(x => x.Name == fabricItem.Name).Count() > 0)
        //        //{
        //        //    Danger("Name exists! Try different.", true);
        //        //}
        //        //else
        //        //{
        //        fabricItem.OpBy = 1;
        //        fabricItem.OpOn = DateTime.Now;
        //        db.FabricItem.Add(fabricItem);
        //        db.SaveChanges();
        //        Success("Saved successfully", true);
        //        return RedirectToAction("Index");
        //        //}

        //    }

        //    return View(fabricItem);
        //}

        // GET: FabricItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricItem fabricItem = db.FabricItem.Find(id);

            //ViewBag.FabricType = new SelectList(db.FabricType, "Id", "Name", fabricItem.FabricTypeId);
            ViewBag.FabricTypeId = new SelectList(db.FabricType, "Id", "Name", fabricItem.FabricTypeId);

            //ViewBag.FabricTypeId = new SelectList(db.FabricType, "Id", "Name", fabricItem.FabricTypeId);
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", fabricItem.FabricType.ProdCategory.BuyerInfoId);
            ViewBag.ProdCategoryId = new SelectList(db.ProdCategory, "Id", "Name", fabricItem.FabricType.ProdCategoryId);


            if (fabricItem == null)
            {
                return HttpNotFound();
            }
            return View(fabricItem);
        }

        // POST: FabricItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn,FabricTypeId")] FabricItem fabricItem)
        {
            if (ModelState.IsValid)
            {
                if (db.FabricItem.Where(x => x.Name == fabricItem.Name && x.Id != fabricItem.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    fabricItem.OpBy = 1;
                    fabricItem.OpOn = DateTime.Now;
                    db.Entry(fabricItem).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully", true);
                    return RedirectToAction("Index");
                }

            }
            return View(fabricItem);
        }

        // GET: FabricItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricItem fabricItem = db.FabricItem.Find(id);
            if (fabricItem == null)
            {
                return HttpNotFound();
            }
            return View(fabricItem);
        }

        // POST: FabricItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FabricItem fabricItem = db.FabricItem.Find(id);
            db.FabricItem.Remove(fabricItem);
            db.SaveChanges();
            Success("Deleted successfully", false);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames()
        {
            var data = db.FabricItem.Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetFabricTypeFromProdType(int Id)
        {
            var data = db.FabricType.Where(x => x.ProdCategoryId == Id).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }



        public JsonResult UpdateFabricInfo(IEnumerable<FabricItemDet> fabricItemDet, FabricItem fabricItem, int[] DeletedItems)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            //return Json(result, JsonRequestBehavior.AllowGet);

            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var fabricMas = db.FabricItem.Find(fabricItem.Id);

                        if (fabricMas == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        fabricMas.FabricTypeId = fabricItem.FabricTypeId;
                        fabricMas.OpBy = 1;
                        fabricMas.OpOn = OpDate;
                        fabricMas.IsAuth = true;
                        db.Entry(fabricMas).State = EntityState.Modified;
                        db.SaveChanges();

                        //---- Delete MasterLCInfoOrderDet, MasterLCInfoDet

                        //if (fabricItemDet != null)
                        //{
                        //    foreach (var item in fabricItemDet)
                        //    {
                        //        var orderDets = db.FabricItemDet.Find(item);

                        //        if (orderDets != null)
                        //        {
                        //            db.FabricItemDet.Remove(orderDets);
                        //        }
                        //        db.SaveChanges();
                        //    }
                        //}

                        //if (DeletedItems != null)
                        //{
                        //    foreach (var item in DeletedItems)
                        //    {

                        //        var orderDets = db.FabricItemDet.Where(x => x.Id == item);

                        //        if (orderDets != null)
                        //        {
                        //            db.FabricItemDet.RemoveRange(orderDets);
                        //        }

                        //        var delLCDet = db.FabricItemDet.Find(item);
                        //        db.FabricItemDet.Remove(delLCDet);
                        //        db.SaveChanges();
                        //    }
                        //}

                        //Dictionary<int, int> dictionary =
                        //        new Dictionary<int, int>();

                        foreach (var item in fabricItemDet)
                        {
                            item.FabricItemId = fabricItem.Id;
                            db.Entry(item).State = item.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            db.SaveChanges();

                            //  dictionary.Add(item.BuyerOrderMasId, item.Id);

                        }


                        if (DeletedItems != null)
                        {
                            foreach (var item in DeletedItems)
                            {
                                var delOrder = db.FabricItemDet.Find(item);
                                db.FabricItemDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Update successful !!"
                        };

                        Success("Updated successfully.", true);


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



        public JsonResult SaveFabricInfo(IEnumerable<FabricItemDet> fabricItemDet, FabricItem fabricItem)
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
                        fabricItem.OpBy = 1;
                        fabricItem.OpOn = OpDate;
                        fabricItem.IsAuth = true;

                        db.FabricItem.Add(fabricItem);
                        db.SaveChanges();

                        //Dictionary<int, int> dictionary =
                        //        new Dictionary<int, int>();


                        foreach (var item in fabricItemDet)
                        {

                            item.FabricItemId = fabricItem.Id;
                            db.FabricItemDet.Add(item);
                            db.SaveChanges();

                            //dictionary.Add(item.TempOrderDetId, OrderD.Id);
                            //dictionary.Add(item.BuyerOrderMasId, item.Id);

                        }




                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = fabricItem.Id
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






        public JsonResult GetDetailData(int Id)
        {
            var list = (from fabricItemDet in db.FabricItemDet
                        where fabricItemDet.FabricItemId == Id
                        select new { fabricItemDet }).AsEnumerable()
                       .Select(x => new
                       {
                           Id = x.fabricItemDet.Id,
                           FabricItemId = x.fabricItemDet.FabricItemId,
                           Name = x.fabricItemDet.Name ?? "",
                           Description = x.fabricItemDet.Description ?? ""
                       });

            return Json(list, JsonRequestBehavior.AllowGet);
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
