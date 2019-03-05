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
    public class ProdCategoryController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ProdCategory
        public ActionResult Index()
        {
            return View(db.ProdCategory.ToList());
        }

        // GET: ProdCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCategory prodCategory = db.ProdCategory.Find(id);
            if (prodCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCategory.BuyerInfoId);
            return View(prodCategory);
        }

        // GET: ProdCategory/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: ProdCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BuyerInfoId,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] ProdCategory prodCategory)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdCategory.Where(x=>x.BuyerInfoId== prodCategory.BuyerInfoId && x.Name==prodCategory.Name).Count()>0)
                {
                    Danger("Name exists! Try different.", true);
                    //ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCategory.BuyerInfoId);
                }
                else
                {
                    prodCategory.OpBy = 1;
                    prodCategory.OpOn = DateTime.Now;
                    db.ProdCategory.Add(prodCategory);
                    db.SaveChanges();
                    Success("Saved successfully", true);
                    return RedirectToAction("Index");
                }                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCategory.BuyerInfoId);
            return View(prodCategory);
        }

        // GET: ProdCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCategory prodCategory = db.ProdCategory.Find(id);
            if (prodCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCategory.BuyerInfoId);

            return View(prodCategory);
        }

        // POST: ProdCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,Name,Description,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] ProdCategory prodCategory)
        {
            if (ModelState.IsValid)
            {

                if (db.ProdCategory.Where(x =>x.BuyerInfoId == prodCategory.BuyerInfoId && x.Name.ToLower() == prodCategory.Name.ToLower() && x.Id!=prodCategory.Id).Count() > 0)
                {
                    Danger("Name exists! Try different.", true);
                }
                else
                {
                    prodCategory.OpBy = 1;
                    prodCategory.OpOn = DateTime.Now;
                    db.Entry(prodCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully", true);
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCategory.BuyerInfoId);
            return View(prodCategory);
        }

        // GET: ProdCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCategory prodCategory = db.ProdCategory.Find(id);
            if (prodCategory == null)
            {
                return HttpNotFound();
            }
            return View(prodCategory);
        }

        // POST: ProdCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProdCategory prodCategory = db.ProdCategory.Find(id);
            db.ProdCategory.Remove(prodCategory);
            db.SaveChanges();
            Success("Deleted successfully!", true);
            return RedirectToAction("Index");
        }


        public JsonResult GetNames()
        {
            var data = db.ProdCategory.Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProductTypeNames(int Id)
        {
            var data = db.ProdCategory.Where(x => x.BuyerInfoId == Id).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
