using Reports.Api.Domain.Entities;

namespace Reports.Common.Exceptions
{
    public static class GehaExtensions
    {
        private static readonly Dictionary<Geha, string> ArabicNames = new()
        {
            { Geha.AZ, "أسلحة وذخيرة" },
            { Geha.NRA, "نائب رئيس الأركان المنوب" },
            { Geha.LM, "لــواء منوب" },
            { Geha.RO, "رئيس أركان" },
            { Geha.RA, "رئيس أركان" },
            { Geha.Eshara, "إشارة" },
            { Geha.Operations, "عمليات" },
            { Geha.Tahrokat, "تحركات" },
            { Geha.None, "غير محدد" }
        };

        public static string ToArabic(this Geha geha)
        {
            return ArabicNames.TryGetValue(geha, out var arabic)
                ? arabic
                : "غير معرف";
        }
    }
}
