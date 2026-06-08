using JsonAniamtion.Presenter;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterView : MonoBehaviour
{
    // 起動したら最初に流れるデフォルトアニメーション
    [SerializeField]
    private string defaultAnimation;

    // シェイプキーを変更するため
    [SerializeField]
    private GameObject face;

    // キャラクターの重心
    [SerializeField]
    private GameObject centerObject;

    private JsonAnimation jsonAnimation;
    private CharacterPresenter characterPresenter;


    void Awake()
    {
        // JsonAnimation用
        var animator = this.GetComponent<Animator>();
        var skinnedMeshRenderer = face.GetComponent<SkinnedMeshRenderer>();
        jsonAnimation = new JsonAnimation(animator, skinnedMeshRenderer);

        characterPresenter = new CharacterPresenter();
        characterPresenter.animationObservable.Subscribe(Pass2JsonAnimation).AddTo(this);
        characterPresenter.positionObservable.Subscribe(CharacterPosition).AddTo(this);
    }
    

    void Start()
    {
        characterPresenter.SearchAndConvension(defaultAnimation);
        MouseControl();
    }


    /// <summary>
    /// マウスでの操作処理
    /// </summary>
    private void MouseControl()
    {
        // 右クリック時の処理
        this.OnMouseOverAsObservable()
            .Where(_ => Input.GetMouseButtonDown(1))
            .SelectMany(_ =>
                this.OnMouseOverAsObservable()
                .Where(__ => Input.GetMouseButtonUp(1))
                .First()
            )
            .Subscribe(_ =>
            {
                characterPresenter.OpenMenu(centerObject.transform.position);
            }).AddTo(this);

        // 左クリックでのドラッグ操作による移動
        this.OnMouseDownAsObservable()
            .SelectMany(_ => this.UpdateAsObservable()
                .Select(_ => Input.mousePosition)
                .Select(mousePosition => new Vector3(mousePosition.x, mousePosition.y, 10))
                .Select(mousePosition => Camera.main.ScreenToWorldPoint(mousePosition))
                .Where(mousePosition => characterPresenter.CheckMouseCarsorPosition(mousePosition) == true)
                .Buffer(2, 1)
                .Select(mousePosition => mousePosition.Last() - mousePosition.First())
                .TakeUntil(this.OnMouseUpAsObservable())
            )
            .Subscribe(move =>
            {
                CharacterPosition(this.transform.position + move);

            }).AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(2))
            .Subscribe(_ =>
            {
                characterPresenter.StateReset();
            })
            .AddTo(this);
    }


    private void CharacterPosition(Vector3 position)
    {
        this.transform.position = position;
    }



    /// <summary>
    /// JsonAnimationにアニメーションデータを渡す
    /// </summary>
    private void Pass2JsonAnimation(JsonClipDataP jsonClipData)
    {
        jsonAnimation.PlayAnimation(jsonClipData);
    }
}
