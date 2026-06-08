using System;
using System.Collections.Generic;
using UniRx;
using WebSocketSharp;
using WebSocketSharp.Net;

public class ConnectManager
{
    private WebSocket webSocket;

    private Subject<string> _dataSubject = new Subject<string>();
    public IObservable<string> dataObservable => _dataSubject;

    public ConnectManager()
    {

    }

    public void SendData(string data)
    {
        if (webSocket == null)
        {
            UnityEngine.Debug.LogError("WebSoclet is null");
            return;
        }
        webSocket.Send(data);
    }


    public void ReceivedData(string data)
    {
        string[] splitData = data.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (splitData[0] == "<SystemMessage>")
        {
            return;
        }

        NotificationModel.notificationData.DataEnqueue(CharacterModel.jsonReader.DecodeJson(data));
    }


    public void ReceivedClose()
    {

    }


    public void CloseWebSocket()
    {
        if(webSocket == null)
        {
            return;
        }
        webSocket.Close();
        webSocket = null;
    }


    public void ConnectWebSocket(string url)
    {
        webSocket = new WebSocket(url);
        webSocket.Connect();

        webSocket.OnMessage += (sender, e) => ReceivedData(e.Data);
        webSocket.OnClose += (sender, e) => ReceivedClose();
    }
}
