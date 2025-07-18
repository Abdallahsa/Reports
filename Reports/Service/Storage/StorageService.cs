using Reports.Common.Exceptions;

namespace Reports.Api.Services;

public class StorageService : IStorageService
{
    private readonly string _webRootPath;

    public static string ImagesFolder => Path.Combine("StaticFiles", "Images");
    public static string DocsFolder => Path.Combine("StaticFiles", "Docs");

    public StorageService(IWebHostEnvironment env)
    {
        _webRootPath = env.WebRootPath;

        Directory.CreateDirectory(Path.Combine(_webRootPath, ImagesFolder));
        Directory.CreateDirectory(Path.Combine(_webRootPath, DocsFolder));
    }

    public async Task<string> SaveFileAsync(IFormFile file, bool isUserDocs = false)
    {
        if (file == null || file.Length == 0)
            throw new BadRequestException("Invalid file.");

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var fullPath = GetFullPath(uniqueFileName, isUserDocs);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(stream);
        }
        catch (Exception)
        {
            throw new PathNotFoundException(fullPath);
        }

        return uniqueFileName;
    }

    public string GetFullPath(string fileName, bool isUserDocs)
    {
        var relativePath = isUserDocs
            ? Path.Combine(DocsFolder, fileName)
            : Path.Combine(ImagesFolder, fileName);

        return Path.Combine(_webRootPath, relativePath);
    }

    public bool DeleteFile(string fileName, bool isUserDocs = false)
    {
        var fullPath = GetFullPath(fileName, isUserDocs);

        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch
        {
            throw new PathNotFoundException(fullPath);
        }

        return true;
    }

    public string GetHostPath(string fileName, bool isUserDocs)
    {
        var relativePath = isUserDocs
            ? Path.Combine(DocsFolder, fileName)
            : Path.Combine(ImagesFolder, fileName);

        return "/" + relativePath.Replace("\\", "/");
    }


}
