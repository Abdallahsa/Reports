namespace Reports.Service.SaveReport
{
    public interface ITemplateReportService
    {
        /// <summary>
        /// Copy template and save encrypted copy to docs folder with formatted name.
        /// </summary>
        /// <param name="templateFileName">Original template file name (e.g., DailyDeputyReport.docx)</param>
        /// <param name="reportType">Report type string (e.g., DailyDeputyReport)</param>
        /// <param name="gehaCode">Geha code string (e.g., AZ, NRA)</param>
        /// <returns>Unique saved file name</returns>
        /// <exception cref="Backend.Exceptions.PathNotFound"></exception>
        string CopyTemplateAndSave(string templateFileName, string reportType, string gehaCode);

        /// <summary>
        /// Get decrypted file bytes from saved encrypted file.
        /// </summary>
        /// <param name="fileName">File name stored in docs folder (e.g., 2024-10-21-AZ-DailyDeputyReport.docx)</param>
        /// <returns>File content as byte array</returns>
        /// <exception cref="Backend.Exceptions.PathNotFound"></exception>
        byte[] GetDecryptedFile(string fileName);
    }
}
