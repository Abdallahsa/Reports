using Reports.Api.Domain.Entities;

namespace Reports.Domain.Primitives
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public BaseAuditableEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
