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
    public class BuyerInfoController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: BuyerInfo
        public ActionResult Index()
        {
            //var buyerInfoes = db.BuyerInfo.Include(b => b.BankBranch).Include(b => b.CompanyResource).Include(b => b.CountryInfo).Include(b => b.MiddleParty);
            var buyerInfoes = db.BuyerInfo;
            return View(buyerInfoes.ToList());
        }

        // GET: BuyerInfo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerInfo buyerInfo = db.BuyerInfo.Find(id);
            if (buyerInfo == null)
            {
                return HttpNotFound();
            }
           
            return View(buyerInfo);
        }

        // GET: BuyerInfo/Create
        public ActionResult Create()
        {
            ViewBag.BankBranchId = new SelectList(db.BankBranch.Where(x => x.IsForeign == true).Select(x => new { Id = x.Id, Name = x.Name + " - " + x.BranchName }), "Id", "Name");
            ViewBag.MerchandiserId = new SelectList(db.CompanyResources, "Id", "Name");
            ViewBag.CountryInfoId = new SelectList(db.CountryInfo, "Id", "Name");
            ViewBag.MiddlePartyId = new SelectList(db.MiddleParty, "Id", "Name");
            //List<SelectListItem> paymentType = new List<SelectListItem>()
            //{
            //    new SelectListItem { Text = "LC", Value = "0" },
            //    new SelectListItem { Text = "TT", Value = "1" }
            //};
            //ViewBag.PaymentTypeId = paymentType;
            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text");
            ViewBag.PaymentTypeId = paymentType;
            //List<SelectListItem> paymentTerm = new List<SelectListItem>()
            //{
            //    new SelectListItem { Text = "At Sight", Value = "0" },
            //    new SelectListItem { Text = "From BL", Value = "1" }
            //};
            //ViewBag.PaymentTerm = paymentTerm;

            var paymentTerm = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "At Sight", Value = "0" }, new SelectListItem { Text = "From BL", Value = "1" }, }, "Value", "Text");
            ViewBag.PaymentTerm = paymentTerm;

            return View();
        }

        // POST: BuyerInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,NameShort,Address,ContactPerson,ContactPhone,CountryInfoId,MerchandiserId,MiddlePartyId,BankBranchId,PaymentTypeId,Tenor,PaymentTerm,BuyerGroup,OpeningBalance,BalanceDate")] BuyerInfo buyerInfo)
        {
            if (ModelState.IsValid)
            {

                if (db.BuyerInfo.Where(x=>x.Name==buyerInfo.Name).Count()>0)
                {
                    Danger("Same name exists!! Try different.",true);
                }
                else
                {
                    buyerInfo.OpBy = 1;
                    buyerInfo.OpOn = DateTime.Now;
                    db.BuyerInfo.Add(buyerInfo);
                    db.SaveChanges();
                    Success("Saved successfully !!", true);
                    return RedirectToAction("Index");
                }

                
            }

            ViewBag.BankBranchId = new SelectList(db.BankBranch.Where(x => x.IsForeign == true).Select(x=> new { Id = x.Id, Name = x.Name + " - " + x.BranchName}), "Id", "Name", buyerInfo.BankBranchId);
            ViewBag.MerchandiserId = new SelectList(db.CompanyResources, "Id", "Name", buyerInfo.MerchandiserId);
            ViewBag.CountryInfoId = new SelectList(db.CountryInfo, "Id", "Name", buyerInfo.CountryInfoId);
            ViewBag.MiddlePartyId = new SelectList(db.MiddleParty, "Id", "Name", buyerInfo.MiddlePartyId);

            var paymentType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "LC", Value = "0" },
                    new SelectListItem { Text = "TT", Value = "1" },
                }, "Value", "Text", buyerInfo.PaymentTypeId);

            ViewBag.PaymentTypeId = paymentType;

            var paymentTerm = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "At Sight", Value = "0" },
                    new SelectListItem { Text = "From BL", Value = "1" },
                }, "Value", "Text", buyerInfo.PaymentTerm);

            ViewBag.PaymentTerm = paymentTerm;

            return View(buyerInfo);
        }

        // GET: BuyerInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerInfo buyerInfo = db.BuyerInfo.Find(id);
            if (buyerInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankBranchId = new SelectList(db.BankBranch.Where(x=>x.IsForeign==true).Select(x=> new { Id = x.Id, Name = x.Name + " - " + x.BranchName}), "Id", "Name", buyerInfo.BankBranchId);
            ViewBag.MerchandiserId = new SelectList(db.CompanyResources, "Id", "Name", buyerInfo.MerchandiserId);
            ViewBag.CountryInfoId = new SelectList(db.CountryInfo, "Id", "Name", buyerInfo.CountryInfoId);
            ViewBag.MiddlePartyId = new SelectList(db.MiddleParty, "Id", "Name", buyerInfo.MiddlePartyId);
            //List<SelectListItem> paymentType = new List<SelectListItem>()
            //{
            //    new SelectListItem { Text = "LC", Value = "0" },
            //    new SelectListItem { Text = "TT", Value = "1" }
            //};
            var paymentType = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "LC", Value = "0" }, new SelectListItem { Text = "TT", Value = "1" }, }, "Value", "Text", buyerInfo.PaymentTypeId);
            ViewBag.PaymentTypeId = paymentType;

            //List<SelectListItem> paymentTerm = new List<SelectListItem>()
            //{
            //    new SelectListItem { Text = "At Sight", Value = "0" },
            //    new SelectListItem { Text = "From BL", Value = "1" }

            //};
            //ViewBag.PaymentTerm = paymentTerm;
            var paymentTerm = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "At Sight", Value = "0" }, new SelectListItem { Text = "From BL", Value = "1" }, }, "Value", "Text",buyerInfo.PaymentTerm);
            ViewBag.PaymentTerm = paymentTerm;

            return View(buyerInfo);
        }

        // POST: BuyerInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,NameShort,Address,ContactPerson,ContactPhone,CountryInfoId,MerchandiserId,MiddlePartyId,BankBranchId,PaymentTypeId,Tenor,PaymentTerm,BuyerGroup,OpeningBalance,BalanceDate")] BuyerInfo buyerInfo)
        {
            if (ModelState.IsValid)
            {

                if (db.BuyerInfo.Where(x => x.Name == buyerInfo.Name && x.Id != buyerInfo.Id).Count() > 0)
                {
                    Danger("Same name exists!! Try different.", true);
                }
                else
                {
                    buyerInfo.OpBy = 1;
                    buyerInfo.OpOn = DateTime.Now;
                    db.Entry(buyerInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully !!", true);
                    return RedirectToAction("Index");
                }

                
            }
            ViewBag.BankBranchId = new SelectList(db.BankBranch.Where(x => x.IsForeign == true).Select(x => new { Id = x.Id, Name = x.Name + " - " + x.BranchName }), "Id", "Name", buyerInfo.BankBranchId);
            ViewBag.MerchandiserId = new SelectList(db.CompanyResources, "Id", "Name", buyerInfo.MerchandiserId);
            ViewBag.CountryInfoId = new SelectList(db.CountryInfo, "Id", "Name", buyerInfo.CountryInfoId);
            ViewBag.MiddlePartyId = new SelectList(db.MiddleParty, "Id", "Name", buyerInfo.MiddlePartyId);

            var paymentType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "LC", Value = "0" },
                    new SelectListItem { Text = "TT", Value = "1" },
                }, "Value", "Text", buyerInfo.PaymentTypeId);

            ViewBag.PaymentTypeId = paymentType;

            var paymentTerm = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "At Sight", Value = "0" },
                    new SelectListItem { Text = "From BL", Value = "1" },
                }, "Value", "Text", buyerInfo.PaymentTerm);

            ViewBag.PaymentTerm = paymentTerm;

            return View(buyerInfo);
        }

        // GET: BuyerInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BuyerInfo buyerInfo = db.BuyerInfo.Find(id);
            if (buyerInfo == null)
            {
                return HttpNotFound();
            }
            return View(buyerInfo);
        }

        // POST: BuyerInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BuyerInfo buyerInfo = db.BuyerInfo.Find(id);
            db.BuyerInfo.Remove(buyerInfo);
            db.SaveChanges();
            Success("Deleted successfully !!", true);
            return RedirectToAction("Index");
        }


        public JsonResult GetNames()
        {
            var data = db.BuyerInfo.OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNamesByType(int typeId)
        {
            var data = db.BuyerInfo.OrderBy(x => x.Name)
                .Where(x=>x.PaymentTypeId== typeId)
                .AsEnumerable().Select(y => new { Name = y.Name, Id = y.Id }).Distinct().ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetBankNames(int Id)
        {
            var data="";
            var buyer = db.BuyerInfo.SingleOrDefault(x => x.Id == Id).BankBranchId;
            var Bankdata = db.BankBranch.Where(x => x.Id == buyer).ToList();
            if(Bankdata.Count>0)
            {
                data = Bankdata.FirstOrDefault().Name;
            }
            else
            {
                data = "";
            }
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
