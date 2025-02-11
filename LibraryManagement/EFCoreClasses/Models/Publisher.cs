using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("Publisher")]
    public class Publisher
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(30)]
        public required string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
