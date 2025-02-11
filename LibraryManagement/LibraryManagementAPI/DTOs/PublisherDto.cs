using static LibraryManagementAPI.DTOs.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class PublisherRequest
    {
        [Required(ErrorMessage = "Publisher name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Publisher name must be between 1 and 30 characters")]
        [RegularExpression(@"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-]+)$", ErrorMessage = "Publisher name contains invalid characters or forbidden words")]
        [DefaultNameNotAllowed("string", ErrorMessage = "Please enter a specific name")]
        public string Name { get; set; }
    }

    public class PublisherResponse
    {
        public int PublisherId { get; set; }
        public string Name { get; set; }
    }
}
