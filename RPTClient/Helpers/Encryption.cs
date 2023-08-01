using System;
using System.IO;
using System.Security.Cryptography;

namespace RPTClient.Helpers;

public static class Encryption
{
    public static string EncryptDataWithAes(string plainText, out string keyBase64, out string vectorBase64)
    {
        using (var aesAlgorithm = Aes.Create())
        {
            //set the parameters with out keyword
            keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
            vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

            // Create encryptor object
            var encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    encryptedData = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }
    }

    public static string DecryptDataWithAes(string cipherText, string keyBase64, string vectorBase64)
    {
        using (var aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

            // Create decryptor object
            var decryptor = aesAlgorithm.CreateDecryptor();

            var cipher = Convert.FromBase64String(cipherText);

            //Decryption will be done in a memory stream through a CryptoStream object
            using (var ms = new MemoryStream(cipher))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}