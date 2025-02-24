using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("BookCondition")]
    public class BookCondition
    {
        [Key]
        public short ID { get; set; }

        [Required, MaxLength(10)]
        public string Condition { get; set; }

        [Required, Column(TypeName = "decimal(3,2)")]
        public decimal FineModifier { get; set; }

        public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
        public virtual ICollection<RentReception> RentReceptions { get; set; } = new List<RentReception>();
    }
}
