using Back.Repositories;
using Back.Repositories.Interfaces;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Back.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasBackServices(this IServiceCollection services)
    {
        // DbContext
        services.AddSingleton(_ => DbBootstrapper.CreateAndMigrate());

        // Repos
        services.AddScoped<IItem, ItemRepo>();
        services.AddScoped<ITag, TagRepo>();
        services.AddScoped<ICategory, CategoryRepo>();
        services.AddScoped<IWorkspace, WorkspaceRepo>();

        // Services
        services.AddScoped<ItemService>();
        services.AddScoped<TagService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<WorkspaceService>();

        return services;
    }
}