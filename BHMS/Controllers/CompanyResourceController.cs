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
    public class CompanyResourceController : AlertController
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: CompanyResource
        public ActionResult Index()
        {
            return View(db.CompanyResources.ToList());
        }

        // GET: CompanyResource/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyResource companyResource = db.CompanyResources.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }
            return View(companyResource);
        }

        // GET: CompanyResource/Create
        public ActionResult Create()
        {
            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text");
            ViewBag.Status = status;
            return View();
        }

        // POST: CompanyResource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Position,DOJ,DOB,Phone,Address,Status,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CompanyResource companyResource)
        {
            if (ModelState.IsValid)
            {
                companyResource.OpBy = 1;
                companyResource.OpOn = DateTime.Now;
                db.CompanyResources.Add(companyResource);
                db.SaveChanges();
                Success("Saved successfully!", true);
                return RedirectToAction("Index");
            }

            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text",companyResource.Status);
            ViewBag.Status = status;

            return View(companyResource);
        }

        // GET: CompanyResource/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyResource companyResource = db.CompanyResources.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }

            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text",companyResource.Status);
            ViewBag.Status = status;
            return View(companyResource);
        }

        // POST: CompanyResource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Position,DOJ,DOB,Phone,Address,Status,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CompanyResource companyResource)
        {
            if (ModelState.IsValid)
            {
                companyResource.OpBy = 1;
                companyResource.OpOn = DateTime.Now;
                db.Entry(companyResource).State = EntityState.Modified;
                db.SaveChanges();
                Success("Saved successfully!", true);
                return RedirectToAction("Index");
            }
            var status = new SelectList(new List<SelectListItem> { new SelectListItem { Text = "Active", Value = "A" }, new SelectListItem { Text = "Inactive", Value = "I" }, }, "Value", "Text",companyResource.Status);
            ViewBag.Status = status;
            return View(companyResource);
        }

        // GET: CompanyResource/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyResource companyResource = db.CompanyResources.Find(id);
            if (companyResource == null)
            {
                return HttpNotFound();
            }
            return View(companyResource);
        }

        // POST: CompanyResource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyResource companyResource = db.CompanyResources.Find(id);
            db.CompanyResources.Remove(companyResource);
            db.SaveChanges();
            Success("Deleted successfully!", true);
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
