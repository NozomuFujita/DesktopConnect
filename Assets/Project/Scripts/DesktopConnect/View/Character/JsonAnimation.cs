using Cysharp.Threading.Tasks;
using JsonAniamtion.Presenter;
using System;
using System.Threading;
using UnityEngine;

public class JsonAnimation
{
    private Animator animator;
    private HumanPoseHandler poseHandler;
    private HumanPose humanPose;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh mesh;

    // 前と後のポーズデータから算出された中間のブレンドポーズを保存するための変数
    private Vector3 blendPosition;
    private Quaternion blendQuaternion;
    private float[] blendPosture;

    private CancellationTokenSource cts;


    /// <summary>
    /// インスタンスの生成、Humanoid機能を使うための初期化
    /// </summary>
    public JsonAnimation(Animator animator, SkinnedMeshRenderer skinnedMeshRenderer)
    {
        this.animator = animator;
        poseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
        humanPose = new HumanPose();
        this.skinnedMeshRenderer = skinnedMeshRenderer;
        mesh = skinnedMeshRenderer.sharedMesh;

        //ブレンドポーズの変数の初期化
        blendPosition = new Vector3(0.0f, 0.0f, 0.0f);
        blendQuaternion = new Quaternion();
        blendQuaternion.Set(0, 0, 0, 1);
        blendPosture = new float[HumanTrait.MuscleCount];
    }


    /// <summary>
    /// 引数で渡されたアニメーションデータの再生
    /// </summary>
    public void PlayAnimation(JsonClipDataP ClipData)
    {
        // nullやループ再生ができるデータ(poseデータが2つ以上)でないとエラー
        if (ClipData == null || ClipData.Poses == null || ClipData.Poses.Count < 2)
        {
            UnityEngine.Debug.LogError("JsonAnimation: 有効なポーズデータがありません。");
            return;
        }

        // 現在のアニメーションを停止
        StopCurrentAnimation();
        cts = new CancellationTokenSource();

        // 再生
        PlayAnimationLoopAsync(ClipData, cts.Token).Forget();
    }


    /// <summary>
    /// アニメーションの再生処理
    /// </summary>
    private async UniTask PlayAnimationLoopAsync(JsonClipDataP clipData, CancellationToken token)
    {
        // 総ポーズ数の取得
        int totalPoses = clipData.Poses.Count;
        int currentIndex = 0;

        // FPS値から、アニメーション全体の秒数を決定
        float totalDuration = (clipData.FPS > 0) ? (float)clipData.TotalFrame / clipData.FPS : (float)clipData.TotalFrame / 60.0f;

        while (!token.IsCancellationRequested)
        {
            int nextIndex = (currentIndex + 1) % totalPoses;

            // 前のポーズデータと後のポーズデータの取得
            var currentPose = clipData.Poses[currentIndex];
            var nextPose = clipData.Poses[nextIndex];

            // ポーズ間の長さを計算(秒数)
            float currentProportion = currentPose.Proportion;
            float nextProportion = nextPose.Proportion;
            float ProportionDiff = nextProportion - currentProportion;
            if (ProportionDiff < 0)
            {
                //ポーズの繰り返し時Proportionがマイナスになるため
                ProportionDiff += 1.0f;
            }
            float segmentDuration = ProportionDiff * totalDuration;

            float timeElapsed = 0f;
            while (timeElapsed < segmentDuration)
            {
                timeElapsed += Time.deltaTime;

                // 進行度0~1
                // シグモイド関数から補間係数を決定(1 / 1 - exp^(-a(x-b)))
                float linearProgress = Mathf.Clamp01(timeElapsed / segmentDuration);
                float sigmoidInput = Mathf.Lerp(-1.0f, 1.0f, linearProgress);
                Vector2 interpParams = new Vector2(1.0f, 0.0f);
                if (currentPose.Interpolation != null)
                {
                    interpParams = currentPose.Interpolation;
                }
                float t = WeightSigmoid.GetSigmoidValue(interpParams, sigmoidInput);

                //前と後のポーズデータと補間係数を使って、ポーズの作成と適用
                ApplyBlendPose(currentPose, nextPose, t);

                await UniTask.NextFrame(token);
            }

            currentIndex = nextIndex;
        }
    }


    /// <summary>
    /// UniTaskで動いている現在のアニメーションを停止
    /// </summary>
    private void StopCurrentAnimation()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }


    /// <summary>
    /// ポーズの作成と適用
    /// from・・・前のポーズデータ
    /// to・・・後のポーズデータ
    /// </summary>
    private void ApplyBlendPose(PoseData from, PoseData to, float t)
    {
        // nullチェック
        if (poseHandler == null || skinnedMeshRenderer == null)
        {
            Debug.LogError("JsonAnimation : poseHandlerかskinnedMeshRendererが参照されていません");
            return;
        }
        //ShapeKeyのListの長さチェック
        if ((mesh.blendShapeCount != from.Facial.Count) || (mesh.blendShapeCount != to.Facial.Count))
        {
            Debug.LogError("JsonAnimation : Facialの個数が一致していません");
            return;
        }

        try
        {
            // bodyPositionのブレンド値を算出
            blendPosition = Vector3.Lerp(from.Position, to.Position, t);

            // bodyRotationのブレンド値を算出
            blendQuaternion = Quaternion.Slerp(from.Rotation, to.Rotation, t);

            // Muscle値のブレンド値を算出
            for(int i = 0; i < blendPosture.Length; i++)
            {
                blendPosture[i] = Mathf.Lerp(from.Posture[i], to.Posture[i], t);
            }

            // HumanPoseに入力
            humanPose.bodyPosition = blendPosition;
            humanPose.bodyRotation = blendQuaternion;
            humanPose.muscles = blendPosture;

            // ポーズを反映
            poseHandler.SetHumanPose(ref humanPose);

            //Shapekeyの反映 
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                var blendValue = Mathf.Lerp(from.Facial[i], to.Facial[i], t);
                skinnedMeshRenderer.SetBlendShapeWeight(i, blendValue);
            }
        }
        catch (ArithmeticException e)
        {
            Debug.LogError("JsonAnimation : 例外が発生しました : " + e);
            return;
        }
    }
}
