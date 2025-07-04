using Reports.Api.Domain.Entities;

namespace Reports.Service.GehaService
{
    public class UserGehaService : IUserGehaService
    {
        private static readonly Dictionary<Level, List<Geha>> LevelToGehaMap = new()
        {
            {
                Level.LevelZero, new List<Geha>
                {
                    Geha.Eshara,
                    Geha.Operations,
                    Geha.Tahrokat,
                    Geha.Elc,
                    Geha.Mar,
                    Geha.Rader,
                    Geha.Sar,
                    Geha.Sat,
                    Geha.Tash,
                    Geha.AZ
                }
            },
            {
                Level.LevelOne, new List<Geha>
                {
                    Geha.NRA
                }
            },
            {
                Level.LevelTwo, new List<Geha>
                {
                    Geha.LM
                }
            },
            {
                Level.LevelThree, new List<Geha>
                {
                    Geha.RO
                }
            },
            {
                Level.LevelFour, new List<Geha>
                {
                    Geha.RA
                }
            },
            {
                Level.Admin, new List<Geha>
                {
                    Geha.None
                }
            }
        };

        public List<Geha> GetAllowedGehaByLevel(Level level)
        {
            if (LevelToGehaMap.TryGetValue(level, out var allowedGehas))
            {
                return allowedGehas;
            }

            return new List<Geha>(); // لو مفيش Level معروف
        }

        public bool IsUserAllowedToApproveReport(string userLevel, Level currentApprovalLevel)
        {
            // المستخدم لازم يكون في نفس المستوى الحالي للتقرير عشان يوافق
            return userLevel == currentApprovalLevel.ToString();
        }

    }
}
