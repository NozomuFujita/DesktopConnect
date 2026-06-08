using System.Collections.Generic;
using UnityEngine;

namespace JsonAniamtion.Presenter
{
    [System.Serializable]
    public class JsonClipDataP
    {
        public int FPS;
        public int TotalFrame;
        public List<PoseData> Poses;
    }

    [System.Serializable]
    public class PoseData
    {
        public int Index;
        public float Proportion;
        public Vector2 Interpolation;

        public Vector3 Position;
        public Quaternion Rotation;

        public float[] Posture;
        public List<float> Facial;
    }
}