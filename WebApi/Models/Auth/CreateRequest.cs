using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities.Auth;

namespace WebApi.Models.Auth
{
    public class CreateRequest
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
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [EmailAddress(ErrorMessage = "EmailAddress")]
        [StringLength(255, ErrorMessage = "StringMaxLength")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [RegularExpression(@"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$",
            ErrorMessage = "PasswordRegularExpression")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        [Compare("Password", ErrorMessage = "PasswordCompare")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date, ErrorMessage ="DateDataType")]
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
    }
}