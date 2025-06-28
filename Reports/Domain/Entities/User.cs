using Microsoft.AspNetCore.Identity;
using Reports.Domain.Entities;

namespace Reports.Api.Domain.Entities
{
    public class User : IdentityUser<int>
    {

        public int Level { get; set; }
        public string Geha { get; set; } = string.Empty;
        public string SignaturePath { get; set; } = string.Empty;

        public ICollection<ReportApproval> Approvals { get; set; } = new List<ReportApproval>();
    }

}
