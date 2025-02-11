using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreClasses.Models
{
    [Table("Client")]
    public class Client
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(30)]
        public required string Name { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public int NIF { get; set; }

        [Required]
        public int Contact {  get; set; }

        [Required]
        public required string Address { get; set; }

        public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
    }
}
