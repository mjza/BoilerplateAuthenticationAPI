using System;

namespace WebApi.Models.Auth
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string TitleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string GenderId { get; set; }
        public string Settings { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerified { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostCode { get; set; }
    }
}