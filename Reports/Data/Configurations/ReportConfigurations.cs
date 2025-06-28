using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reports.Domain.Entities;

namespace Reports.Data.Configurations
{
    public class ReportConfigurations : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasMany(x => x.Paths)
                .WithOne(x => x.Report)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.Approvals)
                .WithOne(x => x.Report)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ReportType);


        }
    }
}
