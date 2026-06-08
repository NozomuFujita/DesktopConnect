using System;
using UniRx;
using UnityEngine;

public class NotificationPresenter
{
    //UIアニメーションの通知
    private Subject<Unit> _uiAnimationStateSubject = new Subject<Unit>();
    public IObservable<Unit> uiAnimationStateObservable => _uiAnimationStateSubject;
    //UIの表示部分を変更するための通知
    private Subject<int> _uiDisplayStateSubject = new Subject<int>();
    public IObservable<int> uiDisplayStateObservable => _uiDisplayStateSubject;
    //UIのサイズを変更するための通知
    private Subject<float> _uiSizeStateSubject = new Subject<float>();
    public IObservable<float> uiSizeStateObservable => _uiSizeStateSubject;
    // メニューの座標を更新するための通知
    private Subject<Vector3> _positionSubject = new Subject<Vector3>();
    public IObservable<Vector3> positionObservable => _positionSubject;
    //SEを鳴らすための通知
    private Subject<Unit> _seSubject = new Subject<Unit>();
    public IObservable<Unit> seObservable => _seSubject;


    public NotificationPresenter()
    {
        NotificationModel.notificationData.notificationObservable.ObserveOnMainThread().Subscribe(UpdateUIState);
        NotificationModel.notificationData.uiSizeObservable.Subscribe(UpdateUISize);
        NotificationModel.notificationPosition.positionObservable.Subscribe(UpdateNotificationPosition);
    }


    /// <summary>
    /// デフォルトポジションへのセット
    /// </summary>
    public void SetDefaultPosition()
    {
        NotificationModel.notificationPosition.InitializeMenuPosition();
    }


    /// <summary>
    /// 通知アイコンがクリック時に呼び出し
    /// キューからデータを取り出す
    /// </summary>
    public void RetrieveData()
    {
        //データの抽出
        var data = NotificationModel.notificationData.DataDequeue();
        // アニメーションへ
        CharacterModel.animationSender.SendAnimation2Character(data);
        // 吹き出しへ
        BubbleModel.bubbleManager.DisplayMessage(data.Message);
        //メニュー消す
        MenuModel.externalInteration.MenuActivate(false);
        // メッセージの登録
        ChatModel.chatManager.RegistOtherMessage(data.Message);
    }


    /// <summary>
    /// UIの状態を更新
    /// 0・・・メッセージが来た時
    /// 1・・・メッセージを取り出す時
    /// </summary>
    private void UpdateUIState(int state)
    {
        switch (state)
        {
            case 0:
                _uiAnimationStateSubject.OnNext(Unit.Default);
                _uiDisplayStateSubject.OnNext(NotificationModel.notificationData.DataCount());
                _seSubject.OnNext(Unit.Default);
                break;
            case 1:
                _uiDisplayStateSubject.OnNext(NotificationModel.notificationData.DataCount());
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// マウスカーソルが画面外に出ていないか
    /// </summary>
    public bool CheckMouseCarsorPosition(Vector3 position)
    {
        return DisplayModel.displayChecker.CheckDisplayIn(position);
    }


    /// <summary>
    /// UIのサイズを変更
    /// </summary>
    private void UpdateUISize(float size)
    {
        _uiSizeStateSubject.OnNext(size);
    }


    /// <summary>
    /// 通知アイコンの座標を変更
    /// </summary>
    private void UpdateNotificationPosition(Vector3 position)
    {
        _positionSubject.OnNext(position);
    }
}
