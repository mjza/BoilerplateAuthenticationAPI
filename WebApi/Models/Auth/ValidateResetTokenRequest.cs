using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}