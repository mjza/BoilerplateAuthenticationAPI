using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        [EmailAddress]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        public string Password { get; set; }
    }
}