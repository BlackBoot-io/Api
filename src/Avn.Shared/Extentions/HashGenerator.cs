using System.Security.Cryptography;
using System.Text;

namespace Avn.Shared.Extentions;

public static class HashGenerator
{
    private static readonly string secretKey = "Bl@ckb00t";
    public static string Hash(string key)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(key);
        byte[] bytes2 = Encoding.Unicode.GetBytes(secretKey);
        byte[] array = new byte[bytes2.Length + bytes.Length];
        Buffer.BlockCopy(bytes2, 0, array, 0, bytes2.Length);
        Buffer.BlockCopy(bytes, 0, array, bytes2.Length, bytes.Length);
        return Convert.ToBase64String(HashAlgorithm.Create("SHA256")!.ComputeHash(array));
    }

    public static string Hash(string key, string salt)
    {

        byte[] bytes = Encoding.Unicode.GetBytes(key);
        byte[] bytes2 = Encoding.Unicode.GetBytes(salt);
        byte[] array = new byte[bytes2.Length + bytes.Length];
        Buffer.BlockCopy(bytes2, 0, array, 0, bytes2.Length);
        Buffer.BlockCopy(bytes, 0, array, bytes2.Length, bytes.Length);
        return Convert.ToBase64String(HashAlgorithm.Create("SHA256")!.ComputeHash(array));
    }
    public static string EncryptString(string plainText)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(secretKey);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                streamWriter.Write(plainText);
            }
            array = memoryStream.ToArray();
        }

        return Convert.ToBase64String(array);
    }

    public static string DecryptString(string cipherText)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(secretKey);
        aes.IV = iv;
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new(cryptoStream);
        return streamReader.ReadToEnd();
    }

}
