using Cysharp.Threading.Tasks;
using JsonAniamtion.Model;
using JsonAniamtion.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CharacterPresenter
{
    // アニメーションを送るためのSubject
    private Subject<JsonClipDataP> _animationSubject = new Subject<JsonClipDataP>();
    public IObservable<JsonClipDataP> animationObservable => _animationSubject;

    private Subject<Vector3> _positionSubject = new Subject<Vector3>();
    public IObservable<Vector3> positionObservable => _positionSubject;

    private Finger2Array finger2Array;
    private Facial2Array facial2Array;

    public CharacterPresenter()
    {
        finger2Array = new Finger2Array();
        facial2Array = new Facial2Array();
        CharacterModel.animationSender.outAnimationObservable.ObserveOnMainThread().Subscribe(DecodeAndConvension);
        CharacterModel.animationSender.idleObservable.ObserveOnMainThread().Subscribe(_ => Idle());
    }


    /// <summary>
    /// 内部での再生したいアニメーションの名前を受け取り、探索、変換
    /// </summary>
    public async void SearchAndConvension(string animationName)
    {
        JsonClipDataM jsonClipDataM = CharacterModel.jsonReader.LoadJsonFile(animationName);
        if(jsonClipDataM != null)
        {
            // 統合されたJSONデータから、アニメーションデータとファイシャルデータの抜き出し
            // WhenAll()で両方の処理が終わるまで待機 => 同時に抜き出すことにしたのでWhenAllにする必要ない　変更する
            var animation = await UniTask.WhenAll(ChanegeJsonAnimationData(jsonClipDataM));
            _animationSubject.OnNext(animation[0]);
        }
    }


    /// <summary>
    /// 外部からの再生したいアニメーションを受け取り、探索、変換
    /// </summary>
    public async void DecodeAndConvension(JsonClipDataM animationData)
    {
        // 統合されたJSONデータから、アニメーションデータとファイシャルデータの抜き出し
        // 同上
        var animation = await UniTask.WhenAll(ChanegeJsonAnimationData(animationData));
        _animationSubject.OnNext(animation[0]);

        //Fear用
        if(animationData.Clip == "Fear")
        {
            Fear();
        }
    }


    /// <summary>
    /// 統合されたJSONデータから、アニメーションデータの抜き出し
    /// </summary>
    private async UniTask<JsonClipDataP> ChanegeJsonAnimationData(JsonClipDataM clipDateM)
    {
        // View用のデータの用意
        JsonClipDataP clipDataP = new JsonClipDataP();

        clipDataP.FPS = clipDateM.FPS;
        clipDataP.TotalFrame = clipDateM.TotalFrame;
        clipDataP.Poses = new List<JsonAniamtion.Presenter.PoseData>();
        foreach (var pose in clipDateM.Poses)
        {
            var newPose = new JsonAniamtion.Presenter.PoseData()
            {
                Index = pose.Index,
                Proportion = pose.Proportion,
                Interpolation = new Vector2(pose.Interpolation.a, pose.Interpolation.b),
                Position = new Vector3(pose.Position.x, pose.Position.y, pose.Position.z),
                Rotation = new Quaternion(pose.Rotation.x, pose.Rotation.y, pose.Rotation.z, pose.Rotation.w)
            };

            // Unity Humanoidに基づく94個のfloat配列
            newPose.Posture = new float[HumanTrait.MuscleCount];

            // --- Body (0-8) ---
            newPose.Posture[0] = pose.Posture.Body.Spine.FrontBack;
            newPose.Posture[1] = pose.Posture.Body.Spine.LeftRight;
            newPose.Posture[2] = pose.Posture.Body.Spine.Twist_LeftRight;
            newPose.Posture[3] = pose.Posture.Body.Chest.FrontBack;
            newPose.Posture[4] = pose.Posture.Body.Chest.LeftRight;
            newPose.Posture[5] = pose.Posture.Body.Chest.Twist_LeftRight;
            newPose.Posture[6] = pose.Posture.Body.UpperChest.FrontBack;
            newPose.Posture[7] = pose.Posture.Body.UpperChest.LeftRight;
            newPose.Posture[8] = pose.Posture.Body.UpperChest.Twist_LeftRight;

            // --- Head & Neck (9-14) ---
            newPose.Posture[9] = pose.Posture.Head.Neck.NodDownUp;
            newPose.Posture[10] = pose.Posture.Head.Neck.TiltLeftRight;
            newPose.Posture[11] = pose.Posture.Head.Neck.TurnLeftRight;
            newPose.Posture[12] = pose.Posture.Head.Head.NodDownUp;
            newPose.Posture[13] = pose.Posture.Head.Head.TiltLeftRight;
            newPose.Posture[14] = pose.Posture.Head.Head.TurnLeftRight;

            // --- Face (15-20) ---
            newPose.Posture[15] = pose.Posture.Face.LeftEye.DownUp;
            newPose.Posture[16] = pose.Posture.Face.LeftEye.InOut;
            newPose.Posture[17] = pose.Posture.Face.RightEye.DownUp;
            newPose.Posture[18] = pose.Posture.Face.RightEye.InOut;
            newPose.Posture[19] = pose.Posture.Face.Jaw.Close;
            newPose.Posture[20] = pose.Posture.Face.Jaw.LeftRight;

            // --- Left Leg (21-28) ---
            newPose.Posture[21] = pose.Posture.LeftLeg.UpperLeg.FrontBack;
            newPose.Posture[22] = pose.Posture.LeftLeg.UpperLeg.InOut;
            newPose.Posture[23] = pose.Posture.LeftLeg.UpperLeg.TwistInOut;
            newPose.Posture[24] = pose.Posture.LeftLeg.LowerLeg.Stretch;
            newPose.Posture[25] = pose.Posture.LeftLeg.LowerLeg.TwistInOut;
            newPose.Posture[26] = pose.Posture.LeftLeg.Foot.UpDown;
            newPose.Posture[27] = pose.Posture.LeftLeg.Foot.TwistInOut;
            newPose.Posture[28] = pose.Posture.LeftLeg.Toe.UpDown;

            // --- Right Leg (29-36) ---
            newPose.Posture[29] = pose.Posture.RightLeg.UpperLeg.FrontBack;
            newPose.Posture[30] = pose.Posture.RightLeg.UpperLeg.InOut;
            newPose.Posture[31] = pose.Posture.RightLeg.UpperLeg.TwistInOut;
            newPose.Posture[32] = pose.Posture.RightLeg.LowerLeg.Stretch;
            newPose.Posture[33] = pose.Posture.RightLeg.LowerLeg.TwistInOut;
            newPose.Posture[34] = pose.Posture.RightLeg.Foot.UpDown;
            newPose.Posture[35] = pose.Posture.RightLeg.Foot.TwistInOut;
            newPose.Posture[36] = pose.Posture.RightLeg.Toe.UpDown;

            // --- Left Arm (37-45) ---
            newPose.Posture[37] = pose.Posture.LeftArm.Shoulder.DownUp;
            newPose.Posture[38] = pose.Posture.LeftArm.Shoulder.FrontBack;
            newPose.Posture[39] = pose.Posture.LeftArm.Arm.DownUp;
            newPose.Posture[40] = pose.Posture.LeftArm.Arm.FrontBack;
            newPose.Posture[41] = pose.Posture.LeftArm.Arm.TwistInOut;
            newPose.Posture[42] = pose.Posture.LeftArm.Forearm.Stretch;
            newPose.Posture[43] = pose.Posture.LeftArm.Forearm.TwistInOut;
            newPose.Posture[44] = pose.Posture.LeftArm.Hand.DownUp;
            newPose.Posture[45] = pose.Posture.LeftArm.Hand.InOut;

            // --- Right Arm (46-54) ---
            newPose.Posture[46] = pose.Posture.RightArm.Shoulder.DownUp;
            newPose.Posture[47] = pose.Posture.RightArm.Shoulder.FrontBack;
            newPose.Posture[48] = pose.Posture.RightArm.Arm.DownUp;
            newPose.Posture[49] = pose.Posture.RightArm.Arm.FrontBack;
            newPose.Posture[50] = pose.Posture.RightArm.Arm.TwistInOut;
            newPose.Posture[51] = pose.Posture.RightArm.Forearm.Stretch;
            newPose.Posture[52] = pose.Posture.RightArm.Forearm.TwistInOut;
            newPose.Posture[53] = pose.Posture.RightArm.Hand.DownUp;
            newPose.Posture[54] = pose.Posture.RightArm.Hand.InOut;

            // --- Left Finger (55-74) ---
            if(finger2Array.fingerData.ContainsKey(pose.Posture.LeftArm.Finger))
            {
                var leftFinger = finger2Array.fingerData[pose.Posture.LeftArm.Finger];
                for (int i = 0; i < leftFinger.Count; i++)
                {
                    newPose.Posture[i + 55] = leftFinger[i];
                }
            }

            // --- Right Finger (75-94) ---
            if (finger2Array.fingerData.ContainsKey(pose.Posture.RightArm.Finger))
            {
                var rightFinger = finger2Array.fingerData[pose.Posture.RightArm.Finger];
                for (int i = 0; i < rightFinger.Count; i++)
                {
                    newPose.Posture[i + 75] = rightFinger[i];
                }
            }

            if (facial2Array.facialData.ContainsKey(pose.Facial))
            {
                var facialData = facial2Array.facialData[pose.Facial];
                newPose.Facial = facialData;
            }

            clipDataP.Poses.Add(newPose);
        }

        await UniTask.CompletedTask;
        return clipDataP;
    }


    /// <summary>
    /// マウスカーソルが画面外に出ていないか
    /// </summary>
    public bool CheckMouseCarsorPosition(Vector3 position)
    {
        return DisplayModel.displayChecker.CheckDisplayIn(position);
    }

    private void Idle()
    {
        SearchAndConvension("Idle.json");
    }


    public void StateReset()
    {
        MenuModel.externalInteration.MenuActivate(false);
        BubbleModel.bubbleManager.HiddenMessage();
        Idle();
    }



    /// <summary>
    /// 右クリック(メニューの表示)を通知
    /// 画面外に配置されないように、候補点も一緒に送る
    /// </summary>
    public void OpenMenu(Vector3 characterPos)
    {
        CharacterModel.objectPoint.CalcCoeffcient(6.0f, 6.0f);

        var points = new List<Vector2>();
        foreach(var point in CharacterModel.objectPoint.candidatePoints)
        {
            points.Add(new Vector2
            {
                x = point.x + characterPos.x,
                y = point.y + characterPos.y,
            });
        }
        MenuModel.externalInteration.CandidateMenuPosition(points);

        //吹き出しも消す
        BubbleModel.bubbleManager.HiddenMessage();

        //アニメーション
        SearchAndConvension("Thinking.json");
    }


    public void Fear()
    {
        var position = new Vector3(-DisplayModel.displayWidthFloat, 0.0f, 0.0f);
        _positionSubject.OnNext(position);
    }
}
