using System;
using UniRx;
using UnityEngine;

public class ChatPresenter
{
    private Subject<string> _myMessageSubject = new Subject<string>();
    public IObservable<string> myMessageObservable => _myMessageSubject;
    private Subject<string> _otherMessageSubject = new Subject<string>();
    public IObservable<string> otherMessageObservable => _otherMessageSubject;


    public ChatPresenter()
    {
        ChatModel.chatManager.otherMessageObservable.Subscribe(RegistOtherMessage);
    }


    /// <summary>
    /// メッセージの送信
    /// </summary>
    public async void SendMessage(string message)
    {
        MenuModel.externalInteration.MenuActivate(false);
        CharacterModel.animationSender.Idle();
        RegistMyMessage(message);
        var data = await GPTModel.gptConnection.SendData(message);
        NetworkModel.connectManager.SendData(data);
    }


    private void RegistMyMessage(string message)
    {
        _myMessageSubject.OnNext(message);
    }

    public void RegistOtherMessage(string message)
    {
        _otherMessageSubject.OnNext(message);
    }
}
