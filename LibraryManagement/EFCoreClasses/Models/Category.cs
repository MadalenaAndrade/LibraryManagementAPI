using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public short ID { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
