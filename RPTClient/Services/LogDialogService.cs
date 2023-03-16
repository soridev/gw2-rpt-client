using System;
using Microsoft.WindowsAPICodePack.Dialogs;
using RPTClient.Services.Contracts;

namespace RPTClient.Services;

public class LogDialogService : ILogDialogService
{
    public string OpenArcFolderDialog()
    {
        var dialog = new CommonOpenFileDialog();
        var intialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                              "\\Documents\\Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs";
        dialog.InitialDirectory = intialDirectory;
        dialog.IsFolderPicker = true;

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok) return dialog.FileName;
        return string.Empty;
    }
}