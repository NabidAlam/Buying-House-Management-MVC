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
    public class ExFactoryInformationController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ExFactoryInformation
        public ActionResult Index()
        {
            var list = db.ExFactoryMas.ToList();
            //.Include(b => b.BuyerInfo).Include(b => b.ProdDepartment).Include(b => b.SeasonInfo).Include(b => b.Supplier);
            return View(list.ToList());
            //return View();
        }


        public ActionResult Create()
        {

            //ViewBag.BuyMasId = db.BuyerOrderMas.FirstOrDefault().Id;

            var factoryUnderBuyer = (from buyDet in db.BuyerOrderDet
                                     join supplier in db.Supplier on buyDet.SupplierId equals supplier.Id
                                     select supplier).Distinct().ToList();



            ViewBag.BuyerInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            //ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");

            ViewBag.FabSupplierId = new SelectList(factoryUnderBuyer, "Id", "Name");
            //ViewBag.RdlRefNo = new SelectList(db.BuyerOrderMas.OrderBy(x => x.OrderRefNo), "Id", "Name");
            ViewBag.RdlRefNo = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");


            return View();
        }


        public ActionResult Edit(int id)
        {
            //ViewBag.BuyMasId = db.BuyerOrderMas.FirstOrDefault().Id;
            ViewBag.ExMasId = id;

            var factoryUnderBuyer = (from buyDet in db.BuyerOrderDet
                                     join supplier in db.Supplier on buyDet.SupplierId equals supplier.Id
                                     select supplier).Distinct().ToList();


            var selectedRdl = (from exFactoryMas in db.ExFactoryMas
                               join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                               join buyOrderMas in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyOrderMas.Id
                               where exFactoryMas.Id == id
                               select buyOrderMas).First();

            var selectedBuyerInfo = (from exFactoryMas in db.ExFactoryMas
                                     join buyInfo in db.BuyerInfo on exFactoryMas.BuyerInfoId equals buyInfo.Id
                                     where exFactoryMas.Id == id
                                     select buyInfo).First();
            var selectedSupplier = (from exFactoryMas in db.ExFactoryMas
                                    join supplier in db.Supplier on exFactoryMas.SupplierId equals supplier.Id
                                    where exFactoryMas.Id == id
                                    select supplier).First();





            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", selectedBuyerInfo.Id);
            ViewBag.FabSupplierId = new SelectList(factoryUnderBuyer, "Id", "Name", selectedSupplier.Id);
            ViewBag.RdlRefNo = new SelectList(db.BuyerOrderMas.OrderBy(x => x.OrderRefNo), "Id", "Name", selectedRdl.OrderRefNo);
            ViewBag.ExFactoryDate = NullHelpers.DateToString(db.ExFactoryMas.SingleOrDefault(x => x.Id == id).ExFactoryDate);







            /*select 
                --shipSumm.BuyersPONo, shipSumm.Id as 'ShipSummId',
                --exShipDet.Id as 'ExShipId', exShipDet.BuyerOrderDetId as 'BuyDetId', exDet.Id as 'ExDetId',
                --exMas.Id as 'ExMasId'
                --, 
                SUM(exShipDet.ShipQuantity) as 'Sum'  
                from [dbo].[ExFactoryMas] exMas
                inner join [dbo].[BuyerInfo] buyInfo on exMas.BuyerInfoId = buyInfo.Id
                inner join [dbo].[Supplier] supp on exMas.SupplierId = supp.Id
                inner join [dbo].[ExFactoryDet] exDet on exMas.Id = exDet.ExFactoryMasId
                inner join [dbo].[BuyerOrderMas] buyMas on exDet.BuyerOrderMasId = buyMas.Id
                inner join [dbo].[ExFactoryShipDet] exShipDet on exDet.Id = exShipDet.ExFactoryDetId
                inner join [dbo].[BuyerOrderDet] buyDet on exShipDet.BuyerOrderDetId = buyDet.Id
                inner join [dbo].[ShipmentSummDet] shipSumm on exShipDet.ShipmentSummDetId = shipSumm.Id
                where exMas.Id=56
                group by exShipDet.BuyerOrderDetId
                --order by shipSumm.BuyersPONo desc
                */






            //var shipQtyTotal = (from exMas in db.ExFactoryMas
            //                    join buyInfo in db.BuyerInfo on exMas.BuyerInfoId equals buyInfo.Id
            //                    join supp in db.Supplier on exMas.SupplierId equals supp.Id
            //                    join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //                    join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                    join exShipDet in db.ExFactoryShipDet on exDet.Id equals exShipDet.ExFactoryDetId
            //                    join buyDet in db.BuyerOrderDet on exShipDet.BuyerOrderDetId equals buyDet.Id
            //                    join shipSumm in db.ShipmentSummDet on exShipDet.ShipmentSummDetId equals shipSumm.Id
            //                    where exMas.Id == id
            //                    select exShipDet).GroupBy(x => x.BuyerOrderDetId)
            //                    .Select(x => new
            //                    {
            //                        ShipQty = x.Sum(m => m.ShipQuantity)

            //                    });


            return View();
        }



        public JsonResult GetSelectedRDL(int Id)
        {

            /*select exDet.BuyerOrderMasId from [dbo].[ExFactoryMas] exMas
                    inner join  [dbo].[Supplier] suppliers on exMas.SupplierId = suppliers.Id
                    inner join [dbo].[BuyerInfo] buyInfo on exMas.BuyerInfoId = buyInfo.Id
                    inner join [dbo].[ExFactoryDet] exDet on exMas.Id = exDet.ExFactoryMasId
                    inner join [dbo].[BuyerOrderMas] buyMas on exDet.BuyerOrderMasId = buyMas.Id
                    where exMas.Id=73*/

            var list = (from exMas in db.ExFactoryMas
                        join supp in db.Supplier on exMas.SupplierId equals supp.Id
                        join buyInfo in db.BuyerInfo on exMas.BuyerInfoId equals buyInfo.Id
                        join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                        join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                        where exMas.Id == Id
                        select new { buyMas, exMas, exDet }).AsEnumerable()
                       .Select(x => new
                       {
                           ExDetId = x.exDet.Id,
                           Id = x.exMas.Id,
                           BuyerOrderMasId = x.buyMas.Id,
                           OrderRef = x.buyMas.OrderRefNo

                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetNames()
        {
            var data = db.BuyerOrderMas.Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetDelivData(int BuyMasId, int SupplierId, int BuyInfoId, int? BuyDetId, int? BuyDetDetId)
        public JsonResult GetDelivData(int BuyMasId, int SupplierId, int BuyInfoId)
        {

            /*select buyMas.OrderRefNo, buyMas.Id as 'BuyMasId', supplier.Name, supplier.Id as 'SupplierId',
                buyInfo.Name, buyInfo.Id as 'BuyInfoId',
                buyDet.StyleNo, buyDetDet.BuyersPONo, prodSize.SizeRange as 'Size', buyDet.ProdSizeId as 'SizeId',
                buyDet.ProdColorId as 'ColorId', destPort.Name, buyDet.Quantity   
                from [dbo].[BuyerOrderDet] buyDet 
                inner join [dbo].[Supplier] supplier on buyDet.SupplierId = supplier.Id
                inner join [dbo].[BuyerOrderMas] buyMas on buyDet.[BuyerOrderMasId] = buyMas.Id
                inner join [dbo].[BuyerInfo] buyInfo on buyMas.BuyerInfoId = buyInfo.Id
                inner join [dbo].[ShipmentSummDet] buyDetDet on buyDet.Id = buyDetDet.BuyerOrderDetId
                inner join [dbo].[ProdSize] prodSize on buyDet.ProdSizeId = prodSize.Id
                inner join [dbo].[ProdColor] prodColor on buyDet.ProdColorId = prodColor.Id
                inner join [dbo].[DestinationPort] destPort  on buyDetDet.DestinationPortId= destPort.Id
                where buyMas.Id=1 and supplier.Id= 2 and buyInfo.Id=1*/



            //--------------QUERY FOR STYLE--------------------------------------------

            //var list = (from buyDet in db.BuyerOrderDet
            //            join supplier in db.Supplier on buyDet.SupplierId equals supplier.Id
            //            join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
            //            join buyInfo in db.BuyerInfo on buyMas.BuyerInfoId equals buyInfo.Id
            //            join buyDetDet in db.ShipmentSummDet on buyDet.Id equals buyDetDet.BuyerOrderDetId
            //            //join prodSize in db.ProdSize on buyDet.ProdSizeId equals prodSize.Id
            //            //join prodColor in db.ProdColor on buyDet.ProdColorId equals prodColor.Id
            //            //join destPort in db.DestinationPort on buyDetDet.DestinationPortId equals destPort.Id into joinDesport
            //            //from destPort in joinDesport.DefaultIfEmpty()
            //            where buyMas.Id == BuyMasId
            //            && supplier.Id == SupplierId && !(from ex in db.ExFactoryShipDet select
            //            new { a = ex.ShipmentSummDetId, b = ex.BuyerOrderDetId }).Contains(new { a = buyDetDet.Id, b = buyDetDet.BuyerOrderDetId }) && buyDet.IsShipClosed!=true
            //            select new
            //            {
            //                BuyerOrderMasId = buyMas.Id,
            //                BuyOrderDetId = buyDet.Id,
            //                BuyOrderDetDetId = buyDetDet.Id,
            //                StyleNo = buyDet.StyleNo,
            //                PONo = buyDetDet.BuyersPONo,
            //                //SizeNo = prodSize.SizeRange,
            //                //Color = prodColor.Name,
            //                SizeNo = buyDet.ProdSize.SizeRange ?? "",
            //                Color = buyDet.ProdColor.Name ?? "",
            //                //DestPort = destPort.Name,
            //                DestPort = buyDetDet.DestinationPort.Name ?? "",
            //                OrderQty = buyDetDet.DelivQuantity,
            //                //ShipClosed = x.buyDet.IsShipClosed,
            //                IsStyleIncluded = buyDet.IsShipClosed,
            //                DelivSlNo = buyDetDet.DelivSlno,
            //                BuyerSlNo = buyDetDet.BuyerSlNo,
            //                CountCheck = (from exMas in db.ExFactoryMas
            //                              join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //                              join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                              join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                              join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                              where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
            //                              select exShipDet.Id
            //                      ).Count(),
            //                PrevShipQty = ((from exMas in db.ExFactoryMas
            //                               join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //                               join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                               join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                               join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                               where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
            //                               select exShipDet.ShipQuantity
            //                      )).Count()==0 ?0 : ((from exMas in db.ExFactoryMas
            //                                      join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //                                      join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                                      join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                                      join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                                      where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
            //                                      select exShipDet.ShipQuantity
            //                      )).Sum()
            //                //}).GroupBy(x => x.BuyOrderDetId, (key, g) => g.OrderBy(e => e.BuyOrderDetId).FirstOrDefault());
            //            }).GroupBy(x => x.BuyOrderDetId, (key, g) => g.OrderBy(e => e.BuyOrderDetId).FirstOrDefault());



            //------------------------------------------------------------------------------------------------------------





            //----------------------------11/08/2018-------------------------------------

            List<ExfactoryHelper> deliveryList = new List<ExfactoryHelper>();

            var list = (from buyDet in db.BuyerOrderDet
                        //join supplier in db.Supplier on buyDet.SupplierId equals supplier.Id
                        join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
                        join buyInfo in db.BuyerInfo on buyMas.BuyerInfoId equals buyInfo.Id
                        join buyDetDet in db.ShipmentSummDet on buyDet.Id equals buyDetDet.BuyerOrderDetId
                        join masterLcDet in db.MasterLCInfoDet on buyMas.Id equals masterLcDet.BuyerOrderMasId
                        where buyMas.Id == BuyMasId
                               //&& supplier.Id == SupplierId
                        && buyDet.SupplierId == SupplierId
                          //&& !(from ex in db.ExFactoryShipDet
                          //     select new { a = ex.ShipmentSummDetId, b = ex.BuyerOrderDetId })
                          //     .Contains(new { a = buyDetDet.Id, b = buyDetDet.BuyerOrderDetId })
                          && !(from ex in db.ExFactoryShipDet
                               select ex.ShipmentSummDetId)
                             .Contains(buyDetDet.Id)
                        && buyDet.IsShipClosed != true
                        select new
                        {
                            BuyerOrderMasId = buyMas.Id,
                            BuyOrderDetId = buyDet.Id,
                            BuyOrderDetDetId = buyDetDet.Id,
                            StyleNo = buyDet.StyleNo,
                            PONo = buyDetDet.BuyersPONo,
                            SizeNo = buyDet.ProdSize.SizeRange ?? "",
                            Color = buyDet.ProdColor.Name ?? "",
                            DestPort = buyDetDet.DestinationPort.Name ?? "",
                            OrderQty = buyDetDet.DelivQuantity,
                            IsStyleIncluded = buyDet.IsShipClosed,
                            DelivSlNo = buyDetDet.DelivSlno,
                            BuyerSlNo = buyDetDet.BuyerSlNo,
                            CountCheck = (from exMas in db.ExFactoryMas
                                          join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                                          join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                                          join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
                                          join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
                                          where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
                                          select exShipDet.Id
                                  ).Count(),
                            PrevShipQty = ((from exMas in db.ExFactoryMas
                                            join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                                            join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                                            join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
                                            join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
                                            where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id && exShipDet.ShipmentSummDet.DestinationPortId == buyDetDet.DestinationPortId
                                            select exShipDet.ShipQuantity
                                  )).Count() == 0 ? 0 : ((from exMas in db.ExFactoryMas
                                                          join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                                                          join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                                                          join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
                                                          join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
                                                          where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
                                                          select exShipDet.ShipQuantity
                                  )).Sum(),
                            ShipmentModePrev = buyDetDet.ShipmentMode == 0 ? "Sea" : "Air",
                            TotalQty = buyDet.Quantity ?? 0
                            //}).GroupBy(x => x.BuyOrderDetId, (key, g) => g.OrderBy(e => e.BuyOrderDetId).FirstOrDefault());
                        }).GroupBy(x => x.BuyOrderDetId, (key, g) => g.OrderBy(e => e.BuyOrderDetId).FirstOrDefault()).AsEnumerable().ToList();


            foreach (var deliv in list)
            {
                //ExfactoryHelper ex = new ExfactoryHelper();
                //ex.BuyerOrderMasId = deliv.BuyerOrderMasId;
                //ex.BuyOrderDetId = deliv.BuyOrderDetId;
                //ex.BuyOrderDetDetId = deliv.BuyOrderDetDetId;
                //ex.StyleNo = deliv.StyleNo;
                //ex.PONo = deliv.PONo;
                //ex.SizeNo = deliv.SizeNo;
                //ex.Color = deliv.Color;
                //ex.DestPort = deliv.DestPort;
                //ex.OrderQty = deliv.OrderQty;
                //ex.IsStyleIncluded = deliv.IsStyleIncluded;
                //ex.DelivSlNo = deliv.DelivSlNo;
                //ex.BuyerSlNo = Convert.ToInt32(deliv.BuyerSlNo);
                //ex.CountCheck = deliv.CountCheck;
                //ex.PrevShipQty = deliv.PrevShipQty;

                //deliveryList.Add(ex);


                var checkSameDeliv = db.ShipmentSummDet.Where(x => x.BuyerSlNo == deliv.BuyerSlNo && x.BuyerOrderDetId == deliv.BuyOrderDetId);



                foreach (var otherDeliv in checkSameDeliv)
                {
                    ExfactoryHelper ex2 = new ExfactoryHelper();
                    ex2.BuyerOrderMasId = deliv.BuyerOrderMasId;
                    ex2.BuyOrderDetId = deliv.BuyOrderDetId;
                    ex2.BuyOrderDetDetId = otherDeliv.Id;
                    ex2.StyleNo = deliv.StyleNo;
                    ex2.PONo = otherDeliv.BuyersPONo;
                    ex2.SizeNo = deliv.SizeNo;
                    ex2.Color = deliv.Color;
                    ex2.DestPort = otherDeliv.DestinationPort.Name;
                    ex2.OrderQty = otherDeliv.DelivQuantity;
                    ex2.IsStyleIncluded = deliv.IsStyleIncluded;
                    //ex2.DelivSlNo = deliv.DelivSlNo;
                    ex2.DelivSlNo = otherDeliv.DelivSlno;
                    ex2.BuyerSlNo = Convert.ToInt32(otherDeliv.BuyerSlNo);
                    ex2.CountCheck = deliv.CountCheck;
                    //ex2.PrevShipQty = db.ExFactoryShipDet.Where(x => x.BuyerOrderDetId == deliv.BuyOrderDetId).Select(t => t.ShipQuantity).DefaultIfEmpty(0).Sum();
                    ex2.PrevShipQty = db.ExFactoryShipDet.Where(x => x.BuyerOrderDetId == deliv.BuyOrderDetId && x.ShipmentSummDet.DestinationPortId == otherDeliv.DestinationPortId).Select(t => t.ShipQuantity).DefaultIfEmpty(0).Sum();
                    ex2.ShipmentModePrev = otherDeliv.ShipmentMode == 0 ? "Sea" : "Air";
                    ex2.TotalQty = deliv.TotalQty;
                    //ex2.PrevShipQty = 0;
                    deliveryList.Add(ex2);
                }
            }


            //--------------------------------END----------------------------------------
            return Json(deliveryList, JsonRequestBehavior.AllowGet);
            //return Json(list, JsonRequestBehavior.AllowGet);
            //}


        }



        public JsonResult GetDelivDataEdit(int ExFactoryDetailId)
        {
            //var lists = (from exMas in db.ExFactoryMas
            //             join suppliers in db.Supplier on exMas.SupplierId equals suppliers.Id
            //             join buyInfo in db.BuyerInfo on exMas.BuyerInfoId equals buyInfo.Id
            //             join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //             join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id                       
            //             join buyDet in db.BuyerOrderDet on new { ColA = buyMas.Id/*, ColB = suppliers.Id*/ } 
            //             equals new { ColA = buyDet.BuyerOrderMasId/*, ColB = buyDet.SupplierId*/ }
            //             join prodCatType in db.ProdCatType on buyDet.ProdCatTypeId equals prodCatType.Id
            //             join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
            //             //join prodSize in db.ProdSize on buyDet.ProdSizeId equals prodSize.Id
            //             //join prodDept in db.ProdDepartment on new { a = prodSize.ProdDepartmentId, b=buyInfo.Id } 
            //             //equals new { a= prodDept.Id, b= prodDept.BuyerInfoId }
            //             //join fabricItem in db.FabricItem on buyDet.FabricItemId equals fabricItem.Id
            //             //join prodColor in db.ProdColor on buyDet.ProdColorId equals prodColor.Id
            //             //join seasonInfo in db.SeasonInfo on prodColor.SeasonInfoId equals seasonInfo.Id
            //             join shipSumm in db.ShipmentSummDet on buyDet.Id equals shipSumm.BuyerOrderDetId
            //             // join destPort in db.DestinationPort on shipSumm.DestinationPortId equals destPort.Id
            //             join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b=buyDet.Id, c=shipSumm.Id} 
            //             equals new { a=exShipDet.ExFactoryDetId, b= exShipDet.BuyerOrderDetId, c=exShipDet.ShipmentSummDetId }
            //             where exDet.Id == ExFactoryDetailId
            //             && (from ex in db.ExFactoryShipDet select                     
            //             new { a=ex.ShipmentSummDetId, b=ex.BuyerOrderDetId}).Contains(new { a=shipSumm.Id, b=shipSumm.BuyerOrderDetId })
            //             //!(from ex in db.ExFactoryShipDet select ex.ShipmentSummDetId).Contains(shipSumm.Id)
            //             select new
            //             {
            //                 BuyerOrderMasId = buyMas.Id,
            //                 BuyOrderDetId = buyDet.Id,
            //                 BuyOrderDetDetId = shipSumm.Id,
            //                 StyleNo = buyDet.StyleNo,
            //                 PONo = shipSumm.BuyersPONo,
            //                 //SizeNo = prodSize.SizeRange,
            //                 //Color = prodColor.Name,
            //                 SizeNo = buyDet.ProdSize.SizeRange ?? "",
            //                 Color = buyDet.ProdColor.Name ?? "",
            //                 //DestPort = destPort.Name,
            //                 DestPort = shipSumm.DestinationPort.Name ?? "",
            //                 OrderQty = shipSumm.DelivQuantity,
            //                 //ShipClosed = x.buyDet.IsShipClosed,
            //                 IsStyleIncluded = buyDet.IsShipClosed,
            //                 DelivSlNo = shipSumm.DelivSlno,
            //                 ShipQty= exShipDet.ShipQuantity,
            //                 PrevShipQty = ((from exMasP in db.ExFactoryMas
            //                   join exDet in db.ExFactoryDet on exMasP.Id equals exDet.ExFactoryMasId
            //                   join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                   join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMasP.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                   join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                   where exMasP.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id && exMasP.ExFactoryDate < exMas.ExFactoryDate
            //                                 select exShipDet.ShipQuantity
            //                      )).Count() == 0 ? 0 : ((from exMasP in db.ExFactoryMas
            //                                              join exDet in db.ExFactoryDet on exMasP.Id equals exDet.ExFactoryMasId
            //                                              join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                                              join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMasP.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                                              join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                                              where exMasP.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id && exMasP.ExFactoryDate < exMas.ExFactoryDate
            //                                              select exShipDet.ShipQuantity
            //                      )).Sum(),
            //                 ExShipDetId = exShipDet.Id,
            //                 ExFactDetId= exDet.Id,
            //                 CountCheck = (from exMas in db.ExFactoryMas
            //                               join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
            //                               join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
            //                               join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
            //                               join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
            //                               where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
            //                               select exShipDet.Id
            //                      ).Count(),
            //                IsShipClosed = exShipDet.IsShipClosed                                 
            //             }).GroupBy(x => x.BuyOrderDetId, (key, g) => g.OrderBy(e => e.BuyOrderDetId).FirstOrDefault());

            //IsStyleIncluded = exShipDet.IsShipClosed == null ? false : true


            var lists = (from exMas in db.ExFactoryMas
                         join suppliers in db.Supplier on exMas.SupplierId equals suppliers.Id
                         join buyInfo in db.BuyerInfo on exMas.BuyerInfoId equals buyInfo.Id
                         join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                         join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                         join buyDet in db.BuyerOrderDet on new { ColA = buyMas.Id/*, ColB = suppliers.Id*/ }
                         equals new { ColA = buyDet.BuyerOrderMasId/*, ColB = buyDet.SupplierId*/ }
                         join prodCatType in db.ProdCatType on buyDet.ProdCatTypeId equals prodCatType.Id
                         join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
                         join shipSumm in db.ShipmentSummDet on buyDet.Id equals shipSumm.BuyerOrderDetId
                         join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDet.Id, c = shipSumm.Id }
                         equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId, c = exShipDet.ShipmentSummDetId }
                         where exShipDet.ExFactoryDetId == ExFactoryDetailId
                         select new
                         {
                             BuyerOrderMasId = buyMas.Id,
                             BuyOrderDetId = buyDet.Id,
                             BuyOrderDetDetId = shipSumm.Id,
                             StyleNo = buyDet.StyleNo,
                             PONo = shipSumm.BuyersPONo,
                             SizeNo = buyDet.ProdSize.SizeRange ?? "",
                             Color = buyDet.ProdColor.Name ?? "",
                             DestPort = shipSumm.DestinationPort.Name ?? "",
                             OrderQty = shipSumm.DelivQuantity,
                             IsStyleIncluded = buyDet.IsShipClosed,
                             DelivSlNo = shipSumm.DelivSlno,
                             ShipQty = exShipDet.ShipQuantity,
                             PrevShipQty = db.ExFactoryShipDet.Where(x => x.BuyerOrderDetId == buyDet.Id && x.ShipmentSummDet.DestinationPortId == shipSumm.DestinationPortId && x.ExFactoryDet.ExFactoryMas.ExFactoryDate < exMas.ExFactoryDate).Select(t => t.ShipQuantity).DefaultIfEmpty(0).Sum(),
                             ExShipDetId = exShipDet.Id,
                             ExFactDetId = exDet.Id,
                             BuyerSlNo = shipSumm.BuyerSlNo,
                             ShipmentModePrev = exShipDet.ShipmentSummDet.ShipmentMode == 0 ? "Sea" : "Air",
                             CurrentShipmentMode = exShipDet.ShipmentMode,
                             //CountCheck = (from exMas in db.ExFactoryMas
                             //              join exDet in db.ExFactoryDet on exMas.Id equals exDet.ExFactoryMasId
                             //              join buyMas in db.BuyerOrderMas on exDet.BuyerOrderMasId equals buyMas.Id
                             //              join buyDets in db.BuyerOrderDet on new { a = buyMas.Id, b = exMas.SupplierId } equals new { a = buyDets.BuyerOrderMasId, b = buyDets.SupplierId }
                             //              join exShipDet in db.ExFactoryShipDet on new { a = exDet.Id, b = buyDets.Id } equals new { a = exShipDet.ExFactoryDetId, b = exShipDet.BuyerOrderDetId }
                             //              where exMas.SupplierId == buyDet.SupplierId && exShipDet.BuyerOrderDetId == buyDet.Id
                             //              select exShipDet.Id
                             //     ).Count(),
                             IsShipClosed = exShipDet.IsShipClosed,
                             TotalQty = buyDet.Quantity
                         }).ToList();




            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBuyerNames(int Id)
        {
            List<SelectListItem> BuyerList = new List<SelectListItem>();


            var buyers = (from supplier in db.Supplier
                          join buyDet in db.BuyerOrderDet on supplier.Id equals buyDet.SupplierId
                          join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
                          join buyInfo in db.BuyerInfo on buyMas.BuyerInfoId equals buyInfo.Id
                          where buyDet.SupplierId == Id
                          select buyInfo).Distinct().ToList();

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


        public JsonResult GetRdlRef(int Id, int SupplierId)
        {
            /*select buyMas.OrderRefNo, buyMas.Id from [dbo].[BuyerOrderMas] buyMas 
                inner join [dbo].[BuyerInfo] buyInfo on buyMas.BuyerInfoId = buyInfo.Id
                where buyInfo.Id=1*/

            List<SelectListItem> RdlList = new List<SelectListItem>();

            var rdlRefs = (from buyMas in db.BuyerOrderMas
                           join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                           join buyInfo in db.BuyerInfo on buyMas.BuyerInfoId equals buyInfo.Id
                           join masterDetailLC in db.MasterLCInfoDet on buyMas.Id equals masterDetailLC.BuyerOrderMasId
                           where buyMas.BuyerInfoId == Id && buyDet.SupplierId == SupplierId
                           select buyMas).Distinct().ToList();

            foreach (var x in rdlRefs)
            {
                RdlList.Add(new SelectListItem { Text = x.OrderRefNo, Value = x.Id.ToString() });
            }

            var result = new
            {
                ListOfRdlRef = RdlList
            };

            return Json(result, JsonRequestBehavior.AllowGet);

            /*select distinct buyInfo.Id, buyInfo.Name, supplier.Id, supplier.Name from [dbo].[BuyerOrderDet] buyDet 
                inner join [dbo].[Supplier] supplier on buyDet.SupplierId = Supplier.Id
                inner join [dbo].[BuyerOrderMas] buyMas on buyDet.BuyerOrderMasId = buyMas.Id
                inner join [dbo].[BuyerInfo] buyInfo on buyMas.BuyerInfoId = buyInfo.Id
                where supplier.Id=2*/
        }


        public JsonResult SaveExFactory(IEnumerable<VMExFactoryShipDet> ExFactoryDetailsDetail, IEnumerable<VMExFactoryDetailEdit> ExFactoryDetails, VMExFactoryMasEdit ExFactoryMas)
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
                totalShipQty = 0,
                storeDetailDetailId = 0,
                storeDetailId = 0

                //   buydetid = 0,
                //   buydetdetid=0
            };

            // var buyDetId = 0;
            //  var buyDetDetId = 0;
            var totalShipQty = 0;
            //int[] storeDetailDetailId = new int[10000];
            //int[] storeDetailId = new int[10000];
            //ArrayList storeDetailDetailId = new ArrayList();
            //ArrayList storeDetailId = new ArrayList();



            //var counter = 0;
            //var Detailcounter = 0;
            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var OrderM = new ExFactoryMas()
                        {
                            Id = 0,
                            ExFactoryDate = ExFactoryMas.ExFactoryDate,
                            SupplierId = ExFactoryMas.SupplierId,
                            BuyerInfoId = ExFactoryMas.BuyerInfoId,
                            IsAuth = ExFactoryMas.IsAuth,
                            OpBy = 1,
                            OpOn = OpDate,
                            AuthBy = 1,
                            AuthOn = OpDate
                        };

                        db.ExFactoryMas.Add(OrderM);
                        db.SaveChanges();

                        Dictionary<int, int> dictionary = new Dictionary<int, int>();


                        foreach (var item in ExFactoryDetails)
                        {
                            var OrderD = new ExFactoryDet()
                            {
                                Id = 0,
                                ExFactoryMasId = OrderM.Id,
                                BuyerOrderMasId = item.BuyerOrderMasId,
                                ShipQuantity = 0

                            };

                            db.ExFactoryDet.Add(OrderD);
                            db.SaveChanges();

                            var ExDetId = OrderD.Id;

                            storeDetailId.Add(OrderD.Id);
                            //Detailcounter++;                        
                            dictionary.Add(item.TempOrderId, OrderD.Id);


                        }

                        if (ExFactoryDetailsDetail != null)
                        {

                            foreach (var ship in ExFactoryDetailsDetail)
                            {
                                var deliv = new ExFactoryShipDet()
                                {
                                    Id = ship.Id,
                                    ExFactoryDetId = dictionary[ship.ExFactoryDetId],
                                    BuyerOrderDetId = ship.BuyerOrderDetId,
                                    ShipmentSummDetId = ship.ShipmentSummDetId,
                                    ShipQuantity = ship.ShipQuantity,
                                    Remarks = "",
                                    IsShipClosed = ship.IsShipClosed,
                                    ShipmentMode = ship.CurrentShipmentMode
                                };

                                if (ship.IsShipClosed == true)
                                {
                                    var buyerDet = db.BuyerOrderDet.Find(ship.BuyerOrderDetId);
                                    buyerDet.IsShipClosed = true;
                                    db.SaveChanges();
                                }

                                db.ExFactoryShipDet.Add(deliv);
                                db.SaveChanges();



                                var findBuyDetId = db.BuyerOrderDet.SingleOrDefault(x => x.Id == ship.BuyerOrderDetId);
                                findBuyDetId.Id = deliv.BuyerOrderDetId;
                                findBuyDetId.IsShipClosed = ship.IsShipClosed;

                                db.Entry(findBuyDetId).State = findBuyDetId.Id == 0 ?
                                                           EntityState.Added :
                                                           EntityState.Modified;



                                totalShipQty = totalShipQty + deliv.ShipQuantity;
                                storeDetailDetailId.Add(deliv.Id);

                                // storeDetailDetailId[counter] = deliv.Id;
                                // counter++;

                            }
                        }



                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!!",
                            Id = OrderM.Id,
                            totalShipQty = totalShipQty,
                            storeDetailDetailId = 0,
                            storeDetailId = 0

                            //storeDetailDetailId = storeDetailDetailId,
                            //storeDetailId = storeDetailId


                            // buydetid = buyDetId,
                            //  buydetdetid = buyDetDetId
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
                            totalShipQty = 0,
                            storeDetailDetailId = 0,
                            storeDetailId = 0

                            //storeDetailDetailId = detdetList,
                            //storeDetailId = detList

                            //  buydetid = buyDetId,
                            //   buydetdetid = buyDetDetId
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
                    Id = 0,
                    totalShipQty = 0,
                    storeDetailDetailId = 0,
                    storeDetailId = 0

                    // storeDetailDetailId = detdetList,
                    //  storeDetailId = detList

                    // buydetid = buyDetId,
                    // buydetdetid = buyDetDetId
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }






        //---------------UPDATE----------------------------------------------UPDATE-----------------------------------------------UPDATE-------------------------------------

        public JsonResult UpdateExFactory(IEnumerable<VMExFactoryShipDet> ExFactoryDetailsDetail, IEnumerable<VMExFactoryDetailEdit> ExFactoryDetails, VMExFactoryMasEdit ExFactoryMas, int[] DelItems)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !",
                ExMasId = 0
            };

            // var totalShipQty = 0;



            try
            {
                var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var OrderMS = db.ExFactoryMas.Find(ExFactoryMas.Id);

                        if (OrderMS == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Saving failed !",
                                ExMasId = 0
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        //OrderMS.ExFactoryDate = ExFactoryMas.ExFactoryDate;

                        db.Entry(OrderMS).State = EntityState.Modified;
                        db.SaveChanges();

                        //Dictionary<int, int> dictionary = new Dictionary<int, int>();

                        var ShipQty = 0;

                        foreach (var item in ExFactoryDetails)
                        {
                            foreach (var det in ExFactoryDetailsDetail)
                            {
                                if (det.ExFactoryDetId == item.Id)
                                {
                                    ShipQty = ShipQty + det.ShipQuantity;
                                }
                            }

                            var OrderD = new ExFactoryDet()
                            {
                                Id = item.Id,
                                ExFactoryMasId = OrderMS.Id,
                                BuyerOrderMasId = item.BuyerOrderMasId,
                                ShipQuantity = ShipQty

                            };

                            db.Entry(OrderD).State = OrderD.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;



                            //storeDetailId.Add(OrderD.Id);
                            ////Detailcounter++;                        
                            //dictionary.Add(item.TempOrderId, OrderD.Id);
                        }


                        if (ExFactoryDetailsDetail != null)
                        {
                            foreach (var DetailDet in ExFactoryDetailsDetail)
                            {
                                var deliv = new ExFactoryShipDet()
                                {
                                    Id = DetailDet.ExFactoryDetDetId,

                                    //for create purpose
                                    //Id = item.Id,
                                    ExFactoryDetId = DetailDet.ExFactoryDetId,
                                    BuyerOrderDetId = DetailDet.BuyerOrderDetId,
                                    ShipmentSummDetId = DetailDet.ShipmentSummDetId,
                                    ShipQuantity = DetailDet.ShipQuantity,
                                    Remarks = "",
                                    IsShipClosed = DetailDet.IsShipClosed,
                                    ShipmentMode = DetailDet.CurrentShipmentMode
                                };

                                var buyerDet = db.BuyerOrderDet.Find(DetailDet.BuyerOrderDetId);

                                if (DetailDet.IsShipClosed == true)
                                {
                                    buyerDet.IsShipClosed = true;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    buyerDet.IsShipClosed = false;
                                    db.SaveChanges();
                                }
                                //-------Update same detail data
                                db.Entry(deliv).State = deliv.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                db.SaveChanges();


                                var findBuyDetId = db.BuyerOrderDet.SingleOrDefault(x => x.Id == DetailDet.BuyerOrderDetId);
                                findBuyDetId.Id = deliv.BuyerOrderDetId;
                                findBuyDetId.IsShipClosed = DetailDet.IsShipClosed;

                                db.Entry(findBuyDetId).State = findBuyDetId.Id == 0 ?
                                                          EntityState.Added :
                                                          EntityState.Modified;

                            }
                        }

                        //---- delete order detail items
                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                //---- if order detail deleted without manual deletion of shipment
                                // ---- find those shipment and delete first

                                var shipDets = db.ExFactoryShipDet.Where(x => x.ExFactoryDetId == item);

                                if (shipDets != null)
                                {

                                    foreach(var det in shipDets)
                                    {
                                        if (det.IsShipClosed == true)
                                        {
                                            var buyerDet = db.BuyerOrderDet.Find(det.BuyerOrderDetId);
                                            buyerDet.IsShipClosed = false;
                                            db.SaveChanges();
                                        }
                                    }

                                    db.ExFactoryShipDet.RemoveRange(shipDets);
  
                                }

                                var delOrder = db.ExFactoryDet.Find(item);
                                db.ExFactoryDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Update successful!!!",
                            ExMasId = OrderMS.Id

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
                            ExMasId = 0
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
                    ExMasId = 0
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //-------------------------------------------------------------------------------------UPDATE END------------------------------------------------------------------------------------


        public JsonResult DeleteOrder(int id)
        {


            bool flag = false;
            try
            {

                var itemToDeleteMas = db.ExFactoryMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nEx Factory No found"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                List<int> buyerDetId = new List<int>();

                var itemsToDeleteDet = db.ExFactoryDet.Where(x => x.ExFactoryMasId == id);

                foreach (var item in itemsToDeleteDet)
                {
                    var itemsToDeleteDeliv = db.ExFactoryShipDet.Where(x => x.ExFactoryDetId == item.Id);
                    foreach(var det in itemsToDeleteDeliv)
                    {
                        buyerDetId.Add(det.BuyerOrderDetId);
                        //var buyerDet = db.BuyerOrderDet.Find(det.BuyerOrderDetId);
                        //buyerDet.IsShipClosed = false;
                        //db.SaveChanges();
                    }
                    db.ExFactoryShipDet.RemoveRange(itemsToDeleteDeliv);
                }

                db.ExFactoryDet.RemoveRange(itemsToDeleteDet);

                db.ExFactoryMas.Remove(itemToDeleteMas);

                flag = db.SaveChanges() > 0;


                foreach (var details in buyerDetId)
                {
                    var buyerDet = db.BuyerOrderDet.Find(details);
                    buyerDet.IsShipClosed = false;
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
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




        public JsonResult GetShipmentModeDeliv()
        {
            var enumData = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Sea", Value = "0" }, new SelectListItem { Text = "Air", Value = "1" }, }, "Value", "Text");
            return Json(enumData, JsonRequestBehavior.AllowGet);
        }




    }
}