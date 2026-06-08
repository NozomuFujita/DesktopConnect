using UnityEngine;

public static class WeightSigmoid
{
    /// <summary>
    /// Interpolationパラメータを使ったシグモイド関数の値を返す
    /// 式: 1 / (1 + exp(-a * (x - b)))
    /// </summary>
    public static float GetSigmoidValue(Vector2 interpolation, float time)
    {
        // 時間を -1 ~ 1 にクランプ
        time = Mathf.Clamp(time, -1.0f, 1.0f);

        float a = 1.0f; // 傾き（ゲイン
        float b = 0.0f; // 中心（オフセット)

        if (interpolation != null)
        {
            a = interpolation.x;
            b = interpolation.y;
        }

        // シグモイド関数の計算

        // 指数部分の計算
        float exponent = -a * (time - b);

        // 指数関数の計算
        float expVal = Mathf.Exp(exponent);

        return 1.0f / (1.0f + expVal);
    }
}