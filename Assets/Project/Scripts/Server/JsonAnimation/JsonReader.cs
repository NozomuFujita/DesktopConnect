using System.IO;
using UnityEngine;

namespace JsonAniamtion
{
    public class JsonReader
    {

        public JsonClipData LoadJson(string fileName)
        {
            // StreamingAssetsフォルダのパスを作成
            string filePath = Path.Combine(Application.streamingAssetsPath + "\\Animation", fileName);

            if (File.Exists(filePath))
            {
                // テキストとして読み込む
                string jsonContent = File.ReadAllText(filePath);

                // JsonUtilityでデシリアライズ
                JsonClipData data = JsonUtility.FromJson<JsonClipData>(jsonContent);

                return data;
            }
            else
            {
                Debug.LogError("ファイルが見つかりません: " + filePath);
                return null;
            }
        }
    }
}