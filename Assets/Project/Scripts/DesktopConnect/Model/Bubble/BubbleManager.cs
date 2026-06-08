using System;
using UniRx;

public class BubbleManager
{
    private Subject<Unit> _hiddenSubject = new Subject<Unit>();
    public IObservable<Unit> hiddenObservable => _hiddenSubject;
    private Subject<string> _displaySubject = new Subject<string>();
    public IObservable<string> displayObservable => _displaySubject;

    public void DisplayMessage(string message)
    {
        _displaySubject.OnNext(message);
    }

    public void HiddenMessage()
    {
        _hiddenSubject.OnNext(Unit.Default);
    }
}
