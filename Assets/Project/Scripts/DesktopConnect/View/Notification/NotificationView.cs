using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class NotificationView : MonoBehaviour
{
    //UIのサイズ
    private float uiSize;
    //UIのポジション
    public Vector2 uiPosition { get; private set; }
    //通知数を表示するためのTMP
    [SerializeField] 
    private TMP_Text displayNumber;
    //TMPのフォントサイズ(通知数が99以下の場合)
    [SerializeField]
    private float fontSizeUnder99 = 4.0f;
    //TMPのフォントサイズ(通知数が99を越える場合)
    [SerializeField]
    private float fontSizeOver99 = 3.0f;
    //クリックをキャンセル扱いにするためのTimeOut時間
    [SerializeField]
    private int timeOutTime = 200;
    //SE用のコンポーネント
    [SerializeField]
    private AudioSource audioSource;
    //SE用のオーディオ
    [SerializeField]
    private AudioClip se;

    private NotificationPresenter notificationPresenter;

    //アニメーションのキャンセルトークン
    private CancellationTokenSource cts;

    void Awake()
    {
        notificationPresenter = new NotificationPresenter();
        notificationPresenter.uiAnimationStateObservable
            .Subscribe(_ =>
            {
                cts?.Cancel();
                cts?.Dispose();

                cts = new CancellationTokenSource();

                UIAnimation(cts.Token).Forget();
            })
            .AddTo(this);
        notificationPresenter.uiDisplayStateObservable.Subscribe(UIDisplay).AddTo(this);
        notificationPresenter.uiSizeStateObservable.Subscribe(SetUISize).AddTo(this);
        notificationPresenter.positionObservable.Subscribe(UIPosition).AddTo(this);
        notificationPresenter.seObservable.Subscribe(_ => PlaySE()).AddTo(this);

        Initialize();
    }

    void Start()
    {
        //左クリックでの通知表示
        //200ms以内にクリックしないとキャンセル扱い
        //このキャンセル扱いをするためにボタンにしていない
        this.OnMouseDownAsObservable()
            .SelectMany(_ => 
                this.OnMouseUpAsObservable()
                    .First()
                    .Timeout(TimeSpan.FromMilliseconds(timeOutTime)))
            .DoOnError(err => UnityEngine.Debug.Log("エラー: TimeOut以内にUpされませんでした"))
            .Retry()
            .Subscribe(_ =>
            {
                notificationPresenter.RetrieveData();
            }).AddTo(this);

        //左クリックでのドラッグ操作による移動
        this.OnMouseDownAsObservable()
            .SelectMany(_ => this.UpdateAsObservable()
                .Select(_ => Input.mousePosition)
                .Select(mousePosition => new Vector3(mousePosition.x, mousePosition.y, 0))
                .Select(mousePosition => Camera.main.ScreenToWorldPoint(mousePosition))
                .Where(mousePosition => notificationPresenter.CheckMouseCarsorPosition(mousePosition) == true)
                .Buffer(2, 1)
                .Select(mousePosition => mousePosition.Last() - mousePosition.First())
                .TakeUntil(this.OnMouseUpAsObservable())
            )
            .Subscribe(move =>
            {
                UIPosition(this.transform.position + move);

            }).AddTo(this);
    }

    /// <summary>
    /// 初期化
    /// サイズ、位置を初期化
    /// 非表示と、通知数を0にする
    /// </summary>
    void Initialize()
    {
        SetUISize(1.0f);
        //UIPosition(new Vector2(0, 0));
        notificationPresenter.SetDefaultPosition();
        displayNumber.text = "0";

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// UIの大きさをセット
    /// </summary>
    public void SetUISize(float size)
    {
        if(this.uiSize != size)
        {
            ChengeUISize(size);
        }
        this.uiSize = size;
    }

    /// <summary>
    /// UIの大きさを変更
    /// </summary>
    private void ChengeUISize(float size)
    {
        this.transform.localScale = Vector3.one * size;
    }

    /// <summary>
    /// UIのポジションを変更
    /// </summary>
    public void UIPosition(Vector3 position)
    {
        position.z = -10.0f;
        this.transform.position = position;
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    private void PlaySE()
    {
        if(audioSource != null && se != null)
        {
            audioSource.PlayOneShot(se);
        }
        else
        {
            UnityEngine.Debug.Log("AudioSource or SE is null");
        }
    }

    /// <summary>
    /// アニメーションの再生
    /// </summary>
    private async UniTask UIAnimation(CancellationToken token)
    {
        var timer = 0.0f;
        while (true)
        {
            if(token.IsCancellationRequested)
            {
                return;
            }

            timer += Time.deltaTime * 4f;
            if (timer > 2.0f)
            {
                break;
            }
            var coef = 1 - Mathf.Exp(-2.0f * timer) * Mathf.Cos(4.0f * timer);
            ChengeUISize(uiSize * coef);
            await UniTask.Yield();
        }
        ChengeUISize(uiSize * 1.0f);
    }

    /// <summary>
    /// 表示の変更
    /// 通知数が0以下のときは非表示
    /// 通知数が99以上になると99+にする
    /// </summary>
    private void UIDisplay(int number)
    {
        var isDisplay = number > 0 ? true : false;
        this.gameObject.SetActive(isDisplay);

        if(number > 99)
        {
            displayNumber.fontSize = fontSizeOver99;
            displayNumber.text = "99+";
        }
        else
        {
            displayNumber.fontSize = fontSizeUnder99;
            displayNumber.text = number.ToString();
        }
    }
}
