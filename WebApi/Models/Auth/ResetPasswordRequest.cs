using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [RegularExpression(@"^(?=.*[a-z���])(?=.*[A-Z����])(?=.*\d)[a-z���A-Z����0-9\s!@#$%^&*�\/\? '+=)(<>;,.:_�`�-]{8,30}$",
            ErrorMessage = "PasswordRegularExpression")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [Compare("Password", ErrorMessage = "PasswordCompare")]
        public string ConfirmPassword { get; set; }
    }
}