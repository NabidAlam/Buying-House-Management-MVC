﻿using System;
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
    public class CompanyController : Controller
    {
        private ModelBHMS db = new ModelBHMS();

        // GET: Company
        public ActionResult Index()
        {
            return View(db.CompanyInformations.ToList());
        }

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Phone,Web,Email,ContactPerson,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CompanyInformation companyInformation)
        {
            if (ModelState.IsValid)
            {
                companyInformation.OpBy = 1;
                companyInformation.OpOn = DateTime.Now;
                db.CompanyInformations.Add(companyInformation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(companyInformation);
        }

        // GET: Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Phone,Web,Email,ContactPerson,IsAuth,OpBy,OpOn,AuthBy,AuthOn")] CompanyInformation companyInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyInformation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(companyInformation);
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            if (companyInformation == null)
            {
                return HttpNotFound();
            }
            return View(companyInformation);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyInformation companyInformation = db.CompanyInformations.Find(id);
            db.CompanyInformations.Remove(companyInformation);
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
