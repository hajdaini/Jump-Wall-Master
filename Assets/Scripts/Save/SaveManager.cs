using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveManager
{
    // look at the bottom to see how to generate it
    const string KEY = "9m4YZRtHzkwIbeystCseXeOOFcLqQasBSJzgCOmfurY=";
    const string IV = "08MPGMDqW7dzWpu5GROVjw==";

    const string PATH = "oXdgzLyWNIfFaVtXhNaqMrGbaa.json";

    public static void SaveData(GameData Data){
        bool encrypted = true;
        if(Global.instance.isTestMode) encrypted = false;
        string path = Application.persistentDataPath + $"/{PATH}";

        try{
            if (File.Exists(path)) File.Delete(path); // If file Deleting old file and writing a new one
            using FileStream stream = File.Create(path);
            if (encrypted) WriteEncryptedData(Data, stream);
            else {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            }
        } catch (Exception e) {
            Debug.LogWarning($"Unable to save gameData due to: {e.Message} {e.StackTrace}");
        }
    }

    static void WriteEncryptedData<GameData>(GameData Data, FileStream Stream){
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(
            Stream,
            cryptoTransform,
            CryptoStreamMode.Write
        );

        // You can uncomment the below to see a generated value for the IV & key.
        // You can also generate your own if you wish
        //Debug.Log($"Initialization Vector: {Convert.ToBase64String(aesProvider.IV)}");
        //Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Data)));
    }


    public static GameData LoadData(){
        bool encrypted = true;
        if(Global.instance.isTestMode) encrypted = false;
        string path = Application.persistentDataPath + $"/{PATH}";

        if (!File.Exists(path)) SaveData(new GameData());

        try{
            GameData gameData;
            if (encrypted) gameData = ReadEncryptedData<GameData>(path);
            else gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(path));
            return gameData;
        } catch (Exception e){
            Debug.LogWarning($"Failed to load gameData due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    static GameData ReadEncryptedData<GameData>(string Path){
        byte[] fileBytes = File.ReadAllBytes(Path);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );
        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(
            decryptionStream,
            cryptoTransform,
            CryptoStreamMode.Read
        );
        using StreamReader reader = new StreamReader(cryptoStream);
        string result = reader.ReadToEnd();

        // if(!Global.instance.isTestMode) Debug.LogWarning($"Decrypted result (if the following is not legible, probably wrong key or iv): {result}");

        return JsonConvert.DeserializeObject<GameData>(result);
    }

    // void CreateAES(){
    //     // Create new AES instance.
    //     Aes aes = Aes.Create();

    //     aes.KeySize = 256;
    //     aes.BlockSize = 128;
    //     aes.Padding = PaddingMode.Zeros;
    //     aes.Mode = CipherMode.CBC;
    
    //     var key = Convert.ToBase64String(aes.Key);
    //     var iv = Convert.ToBase64String(aes.IV);

    //     Debug.Log(key);
    //     Debug.Log(iv);
    // }
}
