using Microsoft.WindowsAPICodePack.Dialogs;
using RPTClient.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Services;

public class LogDialogService : ILogDialogService
{
    public LogDialogService()
    {
        
    }

    public string OpenArcFolderDialog()
    {
        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        string intialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs";
        dialog.InitialDirectory = intialDirectory;
        dialog.IsFolderPicker = true;

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            return dialog.FileName;
        }
        return String.Empty;
    }
}
