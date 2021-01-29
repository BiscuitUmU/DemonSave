using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DemonSave
{
    internal class Program
    {
        private const string SavePassword = "f5461b921115fcd45bcd624588d53cd9402a1d43b7c404cf25b877fb23d75e2b";

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Error("Please drag your save file on top of the executable.");
                return;
            }

            var fileInfo = new FileInfo(args[0]);
            if (!fileInfo.Exists)
            {
                Error("Could not find save file.");
                return;
            }

            Console.WriteLine("Save does not include .dec Assuming it's encrypted...");
            DecryptSave(fileInfo);
            return;
        }

        private static void DecryptSave(FileInfo fileInfo)
        {
            var encryptedSave = File.ReadAllBytes(fileInfo.FullName);
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(encryptedSave))
            {
                var saveString = binaryFormatter.Deserialize(memoryStream) as string;

                if (!EncryptionBullshit.TryDecrypt(saveString, SavePassword, out var json))
                {
                    Error("Failed to read save data!");
                    return;
                }

                var fileDir = fileInfo.Directory?.FullName;
                File.WriteAllText(fileDir + "/DTSaveData.dec.txt", json);

                Console.WriteLine("Save file decrypted successfully... Have a nice day!");
            }
        }

        private static void Error(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }
}