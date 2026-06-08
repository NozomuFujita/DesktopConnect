using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Facial2Array
{
    public Dictionary<string, List<float>> facialData { get; private set; }

    public Facial2Array()
    {
        // StreamingAssetsフォルダのパスを作成
        string filePath = Path.Combine(Application.streamingAssetsPath + "\\PoseData", "FacialData.csv");
        
        // csvファイルが別アプリケーションで使用されていても開けるようにする
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(fs);

        facialData = new Dictionary<string, List<float>>();

        // csvファイルの末尾まで繰り返し
        while(!sr.EndOfStream)
        {
            string line = sr.ReadLine();

            // 列ごとの分割と空白(null)処理
            string[] value = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var values = new List<float>();
            for (int i = 1; i < value.Length; i++)
            {
                values.Add(float.Parse(value[i]));
            }
            facialData.Add(value[0], values);
        }
    }
}
