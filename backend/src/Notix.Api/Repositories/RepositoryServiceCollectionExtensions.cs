namespace Notix.Api.Repositories;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddNotixRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();
        return services;
    }
}
