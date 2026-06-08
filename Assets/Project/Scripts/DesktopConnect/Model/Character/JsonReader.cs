using JsonAniamtion.Model;
using System;
using System.IO;
using UnityEngine;

public class JsonReader
{
    /// <summary>
    /// StreamingAsset内のJsonファイルの読み込み
    /// </summary>
    public JsonClipDataM LoadJsonFile(string fileName)
    {
        // StreamingAssetsフォルダのパスを作成
        string filePath = Path.Combine(Application.streamingAssetsPath + "\\Animation", fileName);

        if (File.Exists(filePath))
        {
            // テキストとして読み込む
            string jsonContent = File.ReadAllText(filePath);
            // JsonUtilityでデシリアライズ
            JsonClipDataM data = JsonUtility.FromJson<JsonClipDataM>(jsonContent);
            
            return data;
        }
        else
        {
            Debug.LogError("JsonReader : ファイルが見つかりません: " + filePath);
            return null;
        }
    }


    /// <summary>
    /// Jsonデータの読み込み
    /// </summary>
    public JsonClipDataM DecodeJson(string jsonData)
    {
        try
        {
            // JsonUtilityでデシリアライズ
            JsonClipDataM data = JsonUtility.FromJson<JsonClipDataM>(jsonData);
            
            return data;
        }
        catch (ArithmeticException e)
        {
            Debug.LogError("JsonReader : 例外が発生しました : " + e);
            return null;
        }
    }
}