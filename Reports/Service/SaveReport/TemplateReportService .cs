using Reports.Common.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Reports.Service.SaveReport
{
    public class TemplateReportService : ITemplateReportService
    {
        private static readonly string TemplatesFolder =
            Path.Combine(Environment.CurrentDirectory, "wwwroot", "StaticFiles", "Templets");

        private static readonly string TargetDocsFolder =
            Path.Combine(Environment.CurrentDirectory, "wwwroot", "StaticFiles", "Docs");

        private static readonly string EncryptionKey = "12345678901234567890123456789012"; // 32 bytes for AES-256

        public TemplateReportService()
        {
            if (!Directory.Exists(TargetDocsFolder))
                Directory.CreateDirectory(TargetDocsFolder);

            if (!Directory.Exists(TemplatesFolder))
                throw new DirectoryNotFoundException($"Templates folder not found: {TemplatesFolder}");
        }

        public string CopyTemplateAndSave(string templateFileName, string reportType, string gehaCode)
        {
            var templateFullPath = Path.Combine(TemplatesFolder, templateFileName);
            if (!File.Exists(templateFullPath))
                throw new PathNotFoundException(templateFullPath);

            var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            var newFileName = $"{today}-{gehaCode}-{reportType}.docx";

            var fullDestinationPath = Path.Combine(TargetDocsFolder, newFileName);

            try
            {
                EncryptFile(templateFullPath, fullDestinationPath, EncryptionKey);
            }
            catch (Exception ex)
            {
                throw new PathNotFoundException($"Error encrypting and saving file: {ex.Message}");
            }

            return newFileName;
        }

        public byte[] GetDecryptedFile(string fileName)
        {
            var fullPath = Path.Combine(TargetDocsFolder, fileName);
            if (!File.Exists(fullPath))
                throw new PathNotFoundException(fullPath);

            return DecryptFileToBytes(fullPath, EncryptionKey);
        }

        public void DecryptFileInPlace(string fileName)
        {
            var fullPath = Path.Combine(TargetDocsFolder, fileName);
            if (!File.Exists(fullPath))
                throw new PathNotFoundException(fullPath);

            var decryptedBytes = DecryptFileToBytes(fullPath, EncryptionKey);
            File.WriteAllBytes(fullPath, decryptedBytes); // replace file with decrypted content
        }

        public void EncryptFileInPlace(string fileName)
        {
            var fullPath = Path.Combine(TargetDocsFolder, fileName);
            if (!File.Exists(fullPath))
                throw new PathNotFoundException(fullPath);

            var decryptedBytes = File.ReadAllBytes(fullPath);
            EncryptBytesToFile(decryptedBytes, fullPath, EncryptionKey);
        }

        private void EncryptFile(string inputPath, string outputPath, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV(); // unique IV per file

            using var outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            outFs.Write(aes.IV, 0, aes.IV.Length); // write IV at start

            using var cryptoStream = new CryptoStream(outFs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var inFs = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            inFs.CopyTo(cryptoStream);
        }

        private void EncryptBytesToFile(byte[] plainBytes, string outputPath, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();

            using var outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            outFs.Write(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(outFs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        }

        private byte[] DecryptFileToBytes(string inputPath, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);

            using var inFs = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            var iv = new byte[aes.BlockSize / 8];
            inFs.Read(iv, 0, iv.Length);
            aes.IV = iv;

            using var cryptoStream = new CryptoStream(inFs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var memory = new MemoryStream();
            cryptoStream.CopyTo(memory);
            return memory.ToArray();
        }
    }
}
