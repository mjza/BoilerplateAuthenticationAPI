using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class AuthenticateRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [RegularExpression(@"^[a-z]{2}(\-[A-Z]{2})?$",
            ErrorMessage = "The LocaleId field must be an ISO 639-1 code, the country code is optional.")]
        public string LocaleId { get; set; }
    }
}