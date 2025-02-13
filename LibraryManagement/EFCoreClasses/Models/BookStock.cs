using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("BookStock")]
    public class BookStock
    {
        [Key]
        public long SerialNumber { get; set; }
        public virtual Book Book { get; set; }

        [Required]
        public short TotalAmount { get; set; }

        [Required]
        public short AvailableAmount { get; set; }
    }
}
