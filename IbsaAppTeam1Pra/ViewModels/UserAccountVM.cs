using IbsaAppTeam1Pra.Models;
using System.ComponentModel.DataAnnotations;

namespace IbsaAppTeam1Pra.ViewModels
{
    public class UserAccountVM
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "OIB is required.")]
        [StringLength(11, ErrorMessage = "OIB must be 11 characters long.", MinimumLength = 11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "OIB must be 11 digits long.")]
        public string Oib { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password must be between 8 and 20 characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(12, ErrorMessage = "Account number must be 12 characters long.", MinimumLength = 12)]
        [RegularExpression(@"^[A-Za-z]{2}\d{10}$", ErrorMessage = "Account number must be in the format 2 letters followed by 10 digits.")]
        public string AccountNumber { get; set; } = null!;

        public decimal Balance { get; set; }

        public string Currency { get; set; } = null!;

        public List<Account> Accounts { get; set; }

        public string NewAccountNumber { get; set; }
    }
}
