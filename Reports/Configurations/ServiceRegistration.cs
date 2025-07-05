using Reports.Api.Auth.Services;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Api.Services.Notifications;
using Reports.Service.ApprovalService;
using Reports.Service.GehaService;
using Reports.Service.ReportService;
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
            services.AddScoped<IUserReportService, UserReportService>();
            services.AddScoped<IUserGehaService, UserGehaService>();
            services.AddScoped<IReportApprovalService, ReportApprovalService>();
            services.AddScoped<INotificationService, NotificationService>();


            //services.AddSingleton(services => new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile(new CartProfile(services.GetRequiredService<ICurrentUserService>(), services.GetRequiredService<IStorageService>()));
            //}).CreateMapper());
        }
    }
}
