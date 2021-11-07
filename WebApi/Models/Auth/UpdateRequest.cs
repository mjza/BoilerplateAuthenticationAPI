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

        [StringLength(5, ErrorMessage = "StringMaxLength")]
        public string TitleId { get; set; }

        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string FirstName { get; set; }

        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string LastName { get; set; }

        [EnumDataType(typeof(Role))]
        public string Role
        {
            get => _role;
            set => _role = ReplaceEmptyWithNull(value);
        }

        [EmailAddress(ErrorMessage = "EmailAddress")]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$",
            ErrorMessage = "PasswordRegularExpression")]
        public string Password
        {
            get => _password;
            set => _password = ReplaceEmptyWithNull(value);
        }

        [Compare("Password", ErrorMessage = "PasswordCompare")]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        [DataType(DataType.Date, ErrorMessage = "DateDataType")]
        public DateTime? Birthday { get; set; }

        [StringLength(5, ErrorMessage = "StringMaxLength")]
        public string GenderId { get; set; }

        public string Settings { get; set; }

        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Street { get; set; }

        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Number { get; set; }

        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string PostCode { get; set; }
        
        // helpers
        private static string ReplaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}