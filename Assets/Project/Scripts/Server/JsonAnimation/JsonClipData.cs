using System.Collections.Generic;
using UnityEngine;

namespace JsonAniamtion
{
    [System.Serializable]
    public class JsonClipData
    {
        public string Clip;
        public int FPS;
        public string Message;
        public string Bubble;
        public int TotalFrame;
        public DefaultChangeData DefaultChange;
        public List<PoseChangeData> PoseChange;
        public List<PoseData> Poses;
    }

    [System.Serializable]
    public class DefaultChangeData
    {
        public string Target;
        public int Min;
        public int Max;
    }

    [System.Serializable]
    public class PoseChangeData
    {
        public int Index;
        public List<ChangeItem> Basic;
        public List<ChangeItem> Posture;
        public List<FingerChangeItem> Finger;
        public List<FacialChangeItem> Facial;
    }

    [System.Serializable]
    public class ChangeItem
    {
        public string Target;
        public float Min;
        public float Max;
    }

    [System.Serializable]
    public class FingerChangeItem
    {
        public string Target;
        public string Value;
    }

    [System.Serializable]
    public class FacialChangeItem
    {
        public string Value;
    }

    [System.Serializable]
    public class PoseData
    {
        public int Index;
        public float Proportion;
        public InterpolationData Interpolation;

        public PositionData Position;
        public RotationData Rotation;

        public PostureData Posture;
        public string Facial;
    }

    [System.Serializable]
    public class InterpolationData
    {
        public float a;
        public float b;
    }

    [System.Serializable]
    public class PositionData
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class RotationData
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    // ----- Posture 階層 -----

    [System.Serializable]
    public class PostureData
    {
        public BodyGroup Body;
        public HeadGroup Head;
        public FaceGroup Face;
        public LegGroup LeftLeg;
        public LegGroup RightLeg;
        public ArmGroup LeftArm;
        public ArmGroup RightArm;
    }

    // Body
    [System.Serializable]
    public class BodyGroup
    {
        public SpineData Spine;
        public SpineData Chest;
        public SpineData UpperChest;
    }

    [System.Serializable]
    public class SpineData
    {
        public float FrontBack;
        public float LeftRight;
        public float Twist_LeftRight;
    }

    // Head
    [System.Serializable]
    public class HeadGroup
    {
        public HeadDetailData Neck;
        public HeadDetailData Head;
    }

    [System.Serializable]
    public class HeadDetailData
    {
        public float NodDownUp;
        public float TiltLeftRight;
        public float TurnLeftRight;
    }

    // Face
    [System.Serializable]
    public class FaceGroup
    {
        public EyeData LeftEye;
        public EyeData RightEye;
        public JawData Jaw;
    }

    [System.Serializable]
    public class EyeData
    {
        public float DownUp;
        public float InOut;
    }

    [System.Serializable]
    public class JawData
    {
        public float Close;
        public float LeftRight;
    }

    // Legs (Left/Right)
    [System.Serializable]
    public class LegGroup
    {
        public LegUpperData UpperLeg;
        public LegLowerData LowerLeg;
        public FootData Foot;
        public ToeData Toe;
    }

    [System.Serializable]
    public class LegUpperData
    {
        public float FrontBack;
        public float InOut;
        public float TwistInOut;
    }

    [System.Serializable]
    public class LegLowerData
    {
        public float Stretch;
        public float TwistInOut;
    }

    [System.Serializable]
    public class FootData
    {
        public float UpDown;
        public float TwistInOut;
    }

    [System.Serializable]
    public class ToeData
    {
        public float UpDown;
    }

    // Arms (Left/Right)
    [System.Serializable]
    public class ArmGroup
    {
        public ShoulderData Shoulder;
        public ArmData Arm;
        public ForearmData Forearm;
        public HandData Hand;
        public string Finger;
    }

    [System.Serializable]
    public class ShoulderData
    {
        public float DownUp;
        public float FrontBack;
    }

    [System.Serializable]
    public class ArmData
    {
        public float DownUp;
        public float FrontBack;
        public float TwistInOut;
    }

    [System.Serializable]
    public class ForearmData
    {
        public float Stretch;
        public float TwistInOut;
    }

    [System.Serializable]
    public class HandData
    {
        public float DownUp;
        public float InOut;
    }
}