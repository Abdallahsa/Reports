using System.Security.Claims;

namespace Reports.Api.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            //Console.WriteLine(_httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString());
        }

        public int UserId => int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? Type => _httpContextAccessor.HttpContext?.User?.FindFirstValue("Type");


        public string Lang => (_httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString() == "ar" ? "ar" : "en");

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public ICollection<string>? Role => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();



    }
}
