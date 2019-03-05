using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHMS.Models;
using BHMS.Helpers;
using BHMS.ViewModels;

namespace BHMS.Controllers
{
    public class InvoiceCommFactController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: InvoiceCommFact
        public ActionResult Index()
        {
            var invoiceCommFactMas = db.InvoiceCommFactMas.Include(i => i.BuyerInfo).Include(i => i.Supplier);
            return View(invoiceCommFactMas.ToList());
        }

        // GET: InvoiceCommFact/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceCommFactMas invoiceCommFactMas = db.InvoiceCommFactMas.Find(id);
            if (invoiceCommFactMas == null)
            {
                return HttpNotFound();
            }
            return View(invoiceCommFactMas);
        }

        // GET: InvoiceCommFact/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }

        public ActionResult CreatePrev()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name");
            return View();
        }

        public JsonResult GetDetailData(int SupplierId, int BuyerInfoId, DateTime ExFactoryDate)
        {
            var data = (from exFactoryMas in db.ExFactoryMas
                        join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                        join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                        join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                        where exFactoryMas.SupplierId == SupplierId && exFactoryMas.BuyerInfoId == BuyerInfoId && exFactoryMas.ExFactoryDate == ExFactoryDate
                        select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();


            var refinedata = data.AsEnumerable().Select(x => new
            {
                //ExFactoryShipDetId = x.exFactoryShipDet.Id,
                ExFactoryDetId = x.exFactoryDet.Id,
                OrderMasId = x.buyerOrder.Id,
                OrderRefNo = x.buyerOrder.OrderRefNo,
                //ShipQuantity = data.Where(m=>m.buyerOrder.Id==x.buyerOrder.Id).Select(k=>k.exFactoryDet.ShipQuantity).Sum(),
                ShipQuantity = data.Where(w => w.buyerOrder.Id == x.buyerOrder.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                //ShipQuantity = x.exFactoryShipDet.ShipQuantity,
                ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate)
            }).Distinct();

            //var result = new
            //{
            //    OrderRefList = refinedata
            //};

            return Json(refinedata, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetOrderMasterInfo(int? SupplierId, int? BuyerInfoId, DateTime ExFactoryDate)
        {
            //Previous Create
            if (SupplierId != null && BuyerInfoId != null)
            {
                //var data = (from exFactoryMas in db.ExFactoryMas
                //            join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                //            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                //            join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                //            where exFactoryMas.SupplierId == SupplierId && exFactoryMas.BuyerInfoId == BuyerInfoId && exFactoryMas.ExFactoryDate == ExFactoryDate
                //            select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();



                var data = (from exFactoryMas in db.ExFactoryMas
                            join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                            join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                            join invoiceDet in db.InvoiceCommFactDet on exFactoryDet.Id equals invoiceDet.ExFactoryDetId into ps
                            from p in ps.DefaultIfEmpty()
                            where exFactoryMas.SupplierId == SupplierId && exFactoryMas.BuyerInfoId == BuyerInfoId && exFactoryMas.ExFactoryDate == ExFactoryDate
                                && exFactoryDet.Id != p.ExFactoryDetId
                            select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();


                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //ExFactoryShipDetId = x.exFactoryShipDet.Id,
                    ExFactoryDetId = x.exFactoryDet.Id,
                    OrderMasId = x.buyerOrder.Id,
                    OrderRefNo = x.buyerOrder.OrderRefNo,
                    //ShipQuantity = data.Where(m=>m.buyerOrder.Id==x.buyerOrder.Id).Select(k=>k.exFactoryDet.ShipQuantity).Sum(),
                    //ShipQuantity = data.Where(w => w.buyerOrder.Id == x.buyerOrder.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                    ShipQuantity = data.Where(w => w.exFactoryDet.Id == x.exFactoryDet.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                    //ShipQuantity = x.exFactoryShipDet.ShipQuantity,
                    ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate),
                    MasterLCId = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? 0 : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMasId,
                    MasterLCnO = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? "" : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMas.LCNo
                }).Distinct();

                return Json(refinedata, JsonRequestBehavior.AllowGet);

            }
            else //List Type
            {
                var data = (from exFactoryMas in db.ExFactoryMas
                            join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                            join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                            join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                            where exFactoryMas.ExFactoryDate == ExFactoryDate
                            select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();

                var refinedata = data.AsEnumerable().Select(x => new
                {
                    //ExFactoryMasId = x.exFactoryMas.Id,
                    BuyerInfoId = x.exFactoryMas.BuyerInfoId,
                    BuyerInfoName = x.exFactoryMas.BuyerInfo.Name,
                    SupplierId = x.exFactoryMas.SupplierId,
                    SupplierName = x.exFactoryMas.Supplier.Name
                    //ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate)
                }).Distinct();

                return Json(refinedata, JsonRequestBehavior.AllowGet);
            }

        }



        public JsonResult GetInvoiceDetailData(int InvoiceMasId)
        {

            List<VMInvoiceDetailList> list = new List<VMInvoiceDetailList>();

            var data = (
                        from exFactoryMas in db.ExFactoryMas
                        join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                        join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                        join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                        join invoiceFactDet in db.InvoiceCommFactDet on exFactoryDet.Id equals invoiceFactDet.ExFactoryDetId
                        // where exFactoryMas.SupplierId == SupplierId && exFactoryMas.BuyerInfoId == BuyerInfoId && exFactoryMas.ExFactoryDate == ExFactoryDate
                        where invoiceFactDet.InvoiceCommFactMasId == InvoiceMasId
                        select new { invoiceFactDet, exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();
            // select new { invoiceFactMas, invoiceFactDet }).ToList();

            //var refinedata = data.AsEnumerable().Select(x => new {
            //    InvoiceDetailId = x.invoiceFactDet.Id,
            //    ExFactoryShipDetId = x.exFactoryShipDet.Id,
            //    OrderMasId = x.buyerOrder.Id,
            //    OrderRefNo = x.buyerOrder.OrderRefNo,
            //    //ShipQuantity = x.exFactoryDet.ShipQuantity,
            //    ShipQuantity = x.exFactoryShipDet.ShipQuantity,
            //    ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate)
            //});

            var refinedata = data.AsEnumerable().Select(x => new
            {
                InvoiceDetailId = x.invoiceFactDet.Id,
                ExFactoryDetId = x.exFactoryDet.Id,
                OrderMasId = x.buyerOrder.Id,
                OrderRefNo = x.buyerOrder.OrderRefNo,
                //ShipQuantity = data.Where(m=>m.buyerOrder.Id==x.buyerOrder.Id).Select(k=>k.exFactoryDet.ShipQuantity).Sum(),
                //ShipQuantity = data.Where(w => w.buyerOrder.Id == x.buyerOrder.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                ShipQuantity = data.Where(w => w.exFactoryDet.Id == x.exFactoryDet.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                //ShipQuantity = x.exFactoryShipDet.ShipQuantity,
                ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate),
                MasterLCId = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? 0 : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMasId,
                MasterLCnO = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? "" : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMas.LCNo
            }).Distinct();

            foreach (var item in refinedata)
            {
                VMInvoiceDetailList detail = new VMInvoiceDetailList();
                detail.InvoiceDetailId = item.InvoiceDetailId;
                detail.ExFactoryDetId = item.ExFactoryDetId;
                detail.OrderMasId = item.OrderMasId;
                detail.OrderRefNo = item.OrderRefNo;
                detail.ShipQuantity = item.ShipQuantity;
                detail.ExFactoryDate = item.ExFactoryDate;
                detail.MasterLCId = item.MasterLCId;
                detail.MasterLCnO = item.MasterLCnO;
                detail.Ischecked = true;
                list.Add(detail);
            }

            var invoicemas = db.InvoiceCommFactMas.FirstOrDefault(x => x.Id == InvoiceMasId);
            var Alldata = (from exFactoryMas in db.ExFactoryMas
                           join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                           join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                           join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                           join invoiceDet in db.InvoiceCommFactDet on exFactoryDet.Id equals invoiceDet.ExFactoryDetId into ps
                           from p in ps.DefaultIfEmpty()
                           where exFactoryMas.SupplierId == invoicemas.SupplierId && exFactoryMas.BuyerInfoId == invoicemas.BuyerInfoId && exFactoryMas.ExFactoryDate == invoicemas.IssueDate
                               && exFactoryDet.Id != p.ExFactoryDetId
                           select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();

            var Allrefinedata = Alldata.AsEnumerable().Select(x => new
            {
                //ExFactoryShipDetId = x.exFactoryShipDet.Id,
                InvoiceDetailId = 0,
                ExFactoryDetId = x.exFactoryDet.Id,
                OrderMasId = x.buyerOrder.Id,
                OrderRefNo = x.buyerOrder.OrderRefNo,
                //ShipQuantity = data.Where(m=>m.buyerOrder.Id==x.buyerOrder.Id).Select(k=>k.exFactoryDet.ShipQuantity).Sum(),
                ShipQuantity = Alldata.Where(w => w.exFactoryDet.Id == x.exFactoryDet.Id).Sum(s => s.exFactoryShipDet.ShipQuantity),
                //ShipQuantity = x.exFactoryShipDet.ShipQuantity,
                ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate),
                MasterLCId = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? 0 : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMasId,
                MasterLCnO = db.MasterLCInfoDet.Where(m => m.BuyerOrderMasId == x.buyerOrder.Id).Count() == 0 ? "" : db.MasterLCInfoDet.FirstOrDefault(m => m.BuyerOrderMasId == x.buyerOrder.Id).MasterLCInfoMas.LCNo
            }).Distinct();


            foreach (var item in Allrefinedata)
            {
                VMInvoiceDetailList detail = new VMInvoiceDetailList();
                detail.InvoiceDetailId = item.InvoiceDetailId;
                detail.ExFactoryDetId = item.ExFactoryDetId;
                detail.OrderMasId = item.OrderMasId;
                detail.OrderRefNo = item.OrderRefNo;
                detail.ShipQuantity = item.ShipQuantity;
                detail.ExFactoryDate = item.ExFactoryDate;
                detail.MasterLCId = item.MasterLCId;
                detail.MasterLCnO = item.MasterLCnO;
                detail.Ischecked = false;
                list.Add(detail);
            }

            //refinedata.Union(Allrefinedata);

            //return Json(refinedata, JsonRequestBehavior.AllowGet);
            return Json(list.Distinct(), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetDelivData(int Id,DateTime? exFactoryDate)
        public JsonResult GetDelivData(int Id, string exFactoryDate, int? ExFactoryDetId)
        {
            var ExFactoryDate = DateTime.Parse(exFactoryDate);

            //var list = (from exFactoryMas in db.ExFactoryMas
            //            join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
            //            join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
            //            join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
            //            join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
            //                                                    new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
            //            join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
            //            join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
            //            join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
            //            from pJoin in joinDest.DefaultIfEmpty()
            //            where buyerOrderMas.Id == Id && exFactoryMas.ExFactoryDate == ExFactoryDate
            //            //where buyerOrderMas.Id == Id && exFactoryMas.ExFactoryDate == exFactoryDate
            //            select new { exfactoryDet, exfactoryShipDet, buyerOrderDet, factoryOrderDet, shipDet, pJoin }).AsEnumerable()
            //           .Select(x => new
            //           {
            //               //Id = x.shipDet.Id,
            //               StyleNo = x.buyerOrderDet.StyleNo,
            //               BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
            //               BuyersPONo = x.shipDet.BuyersPONo ?? "",
            //               DelivSlno = x.shipDet.DelivSlno,
            //               ProdSizeId = x.buyerOrderDet.ProdSizeId==null ? 0 : x.buyerOrderDet.ProdSizeId,
            //               ProdSizeName = x.buyerOrderDet.ProdSize.SizeRange==null ? "" : x.buyerOrderDet.ProdSize.SizeRange,
            //               ProdColorId = x.buyerOrderDet.ProdColorId==null ? 0 : x.buyerOrderDet.ProdColorId,
            //               ProdColorName = x.buyerOrderDet.ProdColor.Name==null ? "" : x.buyerOrderDet.ProdColor.Name,
            //               DestinationPortId = x.shipDet.DestinationPortId ==null? 0 : x.shipDet.DestinationPortId,
            //               DestinationPortName = x.pJoin == null ? "" : x.pJoin.Name,
            //               OrderQuantity = x.buyerOrderDet.Quantity,
            //               //ShipQuantity = x.shipDet.DelivQuantity,
            //               ShipQuantity = x.exfactoryShipDet.ShipQuantity,
            //               FactoryTransferPrice = x.factoryOrderDet.TransferPrice == null ? x.factoryOrderDet.FOBUnitPrice : x.factoryOrderDet.TransferPrice
            //               //FactoryInvoiceValue = x.factoryOrderDet.TransferPrice                  
            //           });



            var list = (from exFactoryMas in db.ExFactoryMas
                        join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                        join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                        join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                        join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                        join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                        join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                        join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                        from pJoin in joinDest.DefaultIfEmpty()
                        where buyerOrderMas.Id == Id && exFactoryMas.ExFactoryDate == ExFactoryDate && exfactoryDet.Id == ExFactoryDetId
                        select new
                        {
                            StyleNo = buyerOrderDet.StyleNo,
                            BuyerOrderDetId = shipDet.BuyerOrderDetId,
                            BuyersPONo = shipDet.BuyersPONo ?? "",
                            DelivSlno = shipDet.DelivSlno,
                            ETD = shipDet.ETD,
                            HandoverDate = shipDet.HandoverDate,
                            ProdSizeId = buyerOrderDet.ProdSizeId == null ? 0 : buyerOrderDet.ProdSizeId,
                            ProdSizeName = buyerOrderDet.ProdSize.SizeRange == null ? "" : buyerOrderDet.ProdSize.SizeRange,
                            ProdColorId = buyerOrderDet.ProdColorId == null ? 0 : buyerOrderDet.ProdColorId,
                            ProdColorName = buyerOrderDet.ProdColor.Name == null ? "" : buyerOrderDet.ProdColor.Name,
                            DestinationPortId = shipDet.DestinationPortId == null ? 0 : shipDet.DestinationPortId,
                            DestinationPortName = pJoin == null ? "" : pJoin.Name,
                            //OrderQuantity = buyerOrderDet.Quantity,
                            OrderQuantity = shipDet.DelivQuantity,
                            ShipQuantity = exfactoryShipDet.ShipQuantity,
                            //FactoryTransferPrice = factoryOrderDet.TransferPrice == null ? factoryOrderDet.FOBUnitPrice : factoryOrderDet.TransferPrice
                            //FactoryTransferPrice = factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice,
                            FactoryTransferPrice = factoryDetDet.FactFOB,
                            FactoryOrderDelivDetId = factoryDetDet.Id,
                            DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
                            //DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0: ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount)/100 * ((factoryDetDet.FactTransferPrice == null ? factoryDetDet.FactFOB : factoryDetDet.FactTransferPrice) *  exfactoryShipDet.ShipQuantity)),
                            DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * (factoryDetDet.FactFOB * exfactoryShipDet.ShipQuantity)),
                            //PrevDiscountPerc = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ? 0 : db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == false).FactoryDiscount,
                            PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ? 0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
                        }).ToList();


            var refinedata = list.AsEnumerable().Select(x => new
            {
                StyleNo = x.StyleNo,
                BuyerOrderDetId= x.BuyerOrderDetId,
                BuyersPONo = x.BuyersPONo,
                DelivSlno = x.DelivSlno,
                ETD = x.ETD == null ? "" : NullHelpers.DateToString(x.ETD),
                HandoverDate = x.HandoverDate == null ? "" : NullHelpers.DateToString(x.HandoverDate),
                ProdSizeId = x.ProdSizeId,
                ProdSizeName = x.ProdSizeName,
                ProdColorId = x.ProdColorId,
                ProdColorName = x.ProdColorName,
                DestinationPortId = x.DestinationPortId,
                DestinationPortName = x.DestinationPortName,
                OrderQuantity = x.OrderQuantity,
                ShipQuantity = x.ShipQuantity,
                FactoryTransferPrice = x.FactoryTransferPrice,
                FactoryOrderDelivDetId = x.FactoryOrderDelivDetId,
                DiscountPerc = x.DiscountPerc,
                DiscountValue = x.DiscountValue,
                PreviousDiscountAdj = x.PreviousDiscountAdj
            }).Distinct();


            return Json(refinedata, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,SupplierId,BuyerInfoId,InvoiceNoFact,IssueDate,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] InvoiceCommFactMas invoiceCommFactMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.InvoiceCommFactMas.Add(invoiceCommFactMas);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", invoiceCommFactMas.BuyerInfoId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", invoiceCommFactMas.SupplierId);
        //    return View(invoiceCommFactMas);
        //}

        public JsonResult SaveInvoiceFactList(IEnumerable<VMInvoiceCommFactMas> InvoiceMas)
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
                        foreach (var master in InvoiceMas)
                        {
                            InvoiceCommFactMas invoiceMas = new InvoiceCommFactMas();

                            invoiceMas.SupplierId = master.SupplierId;
                            invoiceMas.BuyerInfoId = master.BuyerInfoId;
                            invoiceMas.InvoiceNoFact = master.InvoiceNoFact;
                            invoiceMas.IssueDate = master.IssueDate;
                            invoiceMas.OpBy = 1;
                            invoiceMas.OpOn = OpDate;
                            invoiceMas.IsAuth = true;
                            //InvoiceMas.IsLocked = false;

                            db.InvoiceCommFactMas.Add(invoiceMas);
                            db.SaveChanges();

                            //var InvoiceDetails = db.ExFactoryDet.Where(x => x.ExFactoryMasId == master.ExfactoryMasId).ToList();
                            var InvoiceDetails = (from exFactoryMas in db.ExFactoryMas
                                                  join exFactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exFactoryDet.ExFactoryMasId
                                                  join exFactoryShipDet in db.ExFactoryShipDet on exFactoryDet.Id equals exFactoryShipDet.ExFactoryDetId
                                                  join buyerOrder in db.BuyerOrderMas on exFactoryDet.BuyerOrderMasId equals buyerOrder.Id
                                                  where exFactoryMas.SupplierId == master.SupplierId && exFactoryMas.BuyerInfoId == master.BuyerInfoId && exFactoryMas.ExFactoryDate == master.IssueDate
                                                  select new { exFactoryMas, exFactoryDet, exFactoryShipDet, buyerOrder }).ToList();

                            var refinedata = InvoiceDetails.AsEnumerable().Select(x => new
                            {
                                //ExFactoryShipDetId = x.exFactoryShipDet.Id,
                                ExFactoryDetId = x.exFactoryDet.Id,
                                //OrderMasId = x.buyerOrder.Id,
                                //OrderRefNo = x.buyerOrder.OrderRefNo,
                                //ExFactoryDate = NullHelpers.DateToString(x.exFactoryMas.ExFactoryDate)
                            }).Distinct();


                            foreach (var item in refinedata)
                            {
                                InvoiceCommFactDet detail = new InvoiceCommFactDet();
                                detail.InvoiceCommFactMasId = invoiceMas.Id;
                                detail.ExFactoryDetId = item.ExFactoryDetId;
                                //item.IsLocked = false;

                                db.InvoiceCommFactDet.Add(detail);
                                db.SaveChanges();

                            }

                        }




                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = 0
                            //Id = InvoiceMas.Id
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



        public JsonResult SaveInvoiceFact(IEnumerable<InvoiceCommFactDet> InvoiceDetails, InvoiceCommFactMas InvoiceMas)
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

                        InvoiceMas.OpBy = 1;
                        InvoiceMas.OpOn = OpDate;
                        InvoiceMas.IsAuth = true;
                        //InvoiceMas.IsLocked = false;

                        db.InvoiceCommFactMas.Add(InvoiceMas);
                        db.SaveChanges();

                        foreach (var item in InvoiceDetails)
                        {
                            item.InvoiceCommFactMasId = InvoiceMas.Id;
                            //item.ExFactoryDetId = item.ExFactoryDetId;

                           var exDet = db.ExFactoryDet.Find(item.ExFactoryDetId);
                            var buyermas = exDet.BuyerOrderMasId;
                            //var list = GetDelivData(item.ExFactoryDet.BuyerOrderMasId, exfactDate, item.ExFactoryDetId);


                            var list = (from exFactoryMas in db.ExFactoryMas
                                        join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                                        join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                                        join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                        join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                                new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                        join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                        join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                        join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                        from pJoin in joinDest.DefaultIfEmpty()
                                        where buyerOrderMas.Id == exDet.BuyerOrderMasId && exFactoryMas.ExFactoryDate == InvoiceMas.IssueDate && exfactoryDet.Id == item.ExFactoryDetId
                                        select new
                                        {
                                            OrderQuantity = shipDet.DelivQuantity,
                                            ShipQuantity = exfactoryShipDet.ShipQuantity,
                                            FactoryTransferPrice = factoryDetDet.FactFOB,
                                            FactoryOrderDelivDetId = factoryDetDet.Id,
                                            DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 
                                                0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
                                            DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ? 
                                                0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * (factoryDetDet.FactFOB * exfactoryShipDet.ShipQuantity)),
                                            PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ?
                                                0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
                                        }).ToList();

                            decimal invoiceDetDiscountval = 0;

                            foreach (var i in list)
                            {
                                decimal currentFOB = 0;
                                decimal currentValue = 0;

                                if (i.DiscountValue != 0 || i.PreviousDiscountAdj != 0)
                                {
                                    var FactoryValue = Convert.ToDecimal(i.ShipQuantity * i.FactoryTransferPrice);
                                    var currentDiscount = Convert.ToDecimal(i.DiscountValue);
                                    var previousDiscount = Convert.ToDecimal(i.PreviousDiscountAdj);
                                    currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (i.ShipQuantity));
                                    currentValue = (currentFOB * i.ShipQuantity);
                                }
                                else
                                {
                                    currentValue = i.ShipQuantity * (i.FactoryTransferPrice ?? 0);
                                }

                                invoiceDetDiscountval = invoiceDetDiscountval + currentValue;
                            }

                            item.InvoiceTotalAmt = invoiceDetDiscountval;

                            db.InvoiceCommFactDet.Add(item);
                            db.SaveChanges();


                            var exFact = db.ExFactoryShipDet.Where(x => x.ExFactoryDetId == item.ExFactoryDetId).ToList();
                            foreach(var det in exFact)
                            {
                                var getDiscount = db.DiscountMas.SingleOrDefault(x => x.BuyerOrderDetId == det.BuyerOrderDetId);

                                if(getDiscount!=null)
                                {
                                    getDiscount.DiscountFlag = true;
                                    db.SaveChanges();
                                }
                            }
                        
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!",
                            Id = InvoiceMas.Id
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


        // GET: InvoiceCommFact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceCommFactMas invoiceCommFactMas = db.InvoiceCommFactMas.Find(id);
            if (invoiceCommFactMas == null)
            {
                return HttpNotFound();
            }

            ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", invoiceCommFactMas.SupplierId);

            var getBuyer = (from OrderDet in db.BuyerOrderDet
                            join Buyers in db.BuyerInfo on OrderDet.BuyerOrderMas.BuyerInfoId equals Buyers.Id
                            where OrderDet.SupplierId == invoiceCommFactMas.SupplierId
                            select Buyers).Distinct().ToList();

            ViewBag.BuyerInfoId = new SelectList(getBuyer, "Id", "Name", invoiceCommFactMas.BuyerInfoId);

            ViewBag.CheckInvoiceCommExists = db.InvoiceCommDet.Where(x => x.InvoiceCommFactMasId == invoiceCommFactMas.Id).Count() > 0 ? true : false;


            return View(invoiceCommFactMas);
        }


        public JsonResult UpdateInvoiceFact(IEnumerable<InvoiceCommFactDet> InvoiceDetails, InvoiceCommFactMas InvoiceMas, int[] DelItems)
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
                        var InvoiceFactM = db.InvoiceCommFactMas.Find(InvoiceMas.Id);

                        if (InvoiceFactM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invoice not found, Saving Failed !!"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        InvoiceFactM.BuyerInfoId = InvoiceMas.BuyerInfoId;
                        InvoiceFactM.SupplierId = InvoiceMas.SupplierId;
                        InvoiceFactM.InvoiceNoFact = InvoiceMas.InvoiceNoFact;
                        InvoiceFactM.IssueDate = InvoiceMas.IssueDate;


                        db.Entry(InvoiceFactM).State = EntityState.Modified;

                        db.SaveChanges();


                        foreach (var item in InvoiceDetails)
                        {

                            var exDet = db.ExFactoryDet.Find(item.ExFactoryDetId);
                            var buyermas = exDet.BuyerOrderMasId;

                            var list = (from exFactoryMas in db.ExFactoryMas
                                        join exfactoryDet in db.ExFactoryDet on exFactoryMas.Id equals exfactoryDet.ExFactoryMasId
                                        join exfactoryShipDet in db.ExFactoryShipDet on exfactoryDet.Id equals exfactoryShipDet.ExFactoryDetId
                                        join buyerOrderMas in db.BuyerOrderMas on exfactoryDet.BuyerOrderMasId equals buyerOrderMas.Id
                                        join buyerOrderDet in db.BuyerOrderDet on new { ColA = buyerOrderMas.Id, ColB = exfactoryShipDet.BuyerOrderDetId } equals
                                                                                new { ColA = buyerOrderDet.BuyerOrderMasId, ColB = buyerOrderDet.Id }
                                        join factoryOrderDet in db.FactoryOrderDet on buyerOrderDet.Id equals factoryOrderDet.BuyerOrderDetId
                                        join shipDet in db.ShipmentSummDet on new { ColA = buyerOrderDet.Id, ColB = exfactoryShipDet.ShipmentSummDetId } equals new { ColA = shipDet.BuyerOrderDetId, ColB = shipDet.Id }
                                        join factoryDetDet in db.FactoryOrderDelivDet on new { ColA = factoryOrderDet.Id, ColB = shipDet.Id } equals new { ColA = factoryDetDet.FactoryOrderDetId, ColB = factoryDetDet.ShipmentSummDetId ?? 0 }
                                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                        from pJoin in joinDest.DefaultIfEmpty()
                                        where buyerOrderMas.Id == exDet.BuyerOrderMasId && exFactoryMas.ExFactoryDate == InvoiceMas.IssueDate && exfactoryDet.Id == item.ExFactoryDetId
                                        select new
                                        {
                                            OrderQuantity = shipDet.DelivQuantity,
                                            ShipQuantity = exfactoryShipDet.ShipQuantity,
                                            FactoryTransferPrice = factoryDetDet.FactFOB,
                                            FactoryOrderDelivDetId = factoryDetDet.Id,
                                            DiscountPerc = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ?
                                                0 : (db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount),
                                            DiscountValue = db.DiscountDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).Count() == 0 ?
                                                0 : ((db.DiscountDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id && x.AdjustFactoryNow == true).FactoryDiscount) / 100 * (factoryDetDet.FactFOB * exfactoryShipDet.ShipQuantity)),
                                            PreviousDiscountAdj = db.DiscountAdjustmentFactoryDet.Where(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).Count() == 0 ?
                                                0 : (db.DiscountAdjustmentFactoryDet.FirstOrDefault(x => x.FactoryOrderDelivDetId == factoryDetDet.Id).AdjustmentAmt)
                                        }).ToList();

                            decimal invoiceDetDiscountval = 0;

                            foreach (var i in list)
                            {
                                decimal currentFOB = 0;
                                decimal currentValue = 0;

                                if (i.DiscountValue != 0 || i.PreviousDiscountAdj != 0)
                                {
                                    var FactoryValue = Convert.ToDecimal(i.ShipQuantity * i.FactoryTransferPrice);
                                    var currentDiscount = Convert.ToDecimal(i.DiscountValue);
                                    var previousDiscount = Convert.ToDecimal(i.PreviousDiscountAdj);
                                    currentFOB = ((FactoryValue - (currentDiscount + previousDiscount)) / (i.ShipQuantity));
                                    currentValue = (currentFOB * i.ShipQuantity);
                                }
                                else
                                {
                                    currentValue = i.ShipQuantity * (i.FactoryTransferPrice ?? 0);
                                }

                                invoiceDetDiscountval = invoiceDetDiscountval + currentValue;
                            }



                            if (item.Id == 0) //insert
                            {
                                item.InvoiceCommFactMasId = InvoiceFactM.Id;
                                //item.ExFactoryShipDetId = item.ExFactoryShipDetId;
                                item.InvoiceTotalAmt = invoiceDetDiscountval;
                                db.InvoiceCommFactDet.Add(item);

                            }
                            else //update
                            {
                                var oItem = db.InvoiceCommFactDet.Find(item.Id);
                                //oItem.InvoiceCommFactMasId = item.InvoiceCommFactMasId;
                                oItem.ExFactoryDetId = item.ExFactoryDetId;
                                oItem.InvoiceTotalAmt = invoiceDetDiscountval;

                                db.Entry(oItem).State = EntityState.Modified;
                            }

                            db.SaveChanges();


                        }


                        //---- delete detail items
                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {

                                var deleteDetail = db.InvoiceCommFactDet.FirstOrDefault(x => x.Id == item);
                                db.InvoiceCommFactDet.Remove(deleteDetail);
                                db.SaveChanges();
                            }
                        }


                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful !!"
                        };

                        //Success("Updated successfully.", true);


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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,SupplierId,BuyerInfoId,InvoiceNoFact,IssueDate,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] InvoiceCommFactMas invoiceCommFactMas)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(invoiceCommFactMas).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", invoiceCommFactMas.BuyerInfoId);
        //    ViewBag.SupplierId = new SelectList(db.Supplier, "Id", "Name", invoiceCommFactMas.SupplierId);
        //    return View(invoiceCommFactMas);
        //}

        // GET: InvoiceCommFact/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceCommFactMas invoiceCommFactMas = db.InvoiceCommFactMas.Find(id);
            if (invoiceCommFactMas == null)
            {
                return HttpNotFound();
            }
            return View(invoiceCommFactMas);
        }

        // POST: InvoiceCommFact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceCommFactMas invoiceCommFactMas = db.InvoiceCommFactMas.Find(id);
            db.InvoiceCommFactMas.Remove(invoiceCommFactMas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult DeleteInvoice(int id)
        {
            bool flag = false;
            try
            {
                var itemToDeleteMas = db.InvoiceCommFactMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nCommercial Invoice Factory Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var checkInvoiceFact = db.InvoiceCommDet.Where(x => x.InvoiceCommFactMasId == id).ToList();

                if (checkInvoiceFact.Count > 0)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nInvoice exists. Delete invoice data first."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }


                var itemsToDeleteDet = db.InvoiceCommFactDet.Where(x => x.InvoiceCommFactMasId == id);

                //foreach (var item in itemsToDeleteDet)
                //{
                //    var itemsToDeleteDeliv = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == item.Id);
                //    db.ShipmentSummDet.RemoveRange(itemsToDeleteDeliv);
                //}

                db.InvoiceCommFactDet.RemoveRange(itemsToDeleteDet);

                db.InvoiceCommFactMas.Remove(itemToDeleteMas);

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


        public JsonResult GetBuyerNamesBySupplierId(int Id)
        {
            var data = (from OrderDet in db.BuyerOrderDet
                        join Buyers in db.BuyerInfo on OrderDet.BuyerOrderMas.BuyerInfoId equals Buyers.Id
                        where OrderDet.SupplierId == Id
                        select Buyers).Distinct().Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
