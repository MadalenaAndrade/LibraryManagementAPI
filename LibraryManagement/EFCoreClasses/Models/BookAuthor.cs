using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("BookAuthor")]
    public class BookAuthor
    {
        public long SerialNumber { get; set; }
        public virtual Book Book { get; set; }
        

        public long AuthorID { get; set; }
        public virtual Author Author { get; set; } 
    }
}
