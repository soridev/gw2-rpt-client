﻿using RPTClient.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Filesystem;

public class ArcFileSystemWatcher
{
    private string _rootLogLocation;
    private bool _compressionEnabled;
    private PerformanceTrackerRest _restApi;
    private FileSystemWatcher? _fsWatcher;

    public ArcFileSystemWatcher(string rootLogLocation, bool compressionEnabled, PerformanceTrackerRest performanceTrackerRest)
    {
        _rootLogLocation = rootLogLocation;
        _compressionEnabled = compressionEnabled;
        _restApi = performanceTrackerRest;
    }

    public void StartFilesystemWatcher()
    {
        if (!Directory.Exists(_rootLogLocation))
        {
            throw new DirectoryNotFoundException("The given root directory for the arc logs does not exists.");
        }

        _fsWatcher = new FileSystemWatcher(_rootLogLocation);
        
        _fsWatcher.Changed += OnChanged;
        _fsWatcher.Created += OnCreated;
        _fsWatcher.Deleted += OnDeleted;
        _fsWatcher.Renamed += OnRenamed;
        _fsWatcher.Error += OnError;

        if (_compressionEnabled)
        {
            _fsWatcher.Filter = "*.zevtc";
        }
        else
        {
            _fsWatcher.Filter = "*.evtc";
        }

        _fsWatcher.IncludeSubdirectories = true;
        _fsWatcher.EnableRaisingEvents = true;
    }

    public void StopFilesystemWatcher()
    {
        if (_fsWatcher != null)
        {
            _fsWatcher.EnableRaisingEvents = false;
            _fsWatcher.Dispose();
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        // not needed atm.
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine("Created event triggered for  file: " + e.FullPath.ToString());

        string fullLocalPath = e.FullPath;
        UploadFile(fullLocalPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        // not needed atm.
    }

    private void OnRenamed(object sender, FileSystemEventArgs e)
    {
        Debug.WriteLine("Renamed event triggered for  file: " + e.FullPath.ToString());

        string fullLocalPath = e.FullPath;
        UploadFile(fullLocalPath);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        throw new Exception(string.Format("An error occurred while handling filesystem events: {0}", e.ToString()));
    }

    private void UploadFile(string pathToFile)
    {
        _restApi.Upload(pathToFile);
    }
}
