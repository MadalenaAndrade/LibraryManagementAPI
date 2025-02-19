
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

        [Required(ErrorMessage = "Author name is required")]
        public List<string> AuthorName { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public List<string> CategoryName { get; set; }


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
    }

    public class UpdateBookAuthorRequest
    {
        public string? OldAuthorName { get; set; }

        [Required]
        public string NewAuthorName { get; set; }
    }

    public class UpdateBookCategoryRequest
    {
        public string? OldCategoryName { get; set; }

        [Required]
        public string NewCategoryName { get; set; }
    }



    // responses
    public class BaseBookResponse
    {
        [Required]
        [JsonPropertyOrder(1)]
        public long SerialNumber { get; set; }

        [Required]
        [JsonPropertyOrder(2)]
        public string Title { get; set; }

        [Required]
        [JsonPropertyOrder(3)]
        public short Year { get; set; }
    }

    public class BookResponse : BaseBookResponse
    {
        [Required]
        [JsonPropertyOrder(4)]
        public decimal FinePerDay { get; set; }

        [Required]
        [JsonPropertyOrder(5)]
        public string PublisherName { get; set; }

        [Required]
        [JsonPropertyOrder(6)]
        public List<string> AuthorName { get; set; }

        [Required]
        [JsonPropertyOrder(7)]
        public List<string> CategoryName { get; set; }

        [Required]
        [JsonPropertyOrder(8)]
        public short TotalAmount { get; set; }

        [Required]
        [JsonPropertyOrder(9)]
        public short AvailableAmount { get; set; }
    }

    public class UpdateBookResponse : BaseBookResponse
    {
        [Required]
        [JsonPropertyOrder(4)]
        public decimal FinePerDay { get; set; }

        [Required]
        [JsonPropertyOrder(5)]
        public string PublisherName { get; set; }
    }

    public class UpdateBookAuthorResponse : BaseBookResponse
    {
        [Required]
        [JsonPropertyOrder(4)]
        public List<string> AuthorNames { get; set; }
    }

    public class UpdateBookCategoryResponse : BaseBookResponse
    {
        [Required]
        [JsonPropertyOrder(4)]
        public List<string> CategoryNames { get; set; }
    }
}
