
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; //to order the properties in swagger documentation
using Microsoft.OpenApi.MicrosoftExtensions;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using static LibraryManagementAPI.DTOs.CustomAttributes;

namespace LibraryManagementAPI.DTOs
{
    // requests
    public class CreateBookRequest
    {
        [Required(ErrorMessage = "Book serial number is required")]
        [NumberValidation(1, 13, ErrorMessage = "Serial Number must be positive and have exactly 13 digits")]
        public long SerialNumber { get; set; }

        [Required(ErrorMessage = "Title name is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Book published year is required")]
        [YearValidation(1900, ErrorMessage = "Published year must be after 1900 and before the current year")]
        public short Year { get; set; }

        [Required(ErrorMessage = "The default book delayed fine is required")]
        [Range(0.01, 10.00, ErrorMessage = "Fine must be between 0 and 10 Euros")]
        public decimal FinePerDay { get; set; }


        [Required(ErrorMessage = "Publisher name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Publisher name must be between 1 and 30 characters")]
        [RegularExpression(@"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-]+)$", ErrorMessage = "Publisher name contains invalid characters or forbidden words")]
        [DefaultNameNotAllowed("string", ErrorMessage = "Please enter a specific name")]
        public string PublisherName { get; set; }

        [RequiredList(ErrorMessage = "At least one author is required.")]
        public List<AuthorRequest> Authors { get; set; }

        [RequiredList(ErrorMessage = "At least one category is required.")]
        public List<CategoryRequest> Categories { get; set; } 

        [Required(ErrorMessage = "The total amount of books is required")]
        [Range(1, 50, ErrorMessage = "Total amount of books must be between 1 and 50")]
        public short TotalAmount { get; set; }
    }

    public class GetBookRequest
    {
        [NumberValidation(1, 13, ErrorMessage = "Serial Number must be positive and have exactly 13 digits")]
        public long? SerialNumber { get; set; }
        
        public string? Title { get; set;  }

        [YearValidation(1900, ErrorMessage = "Published year must be after 1900 and before the current year")]
        public short? Year { get; set; }

        public string? Publisher {  get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
    }

    public class UpdateBookRequest
    {
        [SwaggerSchema(Description = "New book title (optional)")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title name must be between 1 and 255 characters")]
        public string? Title { get; set; }

        [SwaggerSchema(Description = "New book year (optional)")]
        [YearValidation(1900, ErrorMessage = "Published year must be after 1900 and before the current year")]
        public short? Year { get; set; }

        [SwaggerSchema(Description = "New book fine per day (optional)")]
        [Range(0.01, 10.00, ErrorMessage = "Fine must be between 0 and 10 Euros")]
        public decimal? FinePerDay { get; set; }

        [SwaggerSchema(Description = "New book publisher (optional)")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Publisher name must be between 1 and 30 characters")]
        [RegularExpression(@"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-]+)$", ErrorMessage = "Publisher name contains invalid characters or forbidden words")]
        public string? Publisher { get; set; }

        [SwaggerSchema(Description = "List of authors (optional)")]
        public List<AuthorRequest>? Authors { get; set; }

        [SwaggerSchema(Description = "List of categories(optional)")]
        public List<AuthorRequest>? Categories { get; set; }
    }


    // response
    public class BookResponse
    {
        [Required]
        public long SerialNumber { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public short Year { get; set; }
        [Required]
        public decimal FinePerDay { get; set; }
        [Required]
        public string PublisherName { get; set; }
        [Required]
        public List<string> AuthorName { get; set; }
        [Required]
        public List<string> CategoryName { get; set; }
        [Required]
        public short TotalAmount { get; set; }
        [Required]
        public short AvailableAmount { get; set; }
    }   
}
