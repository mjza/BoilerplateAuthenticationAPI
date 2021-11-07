using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ValidateResetTokenRequest
    {
        [Required(ErrorMessage = "FieldRequired")]
        public string Token { get; set; }
    }
}