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
    public class ProdCatTypeController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ProdCatType
        public ActionResult Index()
        {
            var prodCatTypes = db.ProdCatType.Include(p => p.ProdCategory);
            return View(prodCatTypes.ToList());
        }

        // GET: ProdCatType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCatType prodCatType = db.ProdCatType.Find(id);
            if (prodCatType == null)
            {
                return HttpNotFound();
            }
            return View(prodCatType);
        }

        // GET: ProdCatType/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x=>x.Name) , "Id", "Name");
            ViewBag.ProdCategoryId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            return View();
        }

        // POST: ProdCatType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProdCategoryId,Name,Description")] ProdCatType prodCatType,int? BuyerInfoId)
        {
            if (ModelState.IsValid)
            {
                if (db.ProdCatType.Where(x=>x.ProdCategoryId== prodCatType.ProdCategoryId && x.Name==prodCatType.Name).Count()>0)
                {
                    Danger("Name exists! Try different", true);
                }
                else
                {
                    db.ProdCatType.Add(prodCatType);
                    db.SaveChanges();
                    Success("Saved successfully!",true);
                    return RedirectToAction("Index");
                }

                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfoId);
            ViewBag.ProdCategoryId = new SelectList(db.ProdCategory.OrderBy(x => x.Name), "Id", "Name", prodCatType.ProdCategoryId);
            return View(prodCatType);
        }

        // GET: ProdCatType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCatType prodCatType = db.ProdCatType.Find(id);
            if (prodCatType == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", prodCatType.ProdCategory.BuyerInfoId);
            ViewBag.ProdCategoryId = new SelectList(db.ProdCategory.Where(x=>x.BuyerInfoId == prodCatType.ProdCategory.BuyerInfoId), "Id", "Name", prodCatType.ProdCategoryId);
            return View(prodCatType);
        }

        // POST: ProdCatType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProdCategoryId,Name,Description")] ProdCatType prodCatType, int? BuyerInfoId)
        {
            if (ModelState.IsValid)
            {

                if (db.ProdCatType.Where(x => x.ProdCategoryId == prodCatType.ProdCategoryId && x.Name == prodCatType.Name && x.Id!=prodCatType.Id).Count() > 0)
                {
                    Danger("Name exists! Try different", true);
                }
                else
                {
                    db.Entry(prodCatType).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfoId);
            ViewBag.ProdCategoryId = new SelectList(db.ProdCategory.OrderBy(x => x.Name), "Id", "Name", prodCatType.ProdCategoryId);
            return View(prodCatType);
        }

        // GET: ProdCatType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdCatType prodCatType = db.ProdCatType.Find(id);
            if (prodCatType == null)
            {
                return HttpNotFound();
            }
            return View(prodCatType);
        }

        // POST: ProdCatType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProdCatType prodCatType = db.ProdCatType.Find(id);
            db.ProdCatType.Remove(prodCatType);
            db.SaveChanges();
            Success("Deleted successfully", true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames(int Id)
        {
            var data = db.ProdCatType.Where(x=>x.ProdCategoryId==Id).OrderBy(x=>x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
