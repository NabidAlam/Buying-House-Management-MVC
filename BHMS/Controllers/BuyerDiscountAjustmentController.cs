using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHMS.ViewModels;
using BHMS.Helpers;
using System.Collections;
using System.Data.Entity;

namespace BHMS.Controllers
{
    public class BuyerDiscountAjustmentController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();
        // GET: BuyerDiscountAjustment
        public ActionResult Index()
        {
            var list = db.DiscountAdjustmentBuyerMas.ToList();
            return View(list.ToList());
        }


        // GET: DiscountAdjustmentFactoryMas/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.BuyerOrderMasAdjId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            ViewBag.BuyerOrderMasPrevId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }


        //public JsonResult GetSupplierUnderBuyer(int Id)
        //{
        //    var data = db.BuyerOrderDet.Where(x => x.BuyerOrderMas.BuyerInfoId == Id).Select(y => new { Name = y.Supplier.Name, Id = y.SupplierId }).Distinct().ToList();

        //    return Json(data.Distinct(), JsonRequestBehavior.AllowGet);

        //}


        public JsonResult GetSupplierUnderBuyer(int Id)
        {
            List<SelectListItem> BuyerList = new List<SelectListItem>();


            var buyers = (from supplier in db.Supplier
                          join buyDet in db.BuyerOrderDet on supplier.Id equals buyDet.SupplierId
                          join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
                          join buyInfo in db.BuyerInfo on buyMas.BuyerInfoId equals buyInfo.Id
                          where buyMas.BuyerInfoId == Id
                          select supplier).Distinct().ToList();

            foreach (var x in buyers)
            {
                BuyerList.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }

            var result = new
            {
                ListOfBuyers = BuyerList,

            };

            return Json(result, JsonRequestBehavior.AllowGet);

            /*select distinct buyInfo.Id, buyInfo.Name, supplier.Id, supplier.Name from [dbo].[BuyerOrderDet] buyDet 
                inner join [dbo].[Supplier] supplier on buyDet.SupplierId = Supplier.Id
                inner join [dbo].[BuyerOrderMas] buyMas on buyDet.BuyerOrderMasId = buyMas.Id
                inner join [dbo].[BuyerInfo] buyInfo on buyMas.BuyerInfoId = buyInfo.Id
                where supplier.Id=2*/

        }


        //public JsonResult GetPrevNames(int Id, int SupplierId)
        //{

        //var delivData = (from buyermas in db.BuyerOrderMas
        //                 join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
        //                 join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
        //                 join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
        //                 join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
        //                 where discountDet.AdjustBuyerNow == false && buyerDet.SupplierId == SupplierId && buyermas.BuyerInfoId == Id
        //                 select buyermas
        //             ).Distinct().ToList();


        //    var data = delivData.Select(y => new { Name = y.OrderRefNo, Id = y.Id }).Distinct().ToList();

