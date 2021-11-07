using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [RegularExpression(SharedResource.PasswordRegEx, ErrorMessage = "PasswordRegularExpression")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [Compare("Password", ErrorMessage = "PasswordCompare")]
        public string ConfirmPassword { get; set; }
    }
}