using System;
using System.Data;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Back.Data;
using Microsoft.Extensions.DependencyInjection;
using Velopack;
using Velopack.Sources;


namespace Front;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    private static readonly UpdateService Updater = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddAtlasBackServices();
        Services = services.BuildServiceProvider();
        
        Services.GetRequiredService<AtlasDbContext>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            _ = Task.Run(async () =>
            {
                try
                {
                    var update = await Updater.Check();
                    if (update is null)
                        return;

                    var version = update.TargetFullRelease.Version;
                    var mainWindow = desktop.MainWindow as MainWindow;
                    if (mainWindow?.DataContext is MainWindowViewModel viewModel)
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                            viewModel.ShowNotification(
                                $"Update available: {version}. Restart Atlas to install it."));
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(AppContext.BaseDirectory, "update-check.log"),
                        ex.ToString());
                }
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}