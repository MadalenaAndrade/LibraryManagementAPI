using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("Author")]
    public class Author
    {
        [Key]
        public long ID { get; set; } 
        
        [Required, MaxLength(30)]
        public required string Name { get; set; } 
        
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>(); // Navigation Property
    }
}
