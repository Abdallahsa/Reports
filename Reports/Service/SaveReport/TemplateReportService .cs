using Reports.Common.Exceptions;

namespace Reports.Service.SaveReport
{
    public class TemplateReportService : ITemplateReportService
    {

        private static readonly string TemplatesFolder =
            Path.Combine(Environment.CurrentDirectory, "wwwroot", "StaticFiles", "Templets");

        private static readonly string TargetDocsFolder =
            Path.Combine(Environment.CurrentDirectory, "wwwroot", "StaticFiles", "Docs");

        public TemplateReportService()
        {
            if (!Directory.Exists(TargetDocsFolder))
                Directory.CreateDirectory(TargetDocsFolder);

            if (!Directory.Exists(TemplatesFolder))
                throw new DirectoryNotFoundException($"Templates folder not found: {TemplatesFolder}");
        }

        public string CopyTemplateAndSave(string templateFileName, string reportType, string gehaCode)
        {
            // المسار الكامل للـ template الأصلي
            var templateFullPath = Path.Combine(TemplatesFolder, templateFileName);
            if (!File.Exists(templateFullPath))
                throw new PathNotFoundException(templateFullPath);

            // توليد اسم الملف الجديد: 2024-10-21-AZ-DailyDeputyReport.docx
            var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            var newFileName = $"{today}-{gehaCode}-{reportType}.docx";

            // المسار النهائي النسبي في wwwroot
            var fullDestinationPath = Path.Combine(TargetDocsFolder, newFileName);

            try
            {
                File.Copy(templateFullPath, fullDestinationPath, overwrite: true);
            }
            catch (Exception)
            {
                throw new PathNotFoundException(fullDestinationPath);
            }

            // ترجّع اسم الملف فقط، أو ترجّع المسار النسبي (حسب اللي تحتاجه)
            return newFileName;
        }
    }
}
