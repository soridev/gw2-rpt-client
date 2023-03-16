using System;
using System.IO;
using System.Text.Json;
using RPTClient.Models;
using RPTClient.Services.Contracts;

namespace RPTClient.Services;

internal class SettingsService : ISettingsService
{
    public string BaseDirectory => $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\RPTClient";

    public string SettingsFilePath =>
        $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\RPTClient\settings.json";

    /// <summary>
    ///     Deserializes the settings.json file if exists.
    /// </summary>
    /// <returns>The saves user-settings.</returns>
    public UserSettings DeserializeSettings()
    {
        if (!Directory.Exists(BaseDirectory)) return new UserSettings();

        using (var reader = new StreamReader(SettingsFilePath))
        {
            var json = reader.ReadToEnd();

            if (string.IsNullOrEmpty(json)) return new UserSettings();
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
    }

    /// <summary>
    ///     Serializes the user-settings as a json in the app-folder.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public async void SerializeSettings(UserSettings settings)
    {
        if (!Directory.Exists(SettingsFilePath)) Directory.CreateDirectory(BaseDirectory);

        var json = JsonSerializer.Serialize(settings);

        await File.WriteAllTextAsync(SettingsFilePath, json);
    }
}