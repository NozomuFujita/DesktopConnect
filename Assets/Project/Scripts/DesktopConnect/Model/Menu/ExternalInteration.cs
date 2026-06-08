using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ExternalInteration
{
    // メニューが呼ばれたことを通知
    private Subject<bool> _activateSubject = new Subject<bool>();
    public IObservable<bool> activateObservable => _activateSubject;
    // メニュー位置の候補点を通知
    private Subject<List<Vector2>> _candidatePosSubject = new Subject<List<Vector2>>();
    public IObservable<List<Vector2>> candidatePosObservable => _candidatePosSubject;
    // メニューの座標を初期化するための通知
    private Subject<Unit> _positionSubject = new Subject<Unit>();
    public IObservable<Unit> positionObservable => _positionSubject;


    /// <summary>
    /// メニューを表示するか通知
    /// </summary>
    public void MenuActivate(bool activate)
    {
        _activateSubject.OnNext(activate);
    }

    /// <summary>
    /// メニュー表示が呼ばれたときに、候補点の座標と合わせて通知
    /// </summary>
    public void CandidateMenuPosition(List<Vector2> points)
    {
        MenuActivate(true);
        _candidatePosSubject.OnNext(points);
    }


    /// <summary>
    /// メニューの座標の初期化
    /// </summary>
    public void InitializeMenuPosition()
    {
        _positionSubject.OnNext(Unit.Default);
    }
}
