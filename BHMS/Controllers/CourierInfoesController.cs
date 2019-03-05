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
    public class CourierInfoesController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: CourierInfoes
        public ActionResult Index()
        {
            return View(db.CourierInfo.ToList());
        }

        // GET: CourierInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourierInfo courierInfo = db.CourierInfo.Find(id);
            if (courierInfo == null)
            {
                return HttpNotFound();
            }
            return View(courierInfo);
        }

        // GET: CourierInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourierInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CourierInfo courierInfo)
        {
            if (ModelState.IsValid)
            {
                if (db.CourierInfo.Where(x =>  x.Name.ToLower() == courierInfo.Name.ToLower()).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    courierInfo.OpBy = 1;
                    courierInfo.OpOn = DateTime.Now;
                    db.CourierInfo.Add(courierInfo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(courierInfo);
        }

        // GET: CourierInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourierInfo courierInfo = db.CourierInfo.Find(id);
            if (courierInfo == null)
            {
                return HttpNotFound();
            }
            return View(courierInfo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] CourierInfo courierInfo)
        {
            if (ModelState.IsValid)
            {
                if (db.CourierInfo.Where(x => x.Name.ToLower() == courierInfo.Name.ToLower() && x.Id != courierInfo.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    courierInfo.OpBy = 1;
                    courierInfo.OpOn = DateTime.Now;
                    db.Entry(courierInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(courierInfo);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourierInfo courierInfo = db.CourierInfo.Find(id);
            if (courierInfo == null)
            {
                return HttpNotFound();
            }
            return View(courierInfo);
        }

        // POST: CourierInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourierInfo courierInfo = db.CourierInfo.Find(id);
            db.CourierInfo.Remove(courierInfo);
            db.SaveChanges();
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
