using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reports.Domain.Entities;

namespace Reports.Data.Configurations
{
    public class ForgotPasswordRequestConfigurations : IEntityTypeConfiguration<ForgotPasswordRequest>
    {
        public void Configure(EntityTypeBuilder<ForgotPasswordRequest> builder)
        {
            
        }
    }
}
