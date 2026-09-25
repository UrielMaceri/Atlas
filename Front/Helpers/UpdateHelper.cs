using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace Front;

public sealed class UpdateService
{  
    // Github Repo location
    private readonly UpdateManager _manager = new(
        new GithubSource("https://github.com/UrielMaceri/Atlas", null, false)
    );

    public async Task<UpdateInfo?> Check()
    {
        return await _manager.CheckForUpdatesAsync();
    }

    public async Task DownloadAndRestartAsync(UpdateInfo update)
    {
        await _manager.DownloadUpdatesAsync(update);
        _manager.ApplyUpdatesAndRestart(update);
    }

    public string GetVersion()
    {
        return _manager.CurrentVersion?.ToString() ?? "dev";
    }

}