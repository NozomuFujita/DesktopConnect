using JsonAniamtion;
using System;
using System.Reflection;
using UnityEngine;

public class AnimationChanger
{
    private static AnimationChanger instance;

    private JsonReader jsonReader;

    private AnimationChanger()
    {
        jsonReader = new JsonReader();
    }

    public static AnimationChanger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AnimationChanger();
            }
            return instance;
        }
    }

    public JsonClipData GetJsonAnimation(string emotion1, float percent1, string message)
    {
        string fileName = emotion1 + ".json";
        var clipData = jsonReader.LoadJson(fileName);

        // メッセージの更新
        clipData.Message = message;

        // DefaultChangeの適用 (TotalFrameの変更など)
        if (clipData.DefaultChange != null && !string.IsNullOrEmpty(clipData.DefaultChange.Target))
        {
            if (clipData.DefaultChange.Target == "TotalFrame")
            {
                ChangeBasis(clipData, clipData.DefaultChange.Min, clipData.DefaultChange.Max, percent1);
            }
        }

        // PoseChangeの適用
        if (clipData.PoseChange != null)
        {
            foreach (var poseChange in clipData.PoseChange)
            {
                int targetIndex = poseChange.Index;

                // PosesはListなので .Count で範囲内かチェック
                if (targetIndex >= 0 && targetIndex < clipData.Poses.Count)
                {
                    // 対象のPoseデータを取得
                    var changePose = clipData.Poses[targetIndex];

                    if (poseChange.Posture != null)
                    {
                        foreach (var posture in poseChange.Posture)
                        {
                            ChangePosture(changePose, posture.Target, posture.Min, posture.Max, percent1);
                        }
                    }

                    if (poseChange.Finger != null)
                    {
                        foreach (var finger in poseChange.Finger)
                        {
                            ChangeFinger(changePose, finger.Target, finger.Value);
                        }
                    }
                }
            }
        }

        return clipData;
    }


    private void ChangeBasis(JsonClipData jsonClipData, int min, int max, float percent)
    {
        var frame = Mathf.FloorToInt(Mathf.Lerp(min, max, percent));
        jsonClipData.TotalFrame = frame;
    }


    private void ChangePosture(PoseData changePose, string path, float min, float max, float percent)
    {
        string[] fieldNames = path.Split('.');
        object currentObject = changePose;
        Type currentType = changePose.GetType();

        // 変える値の手前のオブジェクトまで移動
        for (int i = 0; i < fieldNames.Length - 1; i++)
        {
            string field = fieldNames[i];
            var fieldInfo = currentType.GetField(field);
            if (fieldInfo == null)
            {
                Debug.LogWarning($"Field {field} not found in {currentType.Name}");
                return;
            }
            currentObject = fieldInfo.GetValue(currentObject);
            if (currentObject == null)
            {
                return;
            }
            currentType = currentObject.GetType();
        }

        // 値の更新
        var targetName = fieldNames[fieldNames.Length - 1];
        var targetField = currentType.GetField(targetName);

        if (targetField != null)
        {
            var value = Mathf.Lerp(min, max, percent);
            targetField.SetValue(currentObject, value);
        }
        else
        {
            Debug.LogWarning($"Target field {targetName} not found in {currentType.Name}");
        }
    }


    private void ChangeFinger(PoseData changePose, string path, string fingerValue)
    {
        string[] fieldNames = path.Split('.');
        object currentObject = changePose;
        Type currentType = changePose.GetType();

        // 変える値の手前のオブジェクトまで移動
        for (int i = 0; i < fieldNames.Length - 1; i++)
        {
            string field = fieldNames[i];
            var fieldInfo = currentType.GetField(field);
            if (fieldInfo == null)
            {
                Debug.LogWarning($"Field {field} not found in {currentType.Name}");
                return;
            }
            currentObject = fieldInfo.GetValue(currentObject);
            if (currentObject == null)
            {
                return;
            }
            currentType = currentObject.GetType();
        }

        // 値の更新
        var targetName = fieldNames[fieldNames.Length - 1];
        var targetField = currentType.GetField(targetName);

        if (targetField != null)
        {
            targetField.SetValue(currentObject, fingerValue);
        }
        else
        {
            Debug.LogWarning($"Target field {targetName} not found in {currentType.Name}");
        }
    }
}