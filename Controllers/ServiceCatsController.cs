using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskA.Models;

namespace TaskA.Controllers
{
    public class ServiceCatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceCats
        public ActionResult Index()
        {
            return View(db.ServiceCats.ToList());
        }

        // GET: ServiceCats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCat serviceCat = db.ServiceCats.Find(id);
            if (serviceCat == null)
            {
                return HttpNotFound();
            }
            return View(serviceCat);
        }
        /// <summary>
        /// admin adds categories
        /// </summary>
        /// <returns></returns>
        // GET: ServiceCats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServiceCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceCat_ID,Category,Cost_Per_Hour")] ServiceCat serviceCat)
        {
            if (ModelState.IsValid)
            {
                db.ServiceCats.Add(serviceCat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviceCat);
        }

        // GET: ServiceCats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCat serviceCat = db.ServiceCats.Find(id);
            if (serviceCat == null)
            {
                return HttpNotFound();
            }
            return View(serviceCat);
        }

        // POST: ServiceCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceCat_ID,Category,Cost_Per_Hour")] ServiceCat serviceCat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceCat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceCat);
        }

        // GET: ServiceCats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCat serviceCat = db.ServiceCats.Find(id);
            if (serviceCat == null)
            {
                return HttpNotFound();
            }
            return View(serviceCat);
        }

        // POST: ServiceCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceCat serviceCat = db.ServiceCats.Find(id);
            db.ServiceCats.Remove(serviceCat);
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
