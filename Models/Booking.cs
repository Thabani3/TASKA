using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

namespace TaskA.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Bookiing ID")]
        public int Book_ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Booking Date")]
        public DateTime Booking_date { get; set; }

        [Required(ErrorMessage = "Date to render sevicer is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Service Renderng Date")]
        public DateTime Booking_RenderDate { get; set; }

        [Required(ErrorMessage = " Recipient name is required")]
        [DisplayName("Recipient Name")]
        public string Book_RecipientName { get; set; }

        [Required(ErrorMessage = "Recipient surname is required")]
        [DisplayName("Recipient Surname")]
        public string Book_RecipientSurname { get; set; }

		public bool Paymentstatus { get; set; }
	
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        [Required(ErrorMessage = "Recipient contact number is required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                  ErrorMessage = "Entered Contact number format is not valid.")]
        public string Book_RecipientNumber { get; set; }

        [StringLength(250, MinimumLength = 2, ErrorMessage = "The Delivery Note should be between 2 - 250 Characters")]
        [DisplayName("Render Service Note")]
        [DataType(DataType.MultilineText)]
        public string Booking_RenderNote { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Total Booking Cost")]
        public decimal Book_TotalCost { get; set; }

        public int Quote_ID { get; set; }
        public virtual Quote Quote { get; set; }
		public int BookingStatus_ID { get; set; }
		public virtual BookingStatus BookingStatus { get; set; }
		public string Profile_ID { get; set; }
        public virtual Client Client { get; set; }

        public virtual List<Order> Order { get; set; }

       

    }
}
