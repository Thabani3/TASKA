using Microsoft.AspNet.Identity;
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
    public class QuotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quotes
        public ActionResult Index()
        {
            var quotes = db.Quotes.Include(q => q.ServiceCat);
            return View(quotes.ToList());
        }

        // GET: Quotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // GET: Quotes/Create
        public ActionResult Create()
        { 
            Quote quote = new Quote();
            quote.TaskA_Address = "42 Masobiya Mdluli Street, Durban, South Africa";
            ViewBag.ServiceCat_ID = new SelectList(db.ServiceCats, "ServiceCat_ID", "Category");
            return View(quote);
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Quote_ID,Date_Produced,ServiceCat_ID,ServiceRender_Address,TaskA_Address,Destination_Distance,Render_hours,Transport_Cost,RenderService_Cost,Discount_Cost,Quote_Cost")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();
                Operations operations = new Operations();
                quote.Date_Produced = DateTime.Now;
                quote.RenderService_Cost = operations.GetHoursCost(quote);
                quote.Transport_Cost = operations.GetTravelCost(quote);
                var Price = (from c in db.Coupons
                             where c.Profile_ID == uid && c.Coupon_Status != "Used"
                             select c.Coupon_Value).FirstOrDefault();
                quote.Discount_Cost = Price;
                Coupon coupon = new Coupon();
                if(Price>0)
                {
                    coupon.Disc(uid);
                }
                quote.Quote_Cost = ((quote.RenderService_Cost + quote.Transport_Cost)-quote.Discount_Cost);
                db.Quotes.Add(quote);
                db.SaveChanges();
        
                return RedirectToAction("Details", new{id = quote.Quote_ID});
            }

            ViewBag.ServiceCat_ID = new SelectList(db.ServiceCats, "ServiceCat_ID", "Category", quote.ServiceCat_ID);
            return View(quote);
        }
        public JsonResult Getdistance(Quote quote)
        {

            return Json(quote.Destination_Distance, JsonRequestBehavior.AllowGet);
        }
        // GET: Quotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceCat_ID = new SelectList(db.ServiceCats, "ServiceCat_ID", "Category", quote.ServiceCat_ID);
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Quote_ID,Date_Produced,ServiceCat_ID,ServiceRender_Address,TaskA_Address,Destination_Distance,Render_hours,Transport_Cost,RenderService_Cost,Quote_Cost")] Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceCat_ID = new SelectList(db.ServiceCats, "ServiceCat_ID", "Category", quote.ServiceCat_ID);
            return View(quote);
        }

        // GET: Quotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quote quote = db.Quotes.Find(id);
            db.Quotes.Remove(quote);
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
