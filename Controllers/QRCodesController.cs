using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskA.Models;
using ZXing;

namespace TaskA.Controllers
{
    public class QRCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

 

        // GET: QRCodes
        public ActionResult Index()
        {
            var qRCodes = db.QRCodes.Include(q => q.Order);
            return View(qRCodes.ToList());
        }

        // GET: QRCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCode qRCode = db.QRCodes.Find(id);
            if (qRCode == null)
            {
                return HttpNotFound();
            }
            return View(qRCode);
        }

        public ActionResult ViewPDF(int id)
        {
            var report = new Rotativa.ActionAsPdf("Details", new { id = id }) { FileName = "invoice.pdf" };


            return report;

            //return new ViewAsPdf("Invoice", new { id = id });
        }

        // GET: QRCodes/Create
        public ActionResult Create()
        {
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Tasker_ID");
            return View();
        }

        // POST: QRCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QRId,QRCodeText,QRCodeImagePath,Order_ID")] QRCode qRCode)
        {
            if (ModelState.IsValid)
            {
                db.QRCodes.Add(qRCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Tasker_ID", qRCode.Order_ID);
            return View(qRCode);
        }

        // GET: QRCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCode qRCode = db.QRCodes.Find(id);
            if (qRCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Tasker_ID", qRCode.Order_ID);
            return View(qRCode);
        }

        // POST: QRCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QRId,QRCodeText,QRCodeImagePath,Order_ID")] QRCode qRCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qRCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Tasker_ID", qRCode.Order_ID);
            return View(qRCode);
        }

        // GET: QRCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCode qRCode = db.QRCodes.Find(id);
            if (qRCode == null)
            {
                return HttpNotFound();
            }
            return View(qRCode);
        }

        // POST: QRCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QRCode qRCode = db.QRCodes.Find(id);
            db.QRCodes.Remove(qRCode);
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
