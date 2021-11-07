using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.Auth
{
    public partial class Account
    {
        public Account()
        {
            RefreshTokens = new List<RefreshToken>();
        }

        [Key]
        public Guid Id { get; set; }
        public string TitleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string GenderId { get; set; }
        public string Settings { get; set; }
        public string NationalityId { get; set; }
        public Guid? ProfessionId { get; set; }        
        public bool? AcceptTerms { get; set; }
        public Role Role { get; set; }       
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostCode { get; set; }
        public virtual Title Title { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual Profession Profession { get; set; }
        [IgnoreDataMember]
        public virtual List<RefreshToken> RefreshTokens { get; set; }
        [IgnoreDataMember]
        public string VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        [IgnoreDataMember]
        public bool IsVerified => VerifiedAt.HasValue || PasswordResetedAt.HasValue;
        [IgnoreDataMember]
        public string ResetToken { get; set; }
        [IgnoreDataMember]
        public DateTime? ResetTokenExpiresAt { get; set; }
        [IgnoreDataMember]
        public DateTime? PasswordResetedAt { get; set; }
        [IgnoreDataMember]
        public string PasswordHash { get; set; }

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }        
    }
}