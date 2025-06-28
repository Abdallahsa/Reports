using FluentValidation;
using MediatR;
using Reports.Common.Behaviors;
using System.Reflection;

namespace Reports.Api.Configurations
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {




            // register packages
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>)); // for validation pipeline
            });

            services.AddAutoMapper(typeof(ApplicationExtensions).Assembly);

            //services.AddHangfire(opt =>
            //{
            //    opt.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            //       .UseSimpleAssemblyNameTypeSerializer()
            //       .UseRecommendedSerializerSettings()
            //       .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
            //       {
            //           CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //          QueuePollInterval = TimeSpan.Zero,
            //          UseRecommendedIsolationLevel = true,
            //          UsePageLocksOnDequeue = true,
            //          DisableGlobalLocks = true
            //       });
            //});

            return services;
        }
    }
}
