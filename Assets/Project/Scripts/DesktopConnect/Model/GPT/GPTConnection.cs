using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity2GPT
{
    public class GPTConnection
    {
        private string apiKey;
        private string systemPrompt;
        private string model;
        private string endPoint;
        private Dictionary<string, string> headers;

        private List<JsonGPTClass.GPTMessageModel> messageList;

        public GPTConnection()
        {

        }


        public void Initialize(string apiKey, string systemPrompt, string model, string endPoint)
        {
            this.apiKey = apiKey;
            this.systemPrompt = systemPrompt;
            this.model = model;
            this.endPoint = endPoint;
            headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + this.apiKey},
                {"Content-type", "application/json"},
            };

            messageList = new List<JsonGPTClass.GPTMessageModel>();
            messageList.Add(new JsonGPTClass.GPTMessageModel()
            {
                role = "system",
                content = systemPrompt,
            });
        }


        public async UniTask<string> SendData(string data)
        {
            var result = await GPTRequset(data);
            var decodeResult = JsonUtility.FromJson<JsonEmotionClass>(result.choices[0].message.content);
            return decodeResult.emotion + ":" + decodeResult.strength.ToString() + ":" + decodeResult.message;
        }

        public async UniTask<JsonGPTClass.GPTResponseModel> GPTRequset(string userMessage)
        {
            messageList.Add(new JsonGPTClass.GPTMessageModel
            {
                role = "user",
                content = userMessage,
            });

            var options = new JsonGPTClass.GPTCompletionRequestModel()
            {
                model = this.model,
                messages = messageList
            };
            var jsonOptions = JsonUtility.ToJson(options);

            using var request = new UnityWebRequest(endPoint, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach(var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<JsonGPTClass.GPTResponseModel>(responseString);
                messageList.Add(responseObject.choices[0].message);
                return responseObject;
            }
        }

    }
}
