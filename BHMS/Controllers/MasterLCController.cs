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
    public class MasterLCController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: MasterLC
        public ActionResult Index()
        {
            var masterLCInfoMas = db.MasterLCInfoMas.Include(m => m.BuyerInfo);
            return View(masterLCInfoMas.ToList());
        }

        // GET: MasterLC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterLCInfoMas masterLCInfoMas = db.MasterLCInfoMas.Find(id);
            if (masterLCInfoMas == null)
            {
                return HttpNotFound();
            }
            return View(masterLCInfoMas);
        }

        // GET: MasterLC/Create
        public ActionResult Create()
        {            
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.BuyerBankId = new SelectList(db.BankBranch.OrderBy(x => x.Name).Where(x=>x.IsForeign==true), "Id", "Name");

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text");

            ViewBag.PaymentTerm = paymentTerm;
            return View();
        }

        // POST: MasterLC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,LCNo,LCDate,LCExpiryDate,PaymentTerm,Tenor,LCReceiveDate,LatestShipmentDate,Quantity,TotalValue,BuyerInfoId,BuyerBankId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] MasterLCInfoMas masterLCInfoMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.MasterLCInfoMas.Add(masterLCInfoMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", masterLCInfoMas.BuyerInfoId);
        //    return View(masterLCInfoMas);
        //}


        public JsonResult SaveLCInfo(IEnumerable<MasterLCInfoDet> LCMasterDetInfo, IEnumerable<VMMasterLCInfoOrderDet> LCDetailOrderInfo, MasterLCInfoMas LCMasterInfo)
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

                        //var LcInfoM = new BuyerOrderMas()
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

                        LCMasterInfo.OpBy = 1;
                        LCMasterInfo.OpOn = OpDate;
                        LCMasterInfo.IsAuth = true;
                        
                        db.MasterLCInfoMas.Add(LCMasterInfo);
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();


                        foreach (var item in LCMasterDetInfo)
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

                            item.MasterLCInfoMasId = LCMasterInfo.Id;
                            db.MasterLCInfoDet.Add(item);
                            db.SaveChanges();

                            //dictionary.Add(item.TempOrderDetId, OrderD.Id);
                            dictionary.Add(item.BuyerOrderMasId, item.Id);

                        }

                        //---- lc order det data

                        //var slno = 1;

                        if (LCDetailOrderInfo != null)
                        {
                            foreach (var item in LCDetailOrderInfo)
                            {
                                var ordetDet = new MasterLCInfoOrderDet()
                                {
                                    Id = item.Id,
                                    MasterLCInfoDetId = dictionary[item.TempMasterLCInfoDetId],
                                    BuyerOrderDetId = item.BuyerOrderDetId,
                                };

                                //db.Entry(deliv).State = deliv.Id == 0 ?
                                //                            EntityState.Added :
                                //                            EntityState.Modified;

                                db.MasterLCInfoOrderDet.Add(ordetDet);
                                db.SaveChanges();

                            }

                        }



                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = LCMasterInfo.Id
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


        // GET: MasterLC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                        
            var lc = (from lcMas in db.MasterLCInfoMas 
                       join buyer in db.BuyerInfo on lcMas.BuyerInfoId equals buyer.Id                       
                       where lcMas.Id == id
                       select new { lcMas, buyer }).SingleOrDefault();

            if (lc == null)
            {
                return HttpNotFound();
            }

            var masterLCInfoMas = new VMMasterLCInfoMas()
            {
                Id = lc.lcMas.Id,
                LCNo = lc.lcMas.LCNo,
                LCDate = lc.lcMas.LCDate ,
                LCReceiveDate = lc.lcMas.LCReceiveDate,
                LCExpiryDate = lc.lcMas.LCExpiryDate,
                BuyerInfoId = lc.lcMas.BuyerInfoId,
                BuyerName = lc.buyer.Name,
                BuyerBankId = lc.lcMas.BuyerBankId,
                LatestShipmentDate = lc.lcMas.LatestShipmentDate,
                PaymentTerm = lc.lcMas.PaymentTerm,
                Quantity = lc.lcMas.Quantity, 
                Tenor = lc.lcMas.Tenor,
                TotalValue = lc.lcMas.TotalValue
            };

            ViewBag.BuyerBankId = new SelectList(db.BankBranch.OrderBy(x => x.Name).Where(x => x.IsForeign == true), "Id", "Name", masterLCInfoMas.BuyerBankId);

            var paymentTerm = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "At Sight", Value = "0" },
                new SelectListItem { Text = "From BL", Value = "1" },
            }, "Value", "Text", masterLCInfoMas.PaymentTerm);

            ViewBag.PaymentTerm = paymentTerm;

            return View(masterLCInfoMas);
        }

        // POST: MasterLC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,LCNo,LCDate,LCExpiryDate,PaymentTerm,Tenor,LCReceiveDate,LatestShipmentDate,Quantity,TotalValue,BuyerInfoId,BuyerBankId,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] MasterLCInfoMas masterLCInfoMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(masterLCInfoMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", masterLCInfoMas.BuyerInfoId);
        //    return View(masterLCInfoMas);
        //}

        //IEnumerable<VMBuyerOrderDetEdit> OrderDetails, IEnumerable<VMShipmentSummDet> DelivDetails, VMBuyerOrderMasEdit OrderMas, int[] DelItems, int[] DelDelivItems

        public JsonResult UpdateLCInfo(IEnumerable<MasterLCInfoDet> LCMasterDetInfo, IEnumerable<VMMasterLCInfoOrderDet> LCDetailOrderInfo, MasterLCInfoMas LCMasterInfo, int[] DeletedItems, int[] DeletedOrderDetails)
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

                        var LcInfoMas = db.MasterLCInfoMas.Find(LCMasterInfo.Id);

                        if (LcInfoMas == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        /*
                         Id = lc.lcMas.Id,
                LCNo = lc.lcMas.LCNo,
                LCDate = lc.lcMas.LCDate ,
                LCReceiveDate = lc.lcMas.LCReceiveDate,
                LCExpiryDate = lc.lcMas.LCExpiryDate,
                BuyerInfoId = lc.lcMas.BuyerInfoId,
                BuyerName = lc.buyer.Name,
                BuyerBankId = lc.lcMas.BuyerBankId,
                LatestShipmentDate = lc.lcMas.LatestShipmentDate,
                PaymentTerm = lc.lcMas.PaymentTerm,
                Quantity = lc.lcMas.Quantity, 
                Tenor = lc.lcMas.Tenor,
                TotalValue = lc.lcMas.TotalValue
                         */
                        LcInfoMas.LCNo = LCMasterInfo.LCNo;
                        LcInfoMas.LCDate = LCMasterInfo.LCDate;
                        LcInfoMas.LCReceiveDate = LCMasterInfo.LCReceiveDate;
                        LcInfoMas.LCExpiryDate = LCMasterInfo.LCExpiryDate;
                        LcInfoMas.BuyerBankId = LCMasterInfo.BuyerBankId;
                        LcInfoMas.LatestShipmentDate = LCMasterInfo.LatestShipmentDate;
                        LcInfoMas.PaymentTerm = LCMasterInfo.PaymentTerm;
                        LcInfoMas.Quantity = LCMasterInfo.Quantity;
                        LcInfoMas.Tenor = LCMasterInfo.Tenor;
                        LcInfoMas.TotalValue = LCMasterInfo.TotalValue;

                        db.Entry(LcInfoMas).State = EntityState.Modified;
                        db.SaveChanges();

                        //---- Delete MasterLCInfoOrderDet, MasterLCInfoDet
                                                
                        if (DeletedOrderDetails != null)
                        {
                            foreach (var item in DeletedOrderDetails)
                            {
                                var orderDets = db.MasterLCInfoOrderDet.Find(item);

                                if (orderDets != null)
                                {
                                    db.MasterLCInfoOrderDet.Remove(orderDets);
                                }                              
                                db.SaveChanges();
                            }
                        }

                        if (DeletedItems != null)
                        {
                            foreach (var item in DeletedItems)
                            {
                                
                                var orderDets = db.MasterLCInfoOrderDet.Where(x => x.MasterLCInfoDetId == item);

                                if (orderDets != null)
                                {
                                    db.MasterLCInfoOrderDet.RemoveRange(orderDets);
                                }

                                var delLCDet = db.MasterLCInfoDet.Find(item);
                                db.MasterLCInfoDet.Remove(delLCDet);
                                db.SaveChanges();
                            }
                        }

                        
                        // --- Save LC Det

                        // ---- save LC ORder det


                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();

                        foreach (var item in LCMasterDetInfo)
                        {                            
                            item.MasterLCInfoMasId = LCMasterInfo.Id ;
                            db.Entry(item).State = item.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;
                                                        
                            db.SaveChanges();

                            dictionary.Add(item.BuyerOrderMasId, item.Id);

                        }

                        //var slno = 1;

                        if (LCDetailOrderInfo != null)
                        {
                            foreach (var item in LCDetailOrderInfo)
                            {
                                var ordetDet = new MasterLCInfoOrderDet()
                                {
                                    Id = item.Id,
                                    MasterLCInfoDetId = dictionary[item.TempMasterLCInfoDetId],
                                    BuyerOrderDetId = item.BuyerOrderDetId,
                                };

                                db.Entry(ordetDet).State = ordetDet.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                //db.MasterLCInfoOrderDet.Add(ordetDet);
                                db.SaveChanges();

                            }

                        }

                        //if (DelivDetails != null)
                        //{
                        //    foreach (var item in DelivDetails)
                        //    {
                        //        var deliv = new ShipmentSummDet()
                        //        {
                        //            Id = item.Id,
                        //            BuyerOrderDetId = item.BuyerOrderDetId == 0 ? dictionary[item.DelivOrderDetTempId] : item.BuyerOrderDetId,
                        //            DelivSlno = slno++,
                        //            ExFactoryDate = item.ExFactoryDate,
                        //            HandoverDate = item.HandoverDate,
                        //            ETD = item.ETD,
                        //            DestinationPortId = item.DestinationPortId,
                        //            DelivQuantity = item.DelivQuantity,
                        //            BuyersPONo = item.BuyersPONo,
                        //            IsLocked = false
                        //        };

                        //        db.Entry(deliv).State = deliv.Id == 0 ?
                        //                                    EntityState.Added :
                        //                                    EntityState.Modified;

                        //        //db.BuyerOrderDets.Add(OrderD);
                        //        db.SaveChanges();

                        //    }
                        //}




                        ////--- delete shipment detail items
                        //if (DelDelivItems != null)
                        //{
                        //    foreach (var item in DelDelivItems)
                        //    {
                        //        var delOrder = db.ShipmentSummDet.Find(item);
                        //        db.ShipmentSummDet.Remove(delOrder);
                        //        db.SaveChanges();
                        //    }
                        //}

                        //---- delete order detail items
                        //if (DelItems != null)
                        //{
                        //    foreach (var item in DelItems)
                        //    {
                        //        //---- if order detail deleted without manual deletion of shipment
                        //        // ---- find those shipment and delete first

                        //        var shipDets = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == item);

                        //        if (shipDets != null)
                        //        {
                        //            db.ShipmentSummDet.RemoveRange(shipDets);
                        //        }

                        //        var delOrder = db.BuyerOrderDet.Find(item);
                        //        db.BuyerOrderDet.Remove(delOrder);
                        //        db.SaveChanges();
                        //    }
                        //}



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



        public JsonResult GetDetailData(int Id)
        {
            var list = (from lcDet in db.MasterLCInfoDet
                        where lcDet.MasterLCInfoMasId == Id
                        select new { lcDet }).AsEnumerable()                       
                       .Select(x => new
                       {
                           Id = x.lcDet.Id,
                           BuyerOrderMasId = x.lcDet.BuyerOrderMasId,                           
                           PINo = x.lcDet.PINo??"",                           
                           PIDate = x.lcDet.PIDate.HasValue ? x.lcDet.PIDate.Value.ToString("dd/MM/yyyy") : ""
                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDetailOrderData(int Id)
        {


            var list = (from lcDet in db.MasterLCInfoDet
                        join bOrderDet in db.BuyerOrderDet on lcDet.BuyerOrderMasId equals bOrderDet.BuyerOrderMasId
                        join supp in db.Supplier on bOrderDet.SupplierId equals supp.Id
                        join j in db.MasterLCInfoOrderDet on new { a = lcDet.Id, b = bOrderDet.Id } equals new { a = j.MasterLCInfoDetId, b = j.BuyerOrderDetId } into joinDetailOrder
                        from detailOrder in joinDetailOrder.DefaultIfEmpty()
                        where lcDet.MasterLCInfoMasId == Id
                        select new { bOrderDet, supp, detailOrder }).AsEnumerable()
                        .Select(x => new
                        {
                            Id = x.detailOrder == null ? 0 : x.detailOrder.Id,
                            BuyerOrderDetId = x.bOrderDet.Id,
                            BuyerOrderMasId = x.bOrderDet.BuyerOrderMasId,
                            MasterLCInfoDetId = x.detailOrder == null ? 0 : x.detailOrder.MasterLCInfoDetId,
                            StyleNo = x.bOrderDet.StyleNo,
                            Quantity = x.bOrderDet.Quantity,
                            TotalValue = x.bOrderDet.Quantity * x.bOrderDet.UnitPrice,
                            FactoryName = x.supp.Name,
                            //ExFactoryDate = x.bOrderDet.ExFactoryDate.HasValue ? x.bOrderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                            TotalTransValue = (from factOrderDeliv in db.FactoryOrderDelivDet
                                               where factOrderDeliv.FactoryOrderDet.BuyerOrderDetId == x.bOrderDet.Id
                                               select new { TransValue = (factOrderDeliv.FactTransferPrice == null ? factOrderDeliv.FactFOB : factOrderDeliv.FactTransferPrice) * factOrderDeliv.ShipmentSummDet.DelivQuantity }).Sum(m => m.TransValue),
                            IsStyleIncluded = x.detailOrder == null ? false : true
                        });

            //var list = (from orderDet in db.BuyerOrderDet
            //                //join prodCatType in db.ProdCatType on orderDet.ProdCatTypeId equals prodCatType.Id
            //                //join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
            //                //join size in db.ProdSizes on orderDet.ProdSizeId equals size.Id
            //                //join fabItem in db.FabricItems on orderDet.FabricItemId equals fabItem.Id
            //                //join prodColor in db.ProdColors on orderDet.ProdColorId equals prodColor.Id
            //            join supp in db.Supplier on orderDet.SupplierId equals supp.Id
            //            join lcOrderDet in db.MasterLCInfoOrderDet on orderDet.Id equals lcOrderDet.BuyerOrderDetId
            //            where orderDet.BuyerOrderMasId == Id
            //            select new { orderDet, supp }).AsEnumerable()
            //           //select new VMBuyerOrderDetEdit()
            //           .Select(x => new
            //           {
            //               Id = x.orderDet.Id,
            //               BuyerOrderMasId = x.orderDet.BuyerOrderMasId,
            //               //ProdCatId = x.prodCat.Id,
            //               //ProdCatName = prodCat.Name,
            //               //ProdCatTypeId = x.prodCatType.Id,
            //               //ProdCatTypeName = prodCatType.Name,
            //               StyleNo = x.orderDet.StyleNo,
            //               //ProdSizeId = x.orderDet.ProdSizeId,
            //               //ProdSizeName = size.SizeRange,
            //               //FabricItemId = x.orderDet.FabricItemId,
            //               //FabricItemName = fabItem.Name,
            //               //ProdColorId = x.orderDet.ProdColorId,
            //               //ProdColorName = prodColor.Name,
            //               //UnitPrice = x.orderDet.UnitPrice,
            //               Quantity = x.orderDet.Quantity,
            //               TotalValue = x.orderDet.Quantity * x.orderDet.UnitPrice,
            //               //SupplierId = x.orderDet.SupplierId,
            //               FactoryName = x.supp.Name,
            //               ExFactoryDate = x.orderDet.ExFactoryDate.HasValue ? x.orderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : ""
            //           });


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //// GET: MasterLC/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MasterLCInfoMas masterLCInfoMas = db.MasterLCInfoMas.Find(id);
        //    if (masterLCInfoMas == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(masterLCInfoMas);
        //}

        //// POST: MasterLC/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    MasterLCInfoMas masterLCInfoMas = db.MasterLCInfoMas.Find(id);
        //    db.MasterLCInfoMas.Remove(masterLCInfoMas);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        public JsonResult DeleteLCInfo(int Id)
        {
            //string result = "";

            bool flag = false;
            try
            {

                var itemToDeleteMas = db.MasterLCInfoMas.Find(Id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nLC Information Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //var checkFactOrder = db.FactoryOrderMas.Where(x => x.BuyerOrderMasId == id).ToList();

                //if (checkFactOrder.Count > 0)
                //{
                //    var result = new
                //    {
                //        flag = false,
                //        message = "Deletion failed!\nFactory order exists. Delete factory order data first."
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);

                //}


                var itemsToDeleteDet = db.MasterLCInfoDet.Where(x => x.MasterLCInfoMasId == Id);

                foreach (var item in itemsToDeleteDet)
                {
                    var itemsToDeleteOrderDet = db.MasterLCInfoOrderDet.Where(x => x.MasterLCInfoDetId == item.Id);
                    db.MasterLCInfoOrderDet.RemoveRange(itemsToDeleteOrderDet);
                }

                db.MasterLCInfoDet.RemoveRange(itemsToDeleteDet);

                db.MasterLCInfoMas.Remove(itemToDeleteMas);

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

        public JsonResult GetLCInfo(int Id)
        {
            var lc = (from lcMas in db.MasterLCInfoMas
                      join buyer in db.BuyerInfo on lcMas.BuyerInfoId equals buyer.Id
                      where lcMas.Id == Id
                      select new { lcMas, buyer }).SingleOrDefault();

            Dictionary<int, string> dictPayTerm =
                                new Dictionary<int, string>();

            dictPayTerm.Add(0, "At Sight");
            dictPayTerm.Add(1, "From BL");

            var masterLCInfoMas = new 
            {
                Id = lc.lcMas.Id,
                LCNo = lc.lcMas.LCNo,
                LCDate = lc.lcMas.LCDate.HasValue ? lc.lcMas.LCDate.Value.ToString("dd/MM/yyyy"):"",
                LCReceiveDate = lc.lcMas.LCReceiveDate.HasValue ? lc.lcMas.LCReceiveDate.Value.ToString("dd/MM/yyyy") : "",
                LCExpiryDate = lc.lcMas.LCExpiryDate.HasValue ? lc.lcMas.LCExpiryDate.Value.ToString("dd/MM/yyyy") : "",
                BuyerInfoId = lc.lcMas.BuyerInfoId,
                BuyerName = lc.buyer.Name,
                //BuyerBankId = lc.lcMas.BuyerBankId,
                LatestShipmentDate = lc.lcMas.LatestShipmentDate.HasValue ? lc.lcMas.LatestShipmentDate.Value.ToString("dd/MM/yyyy") : "",
                PaymentTerm = dictPayTerm[lc.lcMas.PaymentTerm??0],
                Quantity = lc.lcMas.Quantity,
                Tenor = lc.lcMas.Tenor,
                TotalValue = lc.lcMas.TotalValue
            };
                      

            return Json(masterLCInfoMas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLCFactory(int Id)
        {

            var data = (from lcDet in db.MasterLCInfoDet
                       join lcOrder in db.MasterLCInfoOrderDet on lcDet.Id equals lcOrder.MasterLCInfoDetId
                       join bOrder in db.BuyerOrderDet on lcOrder.BuyerOrderDetId equals bOrder.Id
                       join supp in db.Supplier on bOrder.SupplierId equals supp.Id 
                       where lcDet.MasterLCInfoMasId == Id
                       select new { Name = supp.Name, Id= supp.Id}).Distinct().ToList();

            //var data = db.BuyerOrderMas.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.OrderRefNo).Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetLCFactOrder(int MasLCId, int FactId)
        {

            var data = (from lcDet in db.MasterLCInfoDet
                        join lcOrder in db.MasterLCInfoOrderDet on lcDet.Id equals lcOrder.MasterLCInfoDetId
                        join bOrder in db.BuyerOrderDet on lcOrder.BuyerOrderDetId equals bOrder.Id
                        join supp in db.Supplier on bOrder.SupplierId equals supp.Id
                        //join fOrder in db.FactoryOrderMas on new { a=bOrder.BuyerOrderMasId, b=bOrder.SupplierId??0 } equals new { a=fOrder.BuyerOrderMasId , b=fOrder.SupplierId}
                        join fOrder in db.FactoryOrderMas on new { a = bOrder.BuyerOrderMasId, b = bOrder.SupplierId } equals new { a = fOrder.BuyerOrderMasId, b = fOrder.SupplierId }
                        where lcDet.MasterLCInfoMasId == MasLCId && bOrder.SupplierId == FactId
                        select new { Name = fOrder.SalesContractNo, Id = fOrder.Id }).Distinct().ToList();

            //var data = db.BuyerOrderMas.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.OrderRefNo).Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetLCFactOrderDet(int MasLCId, int FactOrderId)
        {
            // factoryOrderDet
            //var dataFact = from fOrderMas in db.FactoryOrderMas
            //               join fOrderDet in db.FactoryOrderDet on fOrderMas.Id equals fOrderDet.FactoryOrderMasId
            //               join bOrderDet in db.BuyerOrderDet on fOrderDet.BuyerOrderDetId equals bOrderDet.Id 
            //                where fOrderMas.Id == FactOrderId
            //                select new { fOrderDet, bOrderDet};

            var dataFact = from fOrderDet in db.FactoryOrderDet 
                           join bOrderDet in db.BuyerOrderDet on fOrderDet.BuyerOrderDetId equals bOrderDet.Id
                           where fOrderDet.FactoryOrderMasId == FactOrderId
                           select new { fOrderDet, bOrderDet };

            var totFactQnty = 0;
            decimal totFactVal = 0;

            foreach(var item in dataFact)
            {
                totFactQnty = totFactQnty + item.bOrderDet.Quantity??0;
                totFactVal = totFactVal + ((item.bOrderDet.Quantity ?? 0) * (item.bOrderDet.UnitPrice ?? 0));
            }

            //var dataLCFact = from lcDet in db.MasterLCInfoDet
            //            join lcOrder in db.MasterLCInfoOrderDet on lcDet.Id equals lcOrder.MasterLCInfoDetId
            //            join bOrderDet in db.BuyerOrderDet on lcOrder.BuyerOrderDetId equals bOrderDet.Id
            //            join fOrderDet in db.FactoryOrderDet on bOrderDet.Id equals fOrderDet.BuyerOrderDetId
            //            where lcDet.MasterLCInfoMasId == MasLCId && fOrderDet.FactoryOrderMasId == FactOrderId
            //            select new { fOrderDet, bOrderDet };

            var dataLCFact = from lcDet in db.MasterLCInfoDet
                             join lcOrder in db.MasterLCInfoOrderDet on lcDet.Id equals lcOrder.MasterLCInfoDetId
                             join bOrderDet in db.BuyerOrderDet on lcOrder.BuyerOrderDetId equals bOrderDet.Id
                             join shipDet in db.ShipmentSummDet on bOrderDet.Id equals shipDet.BuyerOrderDetId
                             join fOrderDet in db.FactoryOrderDet on bOrderDet.Id equals fOrderDet.BuyerOrderDetId
                             join factdelivDet in db.FactoryOrderDelivDet on fOrderDet.Id equals factdelivDet.FactoryOrderDetId
                             where lcDet.MasterLCInfoMasId == MasLCId && fOrderDet.FactoryOrderMasId == FactOrderId
                             select new { fOrderDet, bOrderDet, factdelivDet, shipDet };

            var totLCFactQnty = 0;
            decimal totLCFactVal = 0;

            foreach (var item in dataLCFact)
            {
                totLCFactQnty = totLCFactQnty + item.bOrderDet.Quantity ?? 0;
                //totLCFactVal = totLCFactVal + ((item.bOrderDet.Quantity ?? 0) * (item.bOrderDet.UnitPrice ?? 0));
                //totLCFactVal = totLCFactVal + ((item.bOrderDet.Quantity ?? 0) * (item.factdelivDet.FactFOB ?? 0));
                totLCFactVal = totLCFactVal + (item.shipDet.DelivQuantity * ((item.factdelivDet.FactTransferPrice==null? item.factdelivDet.FactFOB : item.factdelivDet.FactTransferPrice) ?? 0));
            }
            // TotFactQnty TotFactVal TotLCFactQnty TotLCFactVal
            var data = new
            {
                TotFactQnty = totFactQnty,
                TotFactVal = totFactVal,
                TotLCFactQnty = totLCFactQnty,
                TotLCFactVal = totLCFactVal
            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNamesByBuyer(int Id)
        {
            var data = db.MasterLCInfoMas.OrderBy(x => x.LCNo).Where(x => x.BuyerInfoId == Id).Select(y => new { Name = y.LCNo, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBuyerByLC()
        {
            var data = db.MasterLCInfoMas.OrderBy(x => x.LCNo).Select(y => new { Name = y.BuyerInfo.Name, Id = y.BuyerInfo.Id }).Distinct().ToList();
           
            return Json(data, JsonRequestBehavior.AllowGet);

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
