using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BubblePresenter
{
    private Subject<bool> _activeSubject = new Subject<bool>();
    public IObservable<bool> activeObservable => _activeSubject;
    private Subject<string> _messageSubject = new Subject<string>();
    public IObservable<string> messageObservable => _messageSubject;
    private Subject<Vector3> _positionSubject = new Subject<Vector3>();
    public IObservable<Vector3> positionObservable => _positionSubject;

    public BubblePresenter()
    {
        BubbleModel.bubbleManager.displayObservable.Subscribe(DisplayMessage);
        BubbleModel.bubbleManager.hiddenObservable.Subscribe(_ => HiddenMessage());
    }

    public void DisplayMessage(string message)
    {
        _activeSubject.OnNext(true);
        _messageSubject.OnNext(message);
    }

    public void HiddenMessage()
    {
        _activeSubject.OnNext(false);
    }

    public void BubblePosition(Vector3 characterPos, Vector3 bubblePos , float width, float height)
    {
        float tW = Mathf.InverseLerp(0.5f, 5.0f, width);
        var radiusW =  Mathf.Lerp(3.5f, 6.0f, tW);
        float tH = Mathf.InverseLerp(0.25f, 5.0f, height);
        var radiusH = Mathf.Lerp(2.5f, 5.0f, tH);
        CharacterModel.objectPoint.CalcCoeffcient(radiusW, radiusH);

        var points = new List<Vector2>();
        foreach (var point in CharacterModel.objectPoint.candidatePoints)
        {
            points.Add(new Vector2
            {
                x = point.x + characterPos.x,
                y = point.y + characterPos.y,
            });
        }

        var finalPosition = new Vector3(0.0f, 0.0f, 0.0f);

        foreach (var point in points)
        {
            // 修正: 現在位置(bubblePos)との差分ではなく、候補点の座標(targetPosition)そのものを生成する
            var targetPosition = new Vector3(point.x, point.y, 0.0f);

            // 修正: targetPosition が画面内に収まるか判定する
            bool isInDisplay = CharacterModel.objectPoint.CheckInsideDisplay(targetPosition, width / 4f, -width / 4f, height / 4f, -height / 4f);

            if (isInDisplay)
            {
                finalPosition = targetPosition;
                break;
            }
        }
        _positionSubject.OnNext(finalPosition);
    }
}
