using System.Text.Json.Serialization;
using RPTClient.Helpers;

namespace RPTClient.Models;

public class UserSettings
{
    [JsonInclude] public string PwdKeyBase64 = string.Empty;

    [JsonInclude] public string PwdVectorBase64 = string.Empty;

    [JsonInclude] public string UserKeyBase64 = string.Empty;

    [JsonInclude] public string UserVectorBase64 = string.Empty;

    public string DefaultArcFolderPath { get; set; } = string.Empty;

    public string DiscordWebhookUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public string Username
    {
        get
        {
            if (string.IsNullOrEmpty(UsernameEncrypted)) return "";
            return Encryption.DecryptDataWithAes(UsernameEncrypted, UserKeyBase64, UserVectorBase64);
        }
        set => UsernameEncrypted = Encryption.EncryptDataWithAes(value, out UserKeyBase64, out UserVectorBase64);
    }

    [JsonIgnore]
    public string Password
    {
        get
        {
            if (string.IsNullOrEmpty(PasswordEncrypted)) return "";
            return Encryption.DecryptDataWithAes(PasswordEncrypted, PwdKeyBase64, PwdVectorBase64);
        }
        set => PasswordEncrypted = Encryption.EncryptDataWithAes(value, out PwdKeyBase64, out PwdVectorBase64);
    }

    public string PasswordEncrypted { get; set; } = string.Empty;
    public string UsernameEncrypted { get; set; } = string.Empty;
}