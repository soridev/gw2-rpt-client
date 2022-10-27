using RPTClient.Models;
using RPTClient.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPTClient.Services
{
    internal class SettingsService : ISettingsService
    {
        public string SettingsFilePath
        {
            get
            {
                return $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\RPTClient\settings.json";
            }
        }

        public string BaseDirectory
        {
            get
            {
                return $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\RPTClient";
            }
        }

        /// <summary>
        /// Deserializes the settings.json file if exists.
        /// </summary>
        /// <returns>The saves user-settings.</returns>
        public UserSettings DeserializeSettings()
        {
            try
            {
                if (!Directory.Exists(BaseDirectory))
                {
                    return new UserSettings();
                }

                using (StreamReader reader = new StreamReader(SettingsFilePath))
                {
                    string json = reader.ReadToEnd();

                    if (String.IsNullOrEmpty(json))
                    {
                        return new UserSettings();
                    }
                    return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Serializes the user-settings as a json in the app-folder.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async void SerializeSettings(UserSettings settings)
        {
            try
            {
                if (!Directory.Exists(SettingsFilePath))
                {
                    Directory.CreateDirectory(BaseDirectory);
                }

                string json = JsonSerializer.Serialize(settings);

                await File.WriteAllTextAsync(SettingsFilePath, json);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
