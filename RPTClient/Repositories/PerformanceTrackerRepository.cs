using Microsoft.Toolkit.Mvvm.ComponentModel;
using RPTClient.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Repositories;

public class PerformanceTrackerRepository
{
    private PerformanceTrackerRest _performanceTrackerRest;

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
}
