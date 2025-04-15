using System.ComponentModel.DataAnnotations;
using static LibraryManagementAPI.DTOs.CustomAttributes;

namespace LibraryManagementAPI.DTOs
{
    public class RentRequest
    {
        [Required(ErrorMessage = "Client Id is required")]
        public int ClientId { get; set; }

        [NumberValidation(1, 13, ErrorMessage = "Serial Number must be positive and have exactly 13 digits")]
        public long? BookSerialNumber { get; set; }

        [NumberValidation(1, ErrorMessage = "Id must be positive")]
        public int? BookCopyId { get; set; }

        [DateValidation("dd-MM-yyyy || dd/MM/yyyy", ErrorMessage = "Date must be valid and in the dd-MM-yyyy or dd/MM/yyyy format")]
        public string? StartDate { get; set; }
    }

    public class RentReceptionRequest
    {
        [Required(ErrorMessage = "Rent Id is required")]
        public long RentId { get; set; }

        [DateValidation("dd-MM-yyyy || dd/MM/yyyy", ErrorMessage = "Date must be valid and in the dd-MM-yyyy or dd/MM/yyyy format")]
        public string? ReturnDate { get; set; }

        [Required]
        [RegularExpression("^(?i)(As new|Good|Bad|Used)$", ErrorMessage = "BookCondition must be one of the following: 'As new', 'Good', 'Bad', 'Used'")]
        public string ReceivedCondition { get; set; }
    }

    public class RentResponse
    {
        [Required]
        public long RentId { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public int BookCopyId { get; set; }
        [Required]
        public long SerialNumber { get; set; }
        [Required]
        public string BookTitle { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }

    public class RentReceptionResponse
    {
        [Required]
        public long RentId { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
        [Required]
        public string ReceivedCondition { get; set; }
        [Required]
        public decimal TotalFine { get; set; }
    }
}
