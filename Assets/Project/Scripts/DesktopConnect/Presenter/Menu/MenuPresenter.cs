using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MenuPresenter
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


    public MenuPresenter()
    {
        MenuModel.externalInteration.activateObservable.Subscribe(MenuActivate);
        MenuModel.externalInteration.candidatePosObservable.Subscribe(CandidateMenuPosition);
        MenuModel.externalInteration.positionObservable.Subscribe(_ => InitializeMenuPosition());
    }


    /// <summary>
    /// マウスカーソルが画面外に出ていないか
    /// </summary>
    public bool CheckMouseCarsorPosition(Vector3 position)
    {
        return DisplayModel.displayChecker.CheckDisplayIn(position);
    }


    /// <summary>
    /// メニュー表示が呼ばれたときに、候補点の座標と合わせて通知
    /// </summary>
    private void MenuActivate(bool activate)
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
    private void InitializeMenuPosition()
    {
        _positionSubject.OnNext(Unit.Default);
    }

    public bool CheckInDisplay(Vector3 position, float right, float left, float top, float bottom)
    {
        return CharacterModel.objectPoint.CheckInsideDisplay(position, right, left, top, bottom);
    }


    public void Character2Idle()
    {
        CharacterModel.animationSender.Idle();
    }
}
