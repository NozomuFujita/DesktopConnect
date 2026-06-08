using System;
using UniRx;
using UnityEngine;

public class NotificationPosition
{
    // メニューの座標を更新するための通知
    private Subject<Vector3> _positionSubject = new Subject<Vector3>();
    public IObservable<Vector3> positionObservable => _positionSubject;


    /// <summary>
    /// メニューの座標の初期化
    /// </summary>
    public void InitializeMenuPosition()
    {
        //デフォルトポジション
        var x = -DisplayModel.displayWidthFloat * 3.0f / 4.0f;
        var y = DisplayModel.displayHeightFloat / 5.0f;
        _positionSubject.OnNext(new Vector3(x, y, 0.0f));
    }
}
