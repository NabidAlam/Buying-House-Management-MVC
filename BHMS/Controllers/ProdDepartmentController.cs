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
    public class ProdDepartmentController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ProdDepartment
        public ActionResult Index()
        {
            var prodDepartments = db.ProdDepartment.Include(p => p.Brand);
            return View(prodDepartments.ToList());
        }

        // GET: ProdDepartment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdDepartment prodDepartment = db.ProdDepartment.Find(id);
            if (prodDepartment == null)
            {
                return HttpNotFound();
            }
            return View(prodDepartment);
        }

        // GET: ProdDepartment/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfo = new SelectList(db.BuyerInfo, "Id", "Name");
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            return View();
        }

        // POST: ProdDepartment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BrandId,Name")] ProdDepartment prodDepartment, int? BuyerInfoId)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdDepartment.Where(x=>x.BrandId==prodDepartment.BrandId && x.Name == prodDepartment.Name).Count()>0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    db.ProdDepartment.Add(prodDepartment);
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                    
                }

                
            }

            ViewBag.BuyerInfo = new SelectList(db.BuyerInfo, "Id", "Name", BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(X=>X.BuyerInfoId== BuyerInfoId), "Id", "Name", prodDepartment.BrandId);
            return View(prodDepartment);
        }

        // GET: ProdDepartment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdDepartment prodDepartment = db.ProdDepartment.Find(id);
            if (prodDepartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", prodDepartment.Brand.BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x=>x.BuyerInfoId== prodDepartment.Brand.BuyerInfoId), "Id", "Name", prodDepartment.BrandId);
            return View(prodDepartment);
        }

        // POST: ProdDepartment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BrandId,Name")] ProdDepartment prodDepartment, int? BuyerInfoId)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdDepartment.Where(x => x.BrandId == prodDepartment.BrandId
                    && x.Name == prodDepartment.Name && x.Id != prodDepartment.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    db.Entry(prodDepartment).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }

                
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo, "Id", "Name", BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x => x.BuyerInfoId == BuyerInfoId), "Id", "Name", prodDepartment.BrandId);
            return View(prodDepartment);
        }

        // GET: ProdDepartment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdDepartment prodDepartment = db.ProdDepartment.Find(id);
            if (prodDepartment == null)
            {
                return HttpNotFound();
            }
            return View(prodDepartment);
        }

        // POST: ProdDepartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProdDepartment prodDepartment = db.ProdDepartment.Find(id);
            db.ProdDepartment.Remove(prodDepartment);
            db.SaveChanges();
            Success("Deleted successfully!", true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames(int Id)
        {
            var data = db.ProdDepartment.Where(x => x.BrandId == Id).OrderBy(x=>x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
