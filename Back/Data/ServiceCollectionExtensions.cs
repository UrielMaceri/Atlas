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
        services.AddSingleton<IItem, ItemRepo>();
        services.AddSingleton<ITag, TagRepo>();
        services.AddSingleton<ICategory, CategoryRepo>();
        services.AddSingleton<IWorkspace, WorkspaceRepo>();

        // Services
        services.AddSingleton<ItemService>();
        services.AddSingleton<TagService>();
        services.AddSingleton<CategoryService>();
        services.AddSingleton<WorkspaceService>();

        return services;
    }
}