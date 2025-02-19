using static LibraryManagementAPI.DTOs.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class BookCopyRequest
    {
        [Required(ErrorMessage = "Book serial number is required")]
        [NumberValidation(1, 13, ErrorMessage = "Serial Number must be positive and have exactly 13 digits")]
        public long SerialNumber { get; set; }

        [RegularExpression("^(?i)(As new|Good|Bad|Used)$", ErrorMessage = "BookCondition must be one of the following: 'As new', 'Good', 'Bad', 'Used'")]
        public string? BookCondition { get; set; }

        [Required(ErrorMessage = "The amount of bookcopies is required")]
        [Range(1, 10, ErrorMessage = "Total amount of books must be between 1 and 10")]
        public short Amount { get; set; }

        public string? Notes { get; set; }
    }

    public class GetBookCopyRequest
    {
        [NumberValidation(1, ErrorMessage = "Id must be positive")]
        public int? Id { get; set; }

        [NumberValidation(1, 13, ErrorMessage = "Serial Number must be positive and have exactly 13 digits")]
        public long? SerialNumber { get; set; }

        public string? Title { get; set; }

        [RegularExpression("^(?i)(As new|Good|Bad|Used)$", ErrorMessage = "BookCondition must be one of the following: 'As new', 'Good', 'Bad', 'Used'")]
        public string? Condition { get; set; }
    }

    public class UpdateBookCopyRequest
    {
        [RegularExpression("^(?i)(As new|Good|Bad|Used)$", ErrorMessage = "BookCondition must be one of the following: 'As new', 'Good', 'Bad', 'Used'")]
        public string? NewCondition { get; set; }

        public string? NewNotes { get; set; }
    }


    public class BookCopyResponse
    {
        [Required]
        public long SerialNumber { get; set; }
        [Required]
        public string BookCondition { get; set; }
        [Required]
        public short Amount { get; set; }
        [Required]
        public string Notes { get; set; }
    }

    public class GetBookCopyResponse
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public long SerialNumber { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string BookCondition { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}
