using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuView : MonoBehaviour
{
    //メニューのサイズ
    public float menuSize { get; private set; }
    //メニューのポジション
    public Vector2 menuPosition { get; private set; }
    //Xマークのボタン
    [SerializeField]
    private Button closeButton;
    //メニューで選べるボタン
    [SerializeField]
    private List<Button> menuButton;
    //設定項目が入っているオブジェクト
    [SerializeField]
    private List<GameObject> settingObject;
    // メニューのRectを取得するためのemptyオブジェクト
    [SerializeField]
    private GameObject rightRect;
    [SerializeField]
    private GameObject leftRect;
    [SerializeField]
    private GameObject upRect;
    [SerializeField]
    private GameObject downRect;

    private MenuPresenter menuPresenter;

    void Awake()
    {
        menuSize = 1.0f;
        menuPosition = new Vector3(0.0f, 0.0f, 0.0f);

        for(int i = 1; i < settingObject.Count; i++)
        {
            settingObject[i].SetActive(false);
        }

        menuPresenter = new MenuPresenter();
        menuPresenter.activateObservable.Subscribe(Activation).AddTo(this);
        menuPresenter.candidatePosObservable.Subscribe(MenuDisplayPosition).AddTo(this);
        menuPresenter.positionObservable.Subscribe(_ => MenuPosition(new Vector3(0.0f, 0.0f, -10.0f))).AddTo(this);
    }

    void Start()
    {
        closeButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.gameObject.SetActive(false);
                menuPresenter.Character2Idle();
            })
            .AddTo(this);

        // 左クリックでのドラッグ操作による移動
        this.OnMouseDownAsObservable()
            .SelectMany(_ => this.UpdateAsObservable()
                .Select(_ => Input.mousePosition)
                .Select(mousePosition => new Vector3(mousePosition.x, mousePosition.y, 0))
                .Select(mousePosition => Camera.main.ScreenToWorldPoint(mousePosition))
                .Where(mousePosition => menuPresenter.CheckMouseCarsorPosition(mousePosition) == true)
                .Buffer(2, 1)
                .Select(mousePosition => mousePosition.Last() - mousePosition.First())
                .TakeUntil(this.OnMouseUpAsObservable())
            )
            .Subscribe(move =>
            {
                MenuPosition(this.transform.position + move);

            }).AddTo(this);

        var index = 0;
        foreach (var button in menuButton)
        {
            var currentIndex = index;
            button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SelectedMenuIcon(currentIndex);
            })
            .AddTo(this);

            index++;
        }

        this.gameObject.SetActive(false);
    }

    private void MenuPosition(Vector3 position)
    {
        position.z = -10.0f;
        this.transform.position = position;
    }

    /// <summary>
    /// 選択されたメニューアイコンを強調するための処理
    /// </summary>
    private void SelectedMenuIcon(int index)
    {
        for(int i = 0; i < menuButton.Count; i++)
        {
            if(i == index)
            {
                ColorBlock cb = menuButton[i].colors;
                cb.normalColor = new Color32(235, 209, 207, 255);
                menuButton[i].colors = cb;

                settingObject[i].SetActive(true);
            }
            else
            {
                ColorBlock cb = menuButton[i].colors;
                cb.normalColor = new Color32(239, 238, 235, 255);
                menuButton[i].colors = cb;

                settingObject[i].SetActive(false);
            }
        }
    }


    /// <summary>
    /// メニュー表示の呼び出し時の処理
    /// クリックされるたび位置は計算しなおす
    /// </summary>
    private void MenuDisplayPosition(List<Vector2> points)
    {
        if(points == null || points.Count == 0)
        {
            return;
        }

        var finalPosition = new Vector3(0.0f, 0.0f, 0.0f);

        foreach(var point in points)
        {
            var deltaPosition = new Vector3(point.x - this.transform.position.x, point.y - this.transform.position.y, 0.0f);
            bool isInDisplay = menuPresenter.CheckInDisplay(deltaPosition, rightRect.transform.position.x, leftRect.transform.position.x, upRect.transform.position.y, downRect.transform.position.y);
            if(isInDisplay)
            {
                finalPosition = new Vector3(point.x, point.y, 0.0f);
                break;
            }
        }

        MenuPosition(finalPosition);
    }


    /// <summary>
    /// 表示するか
    /// </summary>
    private void Activation(bool activate)
    {
        this.gameObject.SetActive(activate);
    }


    /// <summary>
    /// Menuが画面内に収まるか判定
    /// </summary>
    //private bool CheckInsideDisplay(Vector3 position)
    //{
    //    // ディスプレイの大きさが必要！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
    //    // 仮置き
    //    var displayX = 8.8f;
    //    var displayY = 10.0f;

    //    // 各辺の座標を計算して変数にまとめる
    //    float rightEdge = position.x + rightRect.transform.position.x;
    //    float leftEdge = position.x + leftRect.transform.position.x;
    //    float topEdge = position.y + upRect.transform.position.y;
    //    float bottomEdge = position.y + downRect.transform.position.y;

    //    return rightEdge <= displayX && leftEdge >= -displayX && topEdge <= displayY && bottomEdge >= 0.0f;
    //}
}