        //    // var data = db.BuyerOrderDet.Where(x => x.BuyerOrderMas.BuyerInfoId == Id && x.SupplierId == SupplierId).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).Distinct().ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetPrevNames(int Id, int SupplierId)
        {
            

            List<SelectListItem> RdlList = new List<SelectListItem>();

            var rdlRefs = (from buyermas in db.BuyerOrderMas
                           join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                           join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                           join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                           join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                           where discountDet.AdjustBuyerNow == false && buyerDet.SupplierId == SupplierId && buyermas.BuyerInfoId == Id
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

            //var result = new
            //{
            //    ListOfRdlRef = RdlList
            //};

            return Json(RdlList, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetDelivDataPrev(int BuyerOrderMasPrevId, int SupplierId, int BuyerInfoId)
        {
            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where buyermas.Id == BuyerOrderMasPrevId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId
                             && discountDet.AdjustBuyerNow == false
                             select new
                             {
                                 FactOrderDelivDetId = factDeliv.Id,
                                 BuyerDetId = buyerDet.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = ship.RdlFOB,
                                 //FactValue = (factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue =  ship.RdlFOB  * ship.DelivQuantity,
                                 DiscountPercent = discountDet.BuyerDiscount,
                                 //DiscountedAmt = ((factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity) * (discountDet.BuyerDiscount / 100)
                                 DiscountedAmt = ( ship.RdlFOB  * ship.DelivQuantity) * (discountDet.BuyerDiscount / 100)
                             }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetDelivDataAdj(int BuyerOrderMasAdjId, int SupplierId, int BuyerInfoId)
        {
          
            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join exFactShipDet in db.ExFactoryShipDet on buyerDet.Id equals exFactShipDet.BuyerOrderDetId 
                             join buyerDiscDet in db.DiscountAdjustmentBuyerDet on exFactShipDet.Id equals buyerDiscDet.ExFactoryShipDetId into res
                             from buyerDiscDet in res.DefaultIfEmpty()
                             //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where buyermas.Id == BuyerOrderMasAdjId 
                             && buyerDet.SupplierId == SupplierId
                             && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId 
                             && ship.Id == exFactShipDet.ShipmentSummDetId

                             select new
                             {
                                 FactOrderDelivDetId = exFactShipDet.Id,
                                 BuyerMasId=buyermas.Id,
                                 BuyerDetId = buyerDet.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = ship.RdlFOB,
                               //  FactValue = (factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue =  ship.RdlFOB * ship.DelivQuantity,
                                 Adjustment = buyerDiscDet.AdjustmentAmt==null ? null : buyerDiscDet.AdjustmentAmt,
                                 FactoryTransFOB= ((ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt==null? 0: buyerDiscDet.AdjustmentAmt))/ ship.DelivQuantity,
                                // FactoryTransFOB = ship.RdlFOB * ship.DelivQuantity
                                FactoryInvVal= (ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt==null ? 0: buyerDiscDet.AdjustmentAmt)

                             }).Distinct().ToList();

            return Json(delivData, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SaveExFactory(IEnumerable<VMDiscountAdjustmentBuyerPrev> DiscountPrev, IEnumerable<VMDiscountAdjustmentBuyerAdj> DiscountAdj, IEnumerable<VMDiscountAdjustmentBuyerDet> AdjustmentDetails, VMDiscountAdjustmentBuyerMas DiscountMas)
        {

            var storeDetailDetailId = new List<int>();
            int[] detdetList = storeDetailDetailId.ToArray();

            var storeDetailId = new List<int>();
            int[] detList = storeDetailId.ToArray();

            var result = new
            {
                flag = false,
                message = "Error occured. !",
                Id = 0,
                storeDetailDetailId = 0,
                storeDetailId = 0
            };
            
            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var OrderM = new DiscountAdjustmentBuyerMas()
                        {
                            Id = 0,
                            BuyerInfoId = DiscountMas.BuyerInfoId,
                            SupplierId = DiscountMas.SupplierId,
                            DateAdj = DiscountMas.DateAdj,                                                   
                            IsAuth = DiscountMas.IsAuth,
                            OpBy = 1,
                            OpOn = OpDate,
                            AuthBy = 1,
                            AuthOn = OpDate
                        };

                        db.DiscountAdjustmentBuyerMas.Add(OrderM);
                        db.SaveChanges();

                        Dictionary<int, int> dictionary = new Dictionary<int, int>();

                        if (DiscountPrev != null)
                        {

                            foreach (var ship in DiscountPrev)
                            {
                                var deliv2 = new DiscountAdjustmentBuyerPrev()
                                {
                                    Id = 0,
                                    DiscountAdjustmentBuyerMasId = OrderM.Id,
                                    BuyerOrderMasId = ship.BuyerOrderMasId
                                };


                                db.DiscountAdjustmentBuyerPrev.Add(deliv2);
                                db.SaveChanges();


                            }
                        }


                        if (DiscountAdj != null)
                        {

                            foreach (var ship in DiscountAdj)
                            {
                                var deliv = new DiscountAdjustmentBuyerAdj()
                                {
                                    Id = 0,
                                    DiscountAdjustmentBuyerMasId = OrderM.Id,
                                    BuyerOrderMasId = ship.BuyerOrderMasId
                                };


                                db.DiscountAdjustmentBuyerAdj.Add(deliv);
                                db.SaveChanges();

                                var ExDetId = deliv.Id;

                                storeDetailId.Add(deliv.Id);
                                //Detailcounter++;                        
                                dictionary.Add(ship.TempOrderIdAdj, deliv.Id);


                            }

                            if (AdjustmentDetails != null)
                            {

                                foreach (var item in AdjustmentDetails)
                                {
                                    var OrderD = new DiscountAdjustmentBuyerDet()
                                    {
                                        Id = 0,
                                        AdjustmentAmt = item.AdjustmentAmt,
                                        DiscountAdjustmentBuyerAdjId = dictionary[item.DelivOrderDetTempIdAdj],
                                        ExFactoryShipDetId = item.ExFactoryShipDetId


                                    };

                                    db.DiscountAdjustmentBuyerDet.Add(OrderD);
                                    db.SaveChanges();

                                    var ExDetId = OrderD.Id;

                                    //storeDetailId.Add(OrderD.Id);
                                    //Detailcounter++;                        
                                    // dictionary.Add(item.TempOrderId, OrderD.Id);


                                }
                            }




                        }                    



                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!!",
                            Id = OrderM.Id,
                            storeDetailDetailId = 0,
                            storeDetailId = 0


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
                            Id = 0,
                            storeDetailDetailId = 0,
                            storeDetailId = 0
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
                    Id = 0 ,
                    storeDetailDetailId = 0,
                    storeDetailId = 0
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Edit(int id)
        {

            ViewBag.ExMasId = id;


            //var selectedRdlPrev = (from buyerDiscountMas in db.DiscountAdjustmentBuyerMas
            //                       join buyerDiscountPrev in db.DiscountAdjustmentBuyerPrev on buyerDiscountMas.Id equals buyerDiscountPrev.DiscountAdjustmentBuyerMasId
            //                       join buyOrderMas in db.BuyerOrderMas on buyerDiscountPrev.BuyerOrderMasId equals buyOrderMas.Id
            //                       where buyerDiscountMas.Id == id
            //                       select buyOrderMas).SingleOrDefault();

            //var selectedRdlAdj = (from buyerDiscountMas in db.DiscountAdjustmentBuyerMas
            //                      join buyerDiscountAdj in db.DiscountAdjustmentBuyerAdj on buyerDiscountMas.Id equals buyerDiscountAdj.DiscountAdjustmentBuyerMasId
            //                      join buyOrderMas in db.BuyerOrderMas on buyerDiscountAdj.BuyerOrderMasId equals buyOrderMas.Id
            //                      where buyerDiscountMas.Id == id
            //                      select buyOrderMas).SingleOrDefault();


            var selectedBuyerSupplier = (from adjustBuyerMas in db.DiscountAdjustmentBuyerMas
                                             // join buyInfo in db.BuyerInfo on adjustBuyerMas.BuyerInfoId equals buyInfo.Id
                                         where adjustBuyerMas.Id == id
                                         select adjustBuyerMas).SingleOrDefault();




            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", selectedBuyerSupplier.BuyerInfoId);
            ViewBag.SupplierId = new SelectList(db.Supplier.OrderBy(x => x.Name), "Id", "Name", selectedBuyerSupplier.SupplierId);
            //ViewBag.RdlRefNoPrev = new SelectList(db.BuyerOrderMas.OrderBy(x => x.OrderRefNo), "Id", "Name", selectedRdlPrev.OrderRefNo);
            //ViewBag.RdlRefNoAdj = new SelectList(db.BuyerOrderMas.OrderBy(x => x.OrderRefNo), "Id", "Name", selectedRdlAdj.OrderRefNo);
            ViewBag.DateAdj = NullHelpers.DateToString(db.DiscountAdjustmentBuyerMas.SingleOrDefault(x => x.Id == id).DateAdj);



            return View();
        }


        public JsonResult GetRDLPrevNos(int Id)
        {
            var data = db.DiscountAdjustmentBuyerPrev.Where(x => x.DiscountAdjustmentBuyerMasId == Id).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetSelectedPrevRDL(int Id)
        {

            var list = (from discountBuyerMas in db.DiscountAdjustmentBuyerMas
                        join supp in db.Supplier on discountBuyerMas.SupplierId equals supp.Id
                        join buyInfo in db.BuyerInfo on discountBuyerMas.BuyerInfoId equals buyInfo.Id
                        join discountBuyerDet in db.DiscountAdjustmentBuyerPrev on discountBuyerMas.Id equals discountBuyerDet.DiscountAdjustmentBuyerMasId
                        join buyMas in db.BuyerOrderMas on discountBuyerDet.BuyerOrderMasId equals buyMas.Id
                        where discountBuyerMas.Id == Id
                        select new { buyMas, discountBuyerMas, discountBuyerDet }).AsEnumerable()
                       .Select(x => new
                       {
                           BuyerOrderMasPrevId = x.discountBuyerDet.BuyerOrderMasId,
                           PrevMasId = x.discountBuyerDet.Id,
                           Id = x.discountBuyerMas.Id,
                           OrderRef = x.buyMas.OrderRefNo

                       }); 

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRDLAdjNos(int Id)
        {
            var data = db.DiscountAdjustmentBuyerAdj.Where(x => x.DiscountAdjustmentBuyerMasId == Id).Select(y => new { Name = y.BuyerOrderMas.OrderRefNo, Id = y.BuyerOrderMasId }).ToList();


            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetSelectedAdjRDL(int Id)
        {
            var list = (from discountBuyerMas in db.DiscountAdjustmentBuyerMas
                        join discountBuyerDet in db.DiscountAdjustmentBuyerAdj on discountBuyerMas.Id equals discountBuyerDet.DiscountAdjustmentBuyerMasId
                        where discountBuyerMas.Id == Id
                        select new { discountBuyerMas, discountBuyerDet }).AsEnumerable()
                       .Select(x => new
                       {
                           BuyerOrderMasAdjId = x.discountBuyerDet.BuyerOrderMasId,
                           AdjMasId = x.discountBuyerDet.Id,
                           Id = x.discountBuyerMas.Id,
                           PrevRDLRef = x.discountBuyerDet.BuyerOrderMas.OrderRefNo
                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetDelivDataAdjSelected(int BuyerOrderMasAdjId, int SupplierId, int BuyerInfoId,int AdjId)
        {
            List<VMDiscountAdjBuyer> list = new List<VMDiscountAdjBuyer>();

            var delivData = (from buyermas in db.BuyerOrderMas
                             join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                             join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                             join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                             join exFactShipDet in db.ExFactoryShipDet on buyerDet.Id equals exFactShipDet.BuyerOrderDetId
                             join buyerDiscDet in db.DiscountAdjustmentBuyerDet on exFactShipDet.Id equals buyerDiscDet.ExFactoryShipDetId
                             join adjMas in db.DiscountAdjustmentBuyerAdj on buyerDiscDet.DiscountAdjustmentBuyerAdjId equals adjMas.Id
                             //into res
                             //from buyerDiscDet in res.DefaultIfEmpty()
                             //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                             where buyermas.Id == BuyerOrderMasAdjId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId && ship.Id == exFactShipDet.ShipmentSummDetId
                             && adjMas.Id == AdjId
                             //&& !(from ex in db.DiscountAdjustmentBuyerDet
                             //     where ex.DiscountAdjustmentBuyerAdj.DiscountAdjustmentBuyerMasId!=MasId
                             //     select new { a = ex.ExFactoryShipDetId}).Contains(new { a = exFactShipDet.Id })
                             select new
                             {
                                 FactOrderDelivDetId = exFactShipDet.Id,
                                 BuyerMasId = buyermas.Id,
                                 BuyerDetId = buyerDet.Id,
                                 StyleNo = buyerDet.StyleNo,
                                 PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                 DelivNo = ship.DelivSlno,
                                 DestPort = ship.DestinationPort.Name,
                                 OrderQty = buyerDet.Quantity,
                                 ShipQty = ship.DelivQuantity,
                                 FactFOB = ship.RdlFOB,
                                 //  FactValue = (factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                 FactValue = ship.RdlFOB * ship.DelivQuantity,
                                 Adjustment = buyerDiscDet.AdjustmentAmt == null ? null : buyerDiscDet.AdjustmentAmt,
                                 FactoryTransFOB = ((ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt == null ? 0 : buyerDiscDet.AdjustmentAmt)) / ship.DelivQuantity,
                                 // FactoryTransFOB = ship.RdlFOB * ship.DelivQuantity
                                 FactoryInvVal = (ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt == null ? 0 : buyerDiscDet.AdjustmentAmt),
                                 DiscountAdjId = buyerDiscDet.DiscountAdjustmentBuyerAdjId,                               
                                 DiscountBuyerDetId =buyerDiscDet.Id,

                             }).Distinct().ToList();



            var delivAllData = (from buyermas in db.BuyerOrderMas
                                join buyerDet in db.BuyerOrderDet on buyermas.Id equals buyerDet.BuyerOrderMasId
                                join ship in db.ShipmentSummDet on buyerDet.Id equals ship.BuyerOrderDetId
                                join factDeliv in db.FactoryOrderDelivDet on ship.Id equals factDeliv.ShipmentSummDetId
                                join exFactShipDet in db.ExFactoryShipDet on buyerDet.Id equals exFactShipDet.BuyerOrderDetId
                                join buyerDiscDet in db.DiscountAdjustmentBuyerDet on exFactShipDet.Id equals buyerDiscDet.ExFactoryShipDetId into res
                                from buyerDiscDet in res.DefaultIfEmpty()
                                    //join discountDet in db.DiscountDet on factDeliv.Id equals discountDet.FactoryOrderDelivDetId
                                where buyermas.Id == BuyerOrderMasAdjId && buyerDet.SupplierId == SupplierId && buyerDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId && ship.Id == exFactShipDet.ShipmentSummDetId

                                select new
                                {
                                    FactOrderDelivDetId = exFactShipDet.Id,
                                    BuyerMasId = buyermas.Id,
                                    BuyerDetId = buyerDet.Id,
                                    StyleNo = buyerDet.StyleNo,
                                    PONo = ship.BuyersPONo == null ? "" : ship.BuyersPONo,
                                    DelivNo = ship.DelivSlno,
                                    DestPort = ship.DestinationPort.Name,
                                    OrderQty = buyerDet.Quantity,
                                    ShipQty = ship.DelivQuantity,
                                    FactFOB = ship.RdlFOB,
                                    //  FactValue = (factDeliv.FactTransferPrice == null ? ship.RdlFOB : factDeliv.FactTransferPrice) * ship.DelivQuantity,
                                    FactValue = ship.RdlFOB * ship.DelivQuantity,
                                    Adjustment = buyerDiscDet.AdjustmentAmt == null ? null : buyerDiscDet.AdjustmentAmt,
                                    FactoryTransFOB = ((ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt == null ? 0 : buyerDiscDet.AdjustmentAmt)) / ship.DelivQuantity,
                                    // FactoryTransFOB = ship.RdlFOB * ship.DelivQuantity
                                    FactoryInvVal = (ship.RdlFOB * ship.DelivQuantity) - (buyerDiscDet.AdjustmentAmt == null ? 0 : buyerDiscDet.AdjustmentAmt),
                                    DiscountAdjId = (db.DiscountAdjustmentBuyerDet.Where(x => x.ExFactoryShipDetId == exFactShipDet.Id).Count() == 0 ? 0 : buyerDiscDet.DiscountAdjustmentBuyerAdj.Id),
                                    DiscountBuyerDetId = (db.DiscountAdjustmentBuyerDet.Where(x => x.ExFactoryShipDetId == exFactShipDet.Id).Count() == 0 ? 0 : buyerDiscDet.Id)

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
                VMDiscountAdjBuyer vm = new VMDiscountAdjBuyer();
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
                vm.DiscountBuyerDetId = item.DiscountBuyerDetId;
                vm.flag = true;
                list.Add(vm);
            }

            var masid = db.DiscountAdjustmentBuyerAdj.SingleOrDefault(x => x.Id == AdjId).DiscountAdjustmentBuyerMasId;

            foreach (var item in delivAllData)
            {
                VMDiscountAdjBuyer vm = new VMDiscountAdjBuyer();
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
                vm.DiscountBuyerDetId = 0;
                vm.flag = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == item.FactOrderDelivDetId).Count() == 0 ? true : false;
                list.Add(vm);
            }

            return Json(list, JsonRequestBehavior.AllowGet);

            //return Json(delivData, JsonRequestBehavior.AllowGet);

        }


        public JsonResult UpdateFactoryOrder(IEnumerable<VMDiscountAdjustmentBuyerPrev> DiscountPrev, IEnumerable<VMDiscountAdjustmentBuyerAdj> DiscountAdj, IEnumerable<VMDiscountAdjustmentBuyerDet> AdjustmentDetails, VMDiscountAdjustmentBuyerMas DiscountMas, int[] deletedPrevItems, int[] deletedAdjItems)
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
                        var OrderMS = db.DiscountAdjustmentBuyerMas.Find(DiscountMas.Id);

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

                        foreach (var item in DiscountPrev)
                        {
                            var OrderPrev = new DiscountAdjustmentBuyerPrev()
                            {
                                Id = item.Id,
                                DiscountAdjustmentBuyerMasId = DiscountMas.Id,
                                BuyerOrderMasId = item.BuyerOrderMasId,
                            };



                            // dictionary.Add(item.DelivOrderDetTempId, OrderPrev.Id);

                            db.Entry(OrderPrev).State = OrderPrev.Id == 0 ?
                                                          EntityState.Added :
                                                          EntityState.Modified;
                            db.SaveChanges();


                        }

                        foreach (var item in DiscountAdj)
                        {
                            var OrderAdj = new DiscountAdjustmentBuyerAdj()
                            {
                                Id = item.Id,
                                DiscountAdjustmentBuyerMasId = DiscountMas.Id,
                                BuyerOrderMasId = item.BuyerOrderMasId,
                            };





                            db.Entry(OrderAdj).State = OrderAdj.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                          
                            db.SaveChanges();
                            dictionary.Add(item.TempOrderIdAdj, OrderAdj.Id);

    
                        }
                        //---- shipment data

                        //    var slno = 1;

                        if (AdjustmentDetails != null)
                        {
                            foreach (var i in AdjustmentDetails)
                            {
                                //var delivDetailAdj = new DiscountAdjustmentBuyerDet()
                                //{
                                //    Id = i.Id,
                                //    DiscountAdjustmentBuyerAdjId = item.Id,
                                //    AdjustmentAmt = i.AdjustmentAmt,
                                //    ExFactoryShipDetId = i.ExFactoryShipDetId,
                                //};


                                ////  db.DiscountAdjustmentFactoryDet.Add(delivDetailAdj);

                                //db.Entry(delivDetailAdj).State = delivDetailAdj.Id == 0 ?
                                //                      EntityState.Added :
                                //                      EntityState.Modified;
                                //db.SaveChanges();

                                if (i.Id == 0)
                                {
                                    var delivDetailAdj = new DiscountAdjustmentBuyerDet()
                                    {
                                        Id = i.Id,
                                        //DiscountAdjustmentFactoryAdjId = item.DiscountAdjustmentFactoryMasId,
                                        DiscountAdjustmentBuyerAdjId = dictionary[i.DelivOrderDetTempIdAdj],
                                        AdjustmentAmt = i.AdjustmentAmt,
                                        ExFactoryShipDetId = i.ExFactoryShipDetId,
                                    };
                                    db.DiscountAdjustmentBuyerDet.Add(delivDetailAdj);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var det = db.DiscountAdjustmentBuyerDet.Find(i.Id);
                                    det.AdjustmentAmt = i.AdjustmentAmt;

                                    db.SaveChanges();
                                }
                            }

                        }


                        if (deletedPrevItems != null)
                        {
                            foreach (var item in deletedPrevItems)
                            {
                                var delPrev = db.DiscountAdjustmentBuyerPrev.Find(item);
                                db.DiscountAdjustmentBuyerPrev.Remove(delPrev);
                                db.SaveChanges();
                            }
                        }


                        if (deletedAdjItems != null)
                        {
                            foreach (var item in deletedAdjItems)
                            {
                                //---- if order detail deleted without manual deletion of shipment
                                // ---- find those shipment and delete first

                                var discountDets = db.DiscountAdjustmentBuyerDet.Where(x => x.DiscountAdjustmentBuyerAdjId == item);

                                if (discountDets != null)
                                {
                                    db.DiscountAdjustmentBuyerDet.RemoveRange(discountDets);
                                }

                                var delAdj = db.DiscountAdjustmentBuyerAdj.Find(item);
                                db.DiscountAdjustmentBuyerAdj.Remove(delAdj);
                                db.SaveChanges();
                            }
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Updated successfully!!",
                            Id = DiscountMas.Id
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




        public JsonResult DeleteOrder(int id)
        {


            bool flag = false;
            try
            {

                var itemToDeleteMas = db.DiscountAdjustmentBuyerMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nBuyer Discount Not found"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }




                var itemsToDeleteDetPrev = db.DiscountAdjustmentBuyerPrev.Where(x => x.DiscountAdjustmentBuyerMasId == id);

                var itemsToDeleteDetAdj = db.DiscountAdjustmentBuyerAdj.Where(x => x.DiscountAdjustmentBuyerMasId == id);

                foreach (var item in itemsToDeleteDetAdj)
                {
                    var itemsToDeleteDeliv = db.DiscountAdjustmentBuyerDet.Where(x => x.DiscountAdjustmentBuyerAdjId == item.Id);
                    db.DiscountAdjustmentBuyerDet.RemoveRange(itemsToDeleteDeliv);
                }

                db.DiscountAdjustmentBuyerPrev.RemoveRange(itemsToDeleteDetPrev);

                db.DiscountAdjustmentBuyerAdj.RemoveRange(itemsToDeleteDetAdj);

                db.DiscountAdjustmentBuyerMas.Remove(itemToDeleteMas);

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




    }
}