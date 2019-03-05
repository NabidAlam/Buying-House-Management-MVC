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

namespace BHMS.Controllers
{
    public class CommissionTemplateController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: CommissionTemplate
        public ActionResult Index()
        {
            var commissionDistTempMas = db.CommissionDistTempMas.Include(c => c.BuyerInfo).Include(c => c.ProdDepartment);
            return View(commissionDistTempMas.ToList());
        }

        // GET: CommissionTemplate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionDistTempMas commissionDistTempMas = db.CommissionDistTempMas.Find(id);
            if (commissionDistTempMas == null)
            {
                return HttpNotFound();
            }
            return View(commissionDistTempMas);
        }

        // GET: CommissionTemplate/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            //ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment, "Id", "Name");
            return View();
        }

        // POST: CommissionTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BuyerInfoId,ProdDepartmentId,CalcType,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CommissionDistTempMas commissionDistTempMas)
        {
            if (ModelState.IsValid)
            {
                db.CommissionDistTempMas.Add(commissionDistTempMas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", commissionDistTempMas.BuyerInfoId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment, "Id", "Name", commissionDistTempMas.ProdDepartmentId);
            return View(commissionDistTempMas);
        }



        public JsonResult SaveComm(IEnumerable<CommissionDistTempDet> CommDetails, CommissionDistTempMas CommMas)
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

                        //var OrderM = new BuyerOrderMas()
                        //{
                        //    Id = 0,
                        //    OrderRefNo = OrderMas.OrderRefNo,
                        //    OrderDate = OrderMas.OrderDate,
                        //    BuyerInfoId = OrderMas.BuyerInfoId,
                        //    ProdDepartmentId = OrderMas.ProdDepartmentId,
                        //    SeasonInfoId = OrderMas.SeasonInfoId,
                        //    FabSupplierId = OrderMas.FabSupplierId,
                        //    OpBy = 1,
                        //    OpOn = OpDate,
                        //    IsAuth = true,
                        //    IsLocked = false
                        //};

                        CommMas.OpBy = 1;
                        CommMas.OpOn = OpDate;
                        CommMas.IsAuth = true;
                        

                        db.CommissionDistTempMas.Add(CommMas);
                        db.SaveChanges();

                        foreach (var item in CommDetails)
                        {
                            //var OrderD = new BuyerOrderDet()
                            //{
                            //    Id = 0,
                            //    BuyerOrderMasId = OrderM.Id,
                            //    ProdCatTypeId = item.ProdCatTypeId,
                            //    StyleNo = item.StyleNo,
                            //    ProdSizeId = item.ProdSizeId,
                            //    ProdColorId = item.ProdColorId,
                            //    FabricItemId = item.FabricItemId,
                            //    SupplierId = item.SupplierId,
                            //    UnitPrice = item.UnitPrice,
                            //    Quantity = item.Quantity,
                            //    IsLocked = false

                            //};

                            item.CommissionDistTempMasId  = CommMas.Id;
                            
                            db.CommissionDistTempDet.Add(item);
                            db.SaveChanges();

                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = CommMas.Id
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


        // GET: CommissionTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                        
            var comm = (from commMas in db.CommissionDistTempMas
                       join buyer in db.BuyerInfo on commMas.BuyerInfoId equals buyer.Id   
                       join dept in db.ProdDepartment on commMas.ProdDepartmentId equals dept.Id                   
                       where commMas.Id == id
                       select new { commMas, buyer, dept }).SingleOrDefault();

            if (comm == null)
            {
                return HttpNotFound();
            }

            var commMaster = new VMCommissionDistTempMas()
            {
                Id = comm.commMas.Id,
                BuyerInfoId = comm.commMas.BuyerInfoId,
                BuyerName = comm.buyer.Name,
                CalcType = comm.commMas.CalcType,
                ProdDepartmentId = comm.commMas.ProdDepartmentId,
                ProdDepartmentName = comm.dept.Name                   
            };


            return View(commMaster);
        }

        // POST: CommissionTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,ProdDepartmentId,CalcType,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CommissionDistTempMas commissionDistTempMas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commissionDistTempMas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", commissionDistTempMas.BuyerInfoId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment, "Id", "Name", commissionDistTempMas.ProdDepartmentId);
            return View(commissionDistTempMas);
        }


        public JsonResult UpdateComm(IEnumerable<CommissionDistTempDet> CommDetails, CommissionDistTempMas CommMas, int[] DelItems)
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

                        var commM = db.CommissionDistTempMas.Find(CommMas.Id);

                        if (commM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Commission Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        commM.CalcType = CommMas.CalcType;
                        commM.OpBy = 1;
                        commM.OpOn = OpDate;

                        db.Entry(commM).State = EntityState.Modified;
                        db.SaveChanges();

                        foreach (var item in CommDetails)
                        {
                            item.CommissionDistTempMasId = commM.Id;
                            item.Remarks = item.Remarks == null ? "" : item.Remarks;
                            //var OrderD = new BuyerOrderDet()
                            //{
                            //    Id = item.Id,
                            //    BuyerOrderMasId = OrderMas.Id,
                            //    ProdCatTypeId = item.ProdCatTypeId,
                            //    StyleNo = item.StyleNo,
                            //    ProdSizeId = item.ProdSizeId,
                            //    ProdColorId = item.ProdColorId,
                            //    FabricItemId = item.FabricItemId,
                            //    SupplierId = item.SupplierId,
                            //    UnitPrice = item.UnitPrice,
                            //    Quantity = item.Quantity,
                            //    IsLocked = false                                 
                            //};

                            db.Entry(item).State = item.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            //db.BuyerOrderDets.Add(OrderD);
                            db.SaveChanges();

                        }

                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                var delcomm = db.CommissionDistTempDet.Find(item);
                                db.CommissionDistTempDet.Remove(delcomm);
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


        // GET: CommissionTemplate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionDistTempMas commissionDistTempMas = db.CommissionDistTempMas.Find(id);
            if (commissionDistTempMas == null)
            {
                return HttpNotFound();
            }
            return View(commissionDistTempMas);
        }

        // POST: CommissionTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommissionDistTempMas commissionDistTempMas = db.CommissionDistTempMas.Find(id);
            db.CommissionDistTempMas.Remove(commissionDistTempMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult GetCommData(int id)
        {
            
            //Id, CommissionDistTempMasId, MinRange, MaxRange, OverseasComm, OthersComm, Remarks
            var list = db.CommissionDistTempDet.Where(x => x.CommissionDistTempMasId == id).AsEnumerable()
                .Select(x => new
                {
                    Id = x.Id,
                    MinRange = x.MinRange,
                    MaxRange = x.MaxRange,
                    OverseasComm = x.OverseasComm,
                    OthersComm = x.OthersComm,
                    Remarks = x.Remarks
                });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCommMaster(int BuyerId, int DeptId)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.CommissionDistTempMas.Where(x => x.BuyerInfoId == BuyerId && x.ProdDepartmentId == DeptId).FirstOrDefault();

            if (data != null)
            {
                result = new
                {
                    flag = true,
                    exists = true,
                    id = data.Id,
                    message = "Exists"
                };
            }
            else
            {
                result = new
                {
                    flag = true,
                    exists = false,
                    id = 0,
                    message = "Not Exists"
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
