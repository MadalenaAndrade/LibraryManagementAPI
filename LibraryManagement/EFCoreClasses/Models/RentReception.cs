using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("RentReception")]
    public class RentReception
    {
        [Key]
        public long RentID { get; set; }
        public virtual required Rent Rent { get; set; }

        [Required] 
        public DateTime ReturnDate { get; set; }

        [ForeignKey("BookCondition")]
        public short ReceivedConditionID { get; set; }
        public virtual required BookCondition BookCondition { get; set; }

        [Required, Column(TypeName = "decimal(7,2)")]
        public decimal TotalFine { get; set; }
    }
}
