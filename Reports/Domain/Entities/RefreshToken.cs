using Reports.Domain.Primitives;

namespace Reports.Api.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        public int UserId { get; set; }
        public User? User { get; set; }


        public RefreshToken()
        {
            Token = string.Empty;
            Expires = DateTime.UtcNow;
        }
    }
}
