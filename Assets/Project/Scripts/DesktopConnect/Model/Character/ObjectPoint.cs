using System.Collections.Generic;
using UnityEngine;


public class ObjectPoint
{
    // キャラクターの周りに置くオブジェクトの候補点の角度
    private readonly float[] radians;
    // キャラクターの周りに置くオブジェクトの候補点の座標リスト
    public List<Vector2> candidatePoints { get; private set; }
    // スーパー楕円の変数
    private readonly float a = 0.8f;
    private readonly float b = 0.85f;
    private readonly float n = 5.0f;


    public ObjectPoint()
    {
        // 角度のセット(度数法から弧度法)
        radians = new float[8];
        var degrees = new float[8] { 45.0f, 135.0f, -45.0f, -135.0f, 90.0f, -90.0f, 0.0f, 180.0f };
        for (int i = 0; i < degrees.Length; i++)
        {
            radians[i] = degrees[i] * Mathf.Deg2Rad;
        }

        candidatePoints = new List<Vector2>();

        // スーパー楕円の変数セット
        CalcCoeffcient(6.0f, 6.0f);
    }


    /// <summary>
    /// スーパー楕円の変数の変更(キャラクターの大きさや各UIのサイズに合わせる) 未実装
    /// </summary>
    public void CalcCoeffcient(float diff1, float diff2)
    {
        var calcA = a * diff1;
        var calcB = b * diff2;

        CalcCandidatePoint(calcA, calcB);
    }


    /// <summary>
    /// 候補点の座標を更新
    /// </summary>
    private void CalcCandidatePoint(float calcA, float calcB)
    {
        candidatePoints.Clear();
        if (n == 0f)
        {
            UnityEngine.Debug.LogError("ObjectPoint : nが0です");
        }

        var exp = 2f / n;
        for (int i = 0; i < radians.Length; i++)
        {
            var cost = Mathf.Cos(radians[i]);
            var sint = Mathf.Sin(radians[i]);
            candidatePoints.Add(new Vector2
            {
                x = calcA * Mathf.Sign(cost) * Mathf.Pow(Mathf.Abs(cost), exp),
                y = calcB * Mathf.Sign(sint) * Mathf.Pow(Mathf.Abs(sint), exp)
            });
        }
    }

    public bool CheckInsideDisplay(Vector3 position, float right, float left, float top, float bottom)
    {
        // 各辺の座標を計算して変数にまとめる
        float rightEdge = position.x + right;
        float leftEdge = position.x + left;
        float topEdge = position.y + top;
        float bottomEdge = position.y + bottom;

        return rightEdge <= DisplayModel.displayWidthFloat && leftEdge >= -DisplayModel.displayWidthFloat && topEdge <= DisplayModel.displayHeightFloat && bottomEdge >= 0.0f;
    }
}
