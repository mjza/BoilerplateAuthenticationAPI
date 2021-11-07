using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class RevokeTokenRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }
    }
}