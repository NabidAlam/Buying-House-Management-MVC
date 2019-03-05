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
using System.IO;
using SautinSoft;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace BHMS.Controllers
{
    public class BuyerOrderController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: BuyerOrder
        public ActionResult Index()
        {
            var buyerOrderMas = db.BuyerOrderMas.Include(b => b.BuyerInfo).Include(b => b.ProdDepartment).Include(b => b.SeasonInfo).Include(b => b.Supplier);
            return View(buyerOrderMas.ToList());
        }

        // GET: BuyerOrder/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BuyerOrderMas buyerOrderMa = db.BuyerOrderMas.Find(id);
        //    if (buyerOrderMa == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(buyerOrderMa);
        //}

        // GET: BuyerOrder/Create


        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.ProdDepartmentId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            //ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.OrderBy(x=>x.Name), "Id", "Name");
            ViewBag.SeasonInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.FabSupplierId = new SelectList(db.Supplier.OrderBy(x => x.Name), "Id", "Name");
            //ViewBag.ProdTypeId = new SelectList(db.ProdCategory, "Id", "Name");

            var fOBType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Style wise", Value = "0" }, new SelectListItem { Text = "Delivery wise", Value = "1" }, }, "Value", "Text");
            ViewBag.FOBType = fOBType;
            var delBased = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Handover Date", Value = "0" }, new SelectListItem { Text = "ETD Date", Value = "1" }, }, "Value", "Text");
            ViewBag.DeliveryBased = delBased;

            return View();
        }



        public ActionResult CreateCottonOn()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.Where(x=>x.Id==4).OrderBy(x => x.Name), "Id", "Name",4);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x => x.BuyerInfoId == 4), "Id", "Name");
            ViewBag.ProdDepartmentId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            //ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.OrderBy(x=>x.Name), "Id", "Name");
            ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.Where(x=>x.BuyerInfoId==4), "Id", "Name");
            ViewBag.FabSupplierId = new SelectList(db.Supplier.OrderBy(x => x.Name), "Id", "Name");
            //ViewBag.ProdTypeId = new SelectList(db.ProdCategory, "Id", "Name");

            var fOBType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Style wise", Value = "0" }, new SelectListItem { Text = "Delivery wise", Value = "1" }, new SelectListItem { Text = "Color wise", Value = "2" } }, "Value", "Text",2);
            ViewBag.FOBType = fOBType;
            var delBased = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Handover Date", Value = "0" }, new SelectListItem { Text = "ETD Date", Value = "1" }, }, "Value", "Text",1);
            ViewBag.DeliveryBased = delBased;

            return View();
        }

        //[HttpPost]
        ////public JsonResult ConvertPdf(HttpPostedFileBase file)
        //public JsonResult ConvertPdf()
        //{

        //    var result = new
        //    {
        //        flag = false,
        //        message = "Error occured. !"
        //    };

        //    try
        //    {


        //        Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
        //        //Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(Excelpath);
        //        Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(@"F:\Projects\Latest\RBHMS\BHMS 28 Oct 2018 4_42 pm DocSubmissionEdit Done\BHMS\Content\CottonOn\excel.xls");
        //        Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
        //        Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;


        //        //int OrderCount = db.ProjectTask.Count();

        //        //for (var row = 2; row <= range.Rows.Count; row++)
        //        //{
        //        // parameter from excel
        //        string styleNumber = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, 45]).Text.Trim();  //Style Number:
        //        string styleParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, 59]).Text.Trim();
        //        string OrderDate = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, 57]).Text.Trim();  //DATE SENT
        //        string OrderParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, 65]).Text.Trim();
        //        string PONO = ((Microsoft.Office.Interop.Excel.Range)range.Cells[12, 2]).Text.Trim();  //ORDER NO
        //        string PONOParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[11, 11]).Text.Trim();
        //        string ProductCategory = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, 2]).Text.Trim(); //STYLE NAME:
        //        string ProductParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, 8]).Text.Trim();
        //        string FabricDescription = ((Microsoft.Office.Interop.Excel.Range)range.Cells[40, 2]).Text.Trim(); //DESCRIPTION:
        //        string FabricDescriptionParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[42, 8]).Text.Trim();
        //        string Season = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, 27]).Text.Trim(); //Production event:
        //        string SeasonParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, 40]).Text.Trim();
        //        string ETD = ((Microsoft.Office.Interop.Excel.Range)range.Cells[36, 27]).Text.Trim();   //GOODS MUST SAIL THE WEEK BEGINNING:
        //        string ETDParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[37, 40]).Text.Trim();
        //        string ShipMode = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, 52]).Text.Trim(); //FREIGHT (AIR/SEA):
        //        string ShipModeParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, 63]).Text.Trim();


        //        for (var row = 76; row <= range.Rows.Count; row = row + 4)
        //        {
        //            string Destination = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text.Trim();  //DESTINATION:
        //            string DestinationParameter = Destination.Substring(13, Destination.Length -13);

        //            string Color = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row+1, 13]).Text.Trim(); //COLOUR DESCRIPTION
        //            string ColorParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 2, 13]).Text.Trim();

        //            string FOB = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 20]).Text.Trim(); //FOB
        //            string FOBParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 2, 20]).Text.Trim();

        //            string SizeStart = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 22]).Text.Trim(); //SizeFrom
        //            string SizeEnd = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 40]).Text.Trim(); //SizeTo

        //            var Size = SizeStart + "-" + SizeEnd;

        //            string OrderQty = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 62]).Text.Trim(); //TOTAL
        //            string OrderQtyParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 2, 62]).Text.Trim();

        //        }
        //        workbook.Close();
        //        application.Quit();
        //        //System.IO.File.Delete(path);
        //        //return View();
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //    }


        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}



        [HttpPost]
        //public JsonResult ConvertPdf(HttpPostedFileBase file)
        public JsonResult ConvertPdf()
        {

            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            var fileName = "";
            string Excelpath = "";

            var file = Request.Files[Request.Files.GetKey(0)];
            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            //    file = Request.Files[i];
            //    // save file as required here...
            //}

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/CottonOn"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";


                    SautinSoft.PdfFocus f = new PdfFocus();
                    f.ExcelOptions.PreservePageLayout = true;

                    f.OpenPdf(path);

                    if (f.PageCount > 0)
                    {
                        fileName = System.IO.Path.GetFileNameWithoutExtension(path) + Convert.ToString(DateTime.Now.Millisecond) + ".xls";
                        //string Excelpath = Server.MapPath("/") + "Content\\CottonOn\\" + System.IO.Path.GetFileNameWithoutExtension(path) + Convert.ToString(DateTime.Now.Millisecond) + ".xls";
                        Excelpath = Server.MapPath("/") + "Content\\CottonOn\\" + fileName;


                        f.ToExcel(Excelpath);
                        //f.ToExcel(@"F:\Projects\Latest\RBHMS\BHMS 28 Oct 2018 4_42 pm DocSubmissionEdit Done\BHMS\Content\CottonOn\excel.xls");

                        //System.IO.File.Delete(path);
                        result = new
                        {
                            flag = true,
                            message = "Successful. !"
                        };

                    }


                    Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(Excelpath);
                    //Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(@"F:\Projects\Latest\RBHMS\BHMS 28 Oct 2018 4_42 pm DocSubmissionEdit Done\BHMS\Content\CottonOn\excel.xls");
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;


                    //int OrderCount = db.ProjectTask.Count();

                    //for (var row = 2; row <= range.Rows.Count; row++)
                    //{
                    // parameter from excel
                    string styleNumber = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, 45]).Text.Trim();  //Style Number:
                    if (styleNumber == "")
                    {
                        var counter = 46;
                        while (styleNumber == "")
                        {
                            styleNumber = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, counter]).Text.Trim();
                            counter++;
                        }

                    }
                    string styleParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, 59]).Text.Trim();
                    if (styleParameter == "")
                    {
                        var counter = 60;
                        while (styleParameter == "")
                        {
                            styleParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[2, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string OrderDate = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, 57]).Text.Trim();  //DATE SENT
                    if (OrderDate == "")
                    {
                        var counter = 58;
                        while (OrderDate == "")
                        {
                            OrderDate = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string OrderDateParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, 65]).Text.Trim();
                    if (OrderDateParameter == "")
                    {
                        var counter = 66;
                        while (OrderDateParameter == "")
                        {
                            OrderDateParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[7, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string PONO = ((Microsoft.Office.Interop.Excel.Range)range.Cells[12, 2]).Text.Trim();  //ORDER NO
                    if (PONO == "")
                    {
                        var counter = 3;
                        while (PONO == "")
                        {
                            PONO = ((Microsoft.Office.Interop.Excel.Range)range.Cells[12, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string PONOParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[11, 11]).Text.Trim();
                    if (PONOParameter == "")
                    {
                        var counter = 12;
                        while (PONOParameter == "")
                        {
                            PONOParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[11, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ProductCategory = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, 2]).Text.Trim(); //STYLE NAME:
                    if (ProductCategory == "")
                    {
                        var counter = 3;
                        while (ProductCategory == "")
                        {
                            ProductCategory = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ProductCategoryParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, 8]).Text.Trim();
                    if (ProductCategoryParameter == "")
                    {
                        var counter = 9;
                        while (ProductCategoryParameter == "")
                        {
                            ProductCategoryParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[35, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string FabricDescription = ((Microsoft.Office.Interop.Excel.Range)range.Cells[40, 2]).Text.Trim(); //DESCRIPTION:
                    if (FabricDescription == "")
                    {
                        var counter = 3;
                        while (FabricDescription == "")
                        {
                            FabricDescription = ((Microsoft.Office.Interop.Excel.Range)range.Cells[40, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string FabricDescriptionParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[42, 8]).Text.Trim();
                    if (FabricDescriptionParameter == "")
                    {
                        var counter = 9;
                        while (FabricDescriptionParameter == "")
                        {
                            FabricDescriptionParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[42, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string Season = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, 27]).Text.Trim(); //Production event:
                    if (Season == "")
                    {
                        var counter = 28;
                        while (Season == "")
                        {
                            Season = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string SeasonParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, 40]).Text.Trim();
                    if (SeasonParameter == "")
                    {
                        var counter = 41;
                        while (SeasonParameter == "")
                        {
                            SeasonParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[47, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ETD = ((Microsoft.Office.Interop.Excel.Range)range.Cells[36, 27]).Text.Trim();   //GOODS MUST SAIL THE WEEK BEGINNING:
                    if (ETD == "")
                    {
                        var counter = 28;
                        while (ETD == "")
                        {
                            ETD = ((Microsoft.Office.Interop.Excel.Range)range.Cells[36, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ETDParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[37, 40]).Text.Trim();
                    if (ETDParameter == "")
                    {
                        var counter = 41;
                        while (ETDParameter == "")
                        {
                            ETDParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[37, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ShipMode = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, 52]).Text.Trim(); //FREIGHT (AIR/SEA):
                    if (ShipMode == "")
                    {
                        var counter = 53;
                        while (ShipMode == "")
                        {
                            ShipMode = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, counter]).Text.Trim();
                            counter++;
                        }

                    }

                    string ShipModeParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, 63]).Text.Trim();
                    if (ShipModeParameter == "")
                    {
                        var counter = 64;
                        while (ShipModeParameter == "")
                        {
                            ShipModeParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[54, counter]).Text.Trim();
                            counter++;
                        }

                    }


                    VMBuyerOrderMasCottonOnEdit master = new VMBuyerOrderMasCottonOnEdit();
                    master.OrderDate = Convert.ToDateTime(DateTime.ParseExact(OrderDateParameter, "dd/MM/yyyy", CultureInfo.InvariantCulture));

                    var seasonId = db.SeasonInfo.FirstOrDefault(x => x.BuyerInfoId == 4 && x.Name == SeasonParameter);
                    if (seasonId == null)
                    {
                        SeasonInfo season = new SeasonInfo()
                        {
                            BuyerInfoId = 4,
                            Name = SeasonParameter,
                            IsAuth = true,
                            OpBy = 1,
                            OpOn = DateTime.Now
                        };

                        db.SeasonInfo.Add(season);
                        db.SaveChanges();
                        master.SeasonInfoId = season.Id;
                    }
                    else
                    {
                        master.SeasonInfoId = seasonId.Id;
                    }

                    List<VMBuyerOrderDetCottonOn> OrderList = new List<VMBuyerOrderDetCottonOn>();
                    List<VMShipmentSummDetCottonOn> DeliveryList = new List<VMShipmentSummDetCottonOn>();


                    var masterCounter = 0;
                    var quitCounter = 0;
                    string DestinationGlobal = "";
                    //for (var row = 76; row <= range.Rows.Count; row = row + 4)
                    for (var row = 76; row <= range.Rows.Count; row++)
                    {
                        string Destination = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text.Trim();  //DESTINATION:
                        //string DestinationParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text.Trim();
                        string DestinationParameter = "";
                        
                        if (Destination == "")
                        {
                            var counter = 2;


                            while (Destination == "")
                            {
                                if (counter == 10)
                                {
                                    masterCounter = counter;
                                    break;
                                    //continue;
                                }

                                DestinationParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, counter]).Text.Trim();
                                if (DestinationParameter!="")
                                {
                                    if (DestinationParameter.Contains("DESTINATION:"))
                                    {
                                        DestinationParameter = DestinationParameter.Substring(13, Destination.Length - 13);
                                        Destination = DestinationParameter;
                                        DestinationGlobal = DestinationParameter;

                                        row = row + 1;
                                        continue;
                                    }
                                    else
                                    {
                                        if (DestinationParameter.Contains("ITEM DESCRIPTION") || DestinationParameter.Contains("COLOUR DESCRIPTION"))
                                        {
                                            continue;
                                        }
                                        else if (DestinationParameter.Contains("EXPORT CARTONS"))
                                        {
                                            quitCounter = 1;
                                        }
                                        else
                                        {
                                            DestinationParameter = DestinationGlobal;
                                        }
                                    }
                                   
                                }
                                else
                                {
                                    counter++;
                                }

                               
                            }

                        }
                        else
                        {
                            if (Destination.Contains("DESTINATION:"))
                            {
                                DestinationParameter = Destination.Substring(13, Destination.Length - 13);
                                DestinationGlobal = DestinationParameter;
                                row = row + 1;

                                continue;
                            }
                            else
                            {
                                if (DestinationParameter.Contains("ITEM DESCRIPTION") || DestinationParameter.Contains("COLOUR DESCRIPTION"))
                                {
                                    continue;
                                }
                                else if(Destination.Contains("EXPORT CARTONS"))
                                {
                                    quitCounter = 1;
                                }
                                else
                                {
                                    DestinationParameter = DestinationGlobal;
                                }
                            }
                        }


                        if(quitCounter==1)
                        {
                            break;
                        }


                        if(masterCounter==10)
                        {
                            masterCounter = 0;
                            continue;
                        }
                        else
                        {
                            masterCounter = 0;
                        }

                        //string Color = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 13]).Text.Trim(); //COLOUR DESCRIPTION
                        string ColorParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 13]).Text.Trim();
                        if (ColorParameter == "")
                        {
                            var counter = 14;
                            while (ColorParameter == "")
                            {
                                ColorParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, counter]).Text.Trim();
                                counter++;
                            }

                        }

                        //string FOB = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 20]).Text.Trim(); //FOB
                        string FOBParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 20]).Text.Trim();
                        if (FOBParameter == "")
                        {
                            var counter = 21;
                            while (FOBParameter == "")
                            {
                                FOBParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row , counter]).Text.Trim();
                                counter++;
                            }

                        }

                        //string SizeStart = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 22]).Text.Trim(); //SizeFrom
                        //if (SizeStart == "")
                        //{
                        //    var counter = 22;
                        //    while (SizeStart == "")
                        //    {
                        //        SizeStart = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, counter]).Text.Trim();
                        //        counter++;
                        //    }

                        //}

                        //string SizeEnd = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 40]).Text.Trim(); //SizeTo
                        //if (SizeEnd == "")
                        //{
                        //    var counter = 41;
                        //    while (SizeEnd == "")
                        //    {
                        //        SizeEnd = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, counter]).Text.Trim();
                        //        counter++;
                        //    }

                        //}

                        //var Size = SizeStart + "-" + SizeEnd;

                        var Size = "XXS-XXL";

                        //string OrderQty = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row + 1, 62]).Text.Trim(); //TOTAL
                        string OrderQtyParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row , 62]).Text.Trim();
                        if (OrderQtyParameter == "")
                        {
                            var counter = 63;
                            while (OrderQtyParameter == "")
                            {
                                OrderQtyParameter = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row , counter]).Text.Trim();
                                counter++;
                            }

                        }


                        VMBuyerOrderDetCottonOn vm = new VMBuyerOrderDetCottonOn();
                        vm.StyleNo = styleParameter;
                        vm.ProdCatId = ProductCategoryParameter;
                        vm.ProdColorName = ColorParameter;
                        vm.ProdSizeName = Size;
                        vm.FabricItemId = FabricDescriptionParameter;
                        vm.Quantity = Convert.ToInt32(OrderQtyParameter);
                        vm.UnitPrice = Convert.ToDecimal(FOBParameter);
                        vm.RDLTotal = Convert.ToInt32(OrderQtyParameter) * Convert.ToDecimal(FOBParameter);

                        OrderList.Add(vm);


                        VMShipmentSummDetCottonOn deliv = new VMShipmentSummDetCottonOn();
                        deliv.ETD = DateTime.Parse(ETDParameter);
                        deliv.BuyersPONo = PONOParameter;
                        deliv.DelivQuantity = Convert.ToInt32(OrderQtyParameter);
                        deliv.BuyerSlNo = "1";
                        deliv.DestinationPortId = DestinationParameter;
                        deliv.ShipmentMode = ShipModeParameter=="SEA" ? 0 : 1;

                        DeliveryList.Add(deliv);
                    }
                    workbook.Close();
                    application.Quit();

                    var data = new
                    {
                        OrderMas = master,
                        OrderDetList = OrderList,
                        ShipmentList = DeliveryList
                    };

                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";

                result = new
                {
                    flag = false,
                    message = "Error occured. !"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetShipmentMode()
        {
            var data = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Sea", Value = "0" }, new SelectListItem { Text = "Air", Value = "1" }, }, "Value", "Text");

            return Json(data, JsonRequestBehavior.AllowGet);

        }



        // POST: BuyerOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,OrderRefNo,OrderDate,BuyerInfoId,ProdDepartmentId,SeasonInfoId,FabSupplierId,IsAuth,OpBy,OpOn,AuthBy,AuthOn,IsLocked")] BuyerOrderMas buyerOrderMa)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.BuyerOrderMas.Add(buyerOrderMa);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", buyerOrderMa.BuyerInfoId);
        //    ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment, "Id", "Name", buyerOrderMa.ProdDepartmentId);
        //    ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo, "Id", "Name", buyerOrderMa.SeasonInfoId);
        //    ViewBag.FabSupplierId = new SelectList(db.Supplier, "Id", "Name", buyerOrderMa.FabSupplierId);
        //    return View(buyerOrderMa);
        //}

        //public JsonResult SaveBuyerOrder(IEnumerable<VMBuyerOrderDetEdit> OrderDetails, int SiteId, DateTime PlanDate, string PlanCode, string Status)

        //public JsonResult SaveBuyerOrder(IEnumerable<VMBuyerOrderDetEdit> OrderDetails, VMBuyerOrderMasEdit OrderMas)
        //{
        //    var result = new
        //    {
        //        flag = false,
        //        message = "Error occured. !",
        //        Id=0
        //    };

        //    try
        //    {
        //        var OpDate = DateTime.Now;
        //        using (var dbContextTransaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {

        //                var OrderM = new BuyerOrderMas()
        //                {
        //                    Id = 0,
        //                    OrderRefNo = OrderMas.OrderRefNo,
        //                    OrderDate = OrderMas.OrderDate,
        //                    BuyerInfoId = OrderMas.BuyerInfoId,
        //                    ProdDepartmentId = OrderMas.ProdDepartmentId,
        //                    SeasonInfoId = OrderMas.SeasonInfoId,
        //                    FabSupplierId = OrderMas.FabSupplierId,
        //                    OpBy = 1,
        //                    OpOn = OpDate,
        //                    IsAuth = true,
        //                    IsLocked = false
        //                };

        //                db.BuyerOrderMas.Add(OrderM);                        
        //                db.SaveChanges();

        //                foreach (var item in OrderDetails)
        //                {
        //                    var OrderD = new BuyerOrderDet()
        //                    {
        //                        Id = 0,
        //                        BuyerOrderMasId = OrderM.Id,
        //                        ProdCatTypeId = item.ProdCatTypeId,
        //                        StyleNo = item.StyleNo,
        //                        ProdSizeId = item.ProdSizeId,
        //                        ProdColorId = item.ProdColorId,
        //                        FabricItemId = item.FabricItemId,
        //                        SupplierId = item.SupplierId,
        //                        UnitPrice = item.UnitPrice,
        //                        Quantity = item.Quantity,
        //                        IsLocked = false

        //                    };

        //                    db.BuyerOrderDet.Add(OrderD);
        //                    db.SaveChanges();

        //                }


        //                dbContextTransaction.Commit();

        //                result = new
        //                {
        //                    flag = true,
        //                    message = "Saving successful !!",
        //                    Id = OrderM.Id 
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

        public JsonResult SaveBuyerOrder(IEnumerable<VMBuyerOrderDetEdit> OrderDetails, IEnumerable<VMShipmentSummDet> DelivDetails, VMBuyerOrderMasEdit OrderMas)
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

                        var OrderM = new BuyerOrderMas()
                        {
                            Id = 0,
                            OrderRefNo = OrderMas.OrderRefNo,
                            OrderDate = OrderMas.OrderDate,
                            BuyerInfoId = OrderMas.BuyerInfoId,
                            BrandId = OrderMas.BrandId,
                            ProdDepartmentId = OrderMas.ProdDepartmentId,
                            SeasonInfoId = OrderMas.SeasonInfoId,
                            FobType = OrderMas.FOBType,
                            DeliveryOn = OrderMas.DeliveryBased,
                            OpBy = 1,
                            OpOn = OpDate,
                            IsAuth = true,
                            IsLocked = false
                        };

                        db.BuyerOrderMas.Add(OrderM);
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();


                        if (OrderDetails != null)
                        {
                            foreach (var item in OrderDetails)
                            {
                                var OrderD = new BuyerOrderDet()
                                {
                                    Id = 0,
                                    BuyerOrderMasId = OrderM.Id,
                                    ProdCatTypeId = item.ProdCatTypeId,
                                    StyleNo = item.StyleNo,
                                    ProdSizeId = item.ProdSizeId,
                                    ProdColorId = item.ProdColorId,
                                    //FabricItemId = item.FabricItemId,
                                    FabricItemDetId = item.FabricItemId,
                                    //HERE FabricItemId IS FabricItemDetId, IT WAS CHANGED AFTER ADDING MASTER DETAIL IN FABRIC ITEM TABLE
                                    //SO HERE THE DATA COMING AS A FabricItemDetId WHICH YOU WILL FIND IN FABRICITEM CONTROLLER IN GetDesc ACTION
                                    SupplierId = item.SupplierId,
                                    UnitPrice = item.UnitPrice,
                                    Quantity = item.Quantity,
                                    IsLocked = false,
                                    IsShipClosed = false,
                                    FabricSupplierId = item.FabricSupplierId,
                                    RdlTotal = item.RDLTotal
                                };

                                db.BuyerOrderDet.Add(OrderD);
                                db.SaveChanges();

                                dictionary.Add(item.TempOrderDetId, OrderD.Id);

                            }
                        }


                        //---- shipment data

                        var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                var deliv = new ShipmentSummDet()
                                {
                                    Id = item.Id,
                                    BuyerOrderDetId = dictionary[item.DelivOrderDetTempId],
                                    DelivSlno = slno++,
                                    BuyerSlNo = item.BuyerSlNo,
                                    HandoverDate = item.HandoverDate,
                                    ETD = item.ETD,
                                    DestinationPortId = item.DestinationPortId,
                                    DelivQuantity = item.DelivQuantity,
                                    RdlFOB = item.RdlFobDetailDet,
                                    BuyersPONo = item.BuyersPONo,
                                    ShipmentMode = item.ShipmentMode,
                                    IsLocked = false
                                };

                                //db.Entry(deliv).State = deliv.Id == 0 ?
                                //                            EntityState.Added :
                                //                            EntityState.Modified;

                                db.ShipmentSummDet.Add(deliv);
                                db.SaveChanges();

                            }

                        }



                        dbContextTransaction.Commit();

                        result = new
                        {
                            flag = true,
                            message = "Saving successful!!",
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

        // GET: BuyerOrder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerOrderMas buyerOrderMa = db.BuyerOrderMas.Find(id);
            if (buyerOrderMa == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", buyerOrderMa.BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.OrderBy(x => x.Name), "Id", "Name", buyerOrderMa.BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.BrandId == buyerOrderMa.BrandId).OrderBy(x => x.Name), "Id", "Name", buyerOrderMa.ProdDepartmentId);
            ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.Where(x => x.BuyerInfoId == buyerOrderMa.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", buyerOrderMa.SeasonInfoId);
            //ViewBag.FabSupplierId = new SelectList(db.Supplier.OrderBy(x=>x.Name), "Id", "Name", buyerOrderMa.FabSupplierId);

            var fobType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Style wise", Value = "0" },
                new SelectListItem { Text = "Delivery wise", Value = "1" }, }, "Value", "Text", buyerOrderMa.FobType);
            ViewBag.FOBType = fobType;

            var delivDateType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Handover Date", Value = "0" },
                new SelectListItem { Text = "ETD Date", Value = "1" }, }, "Value", "Text", buyerOrderMa.DeliveryOn);
            ViewBag.DeliveryBased = delivDateType;

            return View(buyerOrderMa);
        }

        // POST: BuyerOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,OrderRefNo,OrderDate,BuyerInfoId,ProdDepartmentId,SeasonInfoId,FabSupplierId,IsAuth,OpBy,OpOn,AuthBy,AuthOn,IsLocked")] BuyerOrderMas buyerOrderMa)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(buyerOrderMa).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", buyerOrderMa.BuyerInfoId);
        //    ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment, "Id", "Name", buyerOrderMa.ProdDepartmentId);
        //    ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo, "Id", "Name", buyerOrderMa.SeasonInfoId);
        //    ViewBag.FabSupplierId = new SelectList(db.Supplier, "Id", "Name", buyerOrderMa.FabSupplierId);
        //    return View(buyerOrderMa);
        //}


        //public JsonResult UpdateBuyerOrder(IEnumerable<VMBuyerOrderDetEdit> OrderDetails, VMBuyerOrderMasEdit OrderMas, int[] DelItems)
        //{
        //    var result = new
        //    {
        //        flag = false,
        //        message = "Error occured. !"
        //    };

        //    //return Json(result, JsonRequestBehavior.AllowGet);

        //    try
        //    {
        //        var OpDate = DateTime.Now;
        //        using (var dbContextTransaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {

        //                var OrderM = db.BuyerOrderMas.Find(OrderMas.Id);

        //                if (OrderM == null)
        //                {
        //                    result = new
        //                    {
        //                        flag = false,
        //                        message = "Invalid Order Id. Saving failed !"
        //                    };

        //                    return Json(result, JsonRequestBehavior.AllowGet);
        //                }

        //                OrderM.OrderRefNo = OrderMas.OrderRefNo;
        //                OrderM.OrderDate = OrderMas.OrderDate;
        //                OrderM.BuyerInfoId = OrderMas.BuyerInfoId;
        //                OrderM.SeasonInfoId = OrderMas.SeasonInfoId;
        //                OrderM.ProdDepartmentId = OrderMas.ProdDepartmentId;
        //                OrderM.FabSupplierId = OrderMas.FabSupplierId;

        //                db.Entry(OrderM).State = EntityState.Modified;
        //                db.SaveChanges();

        //                foreach (var item in OrderDetails)
        //                {
        //                    var OrderD = new BuyerOrderDet()
        //                    {
        //                        Id = item.Id ,
        //                        BuyerOrderMasId = OrderMas.Id,
        //                        ProdCatTypeId = item.ProdCatTypeId,
        //                        StyleNo = item.StyleNo,
        //                        ProdSizeId = item.ProdSizeId,
        //                        ProdColorId = item.ProdColorId,
        //                        FabricItemId = item.FabricItemId,
        //                        SupplierId = item.SupplierId,
        //                        UnitPrice = item.UnitPrice,
        //                        Quantity = item.Quantity,
        //                        IsLocked = false
        //                    };

        //                    db.Entry(OrderD).State = OrderD.Id == 0 ?
        //                                                EntityState.Added :
        //                                                EntityState.Modified;

        //                    //db.BuyerOrderDets.Add(OrderD);
        //                    db.SaveChanges();

        //                }

        //                if (DelItems != null)
        //                {
        //                    foreach (var item in DelItems)
        //                    {
        //                        var delOrder = db.BuyerOrderDet.Find(item);
        //                        db.BuyerOrderDet.Remove(delOrder);
        //                        db.SaveChanges();
        //                    }
        //                }



        //                dbContextTransaction.Commit();

        //                result = new
        //                {
        //                    flag = true,
        //                    message = "Update successful !!"
        //                };

        //                Success("Updated successfully.", true);


        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();

        //                result = new
        //                {
        //                    flag = false,
        //                    message = ex.Message
        //                };
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        result = new
        //        {
        //            flag = false,
        //            message = ex.Message
        //        };
        //    }


        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult UpdateBuyerOrder(IEnumerable<VMBuyerOrderDetEdit> OrderDetails, IEnumerable<VMShipmentSummDet> DelivDetails, VMBuyerOrderMasEdit OrderMas, int[] DelItems, int[] DelDelivItems)
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

                        var OrderM = db.BuyerOrderMas.Find(OrderMas.Id);

                        if (OrderM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Order Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        OrderM.OrderRefNo = OrderMas.OrderRefNo;
                        OrderM.OrderDate = OrderMas.OrderDate;
                        OrderM.BuyerInfoId = OrderMas.BuyerInfoId;
                        OrderM.SeasonInfoId = OrderMas.SeasonInfoId;
                        OrderM.ProdDepartmentId = OrderMas.ProdDepartmentId;
                        // OrderM.FabSupplierId = OrderMas.FabSupplierId;
                        OrderM.BrandId = OrderMas.BrandId;

                        OrderM.FobType = OrderMas.FOBType;
                        OrderM.DeliveryOn = OrderMas.DeliveryBased;

                        db.Entry(OrderM).State = EntityState.Modified;
                        db.SaveChanges();

                        Dictionary<int, int> dictionary =
                                new Dictionary<int, int>();
                        if (OrderDetails != null)
                        {
                            foreach (var item in OrderDetails)
                            {
                                var OrderD = new BuyerOrderDet()
                                {
                                    Id = item.Id,
                                    BuyerOrderMasId = OrderMas.Id,
                                    ProdCatTypeId = item.ProdCatTypeId,
                                    StyleNo = item.StyleNo,
                                    ProdSizeId = item.ProdSizeId,
                                    ProdColorId = item.ProdColorId,
                                    FabricItemDetId = item.FabricItemId, //HERE FabricItemId IS FabricItemDetId
                                    FabricSupplierId = item.FabricSupplierId,
                                    SupplierId = item.SupplierId,
                                    UnitPrice = item.UnitPrice,
                                    Quantity = item.Quantity,
                                    RdlTotal = item.RDLTotal,
                                    IsShipClosed = false,
                                    //ExFactoryDate = item.ExFactoryDate,
                                    IsLocked = false
                                };

                                db.Entry(OrderD).State = OrderD.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                //db.BuyerOrderDets.Add(OrderD);
                                db.SaveChanges();

                                dictionary.Add(item.TempOrderDetId, OrderD.Id);

                            }
                        }

                        var slno = 1;

                        if (DelivDetails != null)
                        {
                            foreach (var item in DelivDetails)
                            {
                                var deliv = new ShipmentSummDet()
                                {
                                    Id = item.Id,
                                    BuyerOrderDetId = item.BuyerOrderDetId == 0 ? dictionary[item.DelivOrderDetTempId] : item.BuyerOrderDetId,
                                    DelivSlno = slno++,
                                    BuyerSlNo = item.BuyerSlNo,
                                    //ExFactoryDate = item.ExFactoryDate,
                                    HandoverDate = item.HandoverDate,
                                    ETD = item.ETD,
                                    DestinationPortId = item.DestinationPortId,
                                    DelivQuantity = item.DelivQuantity,
                                    RdlFOB = item.RdlFobDetailDet,
                                    BuyersPONo = item.BuyersPONo,
                                    ShipmentMode = item.ShipmentMode,
                                    IsLocked = false
                                };

                                db.Entry(deliv).State = deliv.Id == 0 ?
                                                            EntityState.Added :
                                                            EntityState.Modified;

                                //db.BuyerOrderDets.Add(OrderD);
                                db.SaveChanges();

                            }
                        }




                        //--- delete shipment detail items
                        if (DelDelivItems != null)
                        {
                            foreach (var item in DelDelivItems)
                            {
                                var delOrder = db.ShipmentSummDet.Find(item);
                                db.ShipmentSummDet.Remove(delOrder);
                                db.SaveChanges();
                            }
                        }

                        //---- delete order detail items
                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                //---- if order detail deleted without manual deletion of shipment
                                // ---- find those shipment and delete first

                                var shipDets = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == item);

                                if (shipDets != null)
                                {
                                    db.ShipmentSummDet.RemoveRange(shipDets);
                                }

                                var delOrder = db.BuyerOrderDet.Find(item);
                                db.BuyerOrderDet.Remove(delOrder);
                                db.SaveChanges();
                            }
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


        // GET: BuyerOrder/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BuyerOrderMas buyerOrderMa = db.BuyerOrderMas.Find(id);
        //    if (buyerOrderMa == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(buyerOrderMa);
        //}

        // POST: BuyerOrder/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    BuyerOrderMas buyerOrderMa = db.BuyerOrderMas.Find(id);
        //    db.BuyerOrderMas.Remove(buyerOrderMa);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        public JsonResult DeleteOrder(int id)
        {
            //string result = "";

            bool flag = false;
            try
            {

                var itemToDeleteMas = db.BuyerOrderMas.Find(id);

                if (itemToDeleteMas == null)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nBuyer Order Not found."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var checkFactOrder = db.FactoryOrderMas.Where(x => x.BuyerOrderMasId == id).ToList();

                if (checkFactOrder.Count > 0)
                {
                    var result = new
                    {
                        flag = false,
                        message = "Deletion failed!\nFactory order exists. Delete factory order data first."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }


                var itemsToDeleteDet = db.BuyerOrderDet.Where(x => x.BuyerOrderMasId == id);

                foreach (var item in itemsToDeleteDet)
                {
                    var itemsToDeleteDeliv = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == item.Id);
                    db.ShipmentSummDet.RemoveRange(itemsToDeleteDeliv);
                }

                db.BuyerOrderDet.RemoveRange(itemsToDeleteDet);

                db.BuyerOrderMas.Remove(itemToDeleteMas);

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

        public JsonResult GetOrderData(int Id)
        {
            var list = (from orderDet in db.BuyerOrderDet
                        join prodCatType in db.ProdCatType on orderDet.ProdCatTypeId equals prodCatType.Id
                        join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
                        //join size in db.ProdSizes on orderDet.ProdSizeId equals size.Id
                        //join fabItem in db.FabricItems on orderDet.FabricItemId equals fabItem.Id
                        join prodColor in db.ProdColor on orderDet.ProdColorId equals prodColor.Id into ps
                        from color in ps.DefaultIfEmpty()
                        join fabricSupplier in db.FabricSupplier on orderDet.FabricSupplierId equals fabricSupplier.Id into fs
                        from fabSup in fs.DefaultIfEmpty()

                            //join supp in db.Suppliers on orderDet.SupplierId equals supp.Id
                        where orderDet.BuyerOrderMasId == Id
                        select new { orderDet, prodCat, prodCatType, color, fabSup }).AsEnumerable()
                       //select new VMBuyerOrderDetEdit()
                       .Select(x => new
                       {
                           Id = x.orderDet.Id,
                           BuyerOrderMasId = x.orderDet.BuyerOrderMasId,
                           ProdCatId = x.prodCat.Id,
                           //ProdCatName = prodCat.Name,
                           ProdCatTypeId = x.prodCatType.Id,
                           //ProdCatTypeName = prodCatType.Name,
                           StyleNo = x.orderDet.StyleNo,
                           ProdSizeId = x.orderDet.ProdSizeId,
                           //ProdSizeName = size.SizeRange,
                           //FabricItemId = x.orderDet.FabricItemId,
                           RdlFobDet = x.orderDet.UnitPrice,
                           //FabricItemName = fabItem.Name,
                           //ProdColorId = x.orderDet.ProdColorId,
                           ProdColorId = x.color == null ? 0 : x.color.Id,
                           //ProdColorName = prodColor.Name,
                           //UnitPrice = x.orderDet.UnitPrice,
                           Quantity = x.orderDet.Quantity,
                           SupplierId = x.orderDet.SupplierId,

                           //BEFORE ADDING MASTER DETAIL IN FABRIC ITEM TABLE
                           //FabricType = x.orderDet.FabricItem == null ? 0 : x.orderDet.FabricItem.FabricTypeId,
                           //FabricDes = x.orderDet.FabricItem == null ? 0 : x.orderDet.FabricItem.Id,
                           //END

                           //AFTER ADDING MASTER DETAIL IN FABRIC ITEM
                           FabricType = x.orderDet.FabricItemDetId == null ? 0 : x.orderDet.FabricItemDet.FabricItem.FabricTypeId,
                           FabricDes = x.orderDet.FabricItemDetId == null ? 0 : x.orderDet.FabricItemDetId,
                           //END

                           //FabricSupplier = x.orderDet.FabricSupplier.Id
                           FabricSupplier = x.fabSup == null ? 0 : x.fabSup.Id,

                           //SupplierName = supp.Name
                           //ExFactoryDate = x.orderDet.ExFactoryDate.HasValue ? x.orderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy"):""                           
                       });


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderDataLC(int Id)
        {


            var list = (from orderDet in db.BuyerOrderDet
                            //join prodCatType in db.ProdCatType on orderDet.ProdCatTypeId equals prodCatType.Id
                            //join prodCat in db.ProdCategory on prodCatType.ProdCategoryId equals prodCat.Id
                            //join size in db.ProdSizes on orderDet.ProdSizeId equals size.Id
                            //join fabItem in db.FabricItems on orderDet.FabricItemId equals fabItem.Id
                            //join prodColor in db.ProdColors on orderDet.ProdColorId equals prodColor.Id
                        join supp in db.Supplier on orderDet.SupplierId equals supp.Id
                        where orderDet.BuyerOrderMasId == Id
                        select new { orderDet, supp }).AsEnumerable()
                       //select new VMBuyerOrderDetEdit()
                       .Select(x => new
                       {
                           Id = x.orderDet.Id,
                           BuyerOrderMasId = x.orderDet.BuyerOrderMasId,
                           //ProdCatId = x.prodCat.Id,
                           //ProdCatName = prodCat.Name,
                           //ProdCatTypeId = x.prodCatType.Id,
                           //ProdCatTypeName = prodCatType.Name,
                           StyleNo = x.orderDet.StyleNo,
                           //ProdSizeId = x.orderDet.ProdSizeId,
                           //ProdSizeName = size.SizeRange,
                           //FabricItemId = x.orderDet.FabricItemId,
                           //FabricItemName = fabItem.Name,
                           //ProdColorId = x.orderDet.ProdColorId,
                           //ProdColorName = prodColor.Name,
                           //UnitPrice = x.orderDet.UnitPrice,
                           Quantity = x.orderDet.Quantity,
                           TotalValue = x.orderDet.Quantity * x.orderDet.UnitPrice,
                           //SupplierId = x.orderDet.SupplierId,
                           FactoryName = x.supp.Name,
                           //ExFactoryDate = x.orderDet.ExFactoryDate.HasValue ? x.orderDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                           TotalTransValue = (from factOrderDeliv in db.FactoryOrderDelivDet
                                              where factOrderDeliv.FactoryOrderDet.BuyerOrderDetId == x.orderDet.Id
                                              select new { TransValue = (factOrderDeliv.FactTransferPrice == null ? factOrderDeliv.FactFOB : factOrderDeliv.FactTransferPrice) * factOrderDeliv.ShipmentSummDet.DelivQuantity }).Sum(m => m.TransValue)

                       });


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOrderStyleDataLC(int Id)
        {
            var list = (from orderDet in db.BuyerOrderDet
                        join shipSumm in db.ShipmentSummDet on orderDet.Id equals shipSumm.BuyerOrderDetId
                        join FactDelivDet in db.FactoryOrderDelivDet on shipSumm.Id equals FactDelivDet.ShipmentSummDetId
                        join supp in db.Supplier on orderDet.SupplierId equals supp.Id
                        where orderDet.Id == Id
                        select new { orderDet, supp, shipSumm, FactDelivDet }).AsEnumerable()
                       .Select(x => new
                       {
                           Id = x.orderDet.Id, //BuyerOrderDetId
                           StyleNo = x.orderDet.StyleNo,
                           ShipDetId = x.shipSumm.Id, //BuyerOrderDetId
                           DelivNo = x.shipSumm.BuyerSlNo,
                           FactoryName = x.supp.Name,
                           BuyerOrderMasId = x.orderDet.BuyerOrderMasId,
                           BuyerName = x.orderDet.BuyerOrderMas.BuyerInfo.Name,
                           Quantity = x.orderDet.Quantity, //LC Qty
                           DelivQty = x.shipSumm.DelivQuantity,
                           TotalValue = x.orderDet.Quantity * x.orderDet.UnitPrice,
                           TotalTransValue = x.FactDelivDet.FactTransferPrice == null ? (x.FactDelivDet.FactFOB * x.shipSumm.DelivQuantity) : (x.FactDelivDet.FactTransferPrice * x.shipSumm.DelivQuantity),
                           ExFactoryDate = x.FactDelivDet.ExFactoryDate.HasValue ? x.FactDelivDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                           FactFOB = x.FactDelivDet.FactTransferPrice == null ? x.FactDelivDet.FactFOB : x.FactDelivDet.FactTransferPrice
                       });


            return Json(list, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DelivDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderDetails = db.BuyerOrderDet.Find(id);
            if (orderDetails == null)
            {
                return HttpNotFound();
            }
            //ViewBag.BuyerInfoId = new SelectList(db.BuyerInfoes, "Id", "Name", buyerOrderMa.BuyerInfoId);
            //ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartments, "Id", "Name", buyerOrderMa.ProdDepartmentId);
            //ViewBag.SeasonInfoId = new SelectList(db.SeasonInfoes, "Id", "Name", buyerOrderMa.SeasonInfoId);
            //ViewBag.FabSupplierId = new SelectList(db.Suppliers, "Id", "Name", buyerOrderMa.FabSupplierId);
            return View(orderDetails);
        }


        public JsonResult UpdateDeliveryDetail(IEnumerable<ShipmentSummDet> DelivDetails, int Id, int[] DelItems)
        {
            var result = new
            {
                flag = false,
                message = "Error occured. !"
            };

            //return Json(result, JsonRequestBehavior.AllowGet);

            try
            {
                //var OpDate = DateTime.Now;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var OrderM = db.BuyerOrderDet.Find(Id);

                        if (OrderM == null)
                        {
                            result = new
                            {
                                flag = false,
                                message = "Invalid Order Detail Id. Saving failed !"
                            };

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        int slno = 1;
                        foreach (var item in DelivDetails)
                        {
                            var deliv = new ShipmentSummDet()
                            {
                                Id = item.Id,
                                BuyerOrderDetId = Id,
                                DelivSlno = slno++,
                                BuyerSlNo = item.BuyerSlNo,
                                HandoverDate = item.HandoverDate,
                                ETD = item.ETD,
                                DestinationPortId = item.DestinationPortId,
                                DelivQuantity = item.DelivQuantity,
                                RdlFOB = item.RdlFOB,
                                BuyersPONo = item.BuyersPONo,
                                ShipmentMode = item.ShipmentMode,
                                IsLocked = false
                            };

                            db.Entry(deliv).State = deliv.Id == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;

                            //db.BuyerOrderDets.Add(OrderD);
                            db.SaveChanges();

                        }

                        if (DelItems != null)
                        {
                            foreach (var item in DelItems)
                            {
                                var delOrder = db.ShipmentSummDet.Find(item);
                                db.ShipmentSummDet.Remove(delOrder);
                                db.SaveChanges();
                            }
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

        public JsonResult GetDelivData(int Id)
        {

            var list = (from shipDet in db.ShipmentSummDet
                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                        from pJoin in joinDest.DefaultIfEmpty()
                        where shipDet.BuyerOrderDetId == Id
                        select new { shipDet, pJoin }).AsEnumerable()
                     .Select(x => new
                     {
                         Id = x.shipDet.Id,
                         BuyerOrderDetId = x.shipDet.BuyerOrderDetId,
                         DelivSlno = x.shipDet.DelivSlno,
                         ExFactoryDate = x.shipDet.ExFactoryDate.HasValue ? x.shipDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                         HandoverDate = x.shipDet.HandoverDate.HasValue ? x.shipDet.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
                         ETD = x.shipDet.ETD.HasValue ? x.shipDet.ETD.Value.ToString("dd/MM/yyyy") : "",
                         DestinationPortId = x.shipDet.DestinationPortId,
                         DelivQuantity = x.shipDet.DelivQuantity,
                         RDLFOB = x.shipDet.RdlFOB,
                         RDLValue = x.shipDet.RdlFOB * x.shipDet.DelivQuantity,
                         BuyersPONo = x.shipDet.BuyersPONo ?? "",
                         IsLocked = x.shipDet.IsLocked,
                         DestinationPortName = x.pJoin == null ? "" : x.pJoin.Name,
                         ShipmentMode = x.shipDet.ShipmentMode == 0 ? "Sea" : "Air"
                     });

            //var list = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == Id).AsEnumerable()
            //    .Select( x=> new
            //    {
            //        Id = x.Id,
            //        BuyerOrderDetId = x.BuyerOrderDetId,
            //        DelivSlno = x.DelivSlno,
            //        ExFactoryDate = x.ExFactoryDate.HasValue ? x.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
            //        HandoverDate = x.HandoverDate.HasValue ? x.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
            //        ETD = x.ETD.HasValue ? x.ETD.Value.ToString("dd/MM/yyyy") : "",
            //        DestinationPortId = x.DestinationPortId,
            //        DelivQuantity = x.DelivQuantity,
            //        BuyersPONo = x.BuyersPONo ?? "",
            //        IsLocked = x.IsLocked
            //    });

            return Json(list, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetDelivDataEdit(int Id)
        {
            List<VMFactoryOrderDelivList> finalList = new List<VMFactoryOrderDelivList>();

            var list = (from factoryDet in db.FactoryOrderDet
                        join delivDet in db.FactoryOrderDelivDet on factoryDet.Id equals delivDet.FactoryOrderDetId
                        join orderDet in db.BuyerOrderDet on factoryDet.BuyerOrderDetId equals orderDet.Id
                        join shipDet in db.ShipmentSummDet on delivDet.ShipmentSummDetId equals shipDet.Id
                        join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                        from pJoin in joinDest.DefaultIfEmpty()
                        where shipDet.BuyerOrderDetId == Id
                        select new { delivDet, factoryDet, shipDet, pJoin }).AsEnumerable()
                     .Select(x => new
                     {
                         Id = x.delivDet.Id,
                         BuyerOrderDetId = x.factoryDet.BuyerOrderDetId,
                         FactoryOrderDetId = x.factoryDet.Id,
                         DelivSlno = x.shipDet.DelivSlno,
                         ExFactoryDate = x.delivDet.ExFactoryDate.HasValue ? x.delivDet.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                         HandoverDate = x.shipDet.HandoverDate.HasValue ? x.shipDet.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
                         ETD = x.shipDet.ETD.HasValue ? x.shipDet.ETD.Value.ToString("dd/MM/yyyy") : "",
                         DestinationPortId = x.shipDet.DestinationPortId,
                         DelivQuantity = x.shipDet.DelivQuantity,
                         RDLFOB = x.shipDet.RdlFOB,
                         RDLValue = x.shipDet.RdlFOB * x.shipDet.DelivQuantity,
                         FactFOB = x.delivDet.FactFOB ?? 0,
                         FactTransferValue = x.delivDet.FactTransferPrice.HasValue ? (x.delivDet.FactTransferPrice ?? 0) : (x.delivDet.FactFOB ?? 0),
                         BuyersPONo = x.shipDet.BuyersPONo ?? "",
                         IsLocked = x.shipDet.IsLocked,
                         DestinationPortName = x.pJoin == null ? "" : x.pJoin.Name,
                         ShipmentMode = x.shipDet.ShipmentMode == 0 ? "Sea" : "Air",
                         ShipDetId = x.shipDet.Id,
                         Remarks = x.delivDet.Remarks == null ? "" : x.delivDet.Remarks
                         //FactDiscountValue = x.delivDet.DiscountFOB.HasValue ? x.delivDet.DiscountFOB.ToString() : "",
                         //DiscountFlag = x.delivDet.DiscountFlag
                     });

            foreach (var item in list)
            {
                VMFactoryOrderDelivList vm = new VMFactoryOrderDelivList()
                {
                    BuyerOrderDetId = item.BuyerOrderDetId,
                    BuyersPONo = item.BuyersPONo,
                    DelivQuantity = item.DelivQuantity,
                    DelivSlno = item.DelivSlno,
                    DestinationPortId = item.DestinationPortId,
                    DestinationPortName = item.DestinationPortName,
                    ETD = item.ETD,
                    ExFactoryDate = item.ExFactoryDate,
                    FactFOB = item.FactFOB,
                    FactoryOrderDetId = item.FactoryOrderDetId,
                    FactTransferValue = item.FactTransferValue,
                    HandoverDate = item.HandoverDate,
                    Id = item.Id,
                    IsLocked = item.IsLocked,
                    RDLFOB = item.RDLFOB,
                    RDLValue = item.RDLValue,
                    ShipmentMode = item.ShipmentMode,
                    ShipDetId = item.ShipDetId,
                    Remarks = item.Remarks
                };

                finalList.Add(vm);
            }


            var buyerDetDeliv = (from orderDet in db.BuyerOrderDet
                                 join shipDet in db.ShipmentSummDet on orderDet.Id equals shipDet.BuyerOrderDetId
                                 join dest in db.DestinationPort on shipDet.DestinationPortId equals dest.Id into joinDest
                                 from pJoin in joinDest.DefaultIfEmpty()
                                     //join delivDet in db.FactoryOrderDelivDet on shipDet.Id equals delivDet.ShipmentSummDetId into joinFact
                                     //   from delivJoin in joinFact.DefaultIfEmpty()
                                 where shipDet.BuyerOrderDetId == Id && shipDet.Id != (db.FactoryOrderDelivDet.FirstOrDefault(x => x.ShipmentSummDetId == shipDet.Id).ShipmentSummDetId ?? 0)
                                 select new { orderDet, shipDet, pJoin }).AsEnumerable()
                     .Select(x => new
                     {
                         Id = 0,
                         BuyerOrderDetId = x.orderDet.Id,
                         FactoryOrderDetId = 0,
                         DelivSlno = x.shipDet.DelivSlno,
                         ExFactoryDate = "",
                         HandoverDate = x.shipDet.HandoverDate.HasValue ? x.shipDet.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
                         ETD = x.shipDet.ETD.HasValue ? x.shipDet.ETD.Value.ToString("dd/MM/yyyy") : "",
                         DestinationPortId = x.shipDet.DestinationPortId,
                         DelivQuantity = x.shipDet.DelivQuantity,
                         RDLFOB = x.shipDet.RdlFOB,
                         RDLValue = x.shipDet.RdlFOB * x.shipDet.DelivQuantity,
                         FactFOB = (decimal)0,
                         FactTransferValue = (decimal)0,
                         BuyersPONo = x.shipDet.BuyersPONo ?? "",
                         IsLocked = x.shipDet.IsLocked,
                         DestinationPortName = x.pJoin == null ? "" : x.pJoin.Name,
                         ShipmentMode = x.shipDet.ShipmentMode == 0 ? "Sea" : "Air",
                         ShipDetId = x.shipDet.Id
                         //FactDiscountValue = x.delivDet.DiscountFOB.HasValue ? x.delivDet.DiscountFOB.ToString() : "",
                         //DiscountFlag = x.delivDet.DiscountFlag
                     });

            //list.Union(buyerDetDeliv);

            foreach (var item in buyerDetDeliv)
            {
                VMFactoryOrderDelivList vm = new VMFactoryOrderDelivList()
                {
                    BuyerOrderDetId = item.BuyerOrderDetId,
                    BuyersPONo = item.BuyersPONo,
                    DelivQuantity = item.DelivQuantity,
                    DelivSlno = item.DelivSlno,
                    DestinationPortId = item.DestinationPortId,
                    DestinationPortName = item.DestinationPortName,
                    ETD = item.ETD,
                    ExFactoryDate = item.ExFactoryDate,
                    FactFOB = item.FactFOB,
                    FactoryOrderDetId = item.FactoryOrderDetId,
                    FactTransferValue = item.FactTransferValue,
                    HandoverDate = item.HandoverDate,
                    Id = item.Id,
                    IsLocked = item.IsLocked,
                    RDLFOB = item.RDLFOB,
                    RDLValue = item.RDLValue,
                    ShipmentMode = item.ShipmentMode,
                    ShipDetId = item.ShipDetId
                };

                finalList.Add(vm);
            }


            return Json(finalList, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetDelivDataByOrderMasId(int Id)
        //{

        //    var list = (from orderDet in db.BuyerOrderDet
        //               join delivDet in db.ShipmentSummDet on orderDet.Id equals delivDet.BuyerOrderDetId
        //               where orderDet.BuyerOrderMasId == Id
        //               select delivDet).AsEnumerable()
        //               .Select(x => new
        //               {
        //                   Id = x.Id,
        //                   BuyerOrderDetId = x.BuyerOrderDetId,
        //                   BuyMasId= x.BuyerOrderDet.BuyerOrderMasId,
        //                   DelivSlno = x.DelivSlno,
        //                   ExFactoryDate = x.ExFactoryDate.HasValue ? x.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
        //                   HandoverDate = x.HandoverDate.HasValue ? x.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
        //                   ETD = x.ETD.HasValue ? x.ETD.Value.ToString("dd/MM/yyyy") : "",
        //                   DestinationPortId = x.DestinationPortId,
        //                   DelivQuantity = x.DelivQuantity,
        //                   BuyersPONo = x.BuyersPONo ?? "",
        //                   IsLocked = x.IsLocked,
        //                   BuyerSlNo=x.BuyerSlNo,
        //                   RdlFobDetailDet=x.RdlFOB,
        //                   ShipmentModeValue= x.ShipmentMode,
        //                   ShipmentMode = x.ShipmentMode
        //                   //ShipmentMode=x.ShipmentMode==0? "Sea": "Air"
        //               });
        //    //var list = from orderDet in db.BuyerOrderDets
        //    //           join prodCatType in db.ProdCatTypes on orderDet.ProdCatTypeId equals prodCatType.Id
        //    //           join prodCat in db.ProdCategories on prodCatType.ProdCategoryId equals prodCat.Id
        //    //           //join size in db.ProdSizes on orderDet.ProdSizeId equals size.Id
        //    //           //join fabItem in db.FabricItems on orderDet.FabricItemId equals fabItem.Id
        //    //           //join prodColor in db.ProdColors on orderDet.ProdColorId equals prodColor.Id
        //    //           //join supp in db.Suppliers on orderDet.SupplierId equals supp.Id
        //    //           where orderDet.BuyerOrderMasId == Id
        //    //           select new VMBuyerOrderDetEdit()
        //    //           {
        //    //               Id = orderDet.Id,
        //    //               BuyerOrderMasId = orderDet.BuyerOrderMasId,
        //    //               ProdCatId = prodCat.Id,
        //    //               //ProdCatName = prodCat.Name,
        //    //               ProdCatTypeId = prodCatType.Id,
        //    //               //ProdCatTypeName = prodCatType.Name,
        //    //               StyleNo = orderDet.StyleNo,
        //    //               ProdSizeId = orderDet.ProdSizeId,
        //    //               //ProdSizeName = size.SizeRange,
        //    //               FabricItemId = orderDet.FabricItemId,
        //    //               //FabricItemName = fabItem.Name,
        //    //               ProdColorId = orderDet.ProdColorId,
        //    //               //ProdColorName = prodColor.Name,
        //    //               UnitPrice = orderDet.UnitPrice,
        //    //               Quantity = orderDet.Quantity,
        //    //               SupplierId = orderDet.SupplierId,
        //    //               //SupplierName = supp.Name

        //    //           };

        //    //var list = db.ShipmentSummDet.Where(x => x.BuyerOrderDetId == Id).AsEnumerable()
        //    //    .Select(x => new
        //    //    {
        //    //        Id = x.Id,
        //    //        BuyerOrderDetId = x.BuyerOrderDetId,
        //    //        DelivSlno = x.DelivSlno,
        //    //        ExFactoryDate = x.ExFactoryDate.HasValue ? x.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
        //    //        HandoverDate = x.HandoverDate.HasValue ? x.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
        //    //        ETD = x.ETD.HasValue ? x.ETD.Value.ToString("dd/MM/yyyy") : "",
        //    //        DestinationPortId = x.DestinationPortId,
        //    //        DelivQuantity = x.DelivQuantity,
        //    //        BuyersPONo = x.BuyersPONo ?? "",
        //    //        IsLocked = x.IsLocked
        //    //    });

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}



        public JsonResult GetDelivDataByOrderDetId(int Id)
        {

            var list = (from orderDet in db.BuyerOrderDet
                        join delivDet in db.ShipmentSummDet on orderDet.Id equals delivDet.BuyerOrderDetId
                        where orderDet.Id == Id
                        select delivDet).AsEnumerable()
                       .Select(x => new
                       {
                           Id = x.Id,
                           BuyerOrderDetId = x.BuyerOrderDetId,
                           BuyMasId = x.BuyerOrderDet.BuyerOrderMasId,
                           DelivSlno = x.DelivSlno,
                           ExFactoryDate = x.ExFactoryDate.HasValue ? x.ExFactoryDate.Value.ToString("dd/MM/yyyy") : "",
                           HandoverDate = x.HandoverDate.HasValue ? x.HandoverDate.Value.ToString("dd/MM/yyyy") : "",
                           ETD = x.ETD.HasValue ? x.ETD.Value.ToString("dd/MM/yyyy") : "",
                           DestinationPortId = x.DestinationPortId,
                           DelivQuantity = x.DelivQuantity,
                           BuyersPONo = x.BuyersPONo ?? "",
                           IsLocked = x.IsLocked,
                           BuyerSlNo = x.BuyerSlNo,
                           RdlFobDetailDet = x.RdlFOB,
                           ShipmentModeValue = x.ShipmentMode,
                           ShipmentMode = x.ShipmentMode
                           //ShipmentMode=x.ShipmentMode==0? "Sea": "Air"
                       });

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetShipmentModeDeliv(int Id)
        {

            // var 

            var enumData = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Sea", Value = "0" }, new SelectListItem { Text = "Air", Value = "1" }, }, "Value", "Text");



            //var list = (from orderDet in db.BuyerOrderDet
            //            join delivDet in db.ShipmentSummDet on orderDet.Id equals delivDet.BuyerOrderDetId
            //           // where orderDet.BuyerOrderMasId == Id
            //            where orderDet.Id == Id
            //            select delivDet).AsEnumerable()
            //           .Select(x => new
            //           {
            //               ShipmentModeValue = x.ShipmentMode,
            //               ShipmentMode = x.ShipmentMode == 0 ? "Sea" : "Air",
            //                   }).Distinct();

            return Json(enumData, JsonRequestBehavior.AllowGet);

            //var result = new
            //{
            //    Items = list,
            //    EnumData= enumData

            //};
            //return Json(result, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ImageGallery(int Id)
        {
            ViewBag.OrderDetId = Id;

            string fullPath = Request.MapPath("~/Uploads/OrderImages/" + Id.ToString());
            if (Directory.Exists(fullPath))
            {
                string[] dirs = Directory.GetFiles(fullPath + "\\", "*");
                ViewBag.Files = dirs;
            }
            else
            {
                string[] dirs = Array.Empty<string>();

                ViewBag.Files = dirs;
            }


            return View();
        }

        public ActionResult ImageUpload(int Id)
        {
            var orderDet = db.BuyerOrderDet.Find(Id);

            if (orderDet == null)
            {
                return HttpNotFound();
            }

            return View(orderDet);
        }

        [HttpPost]
        public ActionResult Upload(int Id)
        {

            bool isSavedSuccessfully = true;
            // bool saveinfolder = false;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Path.Combine(Server.MapPath("~/Uploads/OrderImages/" + Id.ToString()));
                        string pathString = System.IO.Path.Combine(path.ToString());
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = System.IO.Directory.Exists(pathString);
                        if (!isExists)
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                        }


                        var uploadpath = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(uploadpath);


                    }
                }
            }
            catch (Exception)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new
                {
                    Message = fName
                });
            }
            else
            {
                return Json(new
                {
                    Message = "Error in saving file"
                });
            }
        }

        public JsonResult GetFactoryNames(int Id)
        {
            var data = (from orderDet in db.BuyerOrderDet
                        join fact in db.Supplier on orderDet.SupplierId equals fact.Id
                        where orderDet.BuyerOrderMasId == Id
                        select new { Id = fact.Id, Name = fact.Name }).Distinct();

            //var data = db.ProdSize.Where(x => x.ProdCatTypeId == Id).Select(y => new { Name = y.SizeRange, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOrderMasterInfo(int Id)
        {
            //var data = db.BuyerOrderMas.Find(Id);

            var data = (from bOrder in db.BuyerOrderMas
                        join buyer in db.BuyerInfo on bOrder.BuyerInfoId equals buyer.Id
                        join dept in db.ProdDepartment on bOrder.ProdDepartmentId equals dept.Id
                        where bOrder.Id == Id
                        select new { bOrder, buyer, dept }).SingleOrDefault();

            var result = new
            {
                OrderDate = data.bOrder.OrderDate.Value.ToString("dd/MM/yyyy"),
                BuyerName = data.buyer.Name,
                DeptName = data.dept.Name
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOrderNos(int Id)
        {
            var data = db.BuyerOrderMas.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.OrderRefNo).Select(y => new { Name = y.OrderRefNo, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
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
