using Reports.Api.Auth.Services;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Service.SaveReport;

namespace Reports.Api.Configurations
{
    public static class ServiceRegistration
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddHttpContextAccessor();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ITemplateReportService, TemplateReportService>();


            //services.AddSingleton(services => new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile(new CartProfile(services.GetRequiredService<ICurrentUserService>(), services.GetRequiredService<IStorageService>()));
            //}).CreateMapper());
        }
    }
}
