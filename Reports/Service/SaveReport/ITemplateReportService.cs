namespace Reports.Service.SaveReport
{
    public interface ITemplateReportService
    {
        /// <summary>
        /// Copy template and save encrypted copy to docs folder with formatted name.
        /// </summary>
        string CopyTemplateAndSave(string templateFileName, string reportType, string gehaCode);

        /// <summary>
        /// Get decrypted file bytes from saved encrypted file (للاستخدام إذا عايز تبعته في response).
        /// </summary>
        byte[] GetDecryptedFile(string fileName);

        /// <summary>
        /// Decrypt the file in place (replace encrypted file with decrypted content).
        /// </summary>
        void DecryptFileInPlace(string fileName);

        /// <summary>
        /// Encrypt the file in place (replace decrypted file with encrypted content).
        /// </summary>
        void EncryptFileInPlace(string fileName);
    }
}
