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
    public class DestinationPortController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: DestinationPort
        public ActionResult Index()
        {
            return View(db.DestinationPort.ToList());
        }

        // GET: DestinationPort/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DestinationPort destinationPort = db.DestinationPort.Find(id);
            if (destinationPort == null)
            {
                return HttpNotFound();
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", destinationPort.BuyerInfoId);
            return View(destinationPort);
        }

        // GET: DestinationPort/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: DestinationPort/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BuyerInfoId,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DestinationPort destinationPort)
        {
            if (ModelState.IsValid)
            {
                if (db.DestinationPort.Where(x =>x.BuyerInfoId == destinationPort.BuyerInfoId &&  x.Name.ToLower() == destinationPort.Name.ToLower()).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    destinationPort.OpBy = 1;
                    destinationPort.OpOn = DateTime.Now;
                    db.DestinationPort.Add(destinationPort);
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", destinationPort.BuyerInfoId);
            return View(destinationPort);
        }

        // GET: DestinationPort/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DestinationPort destinationPort = db.DestinationPort.Find(id);
            if (destinationPort == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", destinationPort.BuyerInfoId);
            return View(destinationPort);
        }

        // POST: DestinationPort/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] DestinationPort destinationPort)
        {
            if (ModelState.IsValid)
            {
                if (db.DestinationPort.Where(x =>x.BuyerInfoId== destinationPort.BuyerInfoId &&  x.Name.ToLower() == destinationPort.Name.ToLower() && x.Id!=destinationPort.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    destinationPort.OpBy = 1;
                    destinationPort.OpOn = DateTime.Now;
                    db.Entry(destinationPort).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", destinationPort.BuyerInfoId);
            return View(destinationPort);
        }

        // GET: DestinationPort/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DestinationPort destinationPort = db.DestinationPort.Find(id);
            if (destinationPort == null)
            {
                return HttpNotFound();
            }
            return View(destinationPort);
        }

        // POST: DestinationPort/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DestinationPort destinationPort = db.DestinationPort.Find(id);
            db.DestinationPort.Remove(destinationPort);
            db.SaveChanges();
            Success("Deleted successfully!", true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames()
        {
            var data = db.DestinationPort.OrderBy(x=>x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();           
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewNames(int id)
        {
            var data = db.DestinationPort.Where(x=>x.BuyerInfoId==id).OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();
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
