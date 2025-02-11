using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("Rent")]
    public class Rent
    {
        [Key]
        public long ID { get; set; }

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public virtual required Client Client { get; set; }

        [ForeignKey("BookCopy")]
        public int BookCopyID { get; set; }
        public virtual required BookCopy BookCopy { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
        
        public virtual required RentReception RentReception { get; set; }
    }
}
