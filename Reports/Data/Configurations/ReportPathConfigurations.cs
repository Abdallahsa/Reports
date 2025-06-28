using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reports.Domain.Entities;

namespace Reports.Data.Configurations
{
    public class ReportPathConfigurations : IEntityTypeConfiguration<ReportPath>
    {
        public void Configure(EntityTypeBuilder<ReportPath> builder)
        {

        }
    }
}
