using RPTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Services.Contracts
{
    public interface ISettingsService
    {
        public string SettingsFilePath { get; }
        public void SerializeSettings(UserSettings settings);
        public UserSettings DeserializeSettings();
    }
}
