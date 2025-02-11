using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("BookCopy")]
    public class BookCopy
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Book")]
        public long SerialNumber { get; set; }
        public virtual required Book Book { get; set; }

        [ForeignKey("BookCondition")]
        public short BookConditionID { get; set; }
        public virtual required BookCondition BookCondition { get; set; }

        [Required, Column(TypeName = "nvarchar(max)")]
        public required string Notes { get; set; }

        public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
    }
}
