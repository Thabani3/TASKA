using Microsoft.AspNet.Identity;
using PayFast;
using PayFast.AspNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskA.Models;

namespace TaskA.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public BookingsController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
        // GET: Bookings
        //get client bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.BookingStatus).Include(b => b.Client).Include(b => b.Quote);
            return View(bookings.ToList());
        }
        public ActionResult Mybookings(string searchString)
        {
            var uid = User.Identity.GetUserId();

            if (!String.IsNullOrEmpty(searchString))
            {
                var book = from s in db.Bookings
                           select s;
                book = book.Where(s =>
               s.Book_RecipientName.ToUpper().Contains(searchString.ToUpper()) || s.Booking_RenderDate.ToString().ToUpper().Contains(searchString.ToUpper()) || s.Book_RecipientSurname.ToString().ToUpper().Contains(searchString.ToUpper()) || s.Quote.ServiceRender_Address.ToString().ToUpper().Contains(searchString.ToUpper())).Where(b => b.Profile_ID == uid);
                return View(book.ToList());
            }

            var bookings = db.Bookings.Include(b => b.Client).Include(b => b.Quote).Where(b => b.Profile_ID == uid).OrderByDescending(b => b.Booking_RenderDate).ToList();
            return View(bookings.ToList());

        }
        //show all bookings
        public ActionResult AllBookings()
        {
            var bookings = db.Bookings.ToList().Where(a => a.BookingStatus.Booking_Status == "Pending" && a.Paymentstatus == true);
            return View(bookings.ToList());
        }

        public ActionResult Decline(int id)
        { 
            Operations operations = new Operations();
            operations.UpdateStatus(id, "Declined");
            return RedirectToAction("AllBookings");
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create(int id)
        {
            Booking booking = new Booking();
            Operations operations = new Operations();
            booking.Quote_ID = id;
            
            var uid = User.Identity.GetUserId();
            booking.BookingStatus_ID = operations.GetStatus("Pending");
            booking.Profile_ID = uid;
            booking.Booking_RenderDate = DateTime.Now;
            ViewBag.BookingStatus_ID = new SelectList(db.BookingStatus, "BookingStatus_ID", "Booking_Status");
            ViewBag.Profile_ID = new SelectList(db.Clients, "Profile_ID", "User_IDNo");
            ViewBag.Quote_ID = new SelectList(db.Quotes, "Quote_ID", "ServiceRender_Address");
            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Book_ID,Booking_date,Booking_RenderDate,Book_RecipientName,Book_RecipientSurname,Paymentstatus,Book_RecipientNumber,Booking_RenderNote,Book_TotalCost,Quote_ID,BookingStatus_ID,Profile_ID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();
                booking.Booking_date = DateTime.Now;
                Operations operations = new Operations();
                booking.Book_TotalCost = operations.GetQuoteCost(booking);
             
                db.Bookings.Add(booking);
                db.SaveChanges();
               
                var check = (from i in db.Spins
                             where i.Profile_ID == uid
                             select i.Profile_ID).SingleOrDefault();
                if (check == null)
                {
                    Spins spins = new Spins();
                    spins.Profile_ID = uid;
                    spins.No_Spins = 0;
                    db.Spins.Add(spins);
                    db.SaveChanges();
                }
                if (booking.Book_TotalCost !=(decimal)0.0)
                {
                    Spins spin = (from b in db.Spins
                                  where b.Profile_ID == uid
                                  select b).FirstOrDefault();
                    spin.No_Spins = spin.No_Spins + 1;
                    db.SaveChanges();
                }
                return RedirectToAction("OnceOff", new { id =booking.Book_ID});
            }

            ViewBag.BookingStatus_ID = new SelectList(db.BookingStatus, "BookingStatus_ID", "Booking_Status", booking.BookingStatus_ID);
            ViewBag.Profile_ID = new SelectList(db.Clients, "Profile_ID", "User_IDNo", booking.Profile_ID);
            ViewBag.Quote_ID = new SelectList(db.Quotes, "Quote_ID", "ServiceRender_Address", booking.Quote_ID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingStatus_ID = new SelectList(db.BookingStatus, "BookingStatus_ID", "Booking_Status", booking.BookingStatus_ID);
            ViewBag.Profile_ID = new SelectList(db.Clients, "Profile_ID", "User_IDNo", booking.Profile_ID);
            ViewBag.Quote_ID = new SelectList(db.Quotes, "Quote_ID", "ServiceRender_Address", booking.Quote_ID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_ID,Booking_date,Booking_RenderDate,Book_RecipientName,Book_RecipientSurname,Paymentstatus,Book_RecipientNumber,Booking_RenderNote,Book_TotalCost,Quote_ID,BookingStatus_ID,Profile_ID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingStatus_ID = new SelectList(db.BookingStatus, "BookingStatus_ID", "Booking_Status", booking.BookingStatus_ID);
            ViewBag.Profile_ID = new SelectList(db.Clients, "Profile_ID", "User_IDNo", booking.Profile_ID);
            ViewBag.Quote_ID = new SelectList(db.Quotes, "Quote_ID", "ServiceRender_Address", booking.Quote_ID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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

        public ActionResult Paynow()
        {
            return RedirectToAction("Success");
        }

        public ActionResult Success()
        {
            //Add email for pay as you go
            return RedirectToAction("Index", "Home");
        }


        public ActionResult SuccessfulBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
            //Add email for contract
        }


        public ActionResult Error()
        {
            return View("Index");
        }


        #region Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            recurringRequest.email_address = "nkosi@finalstride.com";
            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";
            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";
            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;
            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";
            return Redirect(redirectUrl);
        }


        public ActionResult OnceOff(int id)
        {
            Booking booking = db.Bookings.Find(id);

            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            double amount = (double)booking.Book_TotalCost;
            //var products = db.Items.Select(x => x.Item_Name).ToList();
            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = amount;
            onceOffRequest.item_name = "Your appointment Number is " + id;
            onceOffRequest.item_description = "You are now paying your rental fee";
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";

            booking.Paymentstatus = true;
            db.Entry(booking).State = EntityState.Modified;
            db.SaveChanges();
            

                var getclientEmal = (from i in db.Bookings
                                     where i.Book_ID == id
                                     select i.Client.Profile_Email).FirstOrDefault();
                var getclientName = (from i in db.Bookings
                                     where i.Book_ID == id
                                     select i.Client.Profile_Name).FirstOrDefault();
            var getlastName = (from i in db.Bookings
                               where i.Book_ID == id
                               select i.Client.Profile_Surname).FirstOrDefault();
            var getAddress = (from i in db.Orders
                                  where i.Book_ID == id
                                  select i.Bookings.Quote.ServiceRender_Address).FirstOrDefault();
            var getServiceType = (from i in db.Orders
                              where i.Book_ID == id
                              select i.Bookings.Quote.ServiceCat.Category).FirstOrDefault();


            ViewBag.Body = $"Dear " + getclientName + " " + getlastName + "<br/>" +
              $"You have successfully paid for booking of the serrvice renderring of "+getServiceType+". Please wait for admin to approve and schedule your booking."+
              $"<br/>" +
              $"Task A";
                Email email = new Email();
                email.Gmail("Booking confirmation", ViewBag.Body, getclientEmal);
            return Redirect(redirectUrl);
        }


        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion Payment

    }
}
