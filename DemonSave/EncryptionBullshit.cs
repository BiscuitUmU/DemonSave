using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DemonSave
{
    public static class EncryptionBullshit
    { 
        public static bool TryDecrypt(string cipherText, string password, out string plainText)
        {
            if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(password))
            {
                plainText = "";
                return false;
            }
            bool result;
            try
            {
                var desServiceProvider = new DESCryptoServiceProvider();
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    var array = new byte[8];
                    memoryStream.Read(array, 0, array.Length);
                    var bytes = new Rfc2898DeriveBytes(password, array, 1000).GetBytes(8);
                    using (var cryptoStream = new CryptoStream(memoryStream, desServiceProvider.CreateDecryptor(bytes, array), CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            plainText = streamReader.ReadToEnd();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
                plainText = "";
                result = false;
            }
            return result;
        }
    }
}