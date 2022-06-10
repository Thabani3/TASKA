using Microsoft.AspNet.Identity;
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
    //[Authorize]
    public class OrdersController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        
        private string GenerateQRCode(string qrcodeText)
        {
            string folderPath = "~/Images2/";
            string imagePath = "~/Images/QrCode2.jpg";
            // create new Directory if not exist
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        public ActionResult Read()
        {
            return View(ReadQRCode());
        }

        private QRCode ReadQRCode()
        {
            QRCode barcodeModel = new QRCode();
            string barcodeText = "";
            string imagePath = "~/Images/QrCode.jpg";
            string barcodePath = Server.MapPath(imagePath);
            var barcodeReader = new BarcodeReader();
            //Decode the image to text
            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                barcodeText = result.Text;
            }
            return new QRCode() { QRCodeText = barcodeText, QRCodeImagePath = imagePath };
        }


        public ActionResult AddNew(int id)
        {
            QRCode objQR = new QRCode();
            objQR.Order_ID = id;
            objQR.QRCodeText = "https://21grp35.azurewebsites.net/Beneficiary_Signature/Create/" + objQR.Order_ID;

            objQR.QRCodeImagePath = GenerateQRCode(objQR.QRCodeText);
            db.QRCodes.Add(objQR);
            db.SaveChanges();


            var getclientEmal = (from i in db.QRCodes
                                 where i.QRId== id
                                 select i.Order.Bookings.Client.Profile_Email).FirstOrDefault();
            var getclientName = (from i in db.QRCodes
                                 where i.QRId == id
                                 select i.Order.Bookings.Client.Profile_Name).FirstOrDefault();
            var getlastName = (from i in db.QRCodes
                               where i.QRId == id
                               select i.Order.Bookings.Client.Profile_Surname).FirstOrDefault();
            var getAddress = (from i in db.Orders
                              where i.Book_ID == id
                              select i.Bookings.Quote.ServiceRender_Address).FirstOrDefault();
            var getServiceType = (from i in db.Orders
                                  where i.Book_ID == id
                                  select i.Bookings.Quote.ServiceCat.Category).FirstOrDefault();
            var getInvoice = (from i in db.QRCodes
                              where i.Order_ID == id
                              select i.QRId).FirstOrDefault();
            var getReference = (from i in db.Orders
                                where i.Order_ID== id
                                select i.Reference).FirstOrDefault();

            ViewBag.Body = $"Dear " + getclientName + " " + getlastName + "<br/>" +
            $"Here is your reference number: " + getReference + ". you can track and view the progress of the tasker while you are away: https://2021group35.azurewebsites.net/Orders/TrackOrder/ "+
            $"<br/>" + "You can download your invoice here: " + $" https://2021group35.azurewebsites.net/QRCodes/Details/{getInvoice}" +" and provide it to the driver wheh he/she arrives to drop the tasker"+
            $"<br/>" +
            $"Task A";
            Email email = new Email();
            email.Gmail("Order delivery Information", ViewBag.Body, getclientEmal);
            return RedirectToAction("AllBookings", "Bookings");
        }
        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Bookings).Include(o => o.Driver).Include(o => o.Tasker);
            return View(orders.ToList());
        }
        public ActionResult MyOrders()
        {
            var uid = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.Bookings).Include(o => o.Driver).Include(o => o.Tasker).Where(b => b.Tasker_ID == uid && b.OrderStatus.Status_Name != "Finished Service rendering");
            return View(orders.ToList());
        }
        public ActionResult Update()
        {
            var uid = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.Bookings).Include(o => o.Driver).Include(o => o.Tasker).Where(b => b.Tasker_ID == uid);
            return View(orders.ToList());
        }

        public ActionResult DriverRequest()
        {
            var uid = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.Bookings).Include(o => o.Driver).Include(o => o.Tasker).Where(b => b.Driver_ID == uid);
            return View(orders.ToList());
        }

        public ActionResult ReturnTakser()
        {
            var uid = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.Bookings).Include(o => o.Driver).Include(o => o.Tasker).Where(b => b.Driver_ID == uid && b.OrderStatus.Status_Name == "Finished Service rendering");
            return View(orders.ToList());
        }
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        public ActionResult ReturnToBase(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create(int id)
        {
            Order order = new Order();
            order.Book_ID = id;
            order.Driver_ID = null;
            order.Order_DateTime = DateTime.Now;

            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName");
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_IDNo");
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name");
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name");
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Order_ID,Order_DateTime,Book_ID,Tasker_ID,OrderStatus_ID,Driver_ID")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                order.Reference = RandomString(8);
                order.OrderStatus_ID = order.GetStatus("Waiting To assign the Tasker to driver");
                db.SaveChanges();
                var id = (from p in db.Orders
                          where p.Order_ID == order.Order_ID
                          select p.Book_ID).FirstOrDefault();
                Operations operations = new Operations();
                operations.UpdateStatus(id, "Approved");
                return RedirectToAction("AddNew", new { id = order.Order_ID });
            }

            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName", order.Book_ID);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_IDNo", order.Driver_ID);
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name", order.Tasker_ID);
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name", order.OrderStatus_ID);
            return View(order);
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult TrackOrder(string searchString)
        {
            var trackings = from s in db.Orders select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                trackings = trackings.Where(s =>
               s.Reference.ToUpper().Contains(searchString.ToUpper()));
                return View(trackings.ToList());
            }
            trackings = trackings.Where(s =>
                   s.Reference == null);
            return View(trackings.ToList());
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName", order.Book_ID);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", order.Driver_ID);
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name", order.Tasker_ID);
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name", order.OrderStatus_ID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Order_ID,Order_DateTime,Book_ID,Tasker_ID,OrderStatus_ID,Driver_ID")] Order order)
        {
            if (ModelState.IsValid)
            {
                var ordertime = (from p in db.Orders
                                      where p.Order_ID == order.Order_ID
                                      select p.Order_DateTime).FirstOrDefault();
                var taskerid = (from p in db.Orders
                                 where p.Order_ID == order.Order_ID
                                 select p.Tasker_ID).FirstOrDefault();

                var bookid = (from p in db.Orders
                                 where p.Order_ID == order.Order_ID
                                 select p.Book_ID).FirstOrDefault();
                var refe = (from p in db.Orders
                           where p.Order_ID == order.Order_ID
                           select p.Reference).FirstOrDefault();
                order.Order_DateTime = ordertime;
                order.Tasker_ID = taskerid;
                order.Book_ID = bookid;
                order.OrderStatus_ID = order.GetStatus("Assigned to Driver");
                order.Reference = refe;
                Operations operations = new Operations();
                operations.UpdateStatus(bookid, "Assigned to driver");
                
             
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyOrders");
            }
            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName", order.Book_ID);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", order.Driver_ID);
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name", order.Tasker_ID);
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name", order.OrderStatus_ID);
            return View(order);
        }
        // GET: Orders/Edit/5
        public ActionResult UpdateWorkStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName", order.Book_ID);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", order.Driver_ID);
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name", order.Tasker_ID);
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name", order.OrderStatus_ID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateWorkStatus([Bind(Include = "Order_ID,Order_DateTime,Book_ID,Tasker_ID,OrderStatus_ID,Driver_ID")] Order order)
        {
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();
                var ordertime = (from p in db.Orders
                                 where p.Order_ID == order.Order_ID
                                 select p.Order_DateTime).FirstOrDefault();
                var taskerid = (from p in db.Orders
                                where p.Order_ID == order.Order_ID
                                select p.Tasker_ID).FirstOrDefault();

                var bookid = (from p in db.Orders
                              where p.Order_ID == order.Order_ID
                              select p.Book_ID).FirstOrDefault();
                var driverId = (from p in db.Orders
                              where p.Order_ID == order.Order_ID
                              select p.Driver_ID).FirstOrDefault();
                var refe = (from p in db.Orders
                            where p.Order_ID == order.Order_ID
                            select p.Reference).FirstOrDefault();
         
                order.Order_DateTime = ordertime;
                order.Tasker_ID = uid;
                order.Book_ID = bookid;
                order.Driver_ID = driverId;
                order.Reference = refe;
                Operations operations = new Operations();
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                var statusCheck = (from p in db.OrderStatuses
                              where p.OrderStatus_ID == order.OrderStatus_ID
                              select p.Status_Name).FirstOrDefault();
                var ClientName = (from p in db.Orders
                                  where p.Order_ID == order.Order_ID
                                  select p.Bookings.Client.Profile_Name).FirstOrDefault();
                var Clientsurname= (from p in db.Orders
                                    where p.Order_ID == order.Order_ID
                                    select p.Bookings.Client.Profile_Surname).FirstOrDefault();
                var ClientEmial= (from p in db.Orders
                                  where p.Order_ID == order.Order_ID
                                  select p.Bookings.Client.Profile_Email).FirstOrDefault();
                if (statusCheck == "Finished Service rendering") {

                    ViewBag.Body = $"Dear " + ClientName + " " + Clientsurname + "<br/>" +
                    $"The service you requested for rendering is complete, please use this link to rate the service https://2021group35.azurewebsites.net/Comments/Create/"+order.OrderStatus_ID +
                    $"<br/>" +
                    $"Task A";
                    Email email = new Email();
                    email.Gmail("Render service complete ", ViewBag.Body, ClientEmial);
                }
                return RedirectToAction("MyOrders");
            }
            ViewBag.Book_ID = new SelectList(db.Bookings, "Book_ID", "Book_RecipientName", order.Book_ID);
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", order.Driver_ID);
            ViewBag.Tasker_ID = new SelectList(db.Taskers, "Tasker_ID", "Tasker_Name", order.Tasker_ID);
            ViewBag.OrderStatus_ID = new SelectList(db.OrderStatuses, "OrderStatus_ID", "Status_Name", order.OrderStatus_ID);
            return View(order);
        }

        public ActionResult RatingDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order article = db.Orders.Find(id);
            Tasker tasker = db.Taskers.Find(article.Tasker_ID);
            if (tasker == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = id.Value;

            var comments = db.Comments.Where(d => d.Tasker_ID.Equals(id.Value)).ToList();
            ViewBag.Comments = comments;

            var ratings = db.Comments.Where(d => d.Tasker_ID.Equals(id.Value)).ToList();
            if (ratings.Count() > 0)
            {
                var ratingSum = ratings.Sum(d => d.Rating.Value);
                ViewBag.RatingSum = ratingSum;
                var ratingCount = ratings.Count();
                ViewBag.RatingCount = ratingCount;
            }
            else
            {
                ViewBag.RatingSum = 0;
                ViewBag.RatingCount = 0;
            }

            return View(tasker);
        }



        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
