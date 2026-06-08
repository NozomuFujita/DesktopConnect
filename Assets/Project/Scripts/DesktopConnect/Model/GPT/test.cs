//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using TMPro;
//using UniRx;
//using Unity2OpenAI;
//using UnityEngine;
//using UnityEngine;
//using UnityEngine.UI;

//public class test : MonoBehaviour
//{
//    [SerializeField, TextArea(1, 5)]
//    private string apiKey;
//    [SerializeField, TextArea(1, 15)]
//    private string systemPrompt;
//    [SerializeField, TextArea(1, 3)]
//    private string model;
//    [SerializeField, TextArea(1, 3)]
//    private string endPoint;

//    [SerializeField]
//    private GameObject face;
//    [SerializeField]
//    private TMP_InputField inputfield;
//    [SerializeField]
//    private Button button;
//    private Mesh mesh;
//    private SkinnedMeshRenderer skinnedMeshRenderer;

//    private GPTConnection gptConnection;


//    void Start()
//    {
//        gptConnection = new GPTConnection(apiKey, systemPrompt, model, endPoint);
//        skinnedMeshRenderer = face.GetComponent<SkinnedMeshRenderer>();
//        mesh = skinnedMeshRenderer.sharedMesh;

//        button.OnClickAsObservable()
//            .Subscribe(_ =>
//            {
//                SendGPT(inputfield.text);
//            })
//            .AddTo(this);
//    }

//    private async UniTaskVoid SendGPT(string message)
//    {
//        if(message == null)
//        {
//            return;
//        }


//        UnityEngine.Debug.Log(message);
//        //awaitしないとUniTask<JsonGPTClass.GPTResponseModel>になり、choices(以下)が認識されない
//        var result = await gptConnection.GPTRequset(message);
//        UnityEngine.Debug.Log(result.choices[0].message.content);
//        var decodeResult = JsonUtility.FromJson<JsonFacialClass>(result.choices[0].message.content);
//        FacialUpdate(decodeResult);
//    }

//    private void FacialUpdate(JsonFacialClass jsonFacialClass)
//    {
//        for(int i = 0; i < mesh.blendShapeCount; i++)
//        {
//            skinnedMeshRenderer.SetBlendShapeWeight(i, 0.0f);
//        }
//        var browIndex = mesh.GetBlendShapeIndex(jsonFacialClass.Facial.Brow);
//        skinnedMeshRenderer.SetBlendShapeWeight(browIndex, 100.0f);
//        var eyeIndex = mesh.GetBlendShapeIndex(jsonFacialClass.Facial.Eye);
//        skinnedMeshRenderer.SetBlendShapeWeight(eyeIndex, 100.0f);
//        var mouthIndex = mesh.GetBlendShapeIndex(jsonFacialClass.Facial.Mouth);
//        skinnedMeshRenderer.SetBlendShapeWeight(mouthIndex, 100.0f);
//    }
//}
