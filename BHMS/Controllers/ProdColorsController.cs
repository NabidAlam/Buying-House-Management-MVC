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
    public class ProdColorsController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ProdColors
        public ActionResult Index()
        {
            return View(db.ProdColor.ToList());
        }

        // GET: ProdColors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdColor prodColor = db.ProdColor.Find(id);
            if (prodColor == null)
            {
                return HttpNotFound();
            }
            return View(prodColor);
        }

        // GET: ProdColors/Create
        public ActionResult Create()
        {

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.ProdDepartmentId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.SeasonInfoId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            return View();
        }

        // POST: ProdColors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProdDepartmentId,SeasonInfoId,Name")] ProdColor prodColor , int? BuyerInfoId, int? BrandId)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdColor.Where(x=>x.ProdDepartmentId == prodColor.ProdDepartmentId && x.SeasonInfoId == prodColor.SeasonInfoId && x.Name.ToLower() ==prodColor.Name.ToLower()).Count()>0)
                {
                    Danger("Exists. Try different.", true);
                }
                else
                {
                    prodColor.OpBy = 1;
                    prodColor.OpOn = DateTime.Now;
                    db.ProdColor.Add(prodColor);
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x => x.BuyerInfoId == prodColor.SeasonInfo.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.Brand.BuyerInfoId == prodColor.ProdDepartment.Brand.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", prodColor.ProdDepartmentId);
            ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.OrderBy(x => x.Name), "Id", "Name", prodColor.SeasonInfoId);

            return View(prodColor);
        }

        // GET: ProdColors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdColor prodColor = db.ProdColor.Find(id);
            if (prodColor == null)
            {
                return HttpNotFound();
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodColor.SeasonInfo.BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x=>x.BuyerInfoId== prodColor.SeasonInfo.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", prodColor.ProdDepartment.BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x=>x.Brand.BuyerInfoId== prodColor.ProdDepartment.Brand.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", prodColor.ProdDepartmentId);
            ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.OrderBy(x => x.Name), "Id", "Name", prodColor.SeasonInfoId);
            return View(prodColor);
        }

        // POST: ProdColors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProdDepartmentId,SeasonInfoId,Name")] ProdColor prodColor, int? BuyerInfoId, int? BrandId)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdColor.Where(x => x.Name == prodColor.Name && x.Id!=prodColor.Id).Count() > 0)
                {
                    Danger("Exists. Try different.", true);
                }
                else
                {
                    prodColor.OpBy = 1;
                    prodColor.OpOn = DateTime.Now;
                    db.Entry(prodColor).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }

                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x => x.BuyerInfoId == BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.BrandId == BrandId).OrderBy(x => x.Name), "Id", "Name", prodColor.ProdDepartmentId);
            ViewBag.SeasonInfoId = new SelectList(db.SeasonInfo.OrderBy(x => x.Name), "Id", "Name", prodColor.SeasonInfoId);

            return View(prodColor);
        }

        // GET: ProdColors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdColor prodColor = db.ProdColor.Find(id);
            if (prodColor == null)
            {
                return HttpNotFound();
            }
            return View(prodColor);
        }

        // POST: ProdColors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProdColor prodColor = db.ProdColor.Find(id);
            db.ProdColor.Remove(prodColor);
            db.SaveChanges();
            Success("Deleted successfully",true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames()
        {
            var data = db.ProdColor.Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //============================================================
        //===========Added by Tazbirul(26 April,2018)=================

        public JsonResult GetProdColorNames(int DeptId,int SeasonId)
        {
            var data = db.ProdColor.Where(x=>x.SeasonInfoId== SeasonId && x.ProdDepartmentId == DeptId).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //===========================END==============================
        //============================================================

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
