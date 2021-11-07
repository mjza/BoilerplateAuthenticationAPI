using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities.Auth;

namespace WebApi.Models.Auth
{
    public class UpdateRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;
        public string TitleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [EnumDataType(typeof(Role))]
        public string Role
        {
            get => _role;
            set => _role = ReplaceEmptyWithNull(value);
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$",
            ErrorMessage = "The Password field must contain 8 to 30 characters, minimum 1 lowecase, minumum 1 upercase, minimum 1 digits, and can have symbols.")]
        public string Password
        {
            get => _password;
            set => _password = ReplaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        public DateTime? Birthday { get; set; }

        public string GenderId { get; set; }

        public string Settings { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string PostCode { get; set; }
        
        // helpers
        private static string ReplaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}