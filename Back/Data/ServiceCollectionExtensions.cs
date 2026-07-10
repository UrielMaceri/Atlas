using Back.Repositories;
using Back.Repositories.Interfaces;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Back.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasBackServices(this IServiceCollection services)
    {
        // DbContext ya migrado, una sola instancia para toda la app
        services.AddSingleton(_ => DbBootstrapper.CreateAndMigrate());

        // Repos (internal, nadie fuera de Back los ve)
        services.AddScoped<IItem, ItemRepo>();
        services.AddScoped<ITag, TagRepo>();
        services.AddScoped<ICategory, CategoryRepo>();
        services.AddScoped<IWorkspace, WorkspaceRepo>();

        // Services (públicos, esto es lo que Front va a pedir)
        services.AddScoped<ItemService>();
        services.AddScoped<TagService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<WorkspaceService>();

        return services;
    }
}