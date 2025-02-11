using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCoreClasses;
using Microsoft.EntityFrameworkCore;

namespace EFCoreClasses.Models
{
    [Table("Book")]
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // not auto identity
        public long SerialNumber { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required]
        public short Year { get; set; }

        [Required, Column(TypeName = "decimal(4,2)")]
        public decimal FinePerDay { get; set; }

        [ForeignKey("Publisher")]
        public int PublisherID { get; set; }
        public virtual required Publisher Publisher { get; set; }


        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>(); // Navigation Property
        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>(); // Navigation Property
        public virtual required BookStock BookStock { get; set; } // Navigation Property
        public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>(); // Navigation Property
    }
}
