using Microsoft.AspNetCore.Http;

namespace Reports.Api.Services
{
    public interface IStorageService
    {
        /// <summary>
        ///     Save file to static files asynchronous
        /// </summary>
        /// <param name="file">file to be saved</param>
        /// <param name="fileType">file type</param>
        /// <returns></returns>
        /// <exception cref="Backend.Exceptions.PathNotFound"></exception>
        Task<string> SaveFileAsync(IFormFile file, bool isUserDocs = false);

        /// <summary>
        ///     get the full path where file is actually located
        /// </summary>
        /// <param name="uniqueName">the file stored unique name </param>
        /// <returns>string:- full path</returns>
        string GetFullPath(string uniqueName, bool isUserDocs);
        
        /// <summary>
        ///     get the full path where file is actually located
        /// </summary>
        /// <param name="uniqueName">the file stored unique name </param>
        /// <returns>string:- full path</returns>
        string GetHostPath(string uniqueName, bool isUserDocs);

        /// <summary>
        ///     delete file from static files 
        /// </summary>
        /// <param name="uniqueName">the file stored unique name</param>
        /// <returns></returns>
        bool DeleteFile(string uniqueName, bool isUserDocs = false);
    }
}
