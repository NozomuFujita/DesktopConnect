using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DisplayView : MonoBehaviour
{
    // 背景透過で抜くクロマキーカラーの設定(基本は黒(#000000))
    [SerializeField]
    private Color chromakeyColor = Color.black;

    private DisplayPresenter displayPreseneter;

    void Awake()
    {
        // アプリケーションを60FPSに固定
        Application.targetFrameRate = 60;

        displayPreseneter = new DisplayPresenter();
        
#if UNITY_EDITOR
        // Unity Editor上なら、デスクトップマスコット用のセットアップを行わない
#else
        // セットアップを行う
        displayPreseneter.SetUp(chromakeyColor);
#endif

    }

    void Start()
    {
        // Escapeが押されたらアプリケーションを閉じる
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape))
            .Subscribe(_ =>
            {
                UnityEngine.Application.Quit();
            })
            .AddTo(this);

        // F5を押されたら「キャラクター、メニュー、通知アイコン」の位置を(0, 0, 0)にする (緊急用)
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.F5))
            .Subscribe(_ =>
            {
                displayPreseneter.PositionInitialization();
            })
            .AddTo(this);
    }
}
