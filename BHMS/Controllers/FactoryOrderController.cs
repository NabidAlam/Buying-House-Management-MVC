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
    public class FactoryOrderController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: FactoryOrder
        public ActionResult Index()
        {
            var factoryOrderMas = db.FactoryOrderMas.Include(f => f.BuyerOrderMas).Include(f => f.Supplier);
            return View(factoryOrderMas.ToList());
        }

        // GET: FactoryOrder/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    FactoryOrderMas factoryOrderMas = db.FactoryOrderMas.Find(id);
        //    if (factoryOrderMas == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(factoryOrderMas);
        //}

        // GET: FactoryOrder/Create
        public ActionResult Create()
        {
            ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            //ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }

        // POST: FactoryOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,BuyerOrderMasId,SalesContractNo,SalesContractDate,SupplierId,IsAuth,OpBy,OpOn,AuthBy,AuthOn,IsLocked")] FactoryOrderMas factoryOrderMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.FactoryOrderMas.Add(factoryOrderMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", factoryOrderMas.BuyerOrderMasId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryOrderMas.SupplierId);
        //    return View(factoryOrderMas);
        //}


        public JsonResult SaveFactoryOrder(IEnumerable<VMFactoryOrderDet> OrderDetails, IEnumerable<VMFactoryOrderDelivDet> DelivDetails, FactoryOrderMas OrderMas)
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

                        OrderMas.OpBy = 1;
                        OrderMas.OpOn = OpDate;
                        OrderMas.IsAuth = true;
                        OrderMas.IsLocked = false;

                        db.FactoryOrderMas.Add(OrderMas);
                        db.SaveChanges();


                        Dictionary<int, int> dictionary =
                                  new Dictionary<int, int>();

                        if(OrderDetails!=null)
                        {
                            foreach (var item in OrderDetails)
                            {
                                var OrderD = new FactoryOrderDet()
                                {
                                    Id = 0,
                                    FactoryOrderMasId = OrderMas.Id,
                                    BuyerOrderDetId = item.BuyerOrderDetId,
                                    IsLocked = false
                                };
                                 
                                //item.FactoryOrderMasId = OrderMas.Id;
                                //item.IsLocked = false;

                                db.FactoryOrderDet.Add(OrderD);
                                db.SaveChanges();

                                dictionary.Add(item.TempOrderDetId, OrderD.Id);
                            }
                        }





                        //---- shipment data

                        //    var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                var deliv = new FactoryOrderDelivDet()
                                {
                                    Id = item.Id,
                                    ShipmentSummDetId = item.ShipmentSummDetId,
                                    FactoryOrderDetId = dictionary[item.DelivOrderDetTempId],
                                    ExFactoryDate = item.ExFactoryDate,
                                    FactFOB = item.FactFOB
                                    //DiscountFOB = item.DiscountFOB,
                                    //DiscountFlag = false
                                };

                                //db.Entry(deliv).State = deliv.Id == 0 ?
                                //                            EntityState.Added :
                                //                            EntityState.Modified;

                                db.FactoryOrderDelivDet.Add(deliv);
                                db.SaveChanges();

                            }

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


        // GET: FactoryOrder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ord = (from fOrderMas in db.FactoryOrderMas
                       join bOrderMas in db.BuyerOrderMas on fOrderMas.BuyerOrderMasId equals bOrderMas.Id
                       join supp in db.Supplier on fOrderMas.SupplierId equals supp.Id
                       where fOrderMas.Id == id
                       select new { fOrderMas, bOrderMas, supp }).SingleOrDefault();


            if (ord == null)
            {
                return HttpNotFound();
            }

            var orderMas = new VMFactoryOrderMas()
            {
                Id = ord.fOrderMas.Id,
                BuyerOrderRefNo = ord.bOrderMas.OrderRefNo,
                BuyerOrderRefDate = BHMS.Helpers.NullHelpers.DateToString(ord.bOrderMas.OrderDate),
                BuyerOrderMasId = ord.fOrderMas.BuyerOrderMasId,
                SupplierId = ord.fOrderMas.SupplierId,
                SalesContractNo = ord.fOrderMas.SalesContractNo,
                SalesContractDate = ord.fOrderMas.SalesContractDate,
                SupplierName = ord.supp.Name
            };

            //ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", factoryOrderMas.BuyerOrderMasId);
            //ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryOrderMas.SupplierId);
            return View(orderMas);
        }

        public ActionResult EditM(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ord = (from fOrderMas in db.FactoryOrderMas
                       join bOrderMas in db.BuyerOrderMas on fOrderMas.BuyerOrderMasId equals bOrderMas.Id
                       join supp in db.Supplier on fOrderMas.SupplierId equals supp.Id
                       where fOrderMas.Id == id
                       select new { fOrderMas, bOrderMas, supp }).SingleOrDefault();


            if (ord == null)
            {
                return HttpNotFound();
            }

            var orderMas = new VMFactoryOrderMas()
            {
                Id = ord.fOrderMas.Id,
                BuyerOrderRefNo = ord.bOrderMas.OrderRefNo,
                BuyerOrderRefDate = BHMS.Helpers.NullHelpers.DateToString(ord.bOrderMas.OrderDate),
                BuyerOrderMasId = ord.fOrderMas.BuyerOrderMasId,
                SupplierId = ord.fOrderMas.SupplierId,
                SalesContractNo = ord.fOrderMas.SalesContractNo,
                SalesContractDate = ord.fOrderMas.SalesContractDate,
                SupplierName = ord.supp.Name
            };

            //ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", factoryOrderMas.BuyerOrderMasId);
            //ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryOrderMas.SupplierId);
            return View(orderMas);
        }

        // POST: FactoryOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,BuyerOrderMasId,SalesContractNo,SalesContractDate,SupplierId,IsAuth,OpBy,OpOn,AuthBy,AuthOn,IsLocked")] FactoryOrderMas factoryOrderMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(factoryOrderMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", factoryOrderMas.BuyerOrderMasId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", factoryOrderMas.SupplierId);
        //    return View(factoryOrderMas);
        //}


        public JsonResult UpdateFactoryOrder(IEnumerable<VMFactoryOrderDet> OrderDetails, IEnumerable<VMFactoryOrderDelivDet> DelivDetails, FactoryOrderMas OrderMas)
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
                        var OrderM = db.FactoryOrderMas.Find(OrderMas.Id);

                        if (OrderM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Factory Order not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        OrderM.SalesContractDate = OrderMas.SalesContractDate;
                        OrderM.SalesContractDate = OrderMas.SalesContractDate;
                        OrderM.OpBy = 1;
                        OrderM.OpOn = OpDate;

                        db.Entry(OrderM).State = EntityState.Modified;

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

                        Dictionary<int, int> dictionary =
                         new Dictionary<int, int>();

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
                            //    IsLocked = false

                            //};

                            if (item.Id == 0) //insert
                            {
                                //item.FactoryOrderMasId = OrderMas.Id;
                                //item.IsLocked = false;

                                var OrderD = new FactoryOrderDet()
                                {
                                    Id = item.Id,
                                    FactoryOrderMasId = OrderMas.Id,
                                    BuyerOrderDetId = item.BuyerOrderDetId,
                                    IsLocked = false
                                };

                                db.FactoryOrderDet.Add(OrderD);
                                dictionary.Add(item.TempOrderDetId, OrderD.Id);

                            }
                            else //update
                            {
                                var oItem = db.FactoryOrderDet.Find(item.Id);
                                //oItem.FOBUnitPrice = item.FOBUnitPrice;
                                db.Entry(oItem).State = EntityState.Modified;
                                dictionary.Add(item.TempOrderDetId, oItem.Id);
                            }

                            db.SaveChanges();


                        }


                        //  var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                var delivItem = db.FactoryOrderDelivDet.Find(item.Id);

                                if (delivItem != null)
                                {
                                    delivItem.ExFactoryDate = item.ExFactoryDate;
                                    delivItem.FactFOB = item.FactFOB;

                                    //db.FactoryOrderDelivDet.Attach(delivItem);
                                    db.Entry(delivItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var deliv = new FactoryOrderDelivDet()
                                    {
                                        Id = item.Id,
                                        FactoryOrderDetId = dictionary[item.DelivOrderDetTempId],
                                        ExFactoryDate = item.ExFactoryDate,
                                        ShipmentSummDetId = item.ShipmentSummDetId,
                                        FactFOB = item.FactFOB
                                    
                                    };

                                    db.Entry(deliv).State = deliv.Id == 0 ?
                                                                EntityState.Added :
                                                                EntityState.Modified;

                                    //db.FactoryOrderDelivDet.Add(deliv);
                                    db.SaveChanges();
                                }



                            }
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

        // for commercial role
        public JsonResult UpdateFactoryOrderM(IEnumerable<VMFactoryOrderDet> OrderDetails, IEnumerable<VMFactoryOrderDelivDet> DelivDetails, FactoryOrderMas OrderMas)
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
                        var OrderM = db.FactoryOrderMas.Find(OrderMas.Id);

                        if (OrderM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Factory Order not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        OrderM.SalesContractDate = OrderMas.SalesContractDate;
                        OrderM.SalesContractDate = OrderMas.SalesContractDate;
                        OrderM.OpBy = 1;
                        OrderM.OpOn = OpDate;

                        db.Entry(OrderM).State = EntityState.Modified;

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

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();


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
                            //    IsLocked = false
                            //};


                            if (item.Id == 0) //insert
                            {
                                //item.FactoryOrderMasId = OrderMas.Id;
                                //item.IsLocked = false;

                                var OrderD = new FactoryOrderDet()
                                {
                                    Id = item.Id,
                                    FactoryOrderMasId = OrderMas.Id,
                                    BuyerOrderDetId = item.BuyerOrderDetId,
                                    IsLocked = false
                                };

                                db.FactoryOrderDet.Add(OrderD);
                                dictionary.Add(item.TempOrderDetId, OrderD.Id);

                            }
                            else //update
                            {
                                var oItem = db.FactoryOrderDet.Find(item.Id);
                                //oItem.FOBUnitPrice = item.FOBUnitPrice;
                                db.Entry(oItem).State = EntityState.Modified;
                                dictionary.Add(item.TempOrderDetId, oItem.Id);
                            }

                            db.SaveChanges();


                        }


                        //  var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {

                                var delivItem = db.FactoryOrderDelivDet.Find(item.Id);

                                if (delivItem != null)
                                {
                                    delivItem.ExFactoryDate = item.ExFactoryDate;
                                    delivItem.FactFOB = item.FactFOB;
                                    delivItem.FactTransferPrice = item.FactTransferPrice;
                                    delivItem.Remarks = item.Remarks;
                                    //delivItem.DiscountFOB = item.DiscountFOB;
                                    //db.FactoryOrderDelivDet.Attach(delivItem);
                                    db.Entry(delivItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                //var deliv = new FactoryOrderDelivDet()
                                //{
                                //    Id = item.Id,
                                //    FactoryOrderDetId = dictionary[item.DelivOrderDetTempId],
                                //    ExFactoryDate = item.ExFactoryDate,
                                //    FactFOB = item.FactFOB,
                                //    FactTransferPrice = item.FactTransferPrice
                                //};

                                db.Entry(delivItem).State = delivItem.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                //db.FactoryOrderDelivDet.Add(deliv);
                                db.SaveChanges();

                            }
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

        // GET: FactoryOrder/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    FactoryOrderMas factoryOrderMas = db.FactoryOrderMas.Find(id);
        //    if (factoryOrderMas == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(factoryOrderMas);
        //}

        // POST: FactoryOrder/Delete/5


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    FactoryOrderMas factoryOrderMas = db.FactoryOrderMas.Find(id);
        //    db.FactoryOrderMas.Remove(factoryOrderMas);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public JsonResult DeleteOrder(int id)
        {
            //string result = "";

            bool flag = false;
            try
            {

                var itemToDeleteMas = db.FactoryOrderMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nFactory Order Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //var checkComm = db.CommissionDistMas.Where(x => x.BuyerOrderMasId == itemToDeleteMas.BuyerOrderMasId).ToList();
                var checkComm = db.MasterLCInfoDet.Where(x => x.BuyerOrderMasId == itemToDeleteMas.BuyerOrderMasId).ToList();

                if (checkComm.Count == 0)
                {
                    var itemsToDeleteDet = db.FactoryOrderDet.Where(x => x.FactoryOrderMasId == id);


                    foreach (var item in itemsToDeleteDet)
                    {
                        var itemsToDeleteDeliv = db.FactoryOrderDelivDet.Where(x => x.FactoryOrderDetId == item.Id);
                        db.FactoryOrderDelivDet.RemoveRange(itemsToDeleteDeliv);
                    }

                    db.FactoryOrderDet.RemoveRange(itemsToDeleteDet);

                    db.FactoryOrderMas.Remove(itemToDeleteMas);

                    flag = db.SaveChanges() > 0;
                }
                else
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nMaster LC exists. Master LC data first."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


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


        //// ProdCat, CatType,Dept, style,color,size, fabric, order quanty, rdl fob, factory fob,
        //total RDL val, tot fact val, ex fact date, overall commission

        public JsonResult GetOrderData(int buyerOrderId, int factoryId)
        {

            var data = from bOrderDet in db.BuyerOrderDet
                       join catType in db.ProdCatType on bOrderDet.ProdCatTypeId equals catType.Id
                       join cat in db.ProdCategory on catType.ProdCategoryId equals cat.Id
                       join bOrderMas in db.BuyerOrderMas on bOrderDet.BuyerOrderMasId equals bOrderMas.Id
                       join dept in db.ProdDepartment on bOrderMas.ProdDepartmentId equals dept.Id into joinDept
                       from pDept in joinDept.DefaultIfEmpty()
                       join colr in db.ProdColor on bOrderDet.ProdColorId equals colr.Id into joinColr
                       from pColr in joinColr.DefaultIfEmpty()
                       join size in db.ProdSize on bOrderDet.ProdSizeId equals size.Id into joinSize
                       from pSize in joinSize.DefaultIfEmpty()
                           //join fabric in db.FabricItem on bOrderDet.FabricItemId equals fabric.Id into joinFabric
                           //from pFabric in joinFabric.DefaultIfEmpty()
                       join fabric in db.FabricItemDet on bOrderDet.FabricItemDetId equals fabric.Id into joinFabric
                       from pFabric in joinFabric.DefaultIfEmpty()
                       where bOrderDet.BuyerOrderMasId == buyerOrderId && bOrderDet.SupplierId == factoryId
                       select new { bOrderDet, catType, cat, pDept, pColr, pSize, pFabric };

            var refinedata = data.AsEnumerable().Select(x => new
            {
                OrderDetId = x.bOrderDet.Id,
                ProdCatName = x.cat.Name,
                ProdCatTypeName = x.catType.Name,
                DeptName = x.pDept == null ? "" : x.pDept.Name,
                StyleNo = x.bOrderDet.StyleNo,
                ColorName = x.pColr == null ? "" : x.pColr.Name,
                SizeName = x.pSize == null ? "" : x.pSize.SizeRange,
                //FabricType = x.pFabric == null ? "" : x.pFabric.FabricType.Name,
                //FabricName = x.pFabric == null ? "" : x.pFabric.Name,
                FabricType = x.pFabric == null ? "" : x.pFabric.FabricItem.FabricType.Name,
                FabricName = x.pFabric == null ? "" : x.pFabric.Name,
                FabricSupplier = x.bOrderDet.FabricSupplier.Name,
                OrderQuantity = x.bOrderDet.Quantity,
                OrderValue = x.bOrderDet.UnitPrice,
                RDLValue = x.bOrderDet.RdlTotal,
                ExFactoryDate = x.bOrderDet.ExFactoryDate.HasValue ? x.bOrderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : ""
            });


            return Json(refinedata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderEditedData(int id)
        {
            var orderMas = db.FactoryOrderMas.Find(id);

            var data = from bOrderDet in db.BuyerOrderDet
                       join catType in db.ProdCatType on bOrderDet.ProdCatTypeId equals catType.Id
                       join cat in db.ProdCategory on catType.ProdCategoryId equals cat.Id
                       join bOrderMas in db.BuyerOrderMas on bOrderDet.BuyerOrderMasId equals bOrderMas.Id
                       join dept in db.ProdDepartment on bOrderMas.ProdDepartmentId equals dept.Id into joinDept
                       from pDept in joinDept.DefaultIfEmpty()
                       join colr in db.ProdColor on bOrderDet.ProdColorId equals colr.Id into joinColr
                       from pColr in joinColr.DefaultIfEmpty()
                       join size in db.ProdSize on bOrderDet.ProdSizeId equals size.Id into joinSize
                       from pSize in joinSize.DefaultIfEmpty()
                       join fabric in db.FabricItemDet on bOrderDet.FabricItemDetId equals fabric.Id into joinFabric
                       from pFabric in joinFabric.DefaultIfEmpty()
                       join fOrderDet in db.FactoryOrderDet on bOrderDet.Id equals fOrderDet.BuyerOrderDetId into joinFOrder
                       from fo in joinFOrder.Where(x => x.FactoryOrderMasId == id).DefaultIfEmpty()
                       where bOrderDet.BuyerOrderMasId == orderMas.BuyerOrderMasId && bOrderDet.SupplierId == orderMas.SupplierId
                       select new { bOrderDet, catType, cat, pDept, pColr, pSize, pFabric, fo };

            var refinedata = data.AsEnumerable().Select(x => new
            {
                FactOrderDetId = x.fo == null ? 0 : x.fo.Id,
                OrderDetId = x.bOrderDet.Id,
                ProdCatName = x.cat.Name,
                ProdCatTypeName = x.catType.Name,
                DeptName = x.pDept == null ? "" : x.pDept.Name,
                StyleNo = x.bOrderDet.StyleNo,
                ColorName = x.pColr == null ? "" : x.pColr.Name,
                SizeName = x.pSize == null ? "" : x.pSize.SizeRange,
                FabricType = x.pFabric == null ? "" : x.pFabric.FabricItem.FabricType.Name,
                FabricName = x.pFabric == null ? "" : x.pFabric.Name,
                FabricSupplier = x.bOrderDet.FabricSupplier.Name,
                OrderQuantity = x.bOrderDet.Quantity,
                RDLValue = x.bOrderDet.RdlTotal,
                OrderValue = x.bOrderDet.UnitPrice
                //FactValue = x.fo == null ? "" : x.fo.FOBUnitPrice.ToString(),
                //TransferPrice = x.fo == null ? "" : x.fo.TransferPrice.ToString(),
                //ExFactoryDate = x.bOrderDet.ExFactoryDate.HasValue ? x.bOrderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : ""
            });

            return Json(refinedata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewContractNo(string buyerOrderRef, int factoryId)
        {
            var result = new
            {
                flag = true,
                ContractNo = "RDL/DMR/TT/101.1",
                message = "success"
            };

            string contractNo = "";

            var supp = db.Supplier.Find(factoryId);

            contractNo = buyerOrderRef + "/" + supp.ShortName + "/";

            var flist = db.FactoryOrderMas.Where(x => x.SalesContractNo.Contains(contractNo));

            int len = contractNo.Length;
            int num1 = 0;
            int num2 = 0;
            if (flist == null)
            {
                contractNo = contractNo + "1";
            }
            else
            {
                foreach (var item in flist)
                {
                    var partVal = item.SalesContractNo.Substring(len);

                    if (int.TryParse(partVal, out num1))
                    {
                        if (num1 > num2)
                        {
                            num2 = num1;
                        }
                    }
                }

                contractNo = contractNo + (num2 + 1).ToString();
            }


            result = new
            {
                flag = true,
                ContractNo = contractNo,
                message = "success"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOrderMaster(int buyerOrderId, int factoryId)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.FactoryOrderMas.Where(x => x.BuyerOrderMasId == buyerOrderId && x.SupplierId == factoryId).FirstOrDefault();

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




        public JsonResult CheckActivityExists(int factoryOrderDetId)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.ActionActivityMas.Where(x => x.FactoryOrderDelivDet.FactoryOrderDetId == factoryOrderDetId).FirstOrDefault();

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



        public JsonResult GetBuyerInfo(int buyerOrderId)
        {
            var result = new
            {
                flag = false,
                BuyerName = "",
                Department = "",
                Season = "",
                message = "Error"
            };

            var data = (from buyerMas in db.BuyerOrderMas
                        where buyerMas.Id == buyerOrderId
                        select buyerMas).SingleOrDefault();

            if (data != null)
            {
                result = new
                {
                    flag = true,
                    BuyerName = data.BuyerInfo.Name,
                    Department = data.ProdDepartment.Name,
                    Season = data.SeasonInfo.Name,
                    message = "success"
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
