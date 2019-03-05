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
    public class BrandController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: Brand
        public ActionResult Index()
        {
            var brand = db.Brand.Include(b => b.BuyerInfo);
            return View(brand.ToList());
        }

        // GET: Brand/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // GET: Brand/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name");
            return View();
        }

        // POST: Brand/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,BuyerInfoId")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                if (db.Brand.Where(x => x.BuyerInfoId == brand.BuyerInfoId && x.Name.ToLower() == brand.Name.ToLower()).Count() > 0)
                {
                    Danger("Name exists! Try different", true);
                }
                else
                {
                    db.Brand.Add(brand);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", brand.BuyerInfoId);
            return View(brand);
        }

        // GET: Brand/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", brand.BuyerInfoId);
            return View(brand);
        }

        // POST: Brand/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,BuyerInfoId")] Brand brand)
        {
            if (ModelState.IsValid)
            {

                if (db.Brand.Where(x => x.BuyerInfoId == brand.BuyerInfoId && x.Name.ToLower() == brand.Name.ToLower() && x.Id != brand.Id).Count() > 0)
                {
                    Danger("Name exists! Try different", true);
                }
                else
                {
                    db.Entry(brand).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", brand.BuyerInfoId);
            return View(brand);
        }

        // GET: Brand/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brand.Find(id);
            db.Brand.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult GetNames(int Id)
        {
            var data = db.Brand.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
