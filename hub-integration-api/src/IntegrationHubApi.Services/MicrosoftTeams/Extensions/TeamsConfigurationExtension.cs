using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;

namespace IntegrationHubApi.Services.MicrosoftTeams.Extensions
{
    public static class TeamsConfigurationExtension
    {
        public static void Encrypt(this TeamsConfiguration config, string key)
        {
            var keyAsBytes = Encoding.ASCII.GetBytes(key.Split('|')[0]);
            var pairAsBytes = Encoding.ASCII.GetBytes(key.Split('|')[1]);
            byte[] inputbyteArray = Encoding.UTF8.GetBytes(config.OrganizerPassword);

            using DESCryptoServiceProvider des = new();
            using MemoryStream ms = new();
            using var cs = new CryptoStream(ms, des.CreateEncryptor(keyAsBytes, pairAsBytes), CryptoStreamMode.Write);
            
            cs.Write(inputbyteArray, 0, inputbyteArray.Length);
            cs.FlushFinalBlock();
            config.OrganizerPassword = Convert.ToBase64String(ms.ToArray());

        }

        public static void Decrypt(this TeamsConfiguration config, string key)
        {
            var pairAsBytes = Encoding.ASCII.GetBytes(key.Split('|')[1]);
            var keyAsBytes = Encoding.ASCII.GetBytes(key.Split('|')[0]);
            var inputbyteArray = Convert.FromBase64String(config.OrganizerPassword);

            using DESCryptoServiceProvider des = new();
            using MemoryStream ms = new();
            using var cs = new CryptoStream(ms, des.CreateDecryptor(keyAsBytes, pairAsBytes), CryptoStreamMode.Write);

            cs.Write(inputbyteArray, 0, inputbyteArray.Length);
            cs.FlushFinalBlock();
            config.OrganizerPassword = Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
