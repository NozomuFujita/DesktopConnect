using System.Net;
using UnityEngine;

public class GPTPresenter
{
    public void SettingGPT(string apiKey, string systemPrompt, string model, string endPoint)
    {
        GPTModel.gptConnection.Initialize(apiKey, systemPrompt, model, endPoint);   
    }
}
