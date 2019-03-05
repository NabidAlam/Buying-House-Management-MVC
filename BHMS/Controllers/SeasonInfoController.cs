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
    public class SeasonInfoController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: SeasonInfo
        public ActionResult Index()
        {
            return View(db.SeasonInfo.ToList());
        }

        // GET: SeasonInfo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonInfo seasonInfo = db.SeasonInfo.Find(id);
            if (seasonInfo == null)
            {
                return HttpNotFound();
            }
            //ViewBag.BuyerInfoId = new SelectList(db.ProdDepartment.OrderBy(x => x.Name), "Id", "Name", seasonInfo.ProdDepartmentId);
            //ViewBag.ProdDepartmentId = new SelectList(db.ProdDepartment.OrderBy(x => x.Name), "Id", "Name", seasonInfo.ProdDepartmentId);

            return View(seasonInfo);
        }

        // GET: SeasonInfo/Create
        public ActionResult Create()
        {

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: SeasonInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BuyerInfoId,Id,Name,Description")] SeasonInfo seasonInfo)
        {
            if (ModelState.IsValid)
            {
                if (db.SeasonInfo.Where(x=>x.BuyerInfoId == seasonInfo.BuyerInfoId && x.Name.ToLower() ==seasonInfo.Name.ToLower()).Count()>0)
                {
                    Danger("Name exists! Try different",false);
                }
                else
                {
                    seasonInfo.OpBy = 1;
                    seasonInfo.OpOn = DateTime.Now;
                    db.SeasonInfo.Add(seasonInfo);
                    db.SaveChanges();
                    Success("Saved successfully!",true);
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", seasonInfo.BuyerInfoId);
            return View(seasonInfo);
        }

        // GET: SeasonInfo/Edit/5
        public ActionResult Edit(int? id)
        {

          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonInfo seasonInfo = db.SeasonInfo.Find(id);
         
            if (seasonInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", seasonInfo.BuyerInfoId);
            return View(seasonInfo);
        }

        // POST: SeasonInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BuyerInfoId,Name,Description")] SeasonInfo seasonInfo)
        {
            if (ModelState.IsValid)
            {
                if (db.SeasonInfo.Where(x => x.BuyerInfoId == seasonInfo.BuyerInfoId && x.Name.ToLower() == seasonInfo.Name.ToLower() && x.Id!=seasonInfo.Id).Count() > 0)
                {
                    Danger("Name exists! Try different", false);
                }
                else
                {
                    seasonInfo.OpBy = 1;
                    seasonInfo.OpOn = DateTime.Now;
                    db.Entry(seasonInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    Success("Saved successfully!", true);
                    return RedirectToAction("Index");
                }

                
            }
            ViewBag.BuyerInfoId = new SelectList(db.BuyerInfo.OrderBy(x => x.Name), "Id", "Name", seasonInfo.BuyerInfoId);
            return View(seasonInfo);
        }

        // GET: SeasonInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonInfo seasonInfo = db.SeasonInfo.Find(id);
            if (seasonInfo == null)
            {
                return HttpNotFound();
            }
            return View(seasonInfo);
        }

        // POST: SeasonInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SeasonInfo seasonInfo = db.SeasonInfo.Find(id);
            db.SeasonInfo.Remove(seasonInfo);
            db.SaveChanges();
            Success("Deleted successfully",true);
            return RedirectToAction("Index");
        }

        public JsonResult GetNames(int Id)
        {
            var data = db.SeasonInfo.Where(x => x.BuyerInfoId == Id).OrderBy(x => x.Name).Select(y => new { Name = y.Name, Id = y.Id }).ToList();

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
