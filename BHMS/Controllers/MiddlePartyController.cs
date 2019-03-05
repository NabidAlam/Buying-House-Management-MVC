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
    public class MiddlePartyController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: MiddleParty
        public ActionResult Index()
        {
            return View(db.MiddleParty.ToList());
        }

        // GET: MiddleParty/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MiddleParty middleParty = db.MiddleParty.Find(id);
            if (middleParty == null)
            {
                return HttpNotFound();
            }
            return View(middleParty);
        }

        // GET: MiddleParty/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MiddleParty/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] MiddleParty middleParty)
        {
            if (ModelState.IsValid)
            {

                if (db.MiddleParty.Where(x => x.Name == middleParty.Name).Count() > 0)
                {
                    Danger("Name is alreay exist. Try different name.");
                }
                else
                {
                    middleParty.OpBy = 1;
                    middleParty.OpOn = DateTime.Now;
                    db.MiddleParty.Add(middleParty);
                    db.SaveChanges();
                    Success("Saved successfully.", true);
                    return RedirectToAction("Index");
                }

                
            }

            return View(middleParty);
        }

        // GET: MiddleParty/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MiddleParty middleParty = db.MiddleParty.Find(id);
            if (middleParty == null)
            {
                return HttpNotFound();
            }
            return View(middleParty);
        }

        // POST: MiddleParty/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] MiddleParty middleParty)
        {
            if (ModelState.IsValid)
            {

                if (db.MiddleParty.Where(x => x.Name == middleParty.Name && x.Id != middleParty.Id).Count() > 0)
                {
                    Danger("Name is alreay exist. Try different name.");
                }
                else
                {
                    middleParty.OpBy = 1;
                    middleParty.OpOn = DateTime.Now;
                    db.Entry(middleParty).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully.", true);
                    return RedirectToAction("Index");
                }

                
            }
            return View(middleParty);
        }

        // GET: MiddleParty/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MiddleParty middleParty = db.MiddleParty.Find(id);
            if (middleParty == null)
            {
                return HttpNotFound();
            }
            return View(middleParty);
        }

        // POST: MiddleParty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MiddleParty middleParty = db.MiddleParty.Find(id);
            db.MiddleParty.Remove(middleParty);
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
