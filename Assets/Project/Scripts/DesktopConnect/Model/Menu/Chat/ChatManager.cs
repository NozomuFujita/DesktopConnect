using System;
using UniRx;

public class ChatManager
{
    private Subject<string> _otherMessageSubject = new Subject<string>();
    public IObservable<string> otherMessageObservable => _otherMessageSubject;

    public void RegistOtherMessage(string message)
    {
        _otherMessageSubject.OnNext(message);
    }
}
