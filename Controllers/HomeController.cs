using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskA.Models;

namespace TaskA.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
		public ActionResult HomePage()
		{
			return View();
		}
		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		private ApplicationDbContext db = new ApplicationDbContext();
		public ActionResult GetData(){
		var query =db.Orders.GroupBy(p => p.Bookings.Quote.ServiceCat.Category).Select(g => new { name = g.Key, count = g.Sum(w => w.Bookings.Quote.Bookings.Count())}).ToList();
			return Json(query, JsonRequestBehavior.AllowGet);
		}
	
		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}