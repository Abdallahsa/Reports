using Reports.Common.Exceptions;

namespace Reports.Api.Services;

public class StorageService : IStorageService
{
    public static string StaticFilesImagesPath => Path.Combine("StaticFiles", "Images");
    public static string StaticFilesDocsPath => Path.Combine("StaticFiles", "Docs");
    public StorageService()
    {
        if (!Directory.Exists(StaticFilesImagesPath))
        {
            Directory.CreateDirectory(StaticFilesImagesPath);
            Directory.CreateDirectory(StaticFilesDocsPath);
        }
    }
    public async Task<string> SaveFileAsync(IFormFile file, bool isUserDocs = false)
    {
        // Create a unique file name
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

        // get the full path -that will be located on- in the server
        var fullPath = GetFullPath(uniqueFileName, isUserDocs);
        // Save the file
        try
        {
            if (!Directory.Exists(StaticFilesImagesPath))
                Directory.CreateDirectory(StaticFilesImagesPath);

            if (!Directory.Exists(StaticFilesDocsPath))
                Directory.CreateDirectory(StaticFilesDocsPath);

            using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            await file.CopyToAsync(stream);
        }
        catch (Exception)
        {
            throw new PathNotFoundException(fullPath);
        }
        return uniqueFileName;
    }
    public string GetFullPath(string uniqueName, bool isUserDocs)
    {
        // Combine the full path with file name
        var accessPath = GetHostPath(uniqueName, isUserDocs);
        var fullPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", accessPath);
        return fullPath;
    }
    public bool DeleteFile(string uniqueName, bool isUserDocs = false)
    {
        var fullPath = GetFullPath(uniqueName, isUserDocs);
        try
        {
            File.Delete(fullPath);
        }
        catch
        {
            throw new PathNotFoundException(fullPath);
        }
        return true;
    }

    public string GetHostPath(string uniqueName, bool isUserDocs)
    {
        if (isUserDocs)
            return Path.Combine(StaticFilesDocsPath, uniqueName);

        return Path.Combine(StaticFilesImagesPath, uniqueName);

    }
}
