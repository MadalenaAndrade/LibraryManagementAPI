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
        public virtual Book Book { get; set; }


        public short CategoryID { get; set; }
        public virtual Category Category { get; set; }
    }
}
