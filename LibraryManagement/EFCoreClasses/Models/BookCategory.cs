using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("BookCategory")]
    public class BookCategory 
    {
        public long SerialNumber { get; set; }
        public virtual required Book Book { get; set; }


        public short CategoryID { get; set; }
        public virtual required Category Category { get; set; }
    }
}
