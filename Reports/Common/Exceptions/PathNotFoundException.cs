namespace Reports.Common.Exceptions
{
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException(string fullPath)
        : base($"Couldn't Find Directories in this path: {fullPath}\n check the correct path.")
        {
        }
    }
}
