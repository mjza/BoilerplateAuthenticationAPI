using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class VerifyEmailRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }
    }
}