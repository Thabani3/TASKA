using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskA.Models
{
	public class Operations
	{
        ApplicationDbContext db = new ApplicationDbContext();

        // get total cost for hours rendered for requested quotation
        public decimal GetHoursCost(Quote quote)
        { 
            var houly_cost = (from p in db.Quotes
                         where p.ServiceCat_ID == quote.ServiceCat_ID
                         select p.ServiceCat.Cost_Per_Hour).FirstOrDefault();
            return Convert.ToDecimal(Convert.ToDouble(houly_cost)*quote.Render_hours);
        }
        //return total cost for current quotes
		public decimal GetTravelCost(Quote quote)
		{
            decimal travel = (decimal)(quote.Destination_Distance/8)*(decimal)14.69;

             return travel;
		}

        //get total amount for successful quotation 
        public decimal GetQuoteCost(Booking booking)
        {
            var totalQuoteCost = (from p in db.Quotes
                              where p.Quote_ID== booking.Quote_ID
                              select p.Quote_Cost).FirstOrDefault();
            return Convert.ToDecimal(totalQuoteCost);
        }
        public int GetStatus(string status)
        {
            var statusId = (from p in db.BookingStatus
                            where p.Booking_Status == status
                            select p.BookingStatus_ID).FirstOrDefault();
            return statusId;
        }
        //Update status for traking of Reactions
        public void UpdateStatus(int id,string status)
        { 
           Booking updatingstatus = (from c in db.Bookings
                                     where c.Book_ID ==id
                                     select c).SingleOrDefault();
         updatingstatus.BookingStatus_ID = GetStatus(status);
         db.SaveChanges();
        }

    }
}