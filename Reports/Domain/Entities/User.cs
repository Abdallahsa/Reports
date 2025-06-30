using Microsoft.AspNetCore.Identity;
using Reports.Domain.Entities;

namespace Reports.Api.Domain.Entities
{
    public class User : IdentityUser<int>
    {

        public Level Level { get; set; }
        public string Geha { get; set; } = string.Empty;
        public string SignaturePath { get; set; } = string.Empty;

        public ICollection<ReportApproval> Approvals { get; set; } = new List<ReportApproval>();
    }

    public enum Level
    {
        Admin,
        LevelZero,
        LevelOne,
        LevelTwo,
        LevelThree,
        LevelFour

    }
    public enum Geha
    {
        AZ,
        NRA,
        LM,
        RO,
        RA,
        Eshara,
        Operations,
        Tahrokat,
        None 
    }

}
