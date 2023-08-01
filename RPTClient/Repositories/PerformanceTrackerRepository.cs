using System.Threading.Tasks;
using RPTClient.Filesystem;
using RPTClient.Rest;

namespace RPTClient.Repositories;

public class PerformanceTrackerRepository
{
    private ArcFileSystemWatcher? _arcFileSystemWatcher;
    private readonly PerformanceTrackerRest _performanceTrackerRest;

    public PerformanceTrackerRepository()
    {
        _performanceTrackerRest = new PerformanceTrackerRest();
    }

    public void Login(string username, string password)
    {
        _performanceTrackerRest.Login(username, password);
    }

    public Task<int> GetRemoteLogCount()
    {
        return _performanceTrackerRest.GetRemoteLogCount();
    }

    public void StartFSWatcher(string rootLogLocation)
    {
        _arcFileSystemWatcher = new ArcFileSystemWatcher(rootLogLocation, true, _performanceTrackerRest);
        _arcFileSystemWatcher.StartFilesystemWatcher();
    }

    public void StopFSWatcher()
    {
        _arcFileSystemWatcher.StopFilesystemWatcher();
    }
}