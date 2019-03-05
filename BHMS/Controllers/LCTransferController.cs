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
    public class LCTransferController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: LCTransfer
        public ActionResult Index()
        {
            var lCTransferMas = db.LCTransferMas.Include(l => l.MasterLCInfoMas);
            return View(lCTransferMas.ToList());
        }

        // GET: LCTransfer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LCTransferMas lCTransferMas = db.LCTransferMas.Find(id);
            if (lCTransferMas == null)
            {
                return HttpNotFound();
            }
            return View(lCTransferMas);
        }

        // GET: LCTransfer/Create
        public ActionResult Create()
        {
            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo");
            return View();
        }


        public JsonResult SaveOrders(IEnumerable<LCTransferDet> OrderDetails, LCTransferMas OrderMas)
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
                        OrderMas.OpBy = 1;
                        OrderMas.OpOn = OpDate;
                        OrderMas.IsAuth = true;
                        
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

                        db.LCTransferMas.Add(OrderMas);
                        db.SaveChanges();

                        //Dictionary<int, int> dictionary =
                        //        new Dictionary<int, int>();


                        foreach (var item in OrderDetails)
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
                            //    ExFactoryDate = item.ExFactoryDate,
                            //    IsLocked = false

                            //};
                            item.LCTransferMasId = OrderMas.Id;

                            db.LCTransferDet.Add(item);
                            db.SaveChanges();

                            //dictionary.Add(item.TempOrderDetId, OrderD.Id);

                        }
                                               

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = OrderMas.Id
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



        //// POST: LCTransfer/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,MasterLCInfoMasId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] LCTransferMas lCTransferMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.LCTransferMas.Add(lCTransferMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCTransferMas.MasterLCInfoMasId);
        //    return View(lCTransferMas);
        //}

        // GET: LCTransfer/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LCTransferMas lCTransferMas = db.LCTransferMas.Find(id);
            if (lCTransferMas == null)
            {
                return HttpNotFound();
            }


            /* 
             $('#LCDate').val('');
            $('#LCReceiveDate').val('');
            $('#LatestShipmentDate').val('');
            $('#LCExpiryDate').val('');
            $('#BuyerName').val('');
            $('#LCQuantity').val('');
            $('#LCTotalValue').val('');
             */
            var lc = (from lcInfo in db.MasterLCInfoMas
                         join buyer in db.BuyerInfo on lcInfo.BuyerInfoId equals buyer.Id
                         where lcInfo.Id == lCTransferMas.MasterLCInfoMasId
                         select new { lcInfo, buyer }).SingleOrDefault();

            ViewBag.LCNo = lc.lcInfo.LCNo;
            ViewBag.LCDate = lc.lcInfo.LCDate.HasValue ? lc.lcInfo.LCDate.Value.ToString("dd/MM/yyyy"):"";
            ViewBag.LCReceiveDate = lc.lcInfo.LCReceiveDate.HasValue ? lc.lcInfo.LCReceiveDate.Value.ToString("dd/MM/yyyy") : "";
            ViewBag.LatestShipmentDate = lc.lcInfo.LatestShipmentDate.HasValue ? lc.lcInfo.LatestShipmentDate.Value.ToString("dd/MM/yyyy") : "";
            ViewBag.LCExpiryDate = lc.lcInfo.LCExpiryDate.HasValue ? lc.lcInfo.LCExpiryDate.Value.ToString("dd/MM/yyyy") : "";
            ViewBag.BuyerName = lc.buyer.Name;
            ViewBag.LCQuantity = lc.lcInfo.Quantity;
            ViewBag.LCTotalValue = lc.lcInfo.TotalValue;

            ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCTransferMas.MasterLCInfoMasId);
            return View(lCTransferMas);
        }

        // POST: LCTransfer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,MasterLCInfoMasId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] LCTransferMas lCTransferMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(lCTransferMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MasterLCInfoMasId = new SelectList(db.MasterLCInfoMas, "Id", "LCNo", lCTransferMas.MasterLCInfoMasId);
        //    return View(lCTransferMas);
        //}

        // GET: LCTransfer/Delete/5


        public JsonResult UpdateOrders(IEnumerable<LCTransferDet> OrderDetails, LCTransferMas OrderMas, int[] DelItems)
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

                        var OrderM = db.LCTransferMas.Find(OrderMas.Id);

                        if (OrderM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Transfer Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        

                        //---- delete order detail items
                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {                                                                
                                var delOrder = db.LCTransferDet.Find(item);
                                db.LCTransferDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }

                        foreach (var item in OrderDetails)
                        {

                            item.LCTransferMasId = OrderMas.Id;
                            
                            db.Entry(item).State = item.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            //db.BuyerOrderDets.Add(OrderD);
                            db.SaveChanges();

                            //dictionary.Add(item.TempOrderDetId, OrderD.Id);

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


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LCTransferMas lCTransferMas = db.LCTransferMas.Find(id);
            if (lCTransferMas == null)
            {
                return HttpNotFound();
            }
            return View(lCTransferMas);
        }

        // POST: LCTransfer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LCTransferMas lCTransferMas = db.LCTransferMas.Find(id);
            db.LCTransferMas.Remove(lCTransferMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult DeleteTransfer(int id)
        {
            //string result = "";

            bool flag = false;
            try
            {

                var itemToDeleteMas = db.LCTransferMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nTransfer data not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                
                var itemsToDeleteDet = db.LCTransferDet.Where(x => x.LCTransferMasId == id);
                                
                db.LCTransferDet.RemoveRange(itemsToDeleteDet);

                db.LCTransferMas.Remove(itemToDeleteMas);

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


        public JsonResult CheckLCMaster(int Id)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.LCTransferMas.Where(x => x.MasterLCInfoMasId == Id).FirstOrDefault();

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


        public JsonResult GetOrderData(int Id)
        {
            var list = (from tMas in db.LCTransferMas
                         join tDet in db.LCTransferDet on tMas.Id equals tDet.LCTransferMasId
                         join fOrderMas in db.FactoryOrderMas on tDet.FactoryOrderMasId equals fOrderMas.Id
                         where tMas.Id == Id
                         select new { tDet, fOrderMas }).AsEnumerable()
                        .Select(x => new
                        {
                            Id = x.tDet.Id,
                            FactoryOrderMasId = x.tDet.FactoryOrderMasId,
                            TransferDate = x.tDet.TransferDate.ToString("dd/MM/yyyy"),
                            FactoryId = x.fOrderMas.SupplierId
                        });

            //var list = (from orderDet in db.BuyerOrderDet
            //            join prodCatType in db.ProdCatType on orderDet.ProdCatTypeId equals prodCatType.Id
            //            join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
            //            //join size in db.ProdSizes on orderDet.ProdSizeId equals size.Id
            //            //join fabItem in db.FabricItems on orderDet.FabricItemId equals fabItem.Id
            //            //join prodColor in db.ProdColors on orderDet.ProdColorId equals prodColor.Id
            //            //join supp in db.Suppliers on orderDet.SupplierId equals supp.Id
            //            where orderDet.BuyerOrderMasId == Id
            //            select new { orderDet, prodCat, prodCatType }).AsEnumerable()
            //           //select new VMBuyerOrderDetEdit()
            //           .Select(x => new
            //           {
            //               Id = x.orderDet.Id,
            //               BuyerOrderMasId = x.orderDet.BuyerOrderMasId,
            //               ProdCatId = x.prodCat.Id,
            //               //ProdCatName = prodCat.Name,
            //               ProdCatTypeId = x.prodCatType.Id,
            //               //ProdCatTypeName = prodCatType.Name,
            //               StyleNo = x.orderDet.StyleNo,
            //               ProdSizeId = x.orderDet.ProdSizeId,
            //               //ProdSizeName = size.SizeRange,
            //               FabricItemId = x.orderDet.FabricItemId,
            //               //FabricItemName = fabItem.Name,
            //               ProdColorId = x.orderDet.ProdColorId,
            //               //ProdColorName = prodColor.Name,
            //               UnitPrice = x.orderDet.UnitPrice,
            //               Quantity = x.orderDet.Quantity,
            //               SupplierId = x.orderDet.SupplierId,
            //               //SupplierName = supp.Name
            //               ExFactoryDate = x.orderDet.ExFactoryDate.HasValue ? x.orderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : ""
            //           });


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
