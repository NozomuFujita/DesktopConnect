using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GPTView : MonoBehaviour
{
    [SerializeField, TextArea(1, 5)]
    private string apiKey;
    [SerializeField, TextArea(1, 15)]
    private string systemPrompt;
    [SerializeField, TextArea(1, 3)]
    private string model;
    [SerializeField, TextArea(1, 3)]
    private string endPoint;

    private GPTPresenter gptPresenter;


    void Awake()
    {
        gptPresenter = new GPTPresenter();
        gptPresenter.SettingGPT(apiKey, systemPrompt, model, endPoint);
    }
}
