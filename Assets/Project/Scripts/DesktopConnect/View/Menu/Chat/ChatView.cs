using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class ChatView : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputfield;
    [SerializeField]
    private Button sendButton;

    [SerializeField]
    private GameObject myMessageField1;
    [SerializeField]
    private TMP_Text myMessage1;
    [SerializeField]
    private GameObject myMessageField2;
    [SerializeField]
    private TMP_Text myMessage2;
    [SerializeField]
    private GameObject otherMessageField1;
    [SerializeField]
    private TMP_Text otherMessage1;
    [SerializeField]
    private GameObject otherMessageField2;
    [SerializeField]
    private TMP_Text otherMessage2;

    private ChatPresenter chatPresenter;

    private bool isEndFirstMessage;
    private Vector2Int messageHistoryId;

    void Awake()
    {
        chatPresenter = new ChatPresenter();
        isEndFirstMessage = false;
        messageHistoryId = new Vector2Int(0, 0);

        chatPresenter.myMessageObservable.Subscribe(MyMessage).AddTo(this);
        chatPresenter.otherMessageObservable.Subscribe(OtherMessage).AddTo(this);
    }

    void Start()
    {
        // 送信ボタンが押された時の処理
        sendButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Send().Forget();
            })
            .AddTo(this);

        // テキスト入力時の処理
        Observable.EveryUpdate()
            .Where(_ => EventSystem.current != null && EventSystem.current.currentSelectedGameObject == inputfield.gameObject)
            .Where(_ => Input.GetKeyDown(KeyCode.Return))
            .Subscribe(_ =>
            {
                // Enter Keyを押されたとき
                // Shift + Enterの場合・・・改行
                // Enterのみの場合・・・送信
                if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    NewLine().Forget();
                }
                else
                {
                    Send().Forget();
                }
            })
            .AddTo(this);

        myMessageField1.SetActive(false);
        myMessageField2.SetActive(false);
        otherMessageField1.SetActive(false);
        otherMessageField2.SetActive(false);
    }


    /// <summary>
    /// 送信時
    /// </summary>
    private async UniTaskVoid Send()
    {
        await UniTask.Yield();
        if(inputfield.text == string.Empty)
        {
            return;
        }
        chatPresenter.SendMessage(inputfield.text);
        inputfield.text = string.Empty;
    }


    /// <summary>
    /// 改行時
    /// </summary>
    private async UniTaskVoid NewLine()
    {
        inputfield.ActivateInputField();
        int caretPosition = inputfield.caretPosition;
        await UniTask.Yield();
        inputfield.text = inputfield.text.Insert(caretPosition, "\n");
        inputfield.caretPosition = caretPosition + 1;
    }


    /// <summary>
    /// メッセージ履歴の表示
    /// Vector2Intで誰からのメッセージか管理　0・・・自分　1・・・他人
    /// </summary>
    private void MessageHistory(bool isMe, string message)
    {
        if (!isEndFirstMessage)
        {
            if (isMe)
            {
                myMessage2.text = message;
                myMessageField2.SetActive(true);
                otherMessageField2.SetActive(false);
                messageHistoryId.y = 0;
            }
            else
            {
                otherMessage2.text = message;
                otherMessageField2.SetActive(true);
                myMessageField2.SetActive(false);
                messageHistoryId.y = 1;
            }
            myMessageField1.SetActive(false);
            otherMessageField1.SetActive(false);

            isEndFirstMessage = true;
            return;
        }

        if (messageHistoryId.y == 0)
        {
            myMessage1.text = myMessage2.text;
            myMessageField1.SetActive(true);    
            otherMessageField1.SetActive(false); 
        }
        else 
        {
            otherMessage1.text = otherMessage2.text;
            otherMessageField1.SetActive(true);  
            myMessageField1.SetActive(false);    
        }

        messageHistoryId.x = messageHistoryId.y;

        if (isMe) 
        {
            myMessage2.text = message;
            myMessageField2.SetActive(true);    
            otherMessageField2.SetActive(false);
            messageHistoryId.y = 0;
        }
        else
        {
            otherMessage2.text = message;
            otherMessageField2.SetActive(true);  
            myMessageField2.SetActive(false);  
            messageHistoryId.y = 1;
        }
    }


    /// <summary>
    /// 以下2つのものをまとめる (時間がないので後回し)
    /// </summary>
    private void MyMessage(string message)
    {
        MessageHistory(true, message);
    }

    private void OtherMessage(string message)
    {
        MessageHistory(false, message);
    }
}
