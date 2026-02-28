namespace Mala3ib.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi();


            services.AddExceptionHandler<GlobaExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
