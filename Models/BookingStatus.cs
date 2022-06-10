using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskA.Models
{
	public class BookingStatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int BookingStatus_ID { get; set; }

		[DisplayName("Booking Status ")]
		public string Booking_Status  { get; set; }

		public virtual List<Booking> Bookings { get; set; }
	}
}