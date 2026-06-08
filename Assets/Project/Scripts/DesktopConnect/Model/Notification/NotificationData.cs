using System;
using System.Collections.Generic;
using UniRx;
using JsonAniamtion.Model;

public class NotificationData
{
    //データ保存用のキュー
    private Queue<JsonClipDataM> queueData;

    /// <summary>
    /// キューの更新を通知
    /// 0・・・Enqueue
    /// 1・・・Dequeue, Clear
    /// </summary>
    private Subject<int> _notificationSubject = new Subject<int>();
    public IObservable<int> notificationObservable => _notificationSubject;
    //通知アイコンの大きさの変更を通知
    private Subject<float> _uiSizeSubject = new Subject<float>();
    public IObservable<float> uiSizeObservable => _uiSizeSubject;

    public NotificationData()
    {
        queueData = new Queue<JsonClipDataM>();
    }

    /// <summary>
    /// データの挿入
    /// </summary>
    public void DataEnqueue(JsonClipDataM data)
    {
        queueData.Enqueue(data);
        _notificationSubject.OnNext(0);
    }

    /// <summary>
    /// データの取り出し
    /// </summary>
    public JsonClipDataM DataDequeue()
    {
        if(DataCount() < 1)
        {
            return null;
        }
        var data = queueData.Dequeue();
        _notificationSubject.OnNext(1);
        return data;
    }

    /// <summary>
    /// データのクリア
    /// </summary>
    public void DataClear()
    {
        queueData.Clear();
        _notificationSubject.OnNext(1);
    }

    /// <summary>
    /// データ数のカウント
    /// </summary>
    public int DataCount()
    {
        return queueData.Count;
    }

    /// <summary>
    /// UI(通知アイコン)の大きさを変更する
    /// </summary>
    public void UISizeChenge(float size)
    {
        _uiSizeSubject.OnNext(size);
    }
}
