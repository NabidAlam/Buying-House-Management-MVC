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
    public class DiscountController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: Discount
        public ActionResult Index()
        {
            var discountMas = db.DiscountMas.Include(d => d.BuyerOrderDet);
            return View(discountMas.ToList());
        }

        // GET: Discount/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountMas discountMas = db.DiscountMas.Find(id);
            if (discountMas == null)
            {
                return HttpNotFound();
            }
            return View(discountMas);
        }

        // GET: Discount/Create
        public ActionResult Create()
        {
            var buyers = (from buyermas in db.BuyerOrderMas
                          join factmas in db.FactoryOrderMas on buyermas.Id equals factmas.BuyerOrderMasId
                          join buyer in db.BuyerInfo on buyermas.BuyerInfoId equals buyer.Id
                          select buyer).Distinct().ToList();

            ViewBag.BuyerInfoId = new SelectList(buyers.OrderBy(x => x.Name), "Id", "Name");
            //ViewBag.SupplierId = new SelectList(db.Supplier.OrderBy(x => x.Name), "Id", "Name");
            //ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            //ViewBag.BuyerOrderDetId = new SelectList(db.BuyerOrderDet, "Id", "StyleNo");
            return View();
        }


        public JsonResult GetSuppliers(int Id)
        {
                var suppliers = (from buyermas in db.BuyerOrderMas
                                 join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                                 join factmas in db.FactoryOrderMas on new { ColA = buyermas.Id, ColB = buyerDet.SupplierId }
                                 equals new { ColA = factmas.BuyerOrderMasId, ColB = factmas.SupplierId }
                                 join factory in db.Supplier on factmas.SupplierId equals factory.Id
                                 where buyermas.BuyerInfoId == Id
                                 select new { Name = factory.Name, Id = factory.Id }).Distinct().ToList();

                return Json(suppliers, JsonRequestBehavior.AllowGet);
         
        }
        


        public JsonResult GetRDlRef(int Id)
        {
            var RDLRef = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join factmas in db.FactoryOrderMas on new { ColA = buyermas.Id, ColB = buyerDet.SupplierId }
                             equals new { ColA = factmas.BuyerOrderMasId, ColB = factmas.SupplierId }
                             join factory in db.Supplier on factmas.SupplierId equals factory.Id
                             where factory.Id == Id
                             select new { Name = buyermas.OrderRefNo, Id = buyermas.Id }).Distinct().ToList();

            return Json(RDLRef, JsonRequestBehavior.AllowGet);

        }

        
        public JsonResult GetStyle(int Id, int factoryId)
        {
            var RDLRef = (from buyermas in db.BuyerOrderMas
                          join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                          join factmas in db.FactoryOrderMas on new { ColA = buyermas.Id, ColB = buyerDet.SupplierId }
                          equals new { ColA = factmas.BuyerOrderMasId, ColB = factmas.SupplierId }
                          join factOrderMas in db.FactoryOrderMas on buyerDet.SupplierId equals factOrderMas.SupplierId
                          join factory in db.Supplier on factmas.SupplierId equals factory.Id
                          where buyermas.Id == Id && buyerDet.SupplierId== factoryId
                          select new { Name = buyerDet.StyleNo, Id = buyerDet.Id }).Distinct().ToList();

            return Json(RDLRef, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetStyleDelivData(int Id)
        {
            var delivData = (from buyermas in db.BuyerOrderMas
                          join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                          join ship in db.ShipmentSummDet  on buyerDet.Id equals ship.BuyerOrderDetId
                          join factmas in db.FactoryOrderMas on new { ColA = buyermas.Id, ColB = buyerDet.SupplierId }
                          equals new { ColA = factmas.BuyerOrderMasId, ColB = factmas.SupplierId }
                          join factDet in db.FactoryOrderDet on factmas.Id equals factDet.FactoryOrderMasId
                          join factDetDeliv in db.FactoryOrderDelivDet on new { ColA = factDet.Id , ColB = ship.Id } equals new { ColA = factDetDeliv.FactoryOrderDetId, ColB = factDetDeliv.ShipmentSummDetId ?? 0 }
                          join factory in db.Supplier on factmas.SupplierId equals factory.Id
                          where buyerDet.Id == Id
                          select new {
                              BuyerDetId = buyerDet.Id,
                              FactoryOrderDelivDetId = factDetDeliv.Id,
                              DelivNo = factDetDeliv.ShipmentSummDet.DelivSlno,
                              PONo = factDetDeliv.ShipmentSummDet.BuyersPONo == null? "" : factDetDeliv.ShipmentSummDet.BuyersPONo,
                              OrderQty = buyerDet.Quantity,
                              ShipQty = factDetDeliv.ShipmentSummDet.DelivQuantity,
                              RDLFOB = factDetDeliv.ShipmentSummDet.RdlFOB,
                              //FactFOB = factDetDeliv.FactTransferPrice == null ? factDetDeliv.FactFOB : factDetDeliv.FactTransferPrice
                              FactFOB =  factDetDeliv.FactFOB
                          }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }


        //// POST: Discount/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,BuyerOrderDetId,DiscountDate,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DiscountMas discountMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.DiscountMas.Add(discountMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerOrderDetId = new SelectList(db.BuyerOrderDet, "Id", "StyleNo", discountMas.BuyerOrderDetId);
        //    return View(discountMas);
        //}


        public JsonResult CheckDiscountMaster(int Id)
        {
            var result = new
            {
                flag = false,
                exists = false,
                id = 0,
                message = "Error"
            };

            var data = db.DiscountMas.Where(x => x.BuyerOrderDetId ==Id).FirstOrDefault();

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


        public JsonResult SaveDiscount(IEnumerable<DiscountDet> DiscountDetail, DiscountMas discountMas)
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
                        var DiscountM = new DiscountMas()
                        {
                            Id = discountMas.Id,
                            BuyerOrderDetId = discountMas.BuyerOrderDetId,
                            DiscountDate = discountMas.DiscountDate,
                            IsAuth = true,
                            OpBy = 1,
                            OpOn = OpDate,
                            AuthBy = 1,
                            AuthOn = OpDate
                        };

                        db.Entry(DiscountM).State = DiscountM.Id == 0 ? EntityState.Added : EntityState.Modified;

                        //db.DiscountMas.Add(DiscountM);
                        db.SaveChanges();


                        foreach (var item in DiscountDetail)
                        {
                            var DiscountD = new DiscountDet()
                            {
                                Id = item.Id,
                                DiscountMasId = DiscountM.Id,
                                FactoryOrderDelivDetId = item.FactoryOrderDelivDetId,
                                BuyerDiscount = item.BuyerDiscount,
                                AdjustBuyerNow = item.AdjustBuyerNow,
                                FactoryDiscount = item.FactoryDiscount,
                                AdjustFactoryNow = item.AdjustFactoryNow
                            };

                            //db.DiscountDet.Add(DiscountD);
                            db.Entry(DiscountD).State = DiscountD.Id == 0 ? EntityState.Added : EntityState.Modified;
                            //db.SaveChanges();
                            //if(DiscountD.FactoryDiscount > 0 && DiscountD.AdjustFactoryNow == true)
                            //{
                            //    var FactoryDeliv = db.FactoryOrderDelivDet.Find(item.FactoryOrderDelivDetId);
                            //    FactoryDeliv.DiscountFlag = true;
                            //}

                            db.SaveChanges();

                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!!",
                            Id = DiscountM.Id
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






        // GET: Discount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountMas discountMas = db.DiscountMas.Find(id);
            if (discountMas == null)
            {
                return HttpNotFound();
            }

            var buyers = (from buyermas in db.BuyerOrderMas
                          join factmas in db.FactoryOrderMas on buyermas.Id equals factmas.BuyerOrderMasId
                          join buyer in db.BuyerInfo on buyermas.BuyerInfoId equals buyer.Id
                          select buyer).Distinct().ToList();

            ViewBag.BuyerInfoId = new SelectList(buyers.OrderBy(x => x.Name), "Id", "Name", discountMas.BuyerOrderDet.BuyerOrderMas.BuyerInfoId);
            ViewBag.SupplierId = new SelectList(db.Supplier.OrderBy(x => x.Name), "Id", "Name", discountMas.BuyerOrderDet.SupplierId);
            ViewBag.BuyerOrderMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", discountMas.BuyerOrderDet.BuyerOrderMas.Id);
            ViewBag.BuyerOrderDetId = new SelectList(db.BuyerOrderDet, "Id", "StyleNo", discountMas.BuyerOrderDetId);

            return View(discountMas);
        }



        public JsonResult GetDiscountSavedData(int Id)
        {
            var discountData = (from discountmas in db.DiscountMas
                             join discountDet in db.DiscountDet on discountmas.Id equals discountDet.DiscountMasId
                             //join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             //join factmas in db.FactoryOrderMas on new { ColA = buyermas.Id, ColB = buyerDet.SupplierId }
                             //equals new { ColA = factmas.BuyerOrderMasId, ColB = factmas.SupplierId }
                             //join factDet in db.FactoryOrderDet on factmas.Id equals factDet.FactoryOrderMasId
                             //join factDetDeliv in db.FactoryOrderDelivDet on new { ColA = factDet.Id, ColB = ship.Id } equals new { ColA = factDetDeliv.FactoryOrderDetId, ColB = factDetDeliv.ShipmentSummDetId ?? 0 }
                             //join factory in db.Supplier on factmas.SupplierId equals factory.Id
                             where discountmas.Id == Id
                             select new
                             {
                                 DiscountDetId = discountDet.Id,
                                 FactoryOrderDelivDetId = discountDet.FactoryOrderDelivDetId,
                                 DelivNo = discountDet.FactoryOrderDelivDet.ShipmentSummDet.DelivSlno,
                                 PONo = discountDet.FactoryOrderDelivDet.ShipmentSummDet.BuyersPONo == null ? "" : discountDet.FactoryOrderDelivDet.ShipmentSummDet.BuyersPONo,
                                 OrderQty = discountmas.BuyerOrderDet.Quantity,
                                 ShipQty = discountDet.FactoryOrderDelivDet.ShipmentSummDet.DelivQuantity,
                                 RDLFOB = discountDet.FactoryOrderDelivDet.ShipmentSummDet.RdlFOB,
                                 BuyerDiscount = discountDet.BuyerDiscount,
                                 BuyerAdjustNow= discountDet.AdjustBuyerNow,
                                 //FactFOB = discountDet.FactoryOrderDelivDet.FactTransferPrice == null ? discountDet.FactoryOrderDelivDet.FactFOB : discountDet.FactoryOrderDelivDet.FactTransferPrice,
                                 FactFOB =  discountDet.FactoryOrderDelivDet.FactFOB,
                                 FactoryDiscount = discountDet.FactoryDiscount,
                                 FactoryAdjustNow = discountDet.AdjustFactoryNow

                             }).Distinct().ToList();

            return Json(discountData, JsonRequestBehavior.AllowGet);

        }

        //// POST: Discount/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,BuyerOrderDetId,DiscountDate,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DiscountMas discountMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(discountMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerOrderDetId = new SelectList(db.BuyerOrderDet, "Id", "StyleNo", discountMas.BuyerOrderDetId);
        //    return View(discountMas);
        //}

        // GET: Discount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountMas discountMas = db.DiscountMas.Find(id);
            if (discountMas == null)
            {
                return HttpNotFound();
            }
            return View(discountMas);
        }

        // POST: Discount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DiscountMas discountMas = db.DiscountMas.Find(id);
            db.DiscountMas.Remove(discountMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public JsonResult DeleteOrder(int id)
        {


            bool flag = false;
            try
            {

                var itemToDeleteMas = db.DiscountMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nDiscount No found"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                var itemsToDeleteDet = db.DiscountDet.Where(x => x.DiscountMasId == id);

                //foreach(var i in itemsToDeleteDet)
                //{
                //    var FactoryDeliv = db.FactoryOrderDelivDet.Find(i.FactoryOrderDelivDetId);
                //    FactoryDeliv.DiscountFlag = false;
                //    FactoryDeliv.DiscountFOB = null;
                //}

                db.DiscountDet.RemoveRange(itemsToDeleteDet);


                db.DiscountMas.Remove(itemToDeleteMas);

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
