using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private int portNumber = 8080;

    private WebSocketServer webSocketServer;

    void Start()
    {
        DCWebSocketBehavior.clientList.Clear();
        //ポート番号を指定
        webSocketServer = new WebSocketServer(portNumber);
        //クライアントからの通信時の挙動を定義したクラス、「ExWebSocketBehavior」を登録
        webSocketServer.AddWebSocketService<DCWebSocketBehavior>("/");
        //サーバ起動
        webSocketServer.Start();
        Debug.Log("サーバ起動");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("サーバ停止");
        webSocketServer.Stop();
    }

    public class DCWebSocketBehavior : WebSocketBehavior
    {
        //誰が現在接続しているのか管理するリスト。
        public static List<DCWebSocketBehavior> clientList = new List<DCWebSocketBehavior>();
        //接続者に番号を振るための変数。
        private static int globalSeq = 0;
        //自身の番号
        private int seq;

        public static void HandleMessage(string data, int senderSeq)
        {
            string[] splitMessage = data.Split(':', StringSplitOptions.RemoveEmptyEntries);

            string emotion = splitMessage[0];
            float percent = float.Parse(splitMessage[1]);
            string message = "";
            for (int i = 2;  i < splitMessage.Length; i++)
            {
                message += splitMessage[i];
                if (splitMessage.Length - 1 > i)
                {
                    message += ":";
                }
            }
            var jsonAnimation = AnimationChanger.Instance.GetJsonAnimation(emotion, percent, message);

            // 全員（または特定の人）に配る処理
            foreach (var client in clientList)
            {
                //if (client.seq != senderSeq) //相手
                //{
                //    client.Send(JsonUtility.ToJson(jsonAnimation));
                //}

                if (client.seq == senderSeq) //自分のみ
                {
                    client.Send(JsonUtility.ToJson(jsonAnimation));
                }
            }
        }


        //誰かがログインしてきたときに呼ばれるメソッド
        protected override void OnOpen()
        {
            //ログインしてきた人には、番号をつけて、リストに登録。
            globalSeq++;
            this.seq = globalSeq;
            clientList.Add(this);
            Debug.Log("Seq" + this.seq + " Login. (" + this.ID + ")");

            if (clientList.Count > 2)
            {
                foreach (var client in clientList)
                {
                    client.Send("<SystemMessage>:End");
                }
            }

            //接続者にメッセージを送る
            this.Send("<SystemMessage>:" + clientList.Count.ToString());
        }

        //誰かがメッセージを送信してきたときに呼ばれるメソッド
        protected override void OnMessage(MessageEventArgs e)
        {
            HandleMessage(e.Data, this.seq);
        }

        //誰かがログアウトしてきたときに呼ばれるメソッド
        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log("Seq" + this.seq + " Logout. (" + this.ID + ")");

            //ログアウトした人を、リストから削除。
            clientList.Remove(this);

            //接続者全員にメッセージを送る
            foreach (var client in clientList)
            {
                client.Send("<SystemMessage>:End");
            }
        }
    }
}
