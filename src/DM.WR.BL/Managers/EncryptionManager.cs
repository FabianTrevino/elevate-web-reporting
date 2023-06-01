using DM.WR.Models.Config;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DM.WR.BL.Managers
{
    //TODO:  Either create interface for this OR expose this functionality through a DM service
    public static class EncryptionManager
    {
        public static string Decrypt(string message, string securityKey = null)
        {
            if (message == null)
                return null;

            securityKey = securityKey ?? ConfigSettings.DmSecurityKey;
            var crypto = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, KeySize = 128 };
            var keyAndIv = Encoding.ASCII.GetBytes(securityKey);

            using (var ms = new MemoryStream())
            {
                var bytes = Convert.FromBase64String(message);
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                using (var cs = new CryptoStream(ms, crypto.CreateDecryptor(keyAndIv, keyAndIv), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                    return sr.ReadToEnd();
            }
        }

        public static string Encrypt(string message, string securityKey = null)
        {
            if (message == null)
                return null;

            securityKey = securityKey ?? ConfigSettings.DmSecurityKey;
            var crypto = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, KeySize = 128, Mode = CipherMode.CBC };

            var keyAndIv = Encoding.ASCII.GetBytes(securityKey);

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, crypto.CreateEncryptor(keyAndIv, keyAndIv), CryptoStreamMode.Write))
            {
                var data = Encoding.ASCII.GetBytes(message);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}