using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.Auth
{
    public partial class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}