using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class WindowView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text displayWindowList;
    [SerializeField]
    private Button displayButton;

    private WindowPresenter windowPresenter;

    void Awake()
    {
        windowPresenter = new WindowPresenter();
        windowPresenter.windowListObservable.Subscribe(DisplayOpeningWindow).AddTo(this);   
    }

    void Start()
    {
        displayButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                // UniRx使わなくても普通に渡せばいいのでは...?
                windowPresenter.GetOpeningWindow();
            })
            .AddTo(this);
    }

    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.A))
    //    {
    //        windowPresenter.GetOpeningWindow();
    //    }
    //}

    private void DisplayOpeningWindow(List<string> windowList)
    {
        var sb = new StringBuilder();

        for(int i = 0; i < windowList.Count; i++)
        {
            sb.AppendLine(i + ": " +  windowList[i]);
        }

        displayWindowList.text = sb.ToString();
    }
}
