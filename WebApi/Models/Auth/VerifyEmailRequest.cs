using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}