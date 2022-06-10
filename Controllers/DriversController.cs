using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskA.Models;

namespace TaskA.Controllers
{
    public class DriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: Drivers
        public ActionResult Index()
        {
            var drivers = db.Drivers.Include(d => d.Vehicle);
            return View(drivers.ToList());
        }

        // GET: Drivers/Details/5
        public ActionResult Details()
        {
            var uid = User.Identity.GetUserId();
            string id = uid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Drivers/Create
        public ActionResult Create()
        {
            ViewBag.Vehicle_ID = new SelectList(db.Vehicles, "Vehicle_ID", "Vehicle_make");
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Driver_ID,Driver_IDNo,Driver_Name,Driver_Surname,Diver_Image,Driver_CellNo,Driver_Address,Driver_Email,Vehicle_ID")] Driver driver, HttpPostedFileBase filelist)
        {
            if (ModelState.IsValid)
            {
                var pass = "Password@01";

                var user = new ApplicationUser { UserName = driver.Driver_Email, Email = driver.Driver_Email };
                await UserManager.CreateAsync(user, pass);
                driver.Driver_ID = user.Id;
                UserManager.AddToRole(driver.Driver_ID, "Driver");
                if (filelist != null && filelist.ContentLength > 0)
                {
                    driver.Diver_Image = ConvertToBytes(filelist);
                }
                db.Drivers.Add(driver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Vehicle_ID = new SelectList(db.Vehicles, "Vehicle_ID", "Vehicle_make", driver.Vehicle_ID);
            return View(driver);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Drivers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            ViewBag.Vehicle_ID = new SelectList(db.Vehicles, "Vehicle_ID", "Vehicle_make", driver.Vehicle_ID);
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Driver_ID,Driver_IDNo,Driver_Name,Driver_Surname,Diver_Image,Driver_CellNo,Driver_Address,Driver_Email,Vehicle_ID")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Vehicle_ID = new SelectList(db.Vehicles, "Vehicle_ID", "Vehicle_make", driver.Vehicle_ID);
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Driver driver = db.Drivers.Find(id);
            db.Drivers.Remove(driver);
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
