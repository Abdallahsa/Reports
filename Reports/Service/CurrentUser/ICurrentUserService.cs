namespace Reports.Api.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string? Email { get; }
        string? Lang { get; }
        string? Geha { get; }
        bool IsAuthenticated { get; }
        ICollection<string>? Role { get; }
    }
}
