using BHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using BHMS.Helpers;
using System.IO;
using BHMS.ReportDataset;
using System.Data;

namespace BHMS.Controllers
{
    public class ReportController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        //     //=======================================================
        //     //============Order Information Factory Wise=============
        //     //-----------Added By Tazbirul(12/05/2018)---------------

        [HttpGet]
        public ActionResult OrderInfoFactoryWise()
        {
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            var months = new[]
            {
                     new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                     new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                     new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
                 };

            ViewBag.Months = new SelectList(months, "Value", "MonthName");
            return View();
        }

        [HttpPost]
        public ActionResult OrderInfoFactoryWise(DateTime date, int? SupplierId)
        {
            OrderInfoFactoryWiseDS ds = new OrderInfoFactoryWiseDS();

            var firstDay = new DateTime(date.Year, date.Month, 1);

            var data = (from buyerMas in db.BuyerOrderMas
                        join buyerDet in db.BuyerOrderDet on buyerMas.Id equals buyerDet.BuyerOrderMasId
                        //join factoryMas in db.FactoryOrderMas on new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.SupplierId??0 }
                        //                equals new { ColA = factoryMas.BuyerOrderMasId, ColB = factoryMas.SupplierId }
                        join factoryMas in db.FactoryOrderMas on new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.SupplierId }
                         equals new { ColA = factoryMas.BuyerOrderMasId, ColB = factoryMas.SupplierId }
                        join factoryDet in db.FactoryOrderDet on new { ColA = factoryMas.Id, ColB = buyerDet.Id } equals new { ColA = factoryDet.FactoryOrderMasId, ColB = factoryDet.BuyerOrderDetId }
                        join masterLCDet in db.MasterLCInfoDet on buyerMas.Id equals masterLCDet.BuyerOrderMasId into joinLCDet
                        from masterLCDet in joinLCDet.DefaultIfEmpty()
                        join masterLCMas in db.MasterLCInfoMas on masterLCDet.MasterLCInfoMasId equals masterLCMas.Id into joinLCMas
                        from masterLCMas in joinLCMas.DefaultIfEmpty()
                        where factoryMas.SalesContractDate >= firstDay && factoryMas.SalesContractDate <= date
                        select new { factoryMas, factoryDet, buyerMas, buyerDet, masterLCMas } into joined
                        group joined by new
                        {
                            joined.factoryMas.SupplierId,
                            joined.buyerMas.Id,
                            joined.buyerMas.OrderDate,
                            joined.masterLCMas.LatestShipmentDate
                        } into grouped
                        select grouped.Select(g => new
                        {
                            FactoryId = g.factoryMas.SupplierId,
                            FactoryName = g.factoryMas.Supplier.Name,
                            BuyerId = g.buyerMas.BuyerInfoId,
                            BuyerName = g.buyerMas.BuyerInfo.Name,
                            BuyerOrderMasId = g.buyerMas.Id,
                            BuyerOrderRefNo = g.buyerMas.OrderRefNo,
                            //OrderQty = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == g.buyerMas.Id).Sum(x => x.Quantity), //whole Order
                            OrderQty = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == g.buyerMas.Id && x.SupplierId == g.factoryMas.SupplierId).Sum(x => x.Quantity), //only selected Factory Order Qty

                            //RDLValue = (from buyMas in db.BuyerOrderMas
                            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //            where buyMas.Id == g.buyerMas.Id
                            //            select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct), //whole RDL value

                            RDLValue = (from buyMas in db.BuyerOrderMas
                                        join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                                        where buyMas.Id == g.buyerMas.Id && buyDet.SupplierId == g.factoryMas.SupplierId
                                        select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct), //only selected Factory RDL value

                            //Factoryvalue =  (from buyMas in db.BuyerOrderMas
                            //               join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //               join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //               equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //               join factDet in db.FactoryOrderDet on new { ColA=factMas.Id, ColB= buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId,ColB = factDet.BuyerOrderDetId }
                            //               where buyMas.Id == g.buyerMas.Id && factMas.SupplierId==g.factoryMas.SupplierId
                            //               select new { FOBProduct = buyDet.Quantity * factDet.FOBUnitPrice }).ToList().Select(x=>x.FOBProduct).Sum(),



                            //Factoryvalue = (from buyMas in db.BuyerOrderMas
                            //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                            //                where buyMas.Id == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                            //                select new { FOBProduct = factDet.TransferPrice == null ? buyDet.Quantity * factDet.FOBUnitPrice : buyDet.Quantity * factDet.TransferPrice }).ToList().Select(x => x.FOBProduct).Sum(),


                            //Factoryvalue = (from buyMas in db.BuyerOrderMas
                            //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                            //                join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                            //                join shipSummDet in db.ShipmentSummDet on factdelivDet.ShipmentSummDetId equals shipSummDet.Id
                            //                where buyMas.Id == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                            //                select new { FOBProduct = shipSummDet.DelivQuantity * factdelivDet.FactFOB }).ToList().Select(x => x.FOBProduct).Sum(),


                            Factoryvalue = (from factMas in db.FactoryOrderMas
                                            join factDet in db.FactoryOrderDet on factMas.Id equals factDet.FactoryOrderMasId
                                            join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                                            join shipSummDet in db.ShipmentSummDet on factdelivDet.ShipmentSummDetId equals shipSummDet.Id
                                            where factMas.BuyerOrderMasId == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                                            select new { FOBProduct = shipSummDet.DelivQuantity * factdelivDet.FactFOB }).ToList().Select(x => x.FOBProduct).Sum(),

                            OrderDate = g.buyerMas.OrderDate,
                            ExFactoryDate = g.masterLCMas.LatestShipmentDate
                        })).SelectMany(g => g).Distinct().ToList();


            if (SupplierId != null)
            {
                data = data.Where(x => x.FactoryId == SupplierId).ToList();
            }


            foreach (var item in data)
            {
                ds.OrderInfoBuyerWise.AddOrderInfoBuyerWiseRow(
                   item.FactoryId,
                   item.FactoryName,
                   item.BuyerId,
                   item.BuyerName,
                   item.BuyerOrderMasId,
                   item.BuyerOrderRefNo,
                   item.OrderQty ?? 0,
                   item.RDLValue ?? 0,
                   item.Factoryvalue ?? 0,
                   item.OrderDate == null ? "" : NullHelpers.DateToString(item.OrderDate),
                   item.ExFactoryDate == null ? "" : NullHelpers.DateToString(item.ExFactoryDate),
                   //date ?? DateTime.Now 
                   date
                  );
            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OrderInfoFactoryWise.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;
        }

        //-------------------------END-----------------------------
        //---------------------------------------------------------




        //=========================================================
        //============LC Tansfer Information Factory Wise==========
        //-----------Added By Tazbirul(12/05/2018)-----------------

        [HttpGet]
        public ActionResult LCTransferinfoFactoryWise()
        {
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult LCTransferinfoFactoryWise(DateTime date, int? SupplierId)
        {
            LCTransferinfoFactoryWiseDS ds = new LCTransferinfoFactoryWiseDS();

            var firstDay = new DateTime(date.Year, date.Month, 1);

            var data = (from transferDet in db.LCTransferDet
                        join transferMas in db.LCTransferMas on transferDet.LCTransferMasId equals transferMas.Id
                        join LCMaster in db.MasterLCInfoMas on transferMas.MasterLCInfoMasId equals LCMaster.Id
                        join factoryMas in db.FactoryOrderMas on transferDet.FactoryOrderMasId equals factoryMas.Id
                        where transferMas.MasterLCInfoMas.LCDate >= firstDay && transferMas.MasterLCInfoMas.LCDate <= date
                        select new { factoryMas, transferMas, transferDet, LCMaster } into joined
                        group joined by new
                        {
                            joined.factoryMas.SupplierId,
                            joined.transferMas.MasterLCInfoMas.LCNo,
                            joined.transferMas.MasterLCInfoMas.BuyerInfoId
                        } into grouped
                        select grouped.Select(g => new
                        {
                            FactoryId = g.factoryMas.SupplierId,
                            FactoryName = g.factoryMas.Supplier.Name,
                            LCMasterId = g.transferMas.MasterLCInfoMasId,
                            LCMasterNo = g.LCMaster.LCNo,
                            BuyerId = g.LCMaster.BuyerInfoId,
                            BuyerName = g.LCMaster.BuyerInfo.Name,
                            LCTransferQId = g.transferMas.Id,
                            LCQty = g.LCMaster.Quantity,
                            OrderQty = db.MasterLCInfoMas.Where(x => x.Id == g.LCMaster.Id).Select(x => x.Quantity).Sum(),
                            LCValue = db.MasterLCInfoMas.Where(x => x.Id == g.LCMaster.Id).Select(x => x.TotalValue).Sum(),
                            //Transfervalue = (from transferDet in db.LCTransferDet
                            //                 join transferMas in db.LCTransferMas on transferDet.LCTransferMasId equals transferMas.Id
                            //                 join LCMaster in db.MasterLCInfoMas on transferMas.MasterLCInfoMasId equals LCMaster.Id
                            //                 join LCDetail in db.MasterLCInfoDet on LCMaster.Id equals LCDetail.MasterLCInfoMasId
                            //                 join factMas in db.FactoryOrderMas on new { ColA = LCDetail.BuyerOrderMasId }
                            //                 equals new { ColA = factMas.BuyerOrderMasId }
                            //                 join factDet in db.FactoryOrderDet on new { ColA = factMas.Id } equals new { ColA = factDet.FactoryOrderMasId }
                            //                 where LCMaster.Id == g.transferMas.MasterLCInfoMasId && factMas.SupplierId == g.factoryMas.SupplierId
                            //                 select new
                            //                 {
                            //                     Tranfer = factDet.TransferPrice == null ? factDet.FOBUnitPrice * factDet.BuyerOrderDet.Quantity :
                            //                                                         factDet.TransferPrice * factDet.BuyerOrderDet.Quantity
                            //                 }).Sum(x => x.Tranfer),

                            Transfervalue = (from transferDet in db.LCTransferDet
                                             join transferMas in db.LCTransferMas on transferDet.LCTransferMasId equals transferMas.Id
                                             join LCMaster in db.MasterLCInfoMas on transferMas.MasterLCInfoMasId equals LCMaster.Id
                                             join LCDetail in db.MasterLCInfoDet on LCMaster.Id equals LCDetail.MasterLCInfoMasId
                                             join factMas in db.FactoryOrderMas on new { ColA = LCDetail.BuyerOrderMasId }
                                             equals new { ColA = factMas.BuyerOrderMasId }
                                             join factDet in db.FactoryOrderDet on new { ColA = factMas.Id } equals new { ColA = factDet.FactoryOrderMasId }
                                             join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                                             where LCMaster.Id == g.transferMas.MasterLCInfoMasId && factMas.SupplierId == g.factoryMas.SupplierId
                                             select new
                                             {
                                                 Tranfer = factdelivDet.FactFOB * factDet.BuyerOrderDet.Quantity
                                             }).Sum(x => x.Tranfer),

                            TransferDate = g.transferDet.TransferDate
                        })).SelectMany(g => g).Distinct().ToList();


            if (SupplierId != null)
            {
                data = data.Where(x => x.FactoryId == SupplierId).ToList();
            }


            foreach (var item in data)
            {
                ds.LCTransferFactory.AddLCTransferFactoryRow(
                    item.FactoryId,
                    item.FactoryName,
                    item.LCMasterId,
                    item.LCMasterNo,
                    item.BuyerId ?? 0,
                    item.BuyerName,
                    item.LCQty ?? 0,
                    item.OrderQty ?? 0,
                    item.LCValue,
                    item.Transfervalue ?? 0,
                    item.TransferDate == null ? "" : NullHelpers.DateToString(item.TransferDate),
                    date
                    );
            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "LCTransferinfoFactoryWise.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;
        }

        //-------------------------END-----------------------------
        //---------------------------------------------------------



        [HttpGet]
        public ActionResult OrderInfoBuyerWise()
        {
            ViewBag.BuyerId = new SelectList(db.BuyerInfo, "Id", "Name");

            var months = new[]
            {
                     new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                     new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                     new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
                 };

            ViewBag.Months = new SelectList(months, "Value", "MonthName");
            return View();
        }

        [HttpPost]
        public ActionResult OrderInfoBuyerWise(int? BuyerId, DateTime date)
        {
            OrderInfoBuyerWise ds = new OrderInfoBuyerWise();


            //var data = (from buyMas in db.BuyerOrderMas
            //            join lcDet in db.MasterLCInfoDet on buyMas.Id equals lcDet.BuyerOrderMasId into lcDetJoin
            //            from lcDetAfterLeftJoin in lcDetJoin.DefaultIfEmpty()
            //            join lcMas in db.MasterLCInfoMas on buyMas.BuyerInfoId equals lcMas.BuyerInfoId into lcMasJoin
            //            from lcMasAfterLeftJoin in lcMasJoin.DefaultIfEmpty()
            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //            join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
            //            join factDet in db.FactoryOrderDet on buyDet.Id equals factDet.BuyerOrderDetId
            //            where buyMas.BuyerInfoId == BuyerId
            //            select new { buyMas, buyDet, factMas, factDet }
            //               ).ToList();

            var firstDay = new DateTime(date.Year, date.Month, 1);

            var data = (from buyerMas in db.BuyerOrderMas
                        join buyerDet in db.BuyerOrderDet on buyerMas.Id equals buyerDet.BuyerOrderMasId
                        join factoryMas in db.FactoryOrderMas on new { ColA = buyerDet.BuyerOrderMasId, ColB = buyerDet.SupplierId }
                        equals new { ColA = factoryMas.BuyerOrderMasId, ColB = factoryMas.SupplierId }
                        join factoryDet in db.FactoryOrderDet on new { ColA = factoryMas.Id, ColB = buyerDet.Id } equals new { ColA = factoryDet.FactoryOrderMasId, ColB = factoryDet.BuyerOrderDetId }
                        join masterLCDet in db.MasterLCInfoDet on buyerMas.Id equals masterLCDet.BuyerOrderMasId into joinLCDet
                        from masterLCDet in joinLCDet.DefaultIfEmpty()
                        join masterLCMas in db.MasterLCInfoMas on masterLCDet.MasterLCInfoMasId equals masterLCMas.Id into joinLCMas
                        from masterLCMas in joinLCMas.DefaultIfEmpty()
                        where buyerMas.OrderDate >= firstDay && buyerMas.OrderDate <= date
                        select new { factoryMas, factoryDet, buyerMas, buyerDet, masterLCMas } into joined
                        group joined by new
                        {
                            joined.buyerMas.BuyerInfoId,
                            joined.buyerMas.Id,
                            joined.buyerMas.OrderDate,
                            joined.masterLCMas.LatestShipmentDate
                        } into grouped
                        select grouped.Select(g => new
                        {
                            BuyerId = g.buyerMas.BuyerInfoId,
                            BuyerName = g.buyerMas.BuyerInfo.Name,
                            BuyerOrderMasId = g.buyerMas.Id,
                            BuyerOrderRefNo = g.buyerMas.OrderRefNo,
                            OrderQty = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == g.buyerMas.Id).Sum(x => x.Quantity),
                            RDLValue = (from buyMas in db.BuyerOrderMas
                                        join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                                        where buyMas.Id == g.buyerMas.Id
                                        select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct),
                            //Factoryvalue = (from buyMas in db.BuyerOrderMas
                            //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                            //                where buyMas.Id == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                            //                select new { FOBProduct = factDet.TransferPrice == null ? buyDet.Quantity * factDet.FOBUnitPrice : buyDet.Quantity * factDet.TransferPrice }).ToList().Select(x => x.FOBProduct).Sum(),


                            Factoryvalue = (from factMas in db.FactoryOrderMas
                                            join factDet in db.FactoryOrderDet on factMas.Id equals factDet.FactoryOrderMasId
                                            join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                                            join shipSummDet in db.ShipmentSummDet on factdelivDet.ShipmentSummDetId equals shipSummDet.Id
                                            where factMas.BuyerOrderMasId == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                                            select new { FOBProduct = shipSummDet.DelivQuantity * factdelivDet.FactFOB }).ToList().Select(x => x.FOBProduct).Sum(),

                            //Factoryvalue = (from buyMas in db.BuyerOrderMas
                            //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                            //                where buyMas.Id == g.buyerMas.Id && factMas.SupplierId == g.factoryMas.SupplierId
                            //                select new { FOBProduct = buyDet.Quantity * factDet.FOBUnitPrice }).ToList().Select(x => x.FOBProduct).Sum(),
                            OrderDate = g.buyerMas.OrderDate,
                            ExFactoryDate = g.masterLCMas.LatestShipmentDate
                        })).SelectMany(g => g).Distinct().ToList();

            if (BuyerId != null)
            {
                data = data.Where(x => x.BuyerId == BuyerId).ToList();
            }



            foreach (var item in data)
            {
                ds.BuyerWiseDS.AddBuyerWiseDSRow(
                   item.BuyerId,
                   item.BuyerName,
                   item.BuyerOrderMasId,
                   item.BuyerOrderRefNo,
                   item.OrderQty ?? 0,
                   item.RDLValue ?? 0,
                   item.Factoryvalue ?? 0,
                   item.OrderDate == null ? "" : NullHelpers.DateToString(item.OrderDate),
                   item.ExFactoryDate == null ? "" : NullHelpers.DateToString(item.ExFactoryDate),
                   date
                  );
            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OrderInfoBuyerWise.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }


        [HttpGet]
        public ActionResult LCInfoBuyerWise()
        {
            ViewBag.BuyerId = new SelectList(db.BuyerInfo, "Id", "Name");

            var months = new[]
            {
                     new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                     new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                     new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
                 };

            ViewBag.Months = new SelectList(months, "Value", "MonthName");
            return View();
        }

        [HttpPost]
        public ActionResult LCInfoBuyerWise(int? BuyerId, DateTime date)
        {
            LCInfoBuyerWiseDS ds = new LCInfoBuyerWiseDS();


            var firstDay = new DateTime(date.Year, date.Month, 1);

            var data = (from lcMas in db.MasterLCInfoMas
                        where lcMas.LCDate >= firstDay && lcMas.LCDate <= date
                        select new { lcMas } into joined
                        group joined by new
                        {
                            joined.lcMas,

                        } into grouped
                        select grouped.Select(g => new
                        {
                            BuyerId = g.lcMas.BuyerInfoId,
                            BuyerName = g.lcMas.BuyerInfo.Name,
                            LCId = g.lcMas.Id,
                            LCNo = g.lcMas.LCNo,
                            LCOpeningDate = g.lcMas.LCDate,
                            LCReceiveDate = g.lcMas.LCReceiveDate,
                            LCExpireDate = g.lcMas.LCExpiryDate,
                            LCQty = g.lcMas.Quantity,
                            LCValue = g.lcMas.TotalValue,
                            LiasonBank = g.lcMas.BankBranch.Name
                        })).SelectMany(g => g).Distinct().ToList();

            if (BuyerId != null)
            {
                data = data.Where(x => x.BuyerId == BuyerId).ToList();
            }



            foreach (var item in data)
            {
                ds.LCBuyerWise.AddLCBuyerWiseRow(
                   item.BuyerId ?? 0,
                   item.BuyerName,
                   item.LCId,
                   item.LCNo,
                   item.LCOpeningDate == null ? "" : NullHelpers.DateToString(item.LCOpeningDate),
                   item.LCReceiveDate == null ? "" : NullHelpers.DateToString(item.LCReceiveDate),
                   item.LCQty ?? 0,
                   item.LCValue,
                   item.LCExpireDate == null ? "" : NullHelpers.DateToString(item.LCExpireDate),
                   item.LiasonBank,
                   date
                  );
            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "LCInfoBuyerWise.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }




        [HttpGet]
        public ActionResult OrderInfoBuyerWiseDetailReport()
        {
            ViewBag.BuyerId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.BuyerMasId = new SelectList(db.BuyerOrderMas, "Id", "OrderRefNo");

            var months = new[]
            {
                     new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                     new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                     new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
                 };

            ViewBag.Months = new SelectList(months, "Value", "MonthName");
            return View();
        }

        [HttpPost]
        public ActionResult OrderInfoBuyerWiseDetailReport(int? BuyerId, int? BuyerMasId, DateTime date)
        {
            OrderInfoBuyerWiseDetailDS ds = new OrderInfoBuyerWiseDetailDS();

            var firstDay = new DateTime(date.Year, date.Month, 1);

            var data = (from buyMas in db.BuyerOrderMas
                        join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                        join factDet in db.FactoryOrderDet on buyDet.Id equals factDet.BuyerOrderDetId
                        join factMas in db.FactoryOrderMas on factDet.FactoryOrderMasId equals factMas.Id
                        //join shipSumm in db.ShipmentSummDet on buyDet.Id equals shipSumm.BuyerOrderDetId
                        where buyMas.OrderDate >= firstDay && buyMas.OrderDate <= date && buyMas.BuyerInfoId == BuyerId && buyMas.Id == BuyerMasId
                        select new { buyMas, buyDet, factDet, factMas } into joined
                        group joined by new
                        {
                            joined.buyMas.BuyerInfoId,
                            joined.buyMas.Id,
                            joined.buyMas.ProdDepartmentId,
                            joined.buyMas.SeasonInfoId

                        } into grouped
                        select grouped.Select(g => new
                        {
                            BuyerId = g.buyMas.BuyerInfoId,
                            BuyerName = g.buyMas.BuyerInfo.Name,
                            BuyerMasId = g.buyMas.Id,
                            RDLNo = g.buyMas.OrderRefNo,
                            DeptId = g.buyMas.ProdDepartmentId,
                            SeasonId = g.buyMas.SeasonInfoId,
                            Department = g.buyMas.ProdDepartment.Name,
                            Season = g.buyMas.SeasonInfo.Name,
                            //ProductType = g.buyDet.ProdCatType.ProdCategory.Name,
                            //ProdCat = g.buyDet.ProdCatType.Name,
                            //Style = g.buyDet.StyleNo,
                            //SizeRange = g.buyDet.ProdSize.SizeRange,
                            //Color = g.buyDet.ProdColor.Name,
                            //Quantity = g.buyDet.Quantity,
                            //ProductType = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.ProdCatType.ProdCategory.Name).ToList(),
                            //ProdCat = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.ProdCatType.Name).ToList(),
                            //Style = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.StyleNo).ToList(),
                            //SizeRange = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.ProdSize.SizeRange).ToList(),
                            //Color = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.ProdColor.Name).ToList(),
                            //Quantity = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.Quantity).ToList(),
                            //RDLFob = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == BuyerMasId).Select(x => x.UnitPrice).ToList(),
                            //RDLFob = g.buyDet.UnitPrice,
                            //RDLValue = (from buyMas in db.BuyerOrderMas
                            //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //            where buyMas.Id == g.buyMas.Id
                            //            select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct),
                            //FactoryName = g.factMas.Supplier.Name,
                            //FactoryFOB = g.factDet.FOBUnitPrice,
                            //FactoryValue = (from buyMas in db.BuyerOrderMas
                            //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                            //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                            //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                            //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                            //                where buyMas.Id == g.buyMas.Id && factMas.SupplierId == g.factMas.SupplierId
                            //                select new { FOBProduct = factDet.TransferPrice == null ? buyDet.Quantity * factDet.FOBUnitPrice : buyDet.Quantity * factDet.TransferPrice }).ToList().Select(x => x.FOBProduct).Sum(),
                            ////ExFactDate = g.buyDet.ShipmentSummDet.Select(x=>x.ExFactoryDate).LastOrDefault()
                            //DestCountry = g.buyDet.ShipmentSummDet.,
                        })).SelectMany(g => g).Distinct().ToList();



            /*select buD.StyleNo, facMas.SupplierId from [dbo].[BuyerOrderDet] buD
       left join [dbo].[FactoryOrderDet] facDet on buD.Id = facDet.BuyerOrderDetId
       left join [dbo].[FactoryOrderMas] facMas on facDet.FactoryOrderMasId = facMas.Id and buD.SupplierId= facMas.SupplierId
                inner join [dbo].[BuyerOrderMas] buyMas on buD.BuyerOrderMasId = buyMas.Id		
       where buD.BuyerOrderMasId=24*/



            var detailData = (from buyDet in db.BuyerOrderDet
                              join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
                              join factJoin in db.FactoryOrderDet on buyDet.Id equals factJoin.BuyerOrderDetId into joinLeft
                              from factJoin in joinLeft.DefaultIfEmpty()
                              join factJoinMas in db.FactoryOrderMas on factJoin.FactoryOrderMasId equals factJoinMas.Id into joinLeftMas
                              from factJoinMas in joinLeftMas.DefaultIfEmpty()
                                  // join summDet in db.ShipmentSummDet on buyDet.Id equals summDet.BuyerOrderDetId into joinLeftShipSumm
                                  // from summDet in joinLeftShipSumm.DefaultIfEmpty()
                              where buyMas.Id == BuyerMasId
                              select new { buyMas, buyDet, factJoin, factJoinMas } into joined
                              group joined by new
                              {
                                  joined.buyMas.BuyerInfoId,
                                  joined.buyMas.Id,
                                  joined.buyMas.ProdDepartmentId,
                                  joined.buyMas.SeasonInfoId

                              } into grouped
                              select grouped.Select(g => new
                              {
                                  BuyerMasId = g.buyMas.Id,
                                  ProductType = g.buyDet.ProdCatType.ProdCategory.Name,
                                  ProdCat = g.buyDet.ProdCatType.Name,
                                  Style = g.buyDet.StyleNo,
                                  SizeRange = g.buyDet.ProdSize.SizeRange,
                                  Color = g.buyDet.ProdColor.Name,
                                  Quantity = g.buyDet.Quantity,
                                  FactoryName = g.buyDet.Supplier.Name,
                                  RDLFob = g.buyDet.UnitPrice,
                                  SupplierId = g.buyDet.SupplierId,
                                  //ExFactoryDate = g.summDet.ExFactoryDate,

                                  //ExFactoryDate = db.ShipmentSummDet.Where(x=>x.BuyerOrderDetId== g.buyDet.Id).LastOrDefault().ExFactoryDate,                          

                                  //DestCountry = g.summDet.DestinationPort.Name,
                                  BuyDetId = g.buyDet.Id
                                  //RDLValue = (from buyMas in db.BuyerOrderMas
                                  //            join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                                  //            where buyMas.Id == g.buyMas.Id
                                  //            select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct),
                                  //FactoryFOB = g.factJoin.FOBUnitPrice,
                                  //FactoryValue = (from buyMas in db.BuyerOrderMas
                                  //                join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                                  //                join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                                  //                equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                                  //                join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                                  //                where buyMas.Id == g.buyMas.Id && factMas.SupplierId == g.factJoinMas.SupplierId
                                  //                select new { FOBProduct = factDet.TransferPrice == null ? buyDet.Quantity * factDet.FOBUnitPrice : buyDet.Quantity * factDet.TransferPrice }).ToList().Select(x => x.FOBProduct).Sum(),

                              })).SelectMany(g => g).ToList();


            //var detailData = (from buyDet in db.BuyerOrderDet
            //                  join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
            //                  join factJoin in db.FactoryOrderDet on buyDet.Id equals factJoin.BuyerOrderDetId into joinLeft
            //                  from factJoin in joinLeft.DefaultIfEmpty()
            //                  join factJoinMas in db.FactoryOrderMas on factJoin.FactoryOrderMasId equals factJoinMas.Id into joinLeftMas
            //                  from factJoinMas in joinLeftMas.DefaultIfEmpty()
            //                  join summDet in db.ShipmentSummDet on buyDet.Id equals summDet.BuyerOrderDetId into joinLeftShipSumm
            //                  from summDet in joinLeftShipSumm.DefaultIfEmpty()



            var shipSumm = (from buyDet in db.BuyerOrderDet
                            join buyMas in db.BuyerOrderMas on buyDet.BuyerOrderMasId equals buyMas.Id
                            join shipSumms in db.ShipmentSummDet on buyDet.Id equals shipSumms.BuyerOrderDetId into leftShipJoin
                            from shipSumms in leftShipJoin.DefaultIfEmpty()
                            where buyMas.Id == BuyerMasId
                            select shipSumms).GroupBy(x => x.BuyerOrderDetId, (key, g) => g.OrderBy(e => e.BuyerOrderDetId).OrderByDescending(x => x.ExFactoryDate).FirstOrDefault());


            foreach (var item in data)
            {

                ds.BuyerWiseDetailDS.AddBuyerWiseDetailDSRow(
                   item.BuyerId,
                   item.BuyerName,
                   item.BuyerMasId,
                   item.RDLNo,
                   item.DeptId ?? 0,
                   item.SeasonId ?? 0,
                   item.Department,
                   item.Season,
                   //item.RDLFob??0,
                   //item.RDLValue ?? 0,
                   //item.
                   //FactoryName,
                   //item.FactoryFOB,
                   //item.FactoryValue ?? 0,
                   NullHelpers.DateToString(date)
                  );


                foreach (var items in detailData)
                {

                    var exDate = shipSumm.Where(x => x.BuyerOrderDetId == items.BuyDetId).FirstOrDefault().ExFactoryDate.ToString();
                    var destCountry = shipSumm.Where(x => x.BuyerOrderDetId == items.BuyDetId).FirstOrDefault().DestinationPort.Name;

                    var RDLValue = (from buyMas in db.BuyerOrderMas
                                    join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                                    where buyMas.Id == items.BuyerMasId && buyDet.Id == items.BuyDetId
                                    select new { RDProduct = buyDet.Quantity * buyDet.UnitPrice }).Sum(x => x.RDProduct);


                    //var FactoryValue = (from buyMas in db.BuyerOrderMas
                    //                    join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                    //                    join factMas in db.FactoryOrderMas on new { ColA = buyDet.BuyerOrderMasId, ColB = buyDet.SupplierId }
                    //                    equals new { ColA = factMas.BuyerOrderMasId, ColB = factMas.SupplierId }
                    //                    join factDet in db.FactoryOrderDet on new { ColA = factMas.Id, ColB = buyDet.Id } equals new { ColA = factDet.FactoryOrderMasId, ColB = factDet.BuyerOrderDetId }
                    //                    where buyMas.Id == items.BuyerMasId && factMas.SupplierId == items.SupplierId && buyDet.Id == items.BuyDetId
                    //                    select new { FOBProduct = factDet.TransferPrice == null ? buyDet.Quantity * factDet.FOBUnitPrice : buyDet.Quantity * factDet.TransferPrice }).ToList().Select(x => x.FOBProduct).Sum();


                    var FactoryValue = (from factMas in db.FactoryOrderMas
                                        join factDet in db.FactoryOrderDet on factMas.Id equals factDet.FactoryOrderMasId
                                        join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                                        join shipSummDet in db.ShipmentSummDet on factdelivDet.ShipmentSummDetId equals shipSummDet.Id
                                        where factMas.BuyerOrderMasId == items.BuyerMasId && factMas.SupplierId == items.SupplierId
                                        select new { FOBProduct = shipSummDet.DelivQuantity * factdelivDet.FactFOB }).ToList().Select(x => x.FOBProduct).Sum();



                    ds.DetailData.AddDetailDataRow(
                       items.BuyerMasId,
                       items.ProductType,
                       items.ProdCat,
                       items.Style,
                       items.SizeRange,
                       items.Color,
                       items.Quantity ?? 0,
                       items.FactoryName,
                       items.RDLFob ?? 0,
                       //exDate == null ? "" : NullHelpers.DateToString(DateTime.Parse(exDate)),
                       exDate == null ? "" : exDate,
                       destCountry,
                       //items.ExFactoryDate==null ? "" : NullHelpers.DateToString(items.ExFactoryDate),
                       //items.DestCountry
                       RDLValue ?? 0,
                       // items.FactoryFOB,
                       FactoryValue ?? 0
                      );
                }

            }

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "BuyerWiseDetail.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }



        //     /////////////////////////////////////////Nabid////////////////////////////////////////////////
        //     //////////////////////////////////////12-Jul-18///////////////////////////////////////////////
        //     //////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public ActionResult CustomerWiseSalesRecap()
        {
            ViewBag.BuyInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            var months = new[]
            {
                     new{MonthName="January", Value=1},  new{MonthName="February", Value=2},  new{MonthName="March", Value=3},  new{MonthName="April", Value=4},
                     new{MonthName="May", Value=5},  new{MonthName="June", Value=6},  new{MonthName="July", Value=7},  new{MonthName="August", Value=8},
                     new{MonthName="September", Value=9},  new{MonthName="October", Value=10},  new{MonthName="November", Value=11},  new{MonthName="December", Value=12}
                 };

            ViewBag.MonthTo = new SelectList(months, "Value", "MonthName");
            ViewBag.MonthFrom = new SelectList(months, "Value", "MonthName");


            return View();
        }

        [HttpPost]
        public ActionResult CustomerWiseSalesRecap(int? MonthFrom, int? MonthTo, int Year, int? BuyInfoId)
        {

            CustomerWiseSaleDS ds = new CustomerWiseSaleDS();

            /*select * from [dbo].[BuyerOrderMas] buyMas
                inner join [dbo].[BuyerOrderDet] buyDet on buyMas.Id = buyDet.BuyerOrderMasId
                inner join [dbo].[FactoryOrderMas] factMas on buyMas.Id = factMas.BuyerOrderMasId and buyDet.SupplierId = factMas.SupplierId
                inner join [dbo].[FactoryOrderDet] factDet on buyDet.Id = factDet.BuyerOrderDetId and factMas.Id= factDet.FactoryOrderMasId
                */
            var january = 1;
            var february = 2;
            var march = 3;
            var april = 4;
            var may = 5;
            var june = 6;
            var july = 7;
            var august = 8;
            var sept = 9;
            var oct = 10;
            var nov = 11;
            var dec = 12;
            var rowCounter = 0;
            var dictionary = new Dictionary<Tuple<int, int, int>, int>(); // <BuyerId,MonthId,Year>

            //if (MonthFrom != null && MonthTo != null)
            //{
            //    var a = NullHelpers.DateToString(MonthFrom);
            //    var b = NullHelpers.DateToString(MonthTo);
            //    ds.SelectedYear.AddSelectedYearRow(
            //                a,
            //                b)
            //                ;
            //}

            //if (MonthFrom != null && MonthTo != null && Year!=0)
            //{

            ds.SelectedYear.AddSelectedYearRow(
                        MonthHelper.intToString(MonthFrom),
                        MonthHelper.intToString(MonthTo),
                        Year)
                        ;
            //}

            var buyers = (from buyMas in db.BuyerOrderMas
                          join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                          join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
                          join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
                          join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                          //where buyMas.OrderDate.Value.Year == date
                          //where buyMas.OrderDate >= date && buyMas.OrderDate <= date && buyMas.BuyerInfoId == BuyerId 
                          select new { buyMas, factMas, buyDet, factDet, factdelivDet }
                         ).ToList();

            if (MonthFrom != null && MonthTo != null && BuyInfoId != null && Year != 0)
            {
                buyers = buyers.Where(x => x.buyMas.OrderDate.Value.Month >= MonthFrom && x.buyMas.OrderDate.Value.Month <= MonthTo && x.buyMas.BuyerInfoId == BuyInfoId && x.buyMas.OrderDate.Value.Year == Year).ToList();
            }
            else if (MonthFrom != null && MonthTo != null && Year != 0)
            {
                buyers = buyers.Where(x => x.buyMas.OrderDate.Value.Month >= MonthFrom && x.buyMas.OrderDate.Value.Month <= MonthTo && x.buyMas.OrderDate.Value.Year == Year).ToList();
            }

            else if (BuyInfoId != null && Year != 0)
            {
                buyers = buyers.Where(x => x.buyMas.BuyerInfoId == BuyInfoId && x.buyMas.OrderDate.Value.Year == Year).ToList();
            }



            foreach (var item in buyers)
            {

                var key = new Tuple<int, int, int>(item.buyMas.BuyerInfoId, item.buyMas.OrderDate.Value.Month, item.buyMas.OrderDate.Value.Year);
                var value = rowCounter;
                if (dictionary.ContainsKey(key))
                {
                    int getRow = dictionary[key];


                    if (item.buyMas.OrderDate.Value.Month == january)
                    {
                        ds.CustomerSale[getRow].JanRDL = ds.CustomerSale[getRow].JanRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].JanEX = ds.CustomerSale[getRow].JanEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].JanEX = ds.CustomerSale[getRow].JanEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));
                    }
                    else if (item.buyMas.OrderDate.Value.Month == february)
                    {
                        ds.CustomerSale[getRow].FebRDL = ds.CustomerSale[getRow].FebRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].FebEX = ds.CustomerSale[getRow].FebEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].FebEX = ds.CustomerSale[getRow].FebEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == march)
                    {
                        ds.CustomerSale[getRow].MarRDL = ds.CustomerSale[getRow].MarRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].MarEX = ds.CustomerSale[getRow].MarEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].MarEX = ds.CustomerSale[getRow].MarEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == april)
                    {
                        ds.CustomerSale[getRow].AprRDL = ds.CustomerSale[getRow].AprRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        // ds.CustomerSale[getRow].AprEX = ds.CustomerSale[getRow].AprEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].AprEX = ds.CustomerSale[getRow].AprEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == may)
                    {
                        ds.CustomerSale[getRow].MayRDL = ds.CustomerSale[getRow].MayRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].MayEX = ds.CustomerSale[getRow].MayEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].MayEX = ds.CustomerSale[getRow].MayEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == june)
                    {
                        ds.CustomerSale[getRow].JunRDL = ds.CustomerSale[getRow].JunRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        // ds.CustomerSale[getRow].JunEX = ds.CustomerSale[getRow].JunEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].JunEX = ds.CustomerSale[getRow].JunEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == july)
                    {
                        ds.CustomerSale[getRow].JulRDL = ds.CustomerSale[getRow].JulRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].JulEX = ds.CustomerSale[getRow].JulEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].JulEX = ds.CustomerSale[getRow].JulEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == august)
                    {
                        ds.CustomerSale[getRow].AugRDL = ds.CustomerSale[getRow].AugRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].AugEX = ds.CustomerSale[getRow].AugEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].AugEX = ds.CustomerSale[getRow].AugEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == sept)
                    {
                        ds.CustomerSale[getRow].SepRDL = ds.CustomerSale[getRow].SepRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].SepEX = ds.CustomerSale[getRow].SepEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].SepEX = ds.CustomerSale[getRow].SepEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == oct)
                    {
                        ds.CustomerSale[getRow].OctRDL = ds.CustomerSale[getRow].OctRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].OctEX = ds.CustomerSale[getRow].OctEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].OctEX = ds.CustomerSale[getRow].OctEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == nov)
                    {
                        ds.CustomerSale[getRow].NovRDL = ds.CustomerSale[getRow].NovRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].NovEX = ds.CustomerSale[getRow].NovEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].NovEX = ds.CustomerSale[getRow].NovEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                    else if (item.buyMas.OrderDate.Value.Month == dec)
                    {
                        ds.CustomerSale[getRow].DecRDL = ds.CustomerSale[getRow].DecRDL + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
                        //ds.CustomerSale[getRow].DecEX = ds.CustomerSale[getRow].DecEX + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                        ds.CustomerSale[getRow].DecEX = ds.CustomerSale[getRow].DecEX + (item.factdelivDet.FactFOB ?? 0 * (item.buyDet.Quantity ?? 0));

                    }
                }

                else
                {
                    dictionary.Add(key, value);



                    if (item.buyMas.OrderDate.Value.Month == january)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                                  item.buyMas.BuyerInfoId,
                                  item.buyMas.BuyerInfo.Name,
                                  (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                  //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                  (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                   0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                                  );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == february)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                               0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                               0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                               0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == march)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == april)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == may)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == june)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == july)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == august)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                               0, 0, 0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == sept)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0, 0, 0
                               );

                    }
                    else if (item.buyMas.OrderDate.Value.Month == oct)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0,
                                (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                 //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                 (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0, 0, 0
                               );

                    }

                    else if (item.buyMas.OrderDate.Value.Month == nov)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0,
                                0, 0
                               );

                    }

                    else if (item.buyMas.OrderDate.Value.Month == dec)
                    {
                        ds.CustomerSale.AddCustomerSaleRow(
                               item.buyMas.BuyerInfoId,
                               item.buyMas.BuyerInfo.Name,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                                //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0
                                (item.factdelivDet.FactFOB * item.buyDet.Quantity) ?? 0
                               );

                    }

                    rowCounter++;
                }


            }





            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CustomerWiseSales.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }



        [HttpGet]
        public ActionResult CustomerWiseFactoryComm()
        {
            ViewBag.FactoryId = new SelectList(db.Supplier, "Id", "Name");


            return View();
        }

        [HttpPost]
        public ActionResult CustomerWiseFactoryComm(DateTime? dateFrom, DateTime? dateTo, int? FactoryId)
        {

            FactoryWiseExportValDS ds = new FactoryWiseExportValDS();

            var rowCounter = 0;

            var dictionary = new Dictionary<Tuple<int, DateTime?>, int>(); // <BuyerId,MonthId,Year>

            var dictCount = new Dictionary<Tuple<int, int>, int>();

            decimal Comm = 0;
            decimal Commission = 0;

            //var buyers = (from buyMas in db.BuyerOrderMas
            //              join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //              join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
            //              join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //              where buyMas.OrderDate.Value.Year == date
            //              select new { buyMas, factMas, buyDet, factDet }

            //             ).ToList();




            var buyers = (from exFactoryMas in db.ExFactoryMas
                          join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                          join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                          join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                          join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                  new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                          join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                          join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                          join factdelivDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id } equals new { ColA = factdelivDet.FactoryOrderDetId }
                          join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                          from pJoin in joinDest.DefaultIfEmpty()

                          join invFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                          equals new { a = invFactMas.SupplierId, b = invFactMas.BuyerInfoId }

                          join invCommFactDet in db.InvoiceCommFactDet on new { a = invFactMas.Id, b = exfactoryDet.Id }
                          equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }

                          join invComDet in db.InvoiceCommDet on invFactMas.Id equals invComDet.InvoiceCommFactMasId
                          //where invComDet.InvoiceCommMasId == Id
                          where shipDet.Id == factdelivDet.ShipmentSummDetId
                          select new { invFactMas, invComDet, buyerOrderDet, factoryOrderDet, factdelivDet }).AsEnumerable()
              .Select(x => new
              {
                  FactoryName = x.invFactMas.Supplier.Name,
                  FactoryId = x.invFactMas.Supplier.Id,
                  InvCommDetId = x.invComDet.Id,
                  InvCommMasId = x.invFactMas.Id,
                  InvoiceNo = x.invFactMas.InvoiceNoFact,
                  TotalQty = (from exFactoryMas in db.ExFactoryMas
                              join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                              join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                              join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                              join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                      new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                              join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                              join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                              join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                              from pJoin in joinDest.DefaultIfEmpty()

                              join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                              equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }

                              join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                              equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }

                              where invCommFactMas.Id == x.invFactMas.Id
                              select exfactoryShipDet.ShipQuantity
                     ).Sum(),
                  RDLValue = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                              join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                              join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                              join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                              join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                              join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerDet.SupplierId, b = exFactoryDet.BuyerOrderMas.BuyerInfoId }
                                    equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                              join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exFactoryDet.Id }
                                    equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                              where invCommFactMas.Id == x.invFactMas.Id
                              select new
                              {
                                  Quantity = exFactoryShipDet.ShipQuantity,
                                  RDLValue = (exFactoryShipDet.ShipQuantity * buyerDet.UnitPrice)
                              }
                                  ).Distinct().Sum(m => m.RDLValue),

                  //FactoryInvValue = (from exFactoryMas in db.ExFactoryMas
                  //                   join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                  //                   join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                  //                   join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                  //                   join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                  //                                                           new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                  //                   join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                  //                   join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                  //                   join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                  //                   from pJoin in joinDest.DefaultIfEmpty()
                  //                   join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                  //                   equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                  //                   join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                  //                   equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                  //                   where invCommFactMas.Id == x.invFactMas.Id
                  //                   select new
                  //                   {
                  //                       FactoryInv = exfactoryShipDet.ShipQuantity * (factoryOrderDet.TransferPrice == null ? factoryOrderDet.FOBUnitPrice : factoryOrderDet.TransferPrice)
                  //                   }).Distinct()
                  //          .Sum(n => n.FactoryInv),

                  FactoryInvValue = (from exFactoryMas in db.ExFactoryMas
                                     join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                                     join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                                     join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                     join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                             new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                     join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                     join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                     join factdelivDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id } equals new { ColA = factdelivDet.FactoryOrderDetId }
                                     join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                     from pJoin in joinDest.DefaultIfEmpty()
                                     join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerOrderDet.SupplierId, b = buyerOrderMas.BuyerInfoId }
                                     equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                                     join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exfactoryDet.Id }
                                     equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                                     where invCommFactMas.Id == x.invFactMas.Id && shipDet.Id == factdelivDet.ShipmentSummDetId
                                     select new
                                     {
                                         FactoryInv = exfactoryShipDet.ShipQuantity * (factdelivDet.FactFOB)
                                     }).Distinct()
                            .Sum(n => n.FactoryInv),

                  UnitPrc = x.buyerOrderDet.UnitPrice,
                  Qty = x.buyerOrderDet.Quantity,
                  FactFobUnitPrc = x.factdelivDet.FactFOB,
                  BuyInfoIds = x.buyerOrderDet.BuyerOrderMasId,
                  OrderDate = x.buyerOrderDet.BuyerOrderMas.OrderDate,
                  SupplierId = x.factoryOrderDet.FactoryOrderMas.SupplierId,
                  SupplierName = x.factoryOrderDet.FactoryOrderMas.Supplier.Name,
                  InvCommDate = x.invComDet.InvoiceCommMas.IssueDate

              }).Distinct();


            if (dateFrom != null && dateTo != null && FactoryId != null)
            {
                //buyers = buyers.Where(x => x.OrderDate >= dateFrom && x.OrderDate <= dateTo && x.FactoryId == FactoryId).ToList();
                buyers = buyers.Where(x => x.InvCommDate >= dateFrom && x.InvCommDate <= dateTo && x.FactoryId == FactoryId).ToList();
            }
            else if (dateFrom != null && dateTo != null)
            {
                //buyers = buyers.Where(x => x.OrderDate >= dateFrom && x.OrderDate <= dateTo).ToList();
                buyers = buyers.Where(x => x.InvCommDate >= dateFrom && x.InvCommDate <= dateTo).ToList();
            }

            else if (FactoryId != null)
            {
                buyers = buyers.Where(x => x.FactoryId == FactoryId).ToList();
            }





            if (dateFrom != null && dateTo != null)
            {
                var a = NullHelpers.DateToString(dateFrom);
                var b = NullHelpers.DateToString(dateTo);
                ds.SelectedYear.AddSelectedYearRow(
                            a,
                            b)
                            ;
            }


            foreach (var item in buyers)
            {

                Comm = ((item.UnitPrc ?? 0) * (item.Qty ?? 0)) - (item.FactFobUnitPrc ?? 0 * (item.Qty ?? 0));

                Commission = (Comm * 100) / ((item.UnitPrc ?? 0) * (item.Qty ?? 0));


                var key = new Tuple<int, DateTime?>(item.BuyInfoIds, item.OrderDate);
                var value = rowCounter;





                if (dictionary.ContainsKey(key))
                {
                    int getRow = dictionary[key];

                    ds.CustomerFactExport[getRow].BuyerExpVal = ds.CustomerFactExport[getRow].BuyerExpVal + (item.UnitPrc ?? 0) * (item.Qty ?? 0);
                    //ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                    ds.CustomerFactExport[getRow].BuyerCommVal = ds.CustomerFactExport[getRow].BuyerCommVal + Comm;
                    //ds.CustomerExpComm[getRow].BuyerAvgComm =(( ds.CustomerExpComm[getRow].BuyerAvgComm + Commission) * ds.CustomerExpComm[getRow].Count-1)/ ds.CustomerExpComm[getRow].Count;
                    ds.CustomerFactExport[getRow].BuyerAvgComm = ds.CustomerFactExport[getRow].BuyerAvgComm + Commission;
                    ds.CustomerFactExport[getRow].Count = ds.CustomerFactExport[getRow].Count + 1;

                }

                else
                {
                    dictionary.Add(key, value);

                    ds.CustomerFactExport.AddCustomerFactExportRow(
                               item.SupplierId,
                               item.SupplierName,
                               (item.UnitPrc * item.Qty) ?? 0,
                               //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                               Comm,
                               Commission,
                               1
                               );



                    rowCounter++;


                }


            }

            var sum = 0;
            var sumComm = 0;

            foreach (DataRow dr in ds.CustomerFactExport.Rows)
            {
                sum = sum + Convert.ToInt32(dr["BuyerExpVal"]);
                sumComm = sumComm + Convert.ToInt32(dr["BuyerCommVal"]);
            }

            ds.SumTable.AddSumTableRow(sum, sumComm);


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CustomerWiseFactoryComm.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;



        }




        //     [HttpGet]
        //     public ActionResult TotalExRSLComm()
        //     {
        //         return View();
        //     }

        //     [HttpPost]
        //     public ActionResult TotalExRSLComm(int date)
        //     {

        //         FactoryWiseExportValDS ds = new FactoryWiseExportValDS();

        //         var rowCounter = 0;

        //         var dictionary = new Dictionary<Tuple<int, int>, int>(); // <BuyerId,MonthId,Year>

        //         var dictCount = new Dictionary<Tuple<int, int>, int>();

        //         decimal Comm = 0;
        //         decimal Commission = 0;

        //         var buyers = (from buyMas in db.BuyerOrderMas
        //                       join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
        //                       join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
        //                       join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
        //                       where buyMas.OrderDate.Value.Year == date
        //                       select new { buyMas, factMas, buyDet, factDet }

        //                      ).ToList();


        //         foreach (var item in buyers)
        //         {

        //             Comm = ((item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0)) - (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));

        //             Commission = (Comm * 100) / ((item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0));


        //             var key = new Tuple<int, int>(item.buyMas.BuyerInfoId, item.buyMas.OrderDate.Value.Year);
        //             var value = rowCounter;





        //             if (dictionary.ContainsKey(key))
        //             {
        //                 int getRow = dictionary[key];

        //                 ds.CustomerFactExport[getRow].BuyerExpVal = ds.CustomerFactExport[getRow].BuyerExpVal + (item.buyDet.UnitPrice ?? 0) * (item.buyDet.Quantity ?? 0);
        //                 //ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
        //                 ds.CustomerFactExport[getRow].BuyerCommVal = ds.CustomerFactExport[getRow].BuyerCommVal + Comm;
        //                 //ds.CustomerExpComm[getRow].BuyerAvgComm =(( ds.CustomerExpComm[getRow].BuyerAvgComm + Commission) * ds.CustomerExpComm[getRow].Count-1)/ ds.CustomerExpComm[getRow].Count;
        //                 ds.CustomerFactExport[getRow].BuyerAvgComm = ds.CustomerFactExport[getRow].BuyerAvgComm + Commission;
        //                 ds.CustomerFactExport[getRow].Count = ds.CustomerFactExport[getRow].Count + 1;

        //             }

        //             else
        //             {
        //                 dictionary.Add(key, value);

        //                 ds.CustomerFactExport.AddCustomerFactExportRow(
        //                            item.factMas.SupplierId,
        //                            item.factMas.Supplier.Name,
        //                            (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
        //                            //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
        //                            Comm,
        //                            Commission,
        //                            1

        //                            );



        //                 rowCounter++;


        //             }


        //         }

        //         var sum = 0;
        //         var sumComm = 0;

        //         foreach (DataRow dr in ds.CustomerFactExport.Rows)
        //         {
        //             sum = sum + Convert.ToInt32(dr["BuyerExpVal"]);
        //             sumComm = sumComm + Convert.ToInt32(dr["BuyerCommVal"]);
        //         }

        //         ds.SumTable.AddSumTableRow(sum, sumComm);


        //         ReportDocument rd = new ReportDocument();

        //         rd.Load(Path.Combine(Server.MapPath("~/Reports"), "TotalExValRSLComm.rpt"));

        //         rd.SetDataSource(ds);

        //         Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
        //         MemoryStream ms = new MemoryStream();
        //         stream.CopyTo(ms);
        //         Byte[] fileBuffer = ms.ToArray();

        //         Response.Buffer = false;
        //         Response.ClearContent();
        //         Response.ClearHeaders();
        //         Response.ContentType = "application/pdf";
        //         Response.AddHeader("content-length", fileBuffer.Length.ToString());
        //         Response.BinaryWrite(fileBuffer);
        //         return null;



        //     }




        [HttpGet]
        public ActionResult BuyerWiseOrderAllocation()
        {

            var buyerList = (from exFactory in db.ExFactoryMas
                             join factory in db.Supplier on exFactory.SupplierId equals factory.Id
                             //where invComm.BuyerInfoId == buyer.Id
                             select new { exFactory.BuyerInfo.Name, exFactory.BuyerInfoId }).Distinct().ToList();

            List<BuyerInfo> buyersList = new List<BuyerInfo>();

            foreach (var i in buyerList)
            {
                var singleBuyer = db.BuyerInfo.FirstOrDefault(x => x.Id == i.BuyerInfoId);
                buyersList.Add(singleBuyer);
            }

            ViewBag.BuyInfoId = new SelectList(buyersList, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult BuyerWiseOrderAllocation(int? BuyInfoId, DateTime? DateFrom, DateTime? DateTo)
        {
            BuyerWiserOrderAllocation ds = new BuyerWiserOrderAllocation();

            var rowCounter = 0;

            var dictionary = new Dictionary<Tuple<int>, int>(); // <Buyer>                

            //var factory = (from buyMas in db.BuyerOrderMas
            //               join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //               join factMas in db.FactoryOrderMas on buyMas.Id equals factMas.BuyerOrderMasId
            //               join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //               join exFactoryDet in db.ExFactoryDet on buyMas.Id equals exFactoryDet.BuyerOrderMasId
            //               join exFactoryMas in db.ExFactoryMas on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
            //               join exFactoryShipDet in db.ExFactoryShipDet on new { a = exFactoryDet.Id, b = buyDet.Id } equals new { a = exFactoryShipDet.ExFactoryDetId, b = exFactoryShipDet.BuyerOrderDetId }
            //               //join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
            //               where buyMas.BuyerInfoId == exFactoryMas.BuyerInfoId && factMas.SupplierId == exFactoryMas.SupplierId
            //               select new { buyDet, factDet, exFactoryShipDet, exFactoryMas }
            //              ).Distinct().ToList();


            var factory = (from buyMas in db.BuyerOrderMas
                           join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                           join factMas in db.FactoryOrderMas on buyMas.Id equals factMas.BuyerOrderMasId
                           join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
                           join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                           join sub in (from exFactoryDet in db.ExFactoryDet
                                        join exFactoryMas in db.ExFactoryMas.Where(s => s.ExFactoryDate >= (DateFrom ?? DateTime.MinValue) && s.ExFactoryDate <= (DateTo ?? DateTime.MaxValue)) on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                        join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                        join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                        group new { buyerDet, exFactoryShipDet } by new
                                        {
                                            buyerDet.BuyerOrderMasId,
                                            buyerDet.Id,
                                            exFactoryMas.BuyerInfoId,
                                            exFactoryMas.SupplierId
                                        } into g
                                        select new
                                        {
                                            buyMasId = g.Key.BuyerOrderMasId,
                                            suppId = g.Key.SupplierId,
                                            BuyInfoId = g.Key.BuyerInfoId,
                                            ShipQty = g.Sum(x => x.exFactoryShipDet.ShipQuantity),
                                            buyDetId = g.Key.Id
                                            //exDate= db.ExFactoryMas.FirstOrDefault(x=>x.SupplierId== g.Key.SupplierId && x.BuyerInfoId==g.Key.BuyerInfoId).ExFactoryDate                                                                                
                                        }

                                        )
                                    on buyMas.Id equals sub.buyMasId


                           where buyMas.BuyerInfoId == sub.BuyInfoId && factMas.SupplierId == sub.suppId && buyDet.Id == sub.buyDetId
                           select new { buyDet, factDet, sub, factdelivDet }

                       ).Distinct().ToList();

            //if (BuyInfoId != null && DateFrom != null && DateTo != null)
            //{
            //    factory = factory.Where(x => x.exFactoryMas.BuyerInfoId == BuyInfoId && x.exFactoryMas.ExFactoryDate >= DateFrom && x.exFactoryMas.ExFactoryDate <= DateTo).ToList();
            //}
            //else if (BuyInfoId != null)
            //{
            //    factory = factory.Where(x => x.exFactoryMas.BuyerInfoId == BuyInfoId).ToList();
            //}

            //else if (DateFrom != null && DateTo != null)
            //{
            //    factory = factory.Where(x => x.exFactoryMas.ExFactoryDate >= DateFrom && x.exFactoryMas.ExFactoryDate <= DateTo).ToList();

            //}

            if (BuyInfoId != null)
            {
                factory = factory.Where(x => x.sub.BuyInfoId == BuyInfoId).ToList();
            }


            foreach (var item in factory)
            {



                var key = new Tuple<int>(item.buyDet.BuyerOrderMas.BuyerInfoId);
                var value = rowCounter;


                if (dictionary.ContainsKey(key))
                {
                    int getRow = dictionary[key];

                    ds.BuyerWiserOrder[getRow].OrderQty = ds.BuyerWiserOrder[getRow].OrderQty + item.buyDet.Quantity ?? 0;
                    ds.BuyerWiserOrder[getRow].OrderQtyValue = ds.BuyerWiserOrder[getRow].OrderQtyValue + (item.buyDet.UnitPrice * (item.buyDet.Quantity) ?? 0);
                    ds.BuyerWiserOrder[getRow].ShippedQty = ds.BuyerWiserOrder[getRow].ShippedQty + item.sub.ShipQty;
                    // ds.BuyerWiserOrder[getRow].ShippedQtyValue = ds.BuyerWiserOrder[getRow].ShippedQtyValue + (item.factDet.FOBUnitPrice ?? 0 * (item.sub.ShipQty));
                    ds.BuyerWiserOrder[getRow].ShippedQtyValue = ds.BuyerWiserOrder[getRow].ShippedQtyValue + (item.factdelivDet.FactFOB ?? 0 * (item.sub.ShipQty));

                }

                else
                {
                    dictionary.Add(key, value);

                    ds.BuyerWiserOrder.AddBuyerWiserOrderRow(
                               item.buyDet.BuyerOrderMas.BuyerInfoId,
                               item.buyDet.BuyerOrderMas.BuyerInfo.Name,
                               item.buyDet.Quantity ?? 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                               item.sub.ShipQty,
                               //(item.factDet.FOBUnitPrice ?? 0 * item.sub.ShipQty),
                               (item.factdelivDet.FactFOB ?? 0 * item.sub.ShipQty),
                               NullHelpers.DateToString(DateFrom),
                               NullHelpers.DateToString(DateTo)
                               );



                    rowCounter++;


                }


            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "BuyerWiseOrderAllocation.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;



        }




        ///////////////////// ------------- anis --------------------////////////////////////
        //     //////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        public ActionResult CustomerWiseExpComm()
        {

            var buyerName = (from invComm in db.InvoiceCommMas
                             join buyer in db.BuyerInfo on invComm.BuyerInfoId equals buyer.Id
                             //where invComm.BuyerInfoId == buyer.Id
                             select new { invComm.BuyerInfoId, invComm.BuyerInfo.Name }).Distinct().ToList();

            List<BuyerInfo> buyers = new List<BuyerInfo>();

            foreach (var i in buyerName)
            {
                var buyer = db.BuyerInfo.FirstOrDefault(x => x.Id == i.BuyerInfoId);
                buyers.Add(buyer);
            }

            ViewBag.BuyerId = new SelectList(buyers, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult CustomerWiseExpComm(int? BuyerId, DateTime? DateFrom, DateTime? DateTo)
        {


            CustomerWiseExpCommDS ds = new CustomerWiseExpCommDS();

            var rowCounter = 0;

            var dictionary = new Dictionary<Tuple<int>, int>(); // <BuyerId,Year>

            //var dictCount = new Dictionary<Tuple<int, int>, int>();

            decimal Comm = 0;
            decimal Commission = 0;

            //var buyers = (from buyMas in db.BuyerOrderMas
            //              join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //              join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
            //              join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //              where buyMas.OrderDate.Value.Year == date
            //              select new { buyMas, factMas, buyDet, factDet }

            //             ).ToList();


            //var buyers = (from FactoryInvoiceMas in db.InvoiceCommFactMas
            //              join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
            //              join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
            //              join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
            //              join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
            //              join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerDet.SupplierId, b = exFactoryDet.BuyerOrderMas.BuyerInfoId }
            //              equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
            //              join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exFactoryDet.Id }
            //              equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
            //              join factoryOrderDet in db.FactoryOrderDet on buyerDet.Id equals factoryOrderDet.BuyerOrderDetId
            //              join InvoiceCommDet in db.InvoiceCommDet on FactoryInvoiceMas.Id equals InvoiceCommDet.InvoiceCommFactMasId
            //              join InvoiceCommMas in db.InvoiceCommMas on InvoiceCommDet.InvoiceCommMasId equals InvoiceCommMas.Id
            //              where invCommFactMas.BuyerInfoId == InvoiceCommMas.BuyerInfoId
            //              // where invCommFactMas.IssueDate.Year == date    
            //              select new
            //              {
            //                  buyerDet,
            //                  factoryOrderDet,
            //                  invCommFactMas,
            //                  exFactoryShipDet,
            //                  InvoiceCommMas
            //              }).Distinct().ToList();

            var buyers = (from FactoryInvoiceMas in db.InvoiceCommFactMas
                          join FactoryInvoiceDet in db.InvoiceCommFactDet on FactoryInvoiceMas.Id equals FactoryInvoiceDet.InvoiceCommFactMasId
                          join exFactoryDet in db.ExFactoryDet on FactoryInvoiceDet.ExFactoryDetId equals exFactoryDet.Id
                          join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                          join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                          join invCommFactMas in db.InvoiceCommFactMas on new { a = buyerDet.SupplierId, b = exFactoryDet.BuyerOrderMas.BuyerInfoId }
                          equals new { a = invCommFactMas.SupplierId, b = invCommFactMas.BuyerInfoId }
                          join invCommFactDet in db.InvoiceCommFactDet on new { a = invCommFactMas.Id, b = exFactoryDet.Id }
                          equals new { a = invCommFactDet.InvoiceCommFactMasId, b = invCommFactDet.ExFactoryDetId }
                          join factoryOrderDet in db.FactoryOrderDet on buyerDet.Id equals factoryOrderDet.BuyerOrderDetId
                          join InvoiceCommDet in db.InvoiceCommDet on FactoryInvoiceMas.Id equals InvoiceCommDet.InvoiceCommFactMasId
                          join InvoiceCommMas in db.InvoiceCommMas on InvoiceCommDet.InvoiceCommMasId equals InvoiceCommMas.Id
                          join factdelivDet in db.FactoryOrderDelivDet on factoryOrderDet.Id equals factdelivDet.FactoryOrderDetId
                          where invCommFactMas.BuyerInfoId == InvoiceCommMas.BuyerInfoId && exFactoryShipDet.ShipmentSummDetId == factdelivDet.ShipmentSummDetId
                          // where invCommFactMas.IssueDate.Year == date    
                          select new
                          {
                              buyerDet,
                              factoryOrderDet,
                              invCommFactMas,
                              exFactoryShipDet,
                              InvoiceCommMas,
                              factdelivDet
                          }).Distinct().ToList();



            if (BuyerId != null && DateFrom != null && DateTo != null)
            {
                buyers = buyers.Where(x => x.InvoiceCommMas.BuyerInfoId == BuyerId && x.InvoiceCommMas.IssueDate >= DateFrom && x.InvoiceCommMas.IssueDate <= DateTo).ToList();
            }


            else if (BuyerId != null)
            {
                buyers = buyers.Where(x => x.InvoiceCommMas.BuyerInfoId == BuyerId).ToList();
            }




            foreach (var item in buyers)
            {

                Comm = ((item.buyerDet.UnitPrice ?? 0) * (item.exFactoryShipDet.ShipQuantity)) - (item.factdelivDet.FactFOB ?? 0 * (item.exFactoryShipDet.ShipQuantity));

                Commission = (Comm * 100) / ((item.buyerDet.UnitPrice ?? 0) * (item.exFactoryShipDet.ShipQuantity));


                var key = new Tuple<int>(item.invCommFactMas.BuyerInfoId);
                var value = rowCounter;

                //var Commkey = new Tuple<int, int>(item.buyMas.BuyerInfoId, item.buyMas.OrderDate.Value.Year);
                //var Commvalue = rowCounter;



                //if (dictionary.ContainsKey(Commkey))
                //{
                //    int getRow = dictionary[Commkey];

                //    ds.CustomerExpComm[getRow].BuyerAvgComm = ds.CustomerExpComm[getRow].BuyerAvgComm + Commission;
                //   // ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));

                //}

                //else
                //{
                //    dictionary.Add(Commkey, Commvalue);

                //    ds.CustomerExpComm.AddCustomerExpCommRow(
                //               item.buyMas.BuyerInfoId,
                //               item.buyMas.BuyerInfo.Name,
                //               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                //               (item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                //               Commission
                //               );



                //    rowCounterComm++;
                //}




                if (dictionary.ContainsKey(key))
                {
                    int getRow = dictionary[key];

                    ds.CustomerExpComm[getRow].BuyerExpVal = ds.CustomerExpComm[getRow].BuyerExpVal + (item.buyerDet.UnitPrice ?? 0) * (item.exFactoryShipDet.ShipQuantity);
                    //ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));
                    ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + Comm;
                    //ds.CustomerExpComm[getRow].BuyerAvgComm =(( ds.CustomerExpComm[getRow].BuyerAvgComm + Commission) * ds.CustomerExpComm[getRow].Count-1)/ ds.CustomerExpComm[getRow].Count;
                    ds.CustomerExpComm[getRow].BuyerAvgComm = ds.CustomerExpComm[getRow].BuyerAvgComm + Commission;
                    ds.CustomerExpComm[getRow].Count = ds.CustomerExpComm[getRow].Count + 1;

                }

                else
                {
                    dictionary.Add(key, value);

                    ds.CustomerExpComm.AddCustomerExpCommRow(
                               item.invCommFactMas.BuyerInfoId,
                               item.invCommFactMas.BuyerInfo.Name,
                               (item.buyerDet.UnitPrice * item.exFactoryShipDet.ShipQuantity) ?? 0,
                               //(item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                               Comm,
                               Commission,
                               1
                               );

                    rowCounter++;


                }


            }

            var sum = 0;
            var sumComm = 0;

            foreach (DataRow dr in ds.CustomerExpComm.Rows)
            {
                sum = sum + Convert.ToInt32(dr["BuyerExpVal"]);
                sumComm = sumComm + Convert.ToInt32(dr["BuyerCommVal"]);
            }

            ds.SumTable.AddSumTableRow(sum, sumComm, NullHelpers.DateToString(DateFrom),
                                                          NullHelpers.DateToString(DateTo));


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CustomerWiseExpComm.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;



        }


        //     //////////////////------------------------- New one -------------------


        [HttpGet]
        public ActionResult FactoryWiseOrderAllocation()
        {

            var factoryName = (from exFactory in db.ExFactoryMas
                               join factory in db.Supplier on exFactory.SupplierId equals factory.Id
                               //where invComm.BuyerInfoId == buyer.Id
                               select new { exFactory.Supplier.Name, exFactory.SupplierId }).Distinct().ToList();

            List<Supplier> factories = new List<Supplier>();

            foreach (var i in factoryName)
            {
                var factory = db.Supplier.FirstOrDefault(x => x.Id == i.SupplierId);
                factories.Add(factory);
            }

            ViewBag.FactoryId = new SelectList(factories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult FactoryWiseOrderAllocation(int? FactoryId, DateTime? DateFrom, DateTime? DateTo)
        {


            FactoryWiserOrderAllocation ds = new FactoryWiserOrderAllocation();

            var rowCounter = 0;

            var dictionary = new Dictionary<Tuple<int, int>, int>(); // <Factory,Buyer>            


            //var factory = (from buyMas in db.BuyerOrderMas
            //               join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //               join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
            //               join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //               join exFactoryDet in db.ExFactoryDet on buyMas.Id equals exFactoryDet.BuyerOrderMasId
            //               join exFactoryMas in db.ExFactoryMas on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
            //               join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
            //               //join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
            //               where buyMas.BuyerInfoId == exFactoryMas.BuyerInfoId && factMas.SupplierId == exFactoryMas.SupplierId
            //                && exFactoryShipDet.BuyerOrderDetId == buyDet.Id
            //               select new { buyMas, factMas, buyDet, factDet, exFactoryShipDet, exFactoryMas }

            //             ).Distinct().ToList();


            /////////////----------------- last ------------------

            //var factory = (from buyMas in db.BuyerOrderMas
            //               join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //               join factMas in db.FactoryOrderMas on buyMas.Id equals factMas.BuyerOrderMasId
            //               join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //               join exFactoryDet in db.ExFactoryDet on buyMas.Id equals exFactoryDet.BuyerOrderMasId
            //               join exFactoryMas in db.ExFactoryMas on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
            //               join exFactoryShipDet in db.ExFactoryShipDet on new { a = exFactoryDet.Id, b = buyDet.Id } equals new { a = exFactoryShipDet.ExFactoryDetId, b = exFactoryShipDet.BuyerOrderDetId }
            //               //join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
            //               where buyMas.BuyerInfoId == exFactoryMas.BuyerInfoId && factMas.SupplierId == exFactoryMas.SupplierId
            //               select new { buyDet, factDet, exFactoryShipDet, exFactoryMas }

            //             ).Distinct().ToList();


            /////////////////////////////////////

            //var exFactMasFilterd = db.ExFactoryMas.ToList();


            //if (DateFrom != null && DateTo != null)
            //{
            //    //exFactMasFilterd = exFactMasFilterd.Where(x => x.ExFactoryDate >= (DateFrom ?? DateTime.MinValue) && x.ExFactoryDate <= (DateTo ?? DateTime.MaxValue));
            //    exFactMasFilterd = exFactMasFilterd.Where(x => x.ExFactoryDate == DateTime.Today).ToList();
            //}






            var factory = (from buyMas in db.BuyerOrderMas
                           join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
                           join factMas in db.FactoryOrderMas on buyMas.Id equals factMas.BuyerOrderMasId
                           join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
                           join factdelivDet in db.FactoryOrderDelivDet on factDet.Id equals factdelivDet.FactoryOrderDetId
                           join sub in (from exFactoryDet in db.ExFactoryDet
                                        join exFactoryMas in db.ExFactoryMas.Where(s => s.ExFactoryDate >= (DateFrom ?? DateTime.MinValue) && s.ExFactoryDate <= (DateTo ?? DateTime.MaxValue)) on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
                                        join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                        join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
                                        group new { buyerDet, exFactoryShipDet } by new
                                        {
                                            buyerDet.BuyerOrderMasId,
                                            buyerDet.Id,
                                            exFactoryMas.BuyerInfoId,
                                            exFactoryMas.SupplierId
                                        } into g
                                        select new
                                        {

                                            buyMasId = g.Key.BuyerOrderMasId,
                                            suppId = g.Key.SupplierId,
                                            BuyInfoId = g.Key.BuyerInfoId,
                                            ShipQty = g.Sum(x => x.exFactoryShipDet.ShipQuantity),
                                            //exDate = g.Key.ExFactoryDate,
                                            buyDetId = g.Key.Id
                                        }

                                        )
                                    on buyMas.Id equals sub.buyMasId
                           where buyMas.BuyerInfoId == sub.BuyInfoId && factMas.SupplierId == sub.suppId && buyDet.Id == sub.buyDetId
                           select new { buyDet, factDet, sub, factdelivDet }

                         ).Distinct().ToList();






            //var factory = (from buyMas in db.BuyerOrderMas
            //               join buyDet in db.BuyerOrderDet on buyMas.Id equals buyDet.BuyerOrderMasId
            //               join factMas in db.FactoryOrderMas on new { a = buyMas.Id, b = buyDet.SupplierId } equals new { a = factMas.BuyerOrderMasId, b = factMas.SupplierId }
            //               join factDet in db.FactoryOrderDet on new { a = buyDet.Id, b = factMas.Id } equals new { a = factDet.BuyerOrderDetId, b = factDet.FactoryOrderMasId }
            //               join exFactoryDet in db.ExFactoryDet on buyMas.Id equals exFactoryDet.BuyerOrderMasId
            //               join exFactoryMas in db.ExFactoryMas on exFactoryDet.ExFactoryMasId equals exFactoryMas.Id
            //               //join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
            //               //join buyerDet in db.BuyerOrderDet on exFactoryShipDet.BuyerOrderDetId equals buyerDet.Id
            //               where buyMas.BuyerInfoId == exFactoryMas.BuyerInfoId && factMas.SupplierId == exFactoryMas.SupplierId
            //               select new { buyMas, factMas, buyDet, factDet,exFactoryMas, exFactoryDet }

            //             ).Distinct().ToList();






            //if (FactoryId != null && DateFrom != null && DateTo != null)
            //{
            //    factory = factory.Where(x => x.sub.suppId == FactoryId && x.sub.exDate >= DateFrom && x.sub.exDate <= DateTo).ToList();
            //}
            //else if (FactoryId != null)
            //{
            //    factory = factory.Where(x => x.sub.suppId == FactoryId).ToList();
            //}

            //else if (DateFrom != null && DateTo != null)
            //{
            //    factory = factory.Where(x => x.sub.exDate >= DateFrom && x.sub.exDate <= DateTo).ToList();

            //}





            if (FactoryId != null)
            {
                factory = factory.Where(x => x.sub.suppId == FactoryId).ToList();
            }




            foreach (var item in factory)
            {



                var key = new Tuple<int, int>(item.buyDet.SupplierId, item.sub.BuyInfoId);
                var value = rowCounter;

                //var Commkey = new Tuple<int, int>(item.buyMas.BuyerInfoId, item.buyMas.OrderDate.Value.Year);
                //var Commvalue = rowCounter;



                //if (dictionary.ContainsKey(Commkey))
                //{
                //    int getRow = dictionary[Commkey];

                //    ds.CustomerExpComm[getRow].BuyerAvgComm = ds.CustomerExpComm[getRow].BuyerAvgComm + Commission;
                //   // ds.CustomerExpComm[getRow].BuyerCommVal = ds.CustomerExpComm[getRow].BuyerCommVal + (item.factDet.FOBUnitPrice * (item.buyDet.Quantity ?? 0));

                //}

                //else
                //{
                //    dictionary.Add(Commkey, Commvalue);

                //    ds.CustomerExpComm.AddCustomerExpCommRow(
                //               item.buyMas.BuyerInfoId,
                //               item.buyMas.BuyerInfo.Name,
                //               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                //               (item.factDet.FOBUnitPrice * item.buyDet.Quantity) ?? 0,
                //               Commission
                //               );



                //    rowCounterComm++;
                //}




                if (dictionary.ContainsKey(key))
                {
                    int getRow = dictionary[key];

                    ds.FactoryWiserOrder[getRow].OrderQty = ds.FactoryWiserOrder[getRow].OrderQty + item.buyDet.Quantity ?? 0;
                    ds.FactoryWiserOrder[getRow].OrderQtyValue = ds.FactoryWiserOrder[getRow].OrderQtyValue + (item.buyDet.UnitPrice * (item.buyDet.Quantity) ?? 0);
                    ds.FactoryWiserOrder[getRow].ShippedQty = ds.FactoryWiserOrder[getRow].ShippedQty + item.sub.ShipQty;
                    //ds.FactoryWiserOrder[getRow].ShippedQtyValue = ds.FactoryWiserOrder[getRow].ShippedQtyValue + (item.factDet.FOBUnitPrice ?? 0 * (item.sub.ShipQty));
                    ds.FactoryWiserOrder[getRow].ShippedQtyValue = ds.FactoryWiserOrder[getRow].ShippedQtyValue + (item.factdelivDet.FactFOB ?? 0 * (item.sub.ShipQty));

                }

                else
                {
                    dictionary.Add(key, value);

                    ds.FactoryWiserOrder.AddFactoryWiserOrderRow(
                               item.buyDet.SupplierId,
                               item.buyDet.Supplier.Name,
                               item.buyDet.BuyerOrderMas.BuyerInfoId,
                               item.buyDet.BuyerOrderMas.BuyerInfo.Name,
                               item.buyDet.Quantity ?? 0,
                               (item.buyDet.UnitPrice * item.buyDet.Quantity) ?? 0,
                               item.sub.ShipQty,
                               //(item.factDet.FOBUnitPrice ?? 0 * item.sub.ShipQty),
                               (item.factdelivDet.FactFOB ?? 0 * item.sub.ShipQty),
                               NullHelpers.DateToString(DateFrom),
                               NullHelpers.DateToString(DateTo)
                               );



                    rowCounter++;


                }


            }


            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "FactoryWiseOrderAllocation.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;



        }
                


        [HttpGet]
        public ActionResult FactAccDiscount()
        {
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult FactAccDiscount(int SupplierId, DateTime DateFrom, DateTime DateTo)
        {
            FactoryAccountDS ds = new FactoryAccountDS();

            var supplier = db.Supplier.SingleOrDefault(x => x.Id == SupplierId);

            ds.Supplier.AddSupplierRow(supplier.Id, supplier.Name, supplier.OpeningBalance ?? 0, supplier.BalanceDate ?? DateTime.Now);


            var discountdata = (from discountMas in db.DiscountMas
                                join discountDet in db.DiscountDet on discountMas.Id equals discountDet.DiscountMasId
                                where discountMas.BuyerOrderDet.SupplierId == SupplierId && discountMas.DiscountDate >= supplier.BalanceDate && discountMas.DiscountDate <= DateTo
                                select new
                                {
                                    discountDet,
                                    discountMas
                                }).ToList();

            var refinediscountdata = discountdata.AsEnumerable().Select(x => new
            {
                DiscountDate = x.discountMas.DiscountDate ?? DateTime.Now,
                BuyerMasId = x.discountMas.BuyerOrderDet.BuyerOrderMasId,
                BuyerRefNo = x.discountMas.BuyerOrderDet.BuyerOrderMas.OrderRefNo,
                BuyerOrderDetId = x.discountMas.BuyerOrderDetId,
                StyleNo = x.discountMas.BuyerOrderDet.StyleNo,
                //Quantity = db.BuyerOrderDet.Where(m => m.Id == x.discountMas.BuyerOrderDetId).Sum(k => k.Quantity),
                Quantity = db.ShipmentSummDet.Where(m => m.BuyerOrderDetId == x.discountMas.BuyerOrderDetId).Sum(k => k.DelivQuantity),
                DiscountAmount = (from discountDet in db.DiscountDet
                                 join factoryDelivery in db.FactoryOrderDelivDet on discountDet.FactoryOrderDelivDetId equals factoryDelivery.Id
                                 join buyerShip in db.ShipmentSummDet on factoryDelivery.ShipmentSummDetId equals buyerShip.Id
                                 where discountDet.DiscountMasId == x.discountMas.Id
                                 select new { DiscountValue = (buyerShip.DelivQuantity * factoryDelivery .FactFOB)*(x.discountDet.FactoryDiscount / 100)}).Sum(k=>k.DiscountValue) ??0
                //db.DiscountDet.Where(m => m.DiscountMasId == x.discountMas.Id).Sum(k => k.FactoryDiscount)
            }).Distinct().ToList();

            var balance = (decimal)0;

            foreach (var item in refinediscountdata)
            {
                var cash = (decimal)0;
                if (ds.FactoryAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.DiscountAmount;

                    ds.FactoryAccount.AddFactoryAccountRow(
                    item.DiscountDate, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount, "", "", "", 0, 0, balance, 0, cash);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.DiscountAmount;

                    ds.FactoryAccount.AddFactoryAccountRow(
                    item.DiscountDate, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount, "", "", "", 0, 0, balance, 0, cash);
                }

            }


            var adjustData = (from discountadjustMas in db.DiscountAdjustmentFactoryMas
                              join discountadjustFact in db.DiscountAdjustmentFactoryAdj on discountadjustMas.Id equals discountadjustFact.DiscountAdjustmentFactoryMasId
                              join discountadjDet in db.DiscountAdjustmentFactoryDet on discountadjustFact.Id equals discountadjDet.DiscountAdjustmentFactoryAdjId
                              join factoryDet in db.InvoiceCommFactDet on discountadjustFact.BuyerOrderMasId equals factoryDet.ExFactoryDet.BuyerOrderMasId
                              where discountadjustMas.SupplierId == SupplierId && discountadjustMas.DateAdj >= supplier.BalanceDate && discountadjustMas.DateAdj <= DateTo
                              select new
                              {
                                  discountadjustMas,
                                  discountadjustFact,
                                  discountadjDet,
                                  factoryDet
                              }).ToList();




            var refineadjustData = adjustData.AsEnumerable().Select(x => new
            {
                FactInvoiceNo = x.factoryDet.InvoiceCommFactMas.InvoiceNoFact,
                AdjDate = x.discountadjustMas.DateAdj,
                BuyerMasId = x.discountadjustFact.BuyerOrderMasId,
                BuyerRefNo = x.discountadjustFact.BuyerOrderMas.OrderRefNo,
                BuyerOrderDetId = x.discountadjDet.FactoryOrderDelivDet.FactoryOrderDet.BuyerOrderDetId,
                StyleNo = x.discountadjDet.FactoryOrderDelivDet.FactoryOrderDet.BuyerOrderDet.StyleNo,
                //Quantity = db.BuyerOrderDet.Where(m => m.Id == x.discountadjDet.FactoryOrderDelivDet.FactoryOrderDet.BuyerOrderDetId).Sum(k => k.Quantity),
                Quantity = db.ShipmentSummDet.Where(m => m.BuyerOrderDetId == x.discountadjDet.FactoryOrderDelivDet.FactoryOrderDet.BuyerOrderDetId).Sum(k => k.DelivQuantity),
                //DiscountAmount = db.DiscountAdjustmentFactoryDet.Where(m => m.DiscountAdjustmentFactoryAdjId == x.discountadjustFact.Id).Sum(k => k.AdjustmentAmt) ?? 0
                DiscountAmount = db.DiscountAdjustmentFactoryDet.SingleOrDefault(m => m.DiscountAdjustmentFactoryAdjId == x.discountadjustFact.Id && m.FactoryOrderDelivDetId==x.discountadjDet.FactoryOrderDelivDetId).AdjustmentAmt
            }).Distinct().ToList();


            foreach (var item in refineadjustData)
            {
                var cash = (decimal)0;
                if (ds.FactoryAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.DiscountAmount ;

                    ds.FactoryAccount.AddFactoryAccountRow(item.AdjDate, "", "", 0, 0, item.FactInvoiceNo, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount ?? 0, balance, 1, cash);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.DiscountAmount;
                    ds.FactoryAccount.AddFactoryAccountRow(item.AdjDate, "", "", 0, 0, item.FactInvoiceNo, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount ?? 0, balance, 1, cash);
                }

            }


            var cashData = db.FactoryCashAdjustment.Where(x => x.SupplierId == SupplierId && x.FacAdjustDate >= supplier.BalanceDate && x.FacAdjustDate <= DateTo).ToList();

            foreach (var item in cashData)
            {
                var cash = (decimal)0;
                if (ds.FactoryAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.FacAdjustAmount;

                    ds.FactoryAccount.AddFactoryAccountRow(item.FacAdjustDate, "", "", 0, 0, "", "", "", 0, item.FacAdjustAmount, balance, 2, item.FacAdjustAmount);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.FacAdjustAmount;
                    ds.FactoryAccount.AddFactoryAccountRow(item.FacAdjustDate, "", "", 0, 0, "", "", "", 0, item.FacAdjustAmount, balance, 2, item.FacAdjustAmount);
                }

            }


            IEnumerable<DataRow> orderedRows = ds.FactoryAccount.AsEnumerable().OrderBy(r => r.Field<DateTime>("Date"));

            var MainBalance = (decimal)0;
            var rowCounter = 0;

            foreach (var dr in orderedRows)
            {
                if ((DateTime)dr["Date"] >= DateFrom && (DateTime)dr["Date"] <= DateTo)
                {
                    if (ds.OrderedFactoryAccount.Rows.Count > 0)
                    {
                        var length = ds.OrderedFactoryAccount.Rows.Count;
                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = ds.OrderedFactoryAccount[length - 1].Balance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = ds.OrderedFactoryAccount[length - 1].Balance - (decimal)dr["AdjAmt"];
                        }

                    }
                    else
                    {
                        if (rowCounter == 0)
                        {
                            MainBalance = supplier.OpeningBalance ?? 0;
                        }
                        else
                        {
                            ds.Supplier[0].OpeningDate = (DateTime)dr["Date"];
                            ds.Supplier[0].OpeningBalance = MainBalance;
                        }

                        if ((int)dr["RowOrder"] == 0)
                        {

                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];
                       
                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }
                      
                    }

                    ds.OrderedFactoryAccount.AddOrderedFactoryAccountRow(
                        (DateTime)dr["Date"],
                        dr["DiscountRef"].ToString(),
                        dr["DiscountStyle"].ToString(),
                        (decimal)dr["DiscountQty"],
                        (decimal)dr["DiscountAmt"],
                        dr["AdjInvoice"].ToString(),
                        dr["AdjRef"].ToString(),
                        dr["AdjStyle"].ToString(),
                        (decimal)dr["AdjQty"],
                        (decimal)dr["AdjAmt"],
                        MainBalance,
                        (int)dr["RowOrder"],
                        (decimal)dr["CashAmt"]
                        );
                }
                else
                {
                    if(rowCounter==0)
                    {
                        MainBalance = supplier.OpeningBalance ?? 0;

                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }
                    }
                    else
                    {
                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }
                    }


                }

                rowCounter++;
            }

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "FactAccDiscount.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }

        //Nabid
        [HttpGet]
        public ActionResult BuyerAccDiscount()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult BuyerAccDiscount(int BuyerInfoId, DateTime DateFrom, DateTime DateTo)
        {

            
            BuyerAccountDS ds = new BuyerAccountDS();

            var buyer = db.BuyerInfo.SingleOrDefault(x => x.Id == BuyerInfoId);

            ds.Buyer.AddBuyerRow(buyer.Id, buyer.Name, buyer.OpeningBalance ?? 0, buyer.BalanceDate ?? DateTime.Now);


            var discountdata = (from discountMas in db.DiscountMas
                                join discountDet in db.DiscountDet on discountMas.Id equals discountDet.DiscountMasId
                                where discountMas.BuyerOrderDet.BuyerOrderMas.BuyerInfoId == BuyerInfoId && discountMas.DiscountDate >= buyer.BalanceDate && discountMas.DiscountDate <= DateTo
                                select new
                                {
                                    discountDet,
                                    discountMas
                                }).ToList();

            var refinediscountdata = discountdata.AsEnumerable().Select(x => new
            {
                DiscountDate = x.discountMas.DiscountDate ?? DateTime.Now,
                BuyerMasId = x.discountMas.BuyerOrderDet.BuyerOrderMasId,
                BuyerRefNo = x.discountMas.BuyerOrderDet.BuyerOrderMas.OrderRefNo,
                BuyerOrderDetId = x.discountMas.BuyerOrderDetId,
                StyleNo = x.discountMas.BuyerOrderDet.StyleNo,
                Quantity = db.BuyerOrderDet.Where(m => m.Id == x.discountMas.BuyerOrderDetId).Sum(k => k.Quantity),
                //DiscountAmount = db.DiscountDet.Where(m => m.DiscountMasId == x.discountMas.Id).Sum(k => k.FactoryDiscount),
                DiscountAmount = (from discountDet in db.DiscountDet
                                  join factoryDelivery in db.FactoryOrderDelivDet on discountDet.FactoryOrderDelivDetId equals factoryDelivery.Id
                                  join buyerShip in db.ShipmentSummDet on factoryDelivery.ShipmentSummDetId equals buyerShip.Id
                                  where discountDet.DiscountMasId == x.discountMas.Id
                                  select new { DiscountValue = (buyerShip.DelivQuantity * buyerShip.BuyerOrderDet.UnitPrice) * (x.discountDet.BuyerDiscount / 100) }).Sum(k => k.DiscountValue) ?? 0

            }).Distinct().ToList();

            var balance = (decimal)0;

            foreach (var item in refinediscountdata)
            {
                var cash = (decimal)0;
                if (ds.BuyerAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.DiscountAmount;

                    ds.BuyerAccount.AddBuyerAccountRow(
                    item.DiscountDate, item.BuyerRefNo, item.StyleNo, item.Quantity ?? 0, item.DiscountAmount, "", "", "", 0, 0, balance, 0, cash);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.DiscountAmount;

                    ds.BuyerAccount.AddBuyerAccountRow(
                    item.DiscountDate, item.BuyerRefNo, item.StyleNo, item.Quantity ?? 0, item.DiscountAmount, "", "", "", 0, 0, balance, 0, cash);
                }

            }


            var adjustData = (from discountadjustMas in db.DiscountAdjustmentBuyerMas
                              join discountadjustAdj in db.DiscountAdjustmentBuyerAdj on discountadjustMas.Id equals discountadjustAdj.DiscountAdjustmentBuyerMasId
                              join discountadjDet in db.DiscountAdjustmentBuyerDet on discountadjustAdj.Id equals discountadjDet.DiscountAdjustmentBuyerAdjId
                              join factoryDet in db.InvoiceCommFactDet on discountadjustAdj.BuyerOrderMasId equals factoryDet.ExFactoryDet.BuyerOrderMasId
                              join invCommDet in db.InvoiceCommDet on factoryDet.InvoiceCommFactMasId equals invCommDet.InvoiceCommFactMasId
                              where discountadjustMas.BuyerInfoId == BuyerInfoId && discountadjustMas.DateAdj >= buyer.BalanceDate && discountadjustMas.DateAdj <= DateTo
                              select new
                              {
                                  discountadjustMas,
                                  discountadjustAdj,
                                  discountadjDet,
                                  factoryDet,
                                  invCommDet
                              }).ToList();




            var refineadjustData = adjustData.AsEnumerable().Select(x => new
            {
                FactInvoiceNo = x.invCommDet.InvoiceCommMas.InvoiceNo,
                // FactInvoiceNo = db.InvoiceCommDet.Where(p=>p.InvoiceCommFactMasId == x.factoryDet.InvoiceCommFactMasId).SingleOrDefault().InvoiceCommMas.InvoiceNo,
                AdjDate = x.discountadjustMas.DateAdj,
                BuyerMasId = x.discountadjustAdj.BuyerOrderMasId,
                BuyerRefNo = x.discountadjustAdj.BuyerOrderMas.OrderRefNo,
                BuyerOrderDetId = x.discountadjDet.ExFactoryShipDet.BuyerOrderDetId,
                StyleNo = x.discountadjDet.ExFactoryShipDet.BuyerOrderDet.StyleNo,
                Quantity = db.ShipmentSummDet.Where(m => m.Id == x.discountadjDet.ExFactoryShipDet.ShipmentSummDetId).Sum(k => k.DelivQuantity),
                DiscountAmount = x.discountadjDet.AdjustmentAmt

                // DiscountAmount = db.DiscountAdjustmentBuyerDet.Where(m => m.DiscountAdjustmentBuyerAdjId == x.discountadjustMas.Id).Sum(k => k.AdjustmentAmt) ?? 0
            }).Distinct().ToList();


            foreach (var item in refineadjustData)
            {
                var cash = (decimal)0;
                if (ds.BuyerAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.DiscountAmount ;

                    ds.BuyerAccount.AddBuyerAccountRow(item.AdjDate ?? DateTime.Now, "", "", 0, 0, item.FactInvoiceNo, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount ?? 0, balance, 1, cash);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.DiscountAmount;
                    ds.BuyerAccount.AddBuyerAccountRow(item.AdjDate ?? DateTime.Now, "", "", 0, 0, item.FactInvoiceNo, item.BuyerRefNo, item.StyleNo, item.Quantity, item.DiscountAmount ?? 0, balance, 1, cash);
                }

            }


            var cashData = db.BuyerCashAdjustment.Where(x => x.BuyerInfoId == BuyerInfoId && x.BuyerAdjustDate >= buyer.BalanceDate && x.BuyerAdjustDate <= DateTo).ToList();

            foreach (var item in cashData)
            {
                //var cash = (decimal)0;
                if (ds.BuyerAccount.Rows.Count > 0)
                {
                    //var length = ds.FactoryAccount.Rows.Count;
                    //balance = ds.FactoryAccount[length - 1].Balance - item.FacAdjustAmount;

                    ds.BuyerAccount.AddBuyerAccountRow(item.BuyerAdjustDate, "", "", 0, 0, "", "", "", 0, item.BuyerAdjustAmount, balance, 2, item.BuyerAdjustAmount);
                }
                else
                {
                    //balance = (supplier.OpeningBalance ?? 0) - item.FacAdjustAmount;
                    ds.BuyerAccount.AddBuyerAccountRow(item.BuyerAdjustDate, "", "", 0, 0, "", "", "", 0, item.BuyerAdjustAmount, balance, 2, item.BuyerAdjustAmount);
                }

            }


            IEnumerable<DataRow> orderedRows = ds.BuyerAccount.AsEnumerable().OrderBy(r => r.Field<DateTime>("Date"));

            var MainBalance = (decimal)0;
            var rowCounter = 0;
            foreach (var dr in orderedRows)
            {
                if ((DateTime)dr["Date"] >= DateFrom && (DateTime)dr["Date"] <= DateTo)
                {
                    if (ds.OrderedBuyerAccount.Rows.Count > 0)
                    {
                        var length = ds.OrderedBuyerAccount.Rows.Count;
                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = ds.OrderedBuyerAccount[length - 1].Balance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = ds.OrderedBuyerAccount[length - 1].Balance - (decimal)dr["AdjAmt"];
                        }

                    }
                    else
                    {
                        if (rowCounter == 0)
                        {
                            MainBalance = buyer.OpeningBalance ?? 0;
                        }
                        else
                        {
                            ds.Buyer[0].OpeningDate = (DateTime)dr["Date"];
                            ds.Buyer[0].OpeningBalance = MainBalance;
                        }

                        if ((int)dr["RowOrder"] == 0)
                        {

                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];

                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }

                    }

                    ds.OrderedBuyerAccount.AddOrderedBuyerAccountRow(
                        (DateTime)dr["Date"],
                        dr["DiscountRef"].ToString(),
                        dr["DiscountStyle"].ToString(),
                        (decimal)dr["DiscountQty"],
                        (decimal)dr["DiscountAmt"],
                        dr["AdjInvoice"].ToString(),
                        dr["AdjRef"].ToString(),
                        dr["AdjStyle"].ToString(),
                        (decimal)dr["AdjQty"],
                        (decimal)dr["AdjAmt"],
                        MainBalance,
                        (int)dr["RowOrder"],
                        (decimal)dr["CashAmt"]
                        );
                }
                else
                {
                    if (rowCounter == 0)
                    {
                        MainBalance = buyer.OpeningBalance ?? 0;

                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }
                    }
                    else
                    {
                        if ((int)dr["RowOrder"] == 0)
                        {
                            MainBalance = MainBalance + (decimal)dr["DiscountAmt"];
                        }
                        else
                        {
                            MainBalance = MainBalance - (decimal)dr["AdjAmt"];
                        }
                    }


                }

                rowCounter++;
            }

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "BuyerAccDiscount.rpt"));

            rd.SetDataSource(ds);

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            Byte[] fileBuffer = ms.ToArray();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", fileBuffer.Length.ToString());
            Response.BinaryWrite(fileBuffer);
            return null;

        }





    }
}