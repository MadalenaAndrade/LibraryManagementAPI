using static LibraryManagementAPI.DTOs.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class CreateClientRequest
    {
        [Required(ErrorMessage = "Client name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters")]
        [RegularExpression(@"^(?!.*\b(?i)(drop|delete|insert|update|select|alter|table)\b)([\p{L}\s\.\-']+)$", ErrorMessage = "Name contains invalid characters or forbidden words")]
        [DefaultNameNotAllowed("string", ErrorMessage = "Please enter a specific name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Client date of birth is required")]
        [DateValidation("dd-MM-yyyy || dd/MM/yyyy", ErrorMessage = "Date must be valid and in the dd-MM-yyyy or dd/MM/yyyy format")]
        public string DateOfBirth { get; set; }

        // NIF is the portuguese VAT, validation was done considering its own rules (same for contact)
        [Required]
        [NIFValidation(ErrorMessage = "NIF is invalid")]
        public int NIF { get; set; }

        [Required]
        [NumberValidation(1, 9, ErrorMessage = "Contact number must be positive and have exactly 9 digits")]
        public int Contact { get; set; }

        [Required]
        public string Address { get; set; }
    }

    public class GetClientRequest
    {
        [NumberValidation(1, ErrorMessage = "Id must be positive")]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? NIF { get; set; }
    }

    public class UpdateClientRequest
    {
        [NumberValidation(1, 9, ErrorMessage = "Contact number must be positive and have exactly 9 digits")]
        public int? Contact { get; set; }
        public string? Address { get; set; }
    }


    public class ClientResponse
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public int NIF { get; set; }

        [Required]
        public int Contact { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
