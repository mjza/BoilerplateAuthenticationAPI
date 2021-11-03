using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class RevokeTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}