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
    public class FabricSuppliersController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: FabricSuppliers
        public ActionResult Index()
        {
            return View(db.FabricSupplier.ToList());
        }


        public JsonResult GetNames()
        {
            var data = db.FabricSupplier.OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        // GET: FabricSuppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricSupplier fabricSupplier = db.FabricSupplier.Find(id);
            if (fabricSupplier == null)
            {
                return HttpNotFound();
            }
            return View(fabricSupplier);
        }

        // GET: FabricSuppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FabricSuppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] FabricSupplier fabricSupplier)
        {
            if (ModelState.IsValid)
            {
                db.FabricSupplier.Add(fabricSupplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fabricSupplier);
        }

        // GET: FabricSuppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricSupplier fabricSupplier = db.FabricSupplier.Find(id);
            if (fabricSupplier == null)
            {
                return HttpNotFound();
            }
            return View(fabricSupplier);
        }

        // POST: FabricSuppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] FabricSupplier fabricSupplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fabricSupplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fabricSupplier);
        }

        // GET: FabricSuppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricSupplier fabricSupplier = db.FabricSupplier.Find(id);
            if (fabricSupplier == null)
            {
                return HttpNotFound();
            }
            return View(fabricSupplier);
        }

        // POST: FabricSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FabricSupplier fabricSupplier = db.FabricSupplier.Find(id);
            db.FabricSupplier.Remove(fabricSupplier);
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
