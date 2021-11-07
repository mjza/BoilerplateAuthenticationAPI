using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class RegisterRequest
    {
        [StringLength(5, ErrorMessage = "StringMaxLength")]
        public string TitleId { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [EmailAddress]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "FieldRequired")]
        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$", 
            ErrorMessage = "PasswordRegularExpression")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [Compare("Password", ErrorMessage = "PasswordCompare")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "AcceptTermsRange")]
        public bool AcceptTerms { get; set; }
    }
}