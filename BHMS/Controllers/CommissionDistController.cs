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
    public class CommissionDistController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: CommissionDist
        public ActionResult Index()
        {
            var commissionDistMas = db.CommissionDistMas.Include(c => c.BuyerOrderMas);
            return View(commissionDistMas.ToList());
        }

        // GET: CommissionDist/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionDistMas commissionDistMas = db.CommissionDistMas.Find(id);
            if (commissionDistMas == null)
            {
                return HttpNotFound();
            }
            return View(commissionDistMas);
        }

        // GET: CommissionDist/Create
        public ActionResult Create()
        {
            ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            return View();
        }

        // POST: CommissionDist/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BuyerOrderMasId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CommissionDistMas commissionDistMas)
        {
            if (ModelState.IsValid)
            {
                db.CommissionDistMas.Add(commissionDistMas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", commissionDistMas.BuyerOrderMasId);
            return View(commissionDistMas);
        }


        public JsonResult SaveComm(IEnumerable<CommissionDistDet> CommDetails, int BuyerOrderMasId)
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
                        var CommM = new CommissionDistMas()
                        {
                            Id = 0,
                            BuyerOrderMasId = BuyerOrderMasId,
                            OpBy = 1,
                            OpOn = OpDate,
                            IsAuth = true
                        };

                        db.CommissionDistMas.Add(CommM);
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

                            item.CommissionDistMasId = CommM.Id;
                            //item.CheckFlag = true;

                            db.CommissionDistDet.Add(item);
                            db.SaveChanges();

                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = CommM.Id
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


        // GET: CommissionDist/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comm = (from commMas in db.CommissionDistMas
                        join bOrder in db.BuyerOrderMas on commMas.BuyerOrderMasId equals bOrder.Id
                        join buyer in db.BuyerInfo on bOrder.BuyerInfoId equals buyer.Id
                        join dept in db.ProdDepartment on bOrder.ProdDepartmentId equals dept.Id
                        where commMas.Id == id
                        select new { commMas, buyer, dept, bOrder }).SingleOrDefault();

            if (comm == null)
            {
                return HttpNotFound();
            }

            var commMaster = new VMCommissionDistMas()
            {
                Id = comm.commMas.Id,
                BuyerOrderMasId = comm.commMas.BuyerOrderMasId,
                //BuyerInfoId = comm.commMas.BuyerInfoId,
                BuyerName = comm.buyer.Name,
                //CalcType = comm.commMas.CalcType,
                //DepartmentId = comm.commMas.ProdDepartmentId,
                DepartmentName = comm.dept.Name,
                BuyerOrderNo = comm.bOrder.OrderRefNo
            };

            return View(commMaster);
        }

        // POST: CommissionDist/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BuyerOrderMasId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CommissionDistMas commissionDistMas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commissionDistMas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", commissionDistMas.BuyerOrderMasId);
            return View(commissionDistMas);
        }

        public JsonResult UpdateComm(IEnumerable<CommissionDistDet> CommDetails, int Id)
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
                        var CommM = db.CommissionDistMas.Find(Id);

                        if (CommM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Commission date not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }


                        CommM.OpBy = 1;
                        CommM.OpOn = OpDate;

                        db.Entry(CommM).State = EntityState.Modified;

                        db.SaveChanges();
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

                        //OrderMas.OpBy = 1;
                        //OrderMas.OpOn = OpDate;
                        //OrderMas.IsAuth = true;
                        //OrderMas.IsLocked = false;

                        //db.FactoryOrderMas.Add(OrderMas);
                        //db.SaveChanges();

                        foreach (var item in CommDetails)
                        {
                            item.CommissionDistMasId = Id;
                            db.Entry(item).State = item.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            db.SaveChanges();

                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!"
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


        // GET: CommissionDist/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommissionDistMas commissionDistMas = db.CommissionDistMas.Find(id);
            if (commissionDistMas == null)
            {
                return HttpNotFound();
            }
            return View(commissionDistMas);
        }

        // POST: CommissionDist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommissionDistMas commissionDistMas = db.CommissionDistMas.Find(id);
            db.CommissionDistMas.Remove(commissionDistMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult DeleteComm(int id)
        {
            //string result = "";

            bool flag = false;
            try
            {
                var itemsToDeleteDet = db.CommissionDistDet.Where(x => x.CommissionDistMasId == id);
                db.CommissionDistDet.RemoveRange(itemsToDeleteDet);

                var itemToDeleteMas = db.CommissionDistMas.Where(x => x.Id == id).FirstOrDefault();
                db.CommissionDistMas.Remove(itemToDeleteMas);

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


        public JsonResult CheckCommMaster(int buyerOrderId)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.CommissionDistMas.Where(x => x.BuyerOrderMasId == buyerOrderId).FirstOrDefault();

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

        public JsonResult GetCommData(int buyerOrderId)
        {

            var data = from bOrderDet in db.BuyerOrderDet
                       join supp in db.Supplier on bOrderDet.SupplierId equals supp.Id
                       join fOrderDet in db.FactoryOrderDet on bOrderDet.Id equals fOrderDet.BuyerOrderDetId
                       join fOrderDeliv in db.FactoryOrderDelivDet on fOrderDet.Id equals fOrderDeliv.FactoryOrderDetId
                       where bOrderDet.BuyerOrderMasId == buyerOrderId
                       select new { bOrderDet, supp, fOrderDet, fOrderDeliv };

            var refinedata1 = data.AsEnumerable().Select(x => new
            {
                OrderDetId = x.fOrderDet.Id,
                StyleNo = x.bOrderDet.StyleNo,
                FactoryName = x.supp.Name,
                OrderQuantity = x.bOrderDet.Quantity ?? 0,
                RdlOrderValue = x.bOrderDet.UnitPrice ?? 0,
                // FactOrderValue = x.fOrderDet.FOBUnitPrice
                FactOrderValue = x.fOrderDeliv.FactFOB
            });


            //var refinedata1 = data.AsEnumerable().Select(x => new {
            //    OrderDetId = x.fOrderDet.Id,
            //    StyleNo = x.bOrderDet.StyleNo,
            //    FactoryName = x.supp.Name,
            //    OrderQuantity = x.bOrderDet.Quantity ?? 0,
            //    RdlOrderValue = x.bOrderDet.UnitPrice ?? 0,
            //    FactOrderValue = db.FactoryOrderDelivDet.Where(m=>m.FactoryOrderDetId==x.fOrderDet.Id).Select(m=>new { FactFOB = m.FactFOB , DelivQty =  })
            //});

            var refinedata2 = refinedata1.Select(x => new {
                OrderDetId = x.OrderDetId,
                StyleNo = x.StyleNo,
                FactoryName = x.FactoryName,
                OrderQuantity = x.OrderQuantity,
                RdlOrderValue = x.RdlOrderValue,
                FactOrderValue = x.FactOrderValue,
                TotalRdlValue = x.OrderQuantity * x.RdlOrderValue,
                TotalFactValue = x.OrderQuantity * x.FactOrderValue,
                TotalCommValue = (x.OrderQuantity * x.RdlOrderValue) - (x.OrderQuantity * x.FactOrderValue),
                Margin = x.RdlOrderValue - x.FactOrderValue,
                //TotalComm = Math.Round((x.RdlOrderValue - x.FactOrderValue) * 100 / x.RdlOrderValue, 2), //prev
                TotalComm = 0
            });

            decimal totRDLValue = 0;
            decimal totFactValue = 0;
            double overallComm = 0;

            foreach (var item in refinedata2)
            {
                totRDLValue = totRDLValue + item.TotalRdlValue;
                //totFactValue = totFactValue + item.TotalFactValue; //prev
                totFactValue = 0;
            }

            overallComm = (double)(1 - totFactValue / totRDLValue) * 100;

            overallComm = Math.Round(overallComm, 2);

            var comCat = db.CommissionDistTempDet.Where(x => x.MinRange <= overallComm && x.MaxRange >= overallComm).FirstOrDefault();

            double oversComm = 0;
            double otherComm = 0;
            double compComm = 0;

            if (comCat != null)
            {
                oversComm = comCat.OverseasComm;
                otherComm = comCat.OthersComm;
                compComm = 100 - oversComm - otherComm;
            }

            var procdata = refinedata2.Select(x => new {
                OrderDetId = x.OrderDetId,
                StyleNo = x.StyleNo,
                FactoryName = x.FactoryName,
                OrderQuantity = x.OrderQuantity,
                RdlOrderValue = x.RdlOrderValue,
                FactOrderValue = x.FactOrderValue,
                TotalRdlValue = x.OrderQuantity * x.RdlOrderValue,
                TotalFactValue = x.OrderQuantity * x.FactOrderValue,
                TotalCommValue = (x.OrderQuantity * x.RdlOrderValue) - (x.OrderQuantity * x.FactOrderValue),
                Margin = x.RdlOrderValue - x.FactOrderValue,
                //TotalComm = Math.Round((x.RdlOrderValue - x.FactOrderValue) * 100 / x.RdlOrderValue, 2), //prev
                TotalComm = 0,
                OverseasCommPer = oversComm,
                OthersCommPer = otherComm,
                CompCommPer = compComm
            });
            return Json(procdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommEditedData(int id)
        {
            var commMas = db.CommissionDistMas.Find(id);

            var data = from bOrderDet in db.BuyerOrderDet
                       join supp in db.Supplier on bOrderDet.SupplierId equals supp.Id
                       join fOrderDet in db.FactoryOrderDet on bOrderDet.Id equals fOrderDet.BuyerOrderDetId
                       join fOrderDeliv in db.FactoryOrderDelivDet on fOrderDet.Id equals fOrderDeliv.FactoryOrderDetId
                       join commDet in db.CommissionDistDet on fOrderDet.Id equals commDet.FactoryOrderDetId into joinCommDet
                       from co in joinCommDet.Where(x => x.CommissionDistMasId == id).DefaultIfEmpty()
                       where bOrderDet.BuyerOrderMasId == commMas.BuyerOrderMasId
                       select new { bOrderDet, supp, fOrderDet, co, fOrderDeliv };

            // StyleNo FactoryName OrderQuantity RdlOrderValue FactOrderValue TotalRdlValue TotalFactValue TotalCommValue Margin TotalComm
            //var refinedata = data.AsEnumerable().Select(x => new {
            //    OrderDetId = x.fOrderDet.Id,
            //    StyleNo = x.bOrderDet.StyleNo,
            //    FactoryName = x.supp.Name,
            //    OrderQuantity = x.bOrderDet.Quantity,
            //    RdlOrderValue = x.bOrderDet.UnitPrice,
            //    FactOrderValue = x.fOrderDet.FOBUnitPrice,
            //    TotalRdlValue = x.bOrderDet.Quantity * x.bOrderDet.UnitPrice,
            //    TotalFactValue = x.bOrderDet.Quantity * x.fOrderDet.FOBUnitPrice,
            //    TotalCommValue = (x.bOrderDet.Quantity * x.bOrderDet.UnitPrice) - (x.bOrderDet.Quantity * x.fOrderDet.FOBUnitPrice),
            //    Margin = x.bOrderDet.UnitPrice - x.fOrderDet.FOBUnitPrice,
            //    TotalComm = Math.Round((x.bOrderDet.UnitPrice??0 - x.fOrderDet.FOBUnitPrice) * 100 / x.bOrderDet.UnitPrice??0,2)
            //});

            //OverseasCommPer, OverseasCommValue, OthersCommPer, OthersCommValue, CompCommValue
            var refinedata = data.AsEnumerable().Select(x => new {
                Id = x.co == null ? 0 : x.co.Id,
                OrderDetId = x.fOrderDet.Id,
                StyleNo = x.bOrderDet.StyleNo,
                FactoryName = x.supp.Name,
                OrderQuantity = x.bOrderDet.Quantity ?? 0,
                RdlOrderValue = x.bOrderDet.UnitPrice ?? 0,
                FactOrderValue = x.fOrderDeliv.FactFOB,
                OverseasCommPer = x.co == null ? 0 : x.co.OverseasCommPer,
                OverseasCommValue = x.co == null ? 0 : x.co.OverseasCommValue,
                OthersCommPer = x.co == null ? 0 : x.co.OthersCommPer,
                OthersCommValue = x.co == null ? 0 : x.co.OthersCommValue,
                CompCommValue = x.co == null ? 0 : x.co.CompCommValue,
                //TotalRdlValue = x.bOrderDet.Quantity * x.bOrderDet.UnitPrice,
                //TotalFactValue = x.bOrderDet.Quantity * x.fOrderDet.FOBUnitPrice,
                //TotalCommValue = (x.bOrderDet.Quantity * x.bOrderDet.UnitPrice) - (x.bOrderDet.Quantity * x.fOrderDet.FOBUnitPrice),
                //Margin = x.bOrderDet.UnitPrice - x.fOrderDet.FOBUnitPrice
                //TotalComm = Math.Round((x.bOrderDet.UnitPrice ?? 0 - x.fOrderDet.FOBUnitPrice) * 100 / x.bOrderDet.UnitPrice ?? 0, 2)
                CheckFlag = x.co == null ? false : x.co.CheckFlag

            });

            var procdata = refinedata.Select(x => new {
                Id = x.Id,
                OrderDetId = x.OrderDetId,
                StyleNo = x.StyleNo,
                FactoryName = x.FactoryName,
                OrderQuantity = x.OrderQuantity,
                RdlOrderValue = x.RdlOrderValue,
                FactOrderValue = x.FactOrderValue,
                TotalRdlValue = x.OrderQuantity * x.RdlOrderValue,
                TotalFactValue = x.OrderQuantity * x.FactOrderValue,
                TotalCommValue = (x.OrderQuantity * x.RdlOrderValue) - (x.OrderQuantity * x.FactOrderValue),
                Margin = x.RdlOrderValue - x.FactOrderValue,
                //TotalComm = Math.Round((x.RdlOrderValue - x.FactOrderValue) * 100 / x.RdlOrderValue, 2), //prev
                TotalComm = 0,
                OverseasCommPer = x.OverseasCommPer,
                OverseasCommValue = x.OverseasCommValue,
                OthersCommPer = x.OthersCommPer,
                OthersCommValue = x.OthersCommValue,
                CompCommValue = x.CompCommValue,
                CompCommPer = 0,
                CheckFlag = x.CheckFlag
            });

            return Json(procdata, JsonRequestBehavior.AllowGet);
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
