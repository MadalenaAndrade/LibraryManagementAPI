using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using static LibraryManagementAPI.DTOs.CustomAttributes;

namespace LibraryManagementAPI.DTOs
{
    public class AuthorRequest
    {
        [Required(ErrorMessage = "Author name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters")]
        [RegularExpression(@"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-']+)$", ErrorMessage = "Name contains invalid characters or forbidden words")]
        [DefaultNameNotAllowed("string", ErrorMessage = "Please enter a specific name")]
        public string Name { get; set; }
    }

    public class AuthorResponse
    {
        [Required]
        public long AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

