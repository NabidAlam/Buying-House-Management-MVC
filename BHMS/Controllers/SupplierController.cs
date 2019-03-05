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
    public class SupplierController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: Supplier
        public ActionResult Index()
        {
            //var suppliers = db.Supplier.Include(s => s.BankBranch);
            var suppliers = db.Supplier;
            return View(suppliers.ToList());
        }

        // GET: Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            ViewBag.BankBranchId = new SelectList(db.BankBranch.OrderBy(x=>x.Name).Select(x=>new { Id= x.Id, Name= x.Name + " - " + x.BranchName }), "Id", "Name");
            return View();
        }

        // POST: Supplier/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ShortName,Address,Phone,ContactPerson,BankBranchId,AccNo,OpeningBalance,BalanceDate")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (db.Supplier.Where(x=>x.Name.ToLower() ==supplier.Name.ToLower()).Count()>0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    supplier.OpBy = 1;
                    supplier.OpOn = DateTime.Now;
                    db.Supplier.Add(supplier);
                    db.SaveChanges();
                    Success("Saved successfully", true);
                    return RedirectToAction("Index");
                }
               
            }

            ViewBag.BankBranchId = new SelectList(db.BankBranch.OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name = x.Name + " - " + x.BranchName }), "Id", "Name", supplier.BankBranchId);
            return View(supplier);
        }

        // GET: Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankBranchId = new SelectList(db.BankBranch.OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name = x.Name + " - " + x.BranchName }), "Id", "Name", supplier.BankBranchId);
            return View(supplier);
        }

        // POST: Supplier/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortName,Address,Phone,ContactPerson,BankBranchId,AccNo,OpeningBalance,BalanceDate")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (db.Supplier.Where(x => x.Name == supplier.Name && x.Id!=supplier.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    supplier.OpBy = 1;
                    supplier.OpOn = DateTime.Now;
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully", true);
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.BankBranchId = new SelectList(db.BankBranch.OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name = x.Name + " - " + x.BranchName }), "Id", "Name", supplier.BankBranchId);
            return View(supplier);
        }

        // GET: Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Supplier.Find(id);
            db.Supplier.Remove(supplier);
            db.SaveChanges();
            Success("Deleted successfully", true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames()
        {
            var data = db.Supplier.OrderBy(x=>x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
