using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class BubbleView : MonoBehaviour
{
    // 吹き出しのオブジェクト
    [SerializeField]
    private GameObject bubbleObject;
    // 中に表示するテキスト
    [SerializeField]
    private TMP_Text messageText;
    // 吹き出しのエッジをどれだけ丸くするか
    [SerializeField]
    private float edgeRadius;
    // キャラクターオブジェクト
    [SerializeField]
    private GameObject characterObject;
    // キャラクターオブジェクト(頭)
    [SerializeField]
    private GameObject characterHeadObject;

    // 吹き出しオブジェクトのマテリアル
    private Material bubbleMaterial;
    // プロパティID(X軸方向の長さ)
    private int bubbleMaterialX;
    // プロパティID(Y軸方向の長さ)
    private int bubbleMaterialY;
    // プロパティID(角の半径)
    private int bubbleMaterialR;
    // 吹き出しの向き(float4)
    private int bubbleMaterialDir;
    // 吹き出しの最大幅
    private float maxBubbleWidth = 5.0f;

    private BubblePresenter bubblePresenter;

    void Awake()
    {
        messageText.text = string.Empty;
        bubbleMaterial = bubbleObject.gameObject.GetComponent<Renderer>().material;
        bubbleMaterialX = Shader.PropertyToID("_X");
        bubbleMaterialY = Shader.PropertyToID("_Y");
        bubbleMaterialR = Shader.PropertyToID("_R");
        bubbleMaterialDir = Shader.PropertyToID("_TailDirection");

        bubblePresenter = new BubblePresenter();
        bubblePresenter.activeObservable.Subscribe(Activate).AddTo(this);   
        bubblePresenter.messageObservable.Subscribe(MessageBubbleSet).AddTo(this);
        bubblePresenter.positionObservable.Subscribe(BubblePosition).AddTo(this);
    }

    void Start()
    {
        Activate(false);

        Observable.EveryUpdate()
            .Where(_ => gameObject.activeInHierarchy)
            .Subscribe(_ =>
            {
                bubblePresenter.BubblePosition(characterObject.transform.position, this.transform.position, messageText.preferredWidth + 0.5f, messageText.preferredHeight + 0.25f);
            }).AddTo(this);
    }

    private void BubblePosition(Vector3 position)
    {
        position.z = -10.0f;
        this.transform.position = position;

        // 吹き出しの尾
        var dir = characterHeadObject.transform.position - this.transform.position;
        dir = dir.normalized;
        bubbleMaterial.SetVector("_TailDirection", new Vector4(dir.x, dir.y, 0f, 0f));
    }


    private void Activate(bool activate)
    {
        this.gameObject.SetActive(activate);
    }

    private void MessageBubbleSet(string message)
    {
        messageText.text = message;
        // 1. テキストの理想的な幅を取得し、最大幅（maxBubbleWidth）で上限を設ける
        float targetWidth = Mathf.Min(messageText.preferredWidth, maxBubbleWidth);

        // 2. 一旦、制限した幅をRectTransformに適用する
        messageText.rectTransform.sizeDelta = new Vector2(targetWidth, messageText.rectTransform.sizeDelta.y);

        // 3. 改行が反映された状態の高さ(preferredHeight)を取得し、余白を足して最終的なサイズを決定
        var textBoxSize = new Vector2(targetWidth + 0.5f, messageText.preferredHeight + 0.25f);
        messageText.rectTransform.sizeDelta = textBoxSize;

        textBoxSize /= 2;
        bubbleMaterial.SetFloat("_X", textBoxSize.x);
        bubbleMaterial.SetFloat("_Y", textBoxSize.y);
        bubbleMaterial.SetFloat("_R", edgeRadius);
    }
}
