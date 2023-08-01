using RPTClient.Models;

namespace RPTClient.Services.Contracts;

public interface ISettingsService
{
    public string SettingsFilePath { get; }
    public void SerializeSettings(UserSettings settings);
    public UserSettings DeserializeSettings();
}