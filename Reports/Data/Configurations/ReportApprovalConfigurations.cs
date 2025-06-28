using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reports.Domain.Entities;

namespace Reports.Data.Configurations
{
    public class ReportApprovalConfigurations : IEntityTypeConfiguration<ReportApproval>
    {
        public void Configure(EntityTypeBuilder<ReportApproval> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.Approvals)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
