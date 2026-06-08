using UnityEngine;
using System;
using System.Collections.Generic;
using DisplaySystem;

using Cysharp.Threading.Tasks;
using System.Threading;

public class Tester : MonoBehaviour
{
    private List<WindowInfo> windowInfos;
    public int anchor = 0;


    void Start()
    {
        windowInfos = DisplayModel.windowsAPI.GetEnumWindows();
        foreach (var windowInfo in windowInfos)
        {
            UnityEngine.Debug.Log(windowInfo.title);
        }
        //var minInfo = DisplayModel.windowsAPI.GetWindowMinSize(windowInfos[0].hWnd);
        //UnityEngine.Debug.Log(minInfo.ptMinTrackSize.x + ":" + minInfo.ptMinTrackSize.y);

        //var real = DisplayModel.windowsAPI.GetWindowSystemRect(windowInfos[0].hWnd);
        //UnityEngine.Debug.Log(real.width + ":" + real.height);

        //DisplayModel.windowsAPI.SetWindowPos(windowInfos[0].hWnd, 0, 0, minInfo.ptMinTrackSize.x, minInfo.ptMinTrackSize.y);

        AnimateRectLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }


    /// <summary>
    /// //////////////////////////////////////////////////////////////
    /// </summary>

    private async UniTaskVoid AnimateRectLoopAsync(CancellationToken cancellationToken)
    {
        // 状態1と状態2を定義
        var rect1 = new DisplaySystem.RectInt { left = 600, top = 400, right = 1200, bottom = 1000 };
        var rect2 = new DisplaySystem.RectInt { left = 300, top = 650, right = 1500, bottom = 850 };

        float duration = 0.5f; // 遷移にかける時間（秒）

        // キャンセルされるまで無限ループ
        while (!cancellationToken.IsCancellationRequested)
        {
            // --- [1 => 2 の遷移] ---
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                // 0.0 ~ 1.0 の t を計算
                float t = elapsedTime / duration;

                // 変形処理を呼び出し
                Deformation(rect1, rect2, anchor, t);

                // 次のフレームまで待機
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsedTime += Time.deltaTime;
            }
            // ズレ防止のため、最後に確実に t=1 の状態をセット
            Deformation(rect1, rect2, anchor, 1f);


            // --- [2 => 1 の遷移] ---
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                // 0.0 ~ 1.0 の t を計算
                float t = elapsedTime / duration;

                // rect2 から rect1 へ遷移
                Deformation(rect2, rect1, anchor, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsedTime += Time.deltaTime;
            }
            // 確実に最終状態をセット
            Deformation(rect2, rect1, anchor, 1f);
        }
    }


    /// <summary>
    /// //////////////////////////////////////////////////////////////
    /// </summary>



    private DisplaySystem.RectInt blendRectInt = new DisplaySystem.RectInt();

    private void Deformation(DisplaySystem.RectInt from, DisplaySystem.RectInt to, int anchor, float t)
    {
        var currentWidth = (int)Mathf.Lerp(from.width, to.width, t);
        int currentHeight = (int)Mathf.Lerp(from.height, to.height, t);

        switch (anchor)
        {
            // 下を固定した場合
            case 2:
                blendRectInt.left = (int)Mathf.Lerp(from.left, to.left, t);
                
                blendRectInt.right = (int)Mathf.Lerp(from.right, to.right, t);
                blendRectInt.bottom = (int)Mathf.Lerp(from.bottom, to.bottom, t);

                blendRectInt.top = blendRectInt.bottom - currentHeight;
                break;

            // 左を固定した場合
            case 4:
                blendRectInt.left = from.left;
                blendRectInt.top = (int)Mathf.Lerp(from.top, to.top, t);
                blendRectInt.right = (int)Mathf.Lerp(from.right, (to.right - (to.left - from.left)), t);
                blendRectInt.bottom = (int)Mathf.Lerp(from.bottom, to.bottom, t);
                break;

            // 中央を固定した場合
            case 5:
                blendRectInt.left = (int)Mathf.Lerp(from.left, to.left, t);
                blendRectInt.top = (int)Mathf.Lerp(from.top, to.top, t);
                blendRectInt.right = (int)Mathf.Lerp(from.right, to.right, t);
                blendRectInt.bottom = (int)Mathf.Lerp(from.bottom, to.bottom, t);
                break;

            // 右を固定した場合
            case 6:
                blendRectInt.left = (int)Mathf.Lerp(from.left, (to.left - (to.right - from.right)), t);
                blendRectInt.top = (int)Mathf.Lerp(from.top, to.top, t);
                blendRectInt.right = from.right;
                blendRectInt.bottom = (int)Mathf.Lerp(from.bottom, to.bottom, t);
                break;

            // 上を固定した場合
            case 8:
                blendRectInt.left = (int)Mathf.Lerp(from.left, to.left, t);
                blendRectInt.top = from.top;
                blendRectInt.right = (int)Mathf.Lerp(from.right, to.right, t);
                blendRectInt.bottom = (int)Mathf.Lerp(from.bottom, (to.bottom - (to.top - from.top)), t);
                break;
        }

        DisplayModel.windowsAPI.SetWindowPos(windowInfos[0].hWnd, blendRectInt.left, blendRectInt.top, blendRectInt.width, blendRectInt.height);
    }
}
