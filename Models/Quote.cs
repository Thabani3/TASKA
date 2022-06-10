using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace TaskA.Models
{
    public class Quote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Quote ID")]
        public int Quote_ID { get; set; }

        // make current (Don't show user this #ish)
        [DisplayName("Date Produced")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date_Produced { get; set; }

        [DisplayName("Desired Service")]
        public int ServiceCat_ID { get; set; }
        public virtual ServiceCat ServiceCat { get; set; }

        [Required(ErrorMessage = "Render service address is required")]
        [DisplayName("Physical address")]
        public string ServiceRender_Address { get; set; }

        [DisplayName("TaskA Coporation address")]
        public string TaskA_Address { get; set; }

        [DisplayName("Distance (km)")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Out of range")]
        public double Destination_Distance { get; set; }

        [Required(ErrorMessage = "Render service duration is required")]
        [DataType(DataType.Duration)]
        [DisplayName("Estimated rendering hours (minutes)")]
        public int Render_hours { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Transportation Cost")]
        public decimal Transport_Cost { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Service Rendered Cost")]
        public decimal RenderService_Cost { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Discount")]
        public decimal Discount_Cost { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Total cost")]
        public decimal Quote_Cost { get; set; }

        public virtual List<Booking> Bookings { get; set; }

    }
}
