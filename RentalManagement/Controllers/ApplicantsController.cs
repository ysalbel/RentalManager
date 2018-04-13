﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RentalManagement.Models;

namespace RentalManagement.Controllers
{
    public class ApplicantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Applicants
        public ActionResult Index()
        {
            return View(db.Applicants.ToList());
        }

        // GET: Applicants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicant applicant = db.Applicants.Find(id);
            if (applicant == null)
            {
                return HttpNotFound();
            }
            return View(applicant);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        // GET: Applicants/Create
        public ActionResult Create(Guid? id)
        {
            //tempGuid = asset.ID;
            ViewBag.Message = id;
            TempData["Message"] = id;
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            return View();
        }

        // POST: Applicants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApplicantID,Name,Email,Details,Asset")] Applicant applicant, Guid AssetID)
        {   
            if (ModelState.IsValid)
            {
                applicant.AssetID = AssetID;
                db.Assets.Find(AssetID);
                db.Applicants.Add(applicant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicant);
        }

        // GET: Applicants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicant applicant = db.Applicants.Find(id);
            if (applicant == null)
            {
                return HttpNotFound();
            }
            return View(applicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email,Details")] Applicant applicant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicant);
        }

        // GET: Applicants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applicant applicant = db.Applicants.Find(id);
            if (applicant == null)
            {
                return HttpNotFound();
            }
            return View(applicant);
        }

        // POST: Applicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Applicant applicant = db.Applicants.Find(id);
            db.Applicants.Remove(applicant);
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

        public void Approve(int? id)
        {
            using (var db = new ApplicationDbContext())
            {
                Applicant applicant = db.Applicants.Find(id);

                /*
                 * UserName must be Email
                 */
                var user = new ApplicationUser { UserName = applicant.Email, Email = applicant.Email };
                string password = "Password1!";

                var store = new UserStore<ApplicationUser>(db);
                var manager = new UserManager<ApplicationUser, string>(store);

                var result = manager.Create(user, password);
                if (!result.Succeeded)
                    throw new ApplicationException("Unable to create a user.");

                result = manager.AddToRole(user.Id, "Tenant");
                if (!result.Succeeded)
                    throw new ApplicationException("Unable to add user to a role.");

                Tenant tenant = new Tenant { ID = Guid.NewGuid(),
                                             Name = applicant.Name,
                                             Email = applicant.Email,
                                             Details = applicant.Details,
                                             //RequestedAssets = applicant.AssetID
                                           };
                db.Tenants.Add(tenant);
                db.SaveChanges();

                //var client = new SmtpClient("smtp.gmail.com", 587)
                //{
                //    Credentials = new NetworkCredential("myusername@gmail.com", "mypwd"),
                //    EnableSsl = true
                //};
                //client.Send("myusername@gmail.com", "myusername@gmail.com", "Your Tenant Account", "Your password is: " + password);
            }
        }
    }
}
