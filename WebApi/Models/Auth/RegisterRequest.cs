using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class RegisterRequest
    {
        [StringLength(5)]
        public string TitleId { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$", 
            ErrorMessage = "The Password field must contain 8 to 30 characters, minimum 1 lowercase, minumum 1 upercase, minimum 1 digit, and can have some symbols.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "The AcceptTerms field must be true.")]
        public bool AcceptTerms { get; set; }
    }
}