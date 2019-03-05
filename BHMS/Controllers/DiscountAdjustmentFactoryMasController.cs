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
    public class DiscountAdjustmentFactoryMasController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: DiscountAdjustmentFactoryMas
        public ActionResult Index()
        {
            var discountAdjustmentFactoryMas = db.DiscountAdjustmentFactoryMas.Include(d => d.BuyerInfo).Include(d => d.Supplier);
            return View(discountAdjustmentFactoryMas.ToList());
        }

        // GET: DiscountAdjustmentFactoryMas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountAdjustmentFactoryMas discountAdjustmentFactoryMas = db.DiscountAdjustmentFactoryMas.Find(id);
            if (discountAdjustmentFactoryMas == null)
            {
                return HttpNotFound();
            }
            return View(discountAdjustmentFactoryMas);
        }

        // GET: DiscountAdjustmentFactoryMas/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            //ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.BuyerOrderMasAdjId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            ViewBag.BuyerOrderMasPrevId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }


        //// GET: DiscountAdjustmentFactoryMas/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DiscountAdjustmentFactoryMas discountAdjustmentFactoryMas = db.DiscountAdjustmentFactoryMas.Find(id);
        //    if (discountAdjustmentFactoryMas == null)
        //    {
        //        return HttpNotFound();
        //    }


        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", discountAdjustmentFactoryMas.BuyerInfoId);
        //    //  ViewBag.BuyerOrderMasAdjId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", discountAdjustmentFactoryMas.BuyerOrderMasAdjId);
        //    //  ViewBag.BuyerOrderMasPrevId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", discountAdjustmentFactoryMas.BuyerOrderMasPrevId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", discountAdjustmentFactoryMas.SupplierId);
        //    return View(discountAdjustmentFactoryMas);
        //}





        // POST: DiscountAdjustmentFactoryMas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,SupplierId,BuyerInfoId,BuyerOrderMasPrevId,BuyerOrderMasAdjId,DateAdj,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DiscountAdjustmentFactoryMas discountAdjustmentFactoryMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(discountAdjustmentFactoryMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", discountAdjustmentFactoryMas.BuyerInfoId);
        //    //   ViewBag.BuyerOrderMasAdjId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", discountAdjustmentFactoryMas.BuyerOrderMasAdjId);
        //    //   ViewBag.BuyerOrderMasPrevId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo", discountAdjustmentFactoryMas.BuyerOrderMasPrevId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", discountAdjustmentFactoryMas.SupplierId);
        //    return View(discountAdjustmentFactoryMas);
        //}

        // GET: DiscountAdjustmentFactoryMas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountAdjustmentFactoryMas discountAdjustmentFactoryMas = db.DiscountAdjustmentFactoryMas.Find(id);
            if (discountAdjustmentFactoryMas == null)
            {
                return HttpNotFound();
            }
            return View(discountAdjustmentFactoryMas);
        }

        // POST: DiscountAdjustmentFactoryMas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DiscountAdjustmentFactoryMas discountAdjustmentFactoryMas = db.DiscountAdjustmentFactoryMas.Find(id);
            db.DiscountAdjustmentFactoryMas.Remove(discountAdjustmentFactoryMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        //public JsonResult GetNames()
        //{
        //    var data = db.BuyerOrderMas.OrderBy(x => x.OrderRefNo).Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetBuyersUnderSupplier(int Id)
        {
            var data = db.BuyerOrderDet.Where(x => x.SupplierId == Id).Select(y => new { Name = y.BuyerOrderMas.BuyerInfo.Name, Id = y.BuyerOrderMas.BuyerInfoId }).ToList();

            return Json(data.Distinct(), JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetDelivDataPrev(int BuyerOrderMasPrevId, int SupplierId, int BuyerInfoId)
        {
            /*select buyDet.StyleNo, shipSumm.BuyersPONo, shipSumm.DelivSlno,
                shipSumm.DestinationPortId, buyDet.Quantity, shipSumm.DelivQuantity, factDeliv.FactFOB,
                discountDet.FactoryDiscount from [dbo].[BuyerOrderMas] buyMas 
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[ShipmentSummDet] shipSumm on buyDet.Id= shipSumm.BuyerOrderDetId
                inner join [dbo].[FactoryOrderDelivDet] factDeliv on shipSumm.Id = factDeliv.ShipmentSummDetId
                inner join [dbo].[DiscountDet] discountDet on factDeliv.Id = discountDet.FactoryOrderDelivDetId
                */

            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where buyermas.Id == BuyerOrderMasPrevId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId
                             && discountDet.AdjustFactoryNow == false
                             select new
                             {
                                 FactOrderDelivDetId = factDeliv.Id,
                                 //BuyerDetId = buyerDet.Id,
                                 BuyerMasId = buyermas.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = factDeliv.FactFOB,
                                 //FactValue = (factDeliv.FactTransferPrice == null ? factDeliv.FactFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue = factDeliv.FactFOB * ship.DelivQuantity,
                                 DiscountPercent = discountDet.FactoryDiscount,
                                 //DiscountedAmt = ((factDeliv.FactTransferPrice == null ? factDeliv.FactFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity) * (discountDet.FactoryDiscount / 100)
                                 DiscountedAmt = (factDeliv.FactFOB * ship.DelivQuantity) * (discountDet.FactoryDiscount / 100)

                             }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPrevNames(int Id, int SupplierId)
        {

            List<SelectListItem> RdlList = new List<SelectListItem>();

            var rdlRefs = (from buyermas in db.BuyerOrderMas
                           join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                           join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                           join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                           join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                           where discountDet.AdjustFactoryNow == false && buyerDet.SupplierId == SupplierId && buyermas.BuyerInfoId == Id

                           select buyermas
                     ).Distinct().ToList();

            foreach (var x in rdlRefs)
            {
                RdlList.Add(new SelectListItem { Text = x.OrderRefNo, Value = x.Id.ToString() });
            }

            var result = new
            {
                ListOfRdlRef = RdlList
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetAdjNames(int Id, int SupplierId)
        //{

        //    var delivData = (from buyermas in db.BuyerOrderMas
        //                     join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
        //                     join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
        //                     join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
        //                     //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
        //                     where buyerDet.SupplierId == SupplierId && buyermas.BuyerInfoId == Id 
        //                     //&&
        //                     //!(from disMas in db.DiscountMas
        //                     //select new { a = disMas.BuyerOrderDetId }).Contains(new { a = buyerDet.Id })
        //                     select buyermas
        //                 ).Distinct().ToList();


        //    var data = delivData.Select(y => new { Name = y.OrderRefNo, Id = y.Id }).Distinct().ToList();

        //    // var data = db.BuyerOrderDet.Where(x => x.BuyerOrderMas.BuyerInfoId == Id && x.SupplierId == SupplierId).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).Distinct().ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetAdjNames(int Id, int SupplierId)
        {


            List<SelectListItem> RdlList = new List<SelectListItem>();

            var rdlRefs = (from buyermas in db.BuyerOrderMas
                           join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                           join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                           join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                           //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                           where buyerDet.SupplierId == SupplierId && buyermas.BuyerInfoId == Id
                           //&&
                           //!(from disMas in db.DiscountMas
                           //select new { a = disMas.BuyerOrderDetId }).Contains(new { a = buyerDet.Id })
                           select buyermas
                         ).Distinct().ToList();

            foreach (var x in rdlRefs)
            {
                RdlList.Add(new SelectListItem { Text = x.OrderRefNo, Value = x.Id.ToString() });
            }

            var result = new
            {
                ListOfRdlRef = RdlList
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetDelivDataAdj(int BuyerOrderMasAdjId, int SupplierId, int BuyerInfoId)
        {
            /*select buyDet.StyleNo, shipSumm.BuyersPONo, shipSumm.DelivSlno,
                shipSumm.DestinationPortId, buyDet.Quantity, shipSumm.DelivQuantity, factDeliv.FactFOB,
                discountDet.FactoryDiscount from [dbo].[BuyerOrderMas] buyMas 
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[ShipmentSummDet] shipSumm on buyDet.Id= shipSumm.BuyerOrderDetId
                inner join [dbo].[FactoryOrderDelivDet] factDeliv on shipSumm.Id = factDeliv.ShipmentSummDetId
                inner join [dbo].[DiscountDet] discountDet on factDeliv.Id = discountDet.FactoryOrderDelivDetId
                */

            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join factDiscDet in db.DiscountAdjustmentFactoryDet on factDeliv.Id equals factDiscDet.FactoryOrderDelivDetId into res
                             from factDiscDet in res.DefaultIfEmpty()
                                 //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where buyermas.Id == BuyerOrderMasAdjId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId

                             select new
                             {
                                 FactOrderDelivDetId = factDeliv.Id,
                                 BuyerDetId = buyerDet.Id,
                                 BuyerMasId = buyermas.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = factDeliv.FactFOB,
                                 //FactValue = (factDeliv.FactTransferPrice == null ? factDeliv.FactFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue = factDeliv.FactFOB * ship.DelivQuantity,
                                 Adjustment = factDiscDet.AdjustmentAmt == null ? null : factDiscDet.AdjustmentAmt,
                                 FactoryTransFOB = ((factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt)) / ship.DelivQuantity,
                                 FactoryInvVal = (factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt),
                                 flag = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factDiscDet.FactoryOrderDelivDetId).Count() == 0 ? true : false
            // DiscountAdjId= factDiscDet.DiscountAdjustmentFactoryAdjId
        }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }











        //-----------------------------------------------------------------------------EDIT-------------------------------------------------------------------------------


        public ActionResult Edit(int id)
        {
            //ViewBag.DiscountFactoryMasId = id;

            var selectedData = (from adjFactMas in db.DiscountAdjustmentFactoryMas
                                where adjFactMas.Id == id
                                select adjFactMas).Distinct().ToList();



            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", selectedData.SingleOrDefault(x => x.Id == id).SupplierId);
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", selectedData.SingleOrDefault(x => x.Id == id).BuyerInfoId);
            ViewBag.DateAdj = NullHelpers.DateToString(db.DiscountAdjustmentFactoryMas.SingleOrDefault(x => x.Id == id).DateAdj);

            return View();
        }

        public JsonResult GetDelivDataAdjEdit(int DiscountFactoryMasId, int RDLRefIdAdj)
        {
            /*select buyDet.StyleNo, shipSumm.BuyersPONo, shipSumm.DelivSlno,
                shipSumm.DestinationPortId, buyDet.Quantity, shipSumm.DelivQuantity, factDeliv.FactFOB,
                discountDet.FactoryDiscount from [dbo].[BuyerOrderMas] buyMas 
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[ShipmentSummDet] shipSumm on buyDet.Id= shipSumm.BuyerOrderDetId
                inner join [dbo].[FactoryOrderDelivDet] factDeliv on shipSumm.Id = factDeliv.ShipmentSummDetId
                inner join [dbo].[DiscountDet] discountDet on factDeliv.Id = discountDet.FactoryOrderDelivDetId
                */

            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join discAdj in db.DiscountAdjustmentFactoryAdj on buyermas.Id equals discAdj.BuyerOrderMasId
                             join discFactMas in db.DiscountAdjustmentFactoryMas on discAdj.DiscountAdjustmentFactoryMasId equals discFactMas.Id
                             join discFactDet in db.DiscountAdjustmentFactoryDet on new { a = discAdj.Id, b = factDeliv.Id } equals new { a = discFactDet.DiscountAdjustmentFactoryAdjId, b = discFactDet.FactoryOrderDelivDetId }
                             //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where discFactMas.Id == DiscountFactoryMasId && discAdj.BuyerOrderMasId == RDLRefIdAdj

                             select new
                             {
                                 FactOrderDelivDetId = factDeliv.Id,
                                 BuyerDetId = buyerDet.Id,
                                 BuyerMasId = buyermas.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = factDeliv.FactFOB,
                                 //FactValue = (factDeliv.FactTransferPrice == null ? factDeliv.FactFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue = factDeliv.FactFOB * ship.DelivQuantity,
                                 // AdjustVal = discFactDet.AdjustmentAmt,
                                 DiscountFactDetId = discFactDet.Id,
                                 Adjustment = discFactDet.AdjustmentAmt == null ? null : discFactDet.AdjustmentAmt,
                                 FactoryTransFOB = ((factDeliv.FactFOB * ship.DelivQuantity) - (discFactDet.AdjustmentAmt == null ? 0 : discFactDet.AdjustmentAmt)) / ship.DelivQuantity,
                                 FactoryInvVal = (factDeliv.FactFOB * ship.DelivQuantity) - (discFactDet.AdjustmentAmt == null ? 0 : discFactDet.AdjustmentAmt),


                             }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetRDLPrevNos(int Id)
        {
            var data = db.DiscountAdjustmentFactoryPrev.Where(x => x.DiscountAdjustmentFactoryMasId == Id).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);


            //List<SelectListItem> RdlList = new List<SelectListItem>();

            //var rdlRefs = (from buyermas in db.BuyerOrderMas
            //               join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
            //               join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
            //               join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
            //               join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
            //               join discountFactPrev in db.DiscountAdjustmentFactoryPrev on  buyermas.Id equals discountFactPrev.BuyerOrderMasId 
            //               where discountDet.AdjustBuyerNow == false && discountFactPrev.DiscountAdjustmentFactoryMasId==Id
            //               select buyermas
            //         ).Distinct().ToList();

            //foreach (var x in rdlRefs)
            //{
            //    RdlList.Add(new SelectListItem { Text = x.OrderRefNo, Value = x.Id.ToString() });
            //}

            //var result = new
            //{
            //    ListOfRdlRef = RdlList
            //};

            //return Json(result, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetRDLAdjNos(int Id)
        {
            var data = db.DiscountAdjustmentFactoryAdj.Where(x => x.DiscountAdjustmentFactoryMasId == Id).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).ToList();


            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSelectedPrevRDL(int Id)
        {

            var list = (from discountFactMas in db.DiscountAdjustmentFactoryMas
                        join discountPrev in db.DiscountAdjustmentFactoryPrev on discountFactMas.Id equals discountPrev.DiscountAdjustmentFactoryMasId
                        where discountFactMas.Id == Id
                        select new { discountFactMas, discountPrev }).AsEnumerable()
                       .Select(x => new
                       {
                           BuyerOrderMasPrevId = x.discountPrev.BuyerOrderMasId,
                           PrevMasId = x.discountPrev.Id,
                           Id = x.discountFactMas.Id,
                           PrevRDLRef = x.discountPrev.BuyerOrderMas.OrderRefNo
                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedAdjRDL(int Id)
        {
            var list = (from discountFactMas in db.DiscountAdjustmentFactoryMas
                        join discountAdj in db.DiscountAdjustmentFactoryAdj on discountFactMas.Id equals discountAdj.DiscountAdjustmentFactoryMasId
                        where discountFactMas.Id == Id
                        select new { discountFactMas, discountAdj }).AsEnumerable()
                       .Select(x => new
                       {
                           BuyerOrderMasAdjId = x.discountAdj.BuyerOrderMasId,
                           AdjMasId = x.discountAdj.Id,
                           Id = x.discountFactMas.Id,
                           //  PrevRDLRef = x.discountAdj.BuyerOrderMas.OrderRefNo
                           AdjRDLRef = x.discountAdj.BuyerOrderMas.OrderRefNo


                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDelivDataAdjSelected(int BuyerOrderMasAdjId, int SupplierId, int BuyerInfoId, int AdjId)
        {
            List<VMDiscountAdjFact> list = new List<VMDiscountAdjFact>();

            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join factDiscDet in db.DiscountAdjustmentFactoryDet on factDeliv.Id equals factDiscDet.FactoryOrderDelivDetId
                             join adjMas in db.DiscountAdjustmentFactoryAdj on factDiscDet.DiscountAdjustmentFactoryAdjId equals adjMas.Id
                             where buyermas.Id == BuyerOrderMasAdjId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId
                             && ship.Id == factDeliv.ShipmentSummDetId
                             && adjMas.Id == AdjId
                             //&& !(from ex in db.DiscountAdjustmentFactoryDet
                             //     where ex.DiscountAdjustmentFactoryAdj.DiscountAdjustmentFactoryMasId != MasId
                             //     select new { a = ex.FactoryOrderDelivDetId }).Contains(new { a = factDeliv.Id })
                             select new
                             {
                                 FactOrderDelivDetId = factDeliv.Id,
                                 BuyerMasId = buyermas.Id,
                                 BuyerDetId = buyerDet.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = factDeliv.FactFOB,
                                 //  FactValue = (factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue = factDeliv.FactFOB * ship.DelivQuantity,
                                 Adjustment = factDiscDet.AdjustmentAmt == null ? null : factDiscDet.AdjustmentAmt,
                                 FactoryTransFOB = ((factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt)) / ship.DelivQuantity,
                                 // FactoryTransFOB = ship.RdlFOB * ship.DelivQuantity
                                 FactoryInvVal = (factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt),
                                 DiscountAdjId = factDiscDet.DiscountAdjustmentFactoryAdj.Id,
                                 DiscountFactDetId = factDiscDet.Id
                             }).Distinct().ToList();

            var delivAllData = (from buyermas in db.BuyerOrderMas
                                join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                                join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                                join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                                join factDiscDet in db.DiscountAdjustmentFactoryDet on factDeliv.Id equals factDiscDet.FactoryOrderDelivDetId into res
                                from factDiscDet in res.DefaultIfEmpty()
                                    //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetI
                                where buyermas.Id == BuyerOrderMasAdjId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId
                                select new
                                {
                                    FactOrderDelivDetId = factDeliv.Id,
                                    BuyerMasId = buyermas.Id,
                                    BuyerDetId = buyerDet.Id,
                                    StyleNo = buyerDet.StyleNo,
                                    PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                    DelivNo = ship.DelivSlno,
                                    DestPort = ship.DestinationPort.Name,
                                    OrderQty = buyerDet.Quantity,
                                    ShipQty = ship.DelivQuantity,
                                    FactFOB = factDeliv.FactFOB,
                                    //FactValue = (factDeliv.FactTransferPrice == null ? factDeliv.FactFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                    FactValue = factDeliv.FactFOB * ship.DelivQuantity,
                                    Adjustment = factDiscDet.AdjustmentAmt == null ? null : factDiscDet.AdjustmentAmt,
                                    FactoryTransFOB = ((factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt)) / ship.DelivQuantity,
                                    FactoryInvVal = (factDeliv.FactFOB * ship.DelivQuantity) - (factDiscDet.AdjustmentAmt == null ? 0 : factDiscDet.AdjustmentAmt),
                                    DiscountAdjId = (db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factDeliv.Id).Count() == 0 ? 0 : factDiscDet.DiscountAdjustmentFactoryAdj.Id),
                                    DiscountFactDetId = (db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factDeliv.Id).Count() == 0 ? 0 : factDiscDet.Id)
                                    // DiscountAdjId= factDiscDet.DiscountAdjustmentFactoryAdjId
                                }).Distinct().ToList();

            //var refineData = delivAllData.Except(delivData).ToList();
            foreach (var i in delivData)
            {
                if (delivAllData.Contains(i))
                {
                    delivAllData.Remove(i);
                }
            }

            foreach (var item in delivData)
            {
                VMDiscountAdjFact vm = new VMDiscountAdjFact();
                vm.FactOrderDelivDetId = item.FactOrderDelivDetId;
                vm.BuyerMasId = item.BuyerMasId;
                vm.BuyerDetId = item.BuyerDetId;
                vm.StyleNo = item.StyleNo;
                vm.PONo = item.PONo;
                vm.DelivNo = item.DelivNo;
                vm.DestPort = item.DestPort;
                vm.OrderQty = item.OrderQty;
                vm.ShipQty = item.ShipQty;
                vm.FactFOB = item.FactFOB;
                vm.FactValue = item.FactValue;
                vm.Adjustment = item.Adjustment;
                vm.FactoryTransFOB = item.FactoryTransFOB;
                vm.FactoryInvVal = item.FactoryInvVal;
                vm.DiscountAdjId = item.DiscountAdjId;
                vm.DiscountFactDetId = item.DiscountFactDetId;
                vm.flag = true;
                list.Add(vm);
            }

            var masid = db.DiscountAdjustmentFactoryAdj.SingleOrDefault(x => x.Id == AdjId).DiscountAdjustmentFactoryMasId;

            foreach (var item in delivAllData)
            {
                VMDiscountAdjFact vm = new VMDiscountAdjFact();
                vm.FactOrderDelivDetId = item.FactOrderDelivDetId;
                vm.BuyerMasId = item.BuyerMasId;
                vm.BuyerDetId = item.BuyerDetId;
                vm.StyleNo = item.StyleNo;
                vm.PONo = item.PONo;
                vm.DelivNo = item.DelivNo;
                vm.DestPort = item.DestPort;
                vm.OrderQty = item.OrderQty;
                vm.ShipQty = item.ShipQty;
                vm.FactFOB = item.FactFOB;
                vm.FactValue = item.FactValue;
                vm.Adjustment = 0;
                vm.FactoryTransFOB = item.FactoryTransFOB;
                vm.FactoryInvVal = item.FactoryInvVal;
                vm.DiscountAdjId = 0;
                vm.DiscountFactDetId = 0;
                vm.flag = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == item.FactOrderDelivDetId).Count() == 0 ? true : false;
                list.Add(vm);
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }


        public JsonResult UpdateFactoryOrder(IEnumerable<VMDiscountAdjustFactoryPrev> DetailPrev, IEnumerable<VMDiscountAdjustFactoryAdj> DetailAdj, IEnumerable<VMDiscountAdjustFactoryDet> DetailItems, DiscountAdjustmentFactoryMas OrderMas, int[] deletedPrevItems, int[] deletedAdjItems)
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
                        var OrderMS = db.DiscountAdjustmentFactoryMas.Find(OrderMas.Id);

                        if (OrderMS == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Saving failed !",
                                Id = 0
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        db.Entry(OrderMS).State = EntityState.Modified;
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                  new Dictionary<int, int>();

                        foreach (var item in DetailPrev)
                        {
                            var OrderPrev = new DiscountAdjustmentFactoryPrev()
                            {
                                Id = item.Id,
                                DiscountAdjustmentFactoryMasId = OrderMas.Id,
                                BuyerOrderMasId = item.BuyerMasId,
                            };



                            // dictionary.Add(item.DelivOrderDetTempId, OrderPrev.Id);

                            db.Entry(OrderPrev).State = OrderPrev.Id == 0 ?
                                                          EntityState.Added :
                                                          EntityState.Modified;
                            db.SaveChanges();


                        }

                        foreach (var item in DetailAdj)
                        {
                            var OrderAdj = new DiscountAdjustmentFactoryAdj()
                            {
                                Id = item.Id,
                                DiscountAdjustmentFactoryMasId = OrderMas.Id,
                                BuyerOrderMasId = item.BuyerMasIdAdj,
                            };



                            db.Entry(OrderAdj).State = OrderAdj.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            db.SaveChanges();

                            dictionary.Add(item.TempOrderIdAdj, OrderAdj.Id);


                        }


                        if (DetailItems != null)
                        {
                            foreach (var i in DetailItems)
                            {
                                //var delivDetailAdj = new DiscountAdjustmentFactoryDet()
                                //{
                                //    Id = i.Id,
                                //    //DiscountAdjustmentFactoryAdjId = item.DiscountAdjustmentFactoryMasId,
                                //    DiscountAdjustmentFactoryAdjId = i.Id == 0 ? dictionary[i.DelivOrderDetTempIdAdj] : db.DiscountAdjustmentFactoryDet.SingleOrDefault(x=>x.Id==i.Id).DiscountAdjustmentFactoryAdjId,
                                //    AdjustmentAmt = i.Adjustment,
                                //    FactoryOrderDelivDetId = i.FactOrderDelivDetIdAdj,
                                //};

                                if (i.Id == 0)
                                {
                                    var delivDetailAdj = new DiscountAdjustmentFactoryDet()
                                    {
                                        Id = i.Id,
                                        //DiscountAdjustmentFactoryAdjId = item.DiscountAdjustmentFactoryMasId,
                                        DiscountAdjustmentFactoryAdjId = dictionary[i.DelivOrderDetTempIdAdj],
                                        AdjustmentAmt = i.Adjustment,
                                        FactoryOrderDelivDetId = i.FactOrderDelivDetIdAdj,
                                    };
                                    db.DiscountAdjustmentFactoryDet.Add(delivDetailAdj);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var det = db.DiscountAdjustmentFactoryDet.Find(i.Id);
                                    det.AdjustmentAmt = i.Adjustment;

                                    db.SaveChanges();
                                }


                                //db.Entry(delivDetailAdj).State = delivDetailAdj.Id == 0 ?
                                //                      EntityState.Added :
                                //                      EntityState.Modified;
                                //db.SaveChanges();


                            }

                        }

                        //---- shipment data

                        //    var slno = 1;

                        if (deletedPrevItems != null)
                        {
                            foreach (var item in deletedPrevItems)
                            {
                                var delPrev = db.DiscountAdjustmentFactoryPrev.Find(item);
                                db.DiscountAdjustmentFactoryPrev.Remove(delPrev);
                                db.SaveChanges();
                            }
                        }


                        if (deletedAdjItems != null)
                        {
                            foreach (var item in deletedAdjItems)
                            {
                                //---- if order detail deleted without manual deletion of shipment
                                // ---- find those shipment and delete first

                                var discountDets = db.DiscountAdjustmentFactoryDet.Where(x => x.DiscountAdjustmentFactoryAdjId == item);

                                if (discountDets != null)
                                {
                                    db.DiscountAdjustmentFactoryDet.RemoveRange(discountDets);
                                }

                                var delAdj = db.DiscountAdjustmentFactoryAdj.Find(item);
                                db.DiscountAdjustmentFactoryAdj.Remove(delAdj);
                                db.SaveChanges();
                            }
                        }

                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Updated successfully!!",
                            Id = OrderMas.Id
                        };

                        Success("Record updated successfully.", true);


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



        //--------------------------------------------------------------------------EDIT END------------------------------------------------------------------------------

        public JsonResult SaveFactoryOrder(IEnumerable<VMDiscountAdjustFactoryPrev> DetailPrev, IEnumerable<VMDiscountAdjustFactoryAdj> DetailAdj, IEnumerable<VMDiscountAdjustFactoryDet> DetailItems, DiscountAdjustmentFactoryMas OrderMas)
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

                        var OrderM = new DiscountAdjustmentFactoryMas()
                        {
                            Id = 0,
                            SupplierId = OrderMas.SupplierId,
                            BuyerInfoId = OrderMas.BuyerInfoId,
                            DateAdj = OrderMas.DateAdj,
                            IsAuth = OrderMas.IsAuth,
                            OpBy = 1,
                            OpOn = OpDate,
                            AuthBy = 1,
                            AuthOn = OpDate
                        };

                        db.DiscountAdjustmentFactoryMas.Add(OrderM);
                        db.SaveChanges();


                        Dictionary<int, int> dictionary = new Dictionary<int, int>();



                        if (DetailPrev != null)
                        {
                            foreach (var item in DetailPrev)
                            {
                                var OrderPrev = new DiscountAdjustmentFactoryPrev()
                                {
                                    Id = 0,
                                    DiscountAdjustmentFactoryMasId = OrderM.Id,
                                    BuyerOrderMasId = item.BuyerMasId,
                                };

                                db.DiscountAdjustmentFactoryPrev.Add(OrderPrev);
                                db.SaveChanges();

                                //   dictionary.Add(item.DelivOrderDetTempId, OrderPrev.Id);
                            }
                        }



                        if (DetailAdj != null)
                        {

                            foreach (var i in DetailAdj)
                            {
                                var OrderAdj = new DiscountAdjustmentFactoryAdj()
                                {
                                    Id = 0,
                                    DiscountAdjustmentFactoryMasId = OrderM.Id,
                                    BuyerOrderMasId = i.BuyerMasIdAdj
                                };

                                db.DiscountAdjustmentFactoryAdj.Add(OrderAdj);
                                db.SaveChanges();


                                dictionary.Add(i.TempOrderIdAdj, OrderAdj.Id);
                            }


                            if (DetailItems != null)
                            {
                                foreach (var item in DetailItems)
                                {
                                    var delivDetailAdj = new DiscountAdjustmentFactoryDet()
                                    {
                                        //Id = item.Id,
                                        Id = 0,
                                        //DiscountAdjustmentFactoryAdjId = OrderAdj.Id,
                                        DiscountAdjustmentFactoryAdjId = dictionary[item.DelivOrderDetTempIdAdj],
                                        AdjustmentAmt = item.Adjustment,
                                        FactoryOrderDelivDetId = item.FactOrderDelivDetIdAdj,
                                    };

                                    db.DiscountAdjustmentFactoryDet.Add(delivDetailAdj);
                                    db.SaveChanges();
                                }


                            }

                        }






                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!!",
                            Id = OrderM.Id
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


        //public JsonResult SaveFactoryOrder(IEnumerable<VMDiscountAdjustFactoryDet> DetailPrev, IEnumerable<VMDiscountAdjustFactoryDet> DetailAdj, IEnumerable<VMDiscountAdjustFactoryDet> DetailItems, DiscountAdjustmentFactoryMas OrderMas)
        //{
        //    var result = new
        //    {
        //        flag = false,
        //        message = "Error occured. !",
        //        Id = 0
        //    };

        //    try
        //    {
        //        var OpDate = DateTime.Now;
        //        using (var dbContextTransaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {                   
        //                OrderMas.OpBy = 1;
        //                OrderMas.OpOn = OpDate;
        //                OrderMas.IsAuth = true;                       

        //                db.DiscountAdjustmentFactoryMas.Add(OrderMas);
        //                db.SaveChanges();


        //                //Dictionary<int, int> dictionary =
        //                //          new Dictionary<int, int>();

        //                foreach (var item in DetailPrev)
        //                {
        //                    var OrderPrev = new DiscountAdjustmentFactoryPrev()
        //                    {
        //                        Id = 0,
        //                        DiscountAdjustmentFactoryMasId = OrderMas.Id,
        //                        BuyerOrderMasId = item.BuyerMasId,                             
        //                    };

        //                    db.DiscountAdjustmentFactoryPrev.Add(OrderPrev);
        //                    db.SaveChanges();

        //                   // dictionary.Add(item.DelivOrderDetTempId, OrderPrev.Id);
        //                }

        //                foreach (var item in DetailAdj)
        //                {                     
        //                    var OrderAdj = new DiscountAdjustmentFactoryAdj()
        //                    {
        //                        Id = 0,
        //                        DiscountAdjustmentFactoryMasId = OrderMas.Id,
        //                        BuyerOrderMasId = item.BuyerMasIdAdj,
        //                    };     

        //                    db.DiscountAdjustmentFactoryAdj.Add(OrderAdj);
        //                    db.SaveChanges();

        //                   // dictionary.Add(item.DelivOrderDetTempIdAdj, OrderAdj.Id);

        //                }

        //                //---- shipment data

        //                //    var slno = 1;

        //                if (DetailItems != null)
        //                {
        //                    foreach (var item in DetailItems)
        //                    {
        //                        var delivDetailAdj = new DiscountAdjustmentFactoryDet()
        //                        {
        //                            Id = item.Id,
        //                            DiscountAdjustmentFactoryAdjId = item.DelivOrderDetTempIdAdj,
        //                            AdjustmentAmt = item.Adjustment,
        //                            FactoryOrderDelivDetId = item.FactOrderDelivDetIdAdj,                                
        //                        };


        //                        db.DiscountAdjustmentFactoryDet.Add(delivDetailAdj);
        //                        db.SaveChanges();

        //                    }

        //                }


        //                dbContextTransaction.Commit();

        //                result = new
        //                {
        //                    flag = true,
        //                    message = "Saving successful !!",
        //                    Id = OrderMas.Id
        //                };

        //                Success("Record saved successfully.", true);


        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();

        //                result = new
        //                {
        //                    flag = false,
        //                    message = ex.Message,
        //                    Id = 0
        //                };
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        result = new
        //        {
        //            flag = false,
        //            message = ex.Message,
        //            Id = 0
        //        };
        //    }


        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult DeleteOrder(int id)
        {


            bool flag = false;
            try
            {

                var itemToDeleteMas = db.DiscountAdjustmentFactoryMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nBuyer Discount Not found"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }




                var itemsToDeleteDetPrev = db.DiscountAdjustmentFactoryPrev.Where(x => x.DiscountAdjustmentFactoryMasId == id);

                var itemsToDeleteDetAdj = db.DiscountAdjustmentFactoryAdj.Where(x => x.DiscountAdjustmentFactoryMasId == id);

                foreach (var item in itemsToDeleteDetAdj)
                {
                    var itemsToDeleteDeliv = db.DiscountAdjustmentFactoryDet.Where(x => x.DiscountAdjustmentFactoryAdjId == item.Id);
                    db.DiscountAdjustmentFactoryDet.RemoveRange(itemsToDeleteDeliv);
                }

                db.DiscountAdjustmentFactoryPrev.RemoveRange(itemsToDeleteDetPrev);

                db.DiscountAdjustmentFactoryAdj.RemoveRange(itemsToDeleteDetAdj);

                db.DiscountAdjustmentFactoryMas.Remove(itemToDeleteMas);

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
