using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ResendVerificationTokenRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        [EmailAddress(ErrorMessage = "EmailAddress")]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Email { get; set; }
    }
}