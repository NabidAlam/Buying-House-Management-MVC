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
    public class ProdSizeController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: ProdSize
        public ActionResult Index()
        {
            var prodSizes = db.ProdSize.Include(p=>p.ProdDepartment);
            return View(prodSizes.ToList());
        }

        // GET: ProdSize/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdSize prodSize = db.ProdSize.Find(id);
            if (prodSize == null)
            {
                return HttpNotFound();
            }

            var selectedDepartment = db.ProdDepartment.SingleOrDefault(x => x.Id == prodSize.ProdDepartmentId);

            ViewBag.ProdDepartment = selectedDepartment.Name;
            ViewBag.BuyerId = selectedDepartment.Brand.BuyerInfo.Name;

            return View(prodSize);
        }

        // GET: ProdSize/Create
        public ActionResult Create()
        {
            ViewBag.BuyerInfo = new SelectList(db.BuyerInfo.OrderBy(x=>x.Name), "Id", "Name");
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            ViewBag.ProdDepartmentId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");

            // Enumerable.Empty<SelectListItem>()
            return View();
        }

        // POST: ProdSize/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProdDepartmentId,SizeRange,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] ProdSize prodSize, int? BuyerInfo,int? BrandId)
        {
            if (ModelState.IsValid)
            {
                if (prodSize.ProdDepartmentId==0)
                {
                    ModelState.AddModelError("ProdDepartmentId", "Department type cannot be empty");                    
                }
                else
                {
                    if (db.ProdSize.Where(x => x.ProdDepartmentId == prodSize.ProdDepartmentId && x.SizeRange == prodSize.SizeRange).Count() > 0)
                    {
                        Danger("Exists! Try different", true);
                    }
                    else
                    {
                        prodSize.OpBy = 1;
                        prodSize.OpOn = DateTime.Now;
                        db.ProdSize.Add(prodSize);
                        db.SaveChanges();
                        Success("Saved successfully!", true);
                        return RedirectToAction("Index");
                    }
                }
                
                
            }

            ViewBag.BuyerInfo = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfo);
            ViewBag.BrandId = new SelectList(db.Brand.Where(X=>X.BuyerInfoId== BuyerInfo).OrderBy(x => x.Name), "Id", "Name", BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.Brand.BuyerInfoId == BuyerInfo).OrderBy(x => x.Name), "Id", "Name", prodSize.ProdDepartmentId);
            return View(prodSize);
        }

        // GET: ProdSize/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdSize prodSize = db.ProdSize.Find(id);
            if (prodSize == null)
            {
                return HttpNotFound();
            }

            var selectedBuyer= db.BuyerInfo.SingleOrDefault(x => x.Id == prodSize.ProdDepartment.Brand.BuyerInfoId);
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x=>x.Name), "Id", "Name", selectedBuyer.Id);
            ViewBag.BrandId = new SelectList(db.Brand.Where(x=>x.BuyerInfoId== prodSize.ProdDepartment.Brand.BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", prodSize.ProdDepartment.BrandId);
            //ViewBag.ProdCatTypeId = new SelectList(db.ProdCatType, "Id", "Name", prodSize.ProdCatTypeId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.Brand.BuyerInfoId == prodSize.ProdDepartment.Brand.BuyerInfoId).OrderBy(x=>x.Name), "Id", "Name", prodSize.ProdDepartmentId);
            return View(prodSize);
        }

        // POST: ProdSize/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProdDepartmentId,SizeRange")] ProdSize prodSize, int? BuyerInfoId, int? BrandId)
        {
            if (ModelState.IsValid)
            {
                if (prodSize.ProdDepartmentId == 0)
                {
                    ModelState.AddModelError("ProdDepartmentId", "Department type cannot be empty");
                }
                else
                {
                    if (db.ProdSize.Where(x => x.ProdDepartmentId == prodSize.ProdDepartmentId && x.SizeRange == prodSize.SizeRange && x.Id != prodSize.Id).Count() > 0)
                    {
                        Danger("Exists! Try different", true);
                    }
                    else
                    {
                        prodSize.OpOn = DateTime.Now;
                        db.Entry(prodSize).State = EntityState.Modified;
                        db.SaveChanges();
                        Success("Saved successfully!", true);
                        return RedirectToAction("Index");
                    }
                }
                
                
            }


            ViewBag.BuyerInfo = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", BuyerInfoId);
            ViewBag.BrandId = new SelectList(db.Brand.Where(X => X.BuyerInfoId == BuyerInfoId).OrderBy(x => x.Name), "Id", "Name", BrandId);
            ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.Where(x => x.BrandId == BrandId).OrderBy(x => x.Name), "Id", "Name", prodSize.ProdDepartmentId);

            return View(prodSize);
        }

        // GET: ProdSize/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProdSize prodSize = db.ProdSize.Find(id);
            if (prodSize == null)
            {
                return HttpNotFound();
            }
            return View(prodSize);
        }

        // POST: ProdSize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProdSize prodSize = db.ProdSize.Find(id);
            db.ProdSize.Remove(prodSize);
            db.SaveChanges();
            Success("Deleted successfully!",true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames(int Id)
        {
            var data = db.ProdSize.Where(x => x.ProdDepartmentId == Id).OrderBy(x=>x.SizeRange).Select(y => new { Name = y.SizeRange, Id = y.Id }).ToList();
            //System.Threading.Thread.Sleep(5000);
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
