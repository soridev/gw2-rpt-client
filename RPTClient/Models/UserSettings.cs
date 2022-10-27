using Microsoft.Toolkit.Mvvm.ComponentModel;
using RPTClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPTClient.Models
{
    public class UserSettings
    {
        public string DefaultArcFolderPath { get; set; } = String.Empty;
        [JsonIgnore]
        public string Username
        {
            get
            {
                if (String.IsNullOrEmpty(UsernameEncrypted)) return "";
                return Encryption.DecryptDataWithAes(UsernameEncrypted, UserKeyBase64, UserVectorBase64);
            }
            set
            {
                UsernameEncrypted = Encryption.EncryptDataWithAes(value, out UserKeyBase64, out UserVectorBase64);
            }
        }
        [JsonIgnore]
        public string Password
        {
            get
            {
                if (String.IsNullOrEmpty(PasswordEncrypted)) return "";
                return Encryption.DecryptDataWithAes(PasswordEncrypted, PwdKeyBase64, PwdVectorBase64);
            }
            set
            {
                PasswordEncrypted = Encryption.EncryptDataWithAes(value, out PwdKeyBase64, out PwdVectorBase64);
            }
        }
        public string PasswordEncrypted { get; set; } = String.Empty;
        public string UsernameEncrypted { get; set; } = String.Empty;

        [JsonInclude]
        public string PwdKeyBase64 = String.Empty;
        [JsonInclude]
        public string PwdVectorBase64 = String.Empty;
        [JsonInclude]
        public string UserKeyBase64 = String.Empty;
        [JsonInclude]
        public string UserVectorBase64 = String.Empty;
    }
}
