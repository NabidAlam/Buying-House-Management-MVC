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
    public class BankBranchController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: BankBranch
        public ActionResult Index()
        {
            return View(db.BankBranch.ToList());
        }

        // GET: BankBranch/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankBranch bankBranch = db.BankBranch.Find(id);
            if (bankBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankBranch);
        }

        // GET: BankBranch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankBranch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,BranchName,Phone,ContactPerson,SwiftCode,IsForeign,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] BankBranch bankBranch)
        {
            if (ModelState.IsValid)
            {

                if (db.BankBranch.Where(x => x.Name == bankBranch.Name && x.BranchName == bankBranch.BranchName).Count() > 0)
                {
                    Danger("Name exists. Try different.", true);
                }
                else
                {
                    bankBranch.OpBy = 1;
                    bankBranch.OpOn = DateTime.Now;
                    db.BankBranch.Add(bankBranch);
                    db.SaveChanges();
                    Success("Saved successfully.", true);
                    return RedirectToAction("Index");
                }

                
            }

            return View(bankBranch);
        }

        // GET: BankBranch/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankBranch bankBranch = db.BankBranch.Find(id);
            if (bankBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankBranch);
        }

        // POST: BankBranch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,BranchName,Phone,ContactPerson,SwiftCode,IsForeign,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] BankBranch bankBranch)
        {
            if (ModelState.IsValid)
            {

                if (db.BankBranch.Where(x => x.Name == bankBranch.Name && x.BranchName == bankBranch.BranchName && x.Id != bankBranch.Id).Count() > 0)
                {
                    Danger("Name exists. Try different.", true);
                }
                else
                {
                    bankBranch.OpBy = 1;
                    bankBranch.OpOn = DateTime.Now;
                    db.Entry(bankBranch).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully.", true);
                    return RedirectToAction("Index");
                }

                
            }
            return View(bankBranch);
        }

        // GET: BankBranch/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankBranch bankBranch = db.BankBranch.Find(id);
            if (bankBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankBranch);
        }

        // POST: BankBranch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankBranch bankBranch = db.BankBranch.Find(id);
            db.BankBranch.Remove(bankBranch);
            db.SaveChanges();
            Success("Deleted successfully.", true);
            return RedirectToAction("Index");
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
