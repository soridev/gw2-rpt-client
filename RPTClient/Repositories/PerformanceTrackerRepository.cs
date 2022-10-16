using Microsoft.Toolkit.Mvvm.ComponentModel;
using RPTClient.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Repositories;

public class PerformanceTrackerRepository
{
    private int _remoteLogCounter = 0;
    private PerformanceTrackerRest _performanceTrackerRest = PerformanceTrackerRest.Instance;

    public PerformanceTrackerRepository()
    {
        
    }
}
