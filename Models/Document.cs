using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskA.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Document ID")]
        public int Documents_ID { get; set; }

        [DisplayName("ID Document")]
        public byte[] Documents { get; set; }

        public string Tasker_ID { get; set; }
        public virtual Tasker Tasker{ get; set; }
        public byte[] Document_Residence { get; internal set; }


    }
}