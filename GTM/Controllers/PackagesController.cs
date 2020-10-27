using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTM.Models;

namespace GTM.Controllers
{
    public class PackagesController : Controller
    {
        private makemoneyEntities db = new makemoneyEntities();

        // GET: Packages
        public ActionResult Index()
        {
            return View(db.PackagesTbls.ToList());
        }

        // GET: Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackagesTbl packagesTbl = db.PackagesTbls.Find(id);
            if (packagesTbl == null)
            {
                return HttpNotFound();
            }
            return View(packagesTbl);
        }

        // GET: Packages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "packageId,amount,validity,peradClick,perdayEarning,referalBonus,minWithdraw,Title,dailyAds")] PackagesTbl packagesTbl, HttpPostedFileBase pkgImgFile)
        {

            //if (ModelState.IsValid)
            //{


                if (pkgImgFile != null)
                {

                    string fileName = Path.GetFileNameWithoutExtension(pkgImgFile.FileName);
                    string extension = Path.GetExtension(pkgImgFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
                    packagesTbl.PkgImgPath = "~/uploads/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
                    pkgImgFile.SaveAs(fileName);

                }
                else
                {

                    packagesTbl.PkgImgPath = null;

                }

                db.PackagesTbls.Add(packagesTbl);
                db.SaveChanges();
                return RedirectToAction("Index");

            //}
            return View(packagesTbl);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackagesTbl packagesTbl = db.PackagesTbls.Find(id);
            if (packagesTbl == null)
            {
                return HttpNotFound();
            }
            return View(packagesTbl);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "packageId,amount,validity,peradClick,perdayEarning,referalBonus,minWithdraw,Title,dailyAds")] PackagesTbl packagesTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packagesTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(packagesTbl);
        }

        // GET: Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackagesTbl packagesTbl = db.PackagesTbls.Find(id);
            if (packagesTbl == null)
            {
                return HttpNotFound();
            }
            return View(packagesTbl);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackagesTbl packagesTbl = db.PackagesTbls.Find(id);
            db.PackagesTbls.Remove(packagesTbl);
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
