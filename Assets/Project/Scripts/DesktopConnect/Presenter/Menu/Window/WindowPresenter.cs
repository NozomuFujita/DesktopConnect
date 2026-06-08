using DisplaySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WindowPresenter
{
    // 現在開いているウィンドウを送るためののSubject
    private Subject<List<string>> _windowListSubject = new Subject<List<string>>();
    public IObservable<List<string>> windowListObservable => _windowListSubject;

    public WindowPresenter()
    {

    }

    public void GetOpeningWindow()
    {
        var windowList = new List<string>();

        var windows = DisplayModel.windowsAPI.GetEnumWindows();
        foreach (var window in windows)
        {
            windowList.Add(window.title);
        }

        _windowListSubject.OnNext(windowList);
    }
}
