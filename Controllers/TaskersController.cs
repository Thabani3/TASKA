using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Data.Entity;

using System.Net;

using TaskA.Models;

namespace TaskA.Controllers
{
    public class TaskersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        // GET: Taskers
        public ActionResult Index()
        {
            var taskers = db.Taskers.Include(t => t.Service);
            return View(taskers.ToList());
        }

        // GET: Taskers/Details/5
        public ActionResult Details()
        {
            var uid = User.Identity.GetUserId();
            string id = uid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasker tasker = db.Taskers.Find(id);
            if (tasker == null)
            {
                return HttpNotFound();
            }
            return View(tasker);
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
        // GET: Taskers/Create
        public ActionResult Create()
        {
            ViewBag.Service_ID = new SelectList(db.Services, "Service_ID", "Service_Name");
            return View();
        }

        // POST: Taskers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Tasker_ID,Tasker_IDNo,Tasker_Name,Tasker_Surname,Tasker_Cellnumber,Tasker_Address,Tasker_Email,Tasker_Tellnum,Service_ID")] Tasker tasker)
        {
            if (ModelState.IsValid)
            {
                var pass = "Password@01";

                var user = new ApplicationUser { UserName = tasker.Tasker_Email, Email = tasker.Tasker_Email };
                await UserManager.CreateAsync(user, pass);
                tasker.Tasker_ID = user.Id;
                UserManager.AddToRole(tasker.Tasker_ID, "Tasker");
                db.Taskers.Add(tasker);
                db.SaveChanges();
                

                return RedirectToAction("Index");
            }

            ViewBag.Service_ID = new SelectList(db.Services, "Service_ID", "Service_Name", tasker.Service_ID);
            return View(tasker);
        }

        // GET: Taskers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasker tasker = db.Taskers.Find(id);
            if (tasker == null)
            {
                return HttpNotFound();
            }
            ViewBag.Service_ID = new SelectList(db.Services, "Service_ID", "Service_Name", tasker.Service_ID);
            return View(tasker);
        }

        // POST: Taskers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Tasker_ID,Tasker_IDNo,Tasker_Name,Tasker_Surname,Tasker_Cellnumber,Tasker_Address,Tasker_Email,Tasker_Tellnum,Service_ID")] Tasker tasker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tasker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Service_ID = new SelectList(db.Services, "Service_ID", "Service_Name", tasker.Service_ID);
            return View(tasker);
        }

        // GET: Taskers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasker tasker = db.Taskers.Find(id);
            if (tasker == null)
            {
                return HttpNotFound();
            }
            return View(tasker);
        }

        // POST: Taskers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Tasker tasker = db.Taskers.Find(id);
            db.Taskers.Remove(tasker);
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
