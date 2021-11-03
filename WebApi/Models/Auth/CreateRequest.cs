using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities.Auth;

namespace WebApi.Models.Auth
{
    public class CreateRequest
    {
        [Required]
        [RegularExpression(@"^[a-z]{2}(\-[A-Z]{2})?$",
            ErrorMessage = "The LocaleId field must be an ISO 639-1 code, the country code is optional.")]
        public string LocaleId { get; set; }

        [StringLength(5)]
        public string TitleId { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$",
            ErrorMessage = "The Password field must contain 8 to 30 characters, minimum 1 lowecase, minumum 1 upercase, minimum 1 digits, and can have symbols.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(5)]
        public string GenderId { get; set; }
        
        public string Settings { get; set; }

        [StringLength(255)]
        public string Street { get; set; }

        [StringLength(255)]
        public string Number { get; set; }

        [StringLength(255)]
        public string PostCode { get; set; }
    }
}