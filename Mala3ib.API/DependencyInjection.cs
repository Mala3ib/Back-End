using Hangfire;
using Mala3ib.BLL.Helpers;
using Mala3ib.BLL.Settings;
using Mala3ib.DAL.Repo.Abstraction;
using Mala3ib.DAL.Repo.Implementation;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Mala3ib.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthConfig(configuration)
                    .AddValidationConfig()
                    .AddBackGroundJobsConfig(configuration);

            services.AddControllers();
            services.AddOpenApi();

            var connectionString = configuration.GetConnectionString("defaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJwtProvider, JwtProvider>();

            #region Repo
            services.AddScoped<IPlayerRepo, PlayerRepo>();
            #endregion

            services.AddHttpContextAccessor();
            services.AddExceptionHandler<GlobaExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        public static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.Name)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Mail Service
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.AddScoped<EmailBodyBuilder>();

            var jwtSettings = configuration.GetSection(JwtOptions.Name).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }

        public static IServiceCollection AddValidationConfig(this IServiceCollection services)
        {
            // Diffrent Assembly
            services.AddBLL();
            services.AddFluentValidationAutoValidation();
            return services;
        }
        public static IServiceCollection AddBackGroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            services.AddHangfireServer();

            return services;
        }
    }
}
