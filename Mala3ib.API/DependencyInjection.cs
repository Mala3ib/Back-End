using Mala3ib.BLL.Helpers;
using Mala3ib.BLL.Settings;

namespace Mala3ib.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthConfig(configuration)
                    .AddValidationConfig();

            services.AddControllers();
            services.AddOpenApi();

            var connectionString = configuration.GetConnectionString("defaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJwtProvider, JwtProvider>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KTVcuuOTiQkGaFkwtoUe7BKR8rrE7CKo")),
                    ValidIssuer = "Mala3ibApp",
                    ValidAudience = "Mala3ibApp users"
                };
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
    }
}
