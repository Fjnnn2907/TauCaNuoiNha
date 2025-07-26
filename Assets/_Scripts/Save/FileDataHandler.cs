using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FileDataHandler
{
    private string _fullPath;
    private bool _useEncryption;
    private readonly string _encryptionKey = "Your32ByteEncryptionKey123456789"; // Key phải 32 ký tự


    public string FullPath => _fullPath;

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _fullPath = Path.Combine(dataDirPath, dataFileName);
        _useEncryption = useEncryption;
    }

    // Save Data
    public async Task SaveDataAsync(GameData data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            string dataToSave = JsonUtility.ToJson(data, true);

            // Backup trước khi ghi đè
            if (File.Exists(_fullPath))
                File.Copy(_fullPath, _fullPath + ".backup", true);

            if (_useEncryption)
                dataToSave = await EncryptAsync(dataToSave);

            await File.WriteAllTextAsync(_fullPath, dataToSave);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {_fullPath}\n{e}");
        }
    }

    // Load Data
    public async Task<GameData> LoadDataAsync()
    {
        if (!File.Exists(_fullPath)) return null;

        try
        {
            string dataToLoad = await File.ReadAllTextAsync(_fullPath);

            if (_useEncryption)
                dataToLoad = await DecryptAsync(dataToLoad);

            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {_fullPath}\n{e}");
            return null;
        }
    }

    // Ma Hoa
    private async Task<string> EncryptAsync(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aes.IV = new byte[16];

        using MemoryStream ms = new();
        await using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            await cs.WriteAsync(plainBytes, 0, plainBytes.Length);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    // Giai ma hoa 
    private async Task<string> DecryptAsync(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
        aes.IV = new byte[16];

        using MemoryStream ms = new();
        await using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            await cs.WriteAsync(cipherBytes, 0, cipherBytes.Length);
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    // Xoa Data
    public void DeleteData()
    {
        if (File.Exists(_fullPath))
            File.Delete(_fullPath);
    }
}