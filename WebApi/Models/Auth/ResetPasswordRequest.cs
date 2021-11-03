using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$",
            ErrorMessage = "The Password field must contain 8 to 30 characters, minimum 1 lowecase, minumum 1 upercase, minimum 1 digits, and can have symbols.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}