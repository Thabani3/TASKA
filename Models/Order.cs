using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskA.Models
{

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Order ID")]
        public int Order_ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Order_DateTime { get; set; }

        [DisplayName("Booking")]
        public int Book_ID { get; set; }
        public virtual Booking Bookings { get; set; }

        [ScaffoldColumn(false)]
        public string Reference { get; set; }
        [DisplayName("Tasker Name")]
        public string Tasker_ID { get; set; }
        public virtual Tasker Tasker { get; set; }
        [DisplayName("Work Status")]
        public int OrderStatus_ID { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public string Driver_ID { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual List<QRCode> QRCodes { get; set; }
        public virtual List<Beneficiary_Signature> Beneficiary_Signatures { get; set; }
    
        ApplicationDbContext db = new ApplicationDbContext();
        public int GetStatus(string status)
        {
            var statusId = (from p in db.OrderStatuses
                            where p.Status_Name == status
                            select p.OrderStatus_ID).FirstOrDefault();
            return statusId;
        }
        public void UpdateStatus(int id, string status)
        {
            Order upDatestatus = (from c in db.Orders
                                  where c.Order_ID == id
                                  select c).SingleOrDefault();
            upDatestatus.OrderStatus_ID = GetStatus(status);
            db.SaveChanges();
        }
    }
}