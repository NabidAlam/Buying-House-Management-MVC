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
    public class FabricTypesController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: FabricTypes
        public ActionResult Index()
        {
            return View(db.FabricType.ToList());
        }


        public JsonResult GetNames()
        {
            var data = db.FabricType.OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetNamesOnProdType(int Id)
        {
            var data = db.FabricType.Where(x => x.ProdCategoryId == Id).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

            
            return Json(data, JsonRequestBehavior.AllowGet);

        }
 
        // GET: FabricTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricType fabricType = db.FabricType.Find(id);
            if (fabricType == null)
            {
                return HttpNotFound();
            }
            return View(fabricType);
        }

        // GET: FabricTypes/Create
        public ActionResult Create()
        {
            ViewBag.ProdCategoryId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo,"Id","Name");

            return View();
        }

        // POST: FabricTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ProdCategoryId")] FabricType fabricType)
        {
            if (ModelState.IsValid)
            {
                db.FabricType.Add(fabricType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fabricType);
        }

        // GET: FabricTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricType fabricType = db.FabricType.Find(id);

            ViewBag.ProdCategoryId = new SelectList(db.ProdCategory.Where(x => x.BuyerInfoId == fabricType.ProdCategory.BuyerInfoId), "Id", "Name", fabricType.ProdCategoryId);

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", fabricType.ProdCategory.BuyerInfoId);




            if (fabricType == null)
            {
                return HttpNotFound();
            }
            return View(fabricType);
        }

        // POST: FabricTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ProdCategoryId")] FabricType fabricType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fabricType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(fabricType);
        }

        // GET: FabricTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FabricType fabricType = db.FabricType.Find(id);
            if (fabricType == null)
            {
                return HttpNotFound();
            }
            return View(fabricType);
        }

        // POST: FabricTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FabricType fabricType = db.FabricType.Find(id);
            db.FabricType.Remove(fabricType);
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
