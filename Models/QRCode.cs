using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskA.Models
{
	public class QRCode
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int QRId { get; set; }
        [Display(Name = "QRCode Text")]
        public string QRCodeText { get; set; }
        [Display(Name = "QRCode Image")]
        public string QRCodeImagePath { get; set; }

         public int Order_ID { get; set; }
        public virtual Order Order{ get; set; }
    }
}