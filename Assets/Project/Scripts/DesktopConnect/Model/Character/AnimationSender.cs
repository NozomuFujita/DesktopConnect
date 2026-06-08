using JsonAniamtion.Model;
using System;
using UniRx;

public class AnimationSender
{
    private Subject<JsonClipDataM> _outAnimationSubject = new Subject<JsonClipDataM>();
    public IObservable<JsonClipDataM> outAnimationObservable => _outAnimationSubject;
    private Subject<Unit> _idleSubject = new Subject<Unit>();
    public IObservable<Unit> idleObservable => _idleSubject;

    public void SendAnimation2Character(JsonClipDataM data)
    {
        _outAnimationSubject.OnNext(data);
    }

    public void Idle()
    {
        _idleSubject.OnNext(Unit.Default);
    }
}
