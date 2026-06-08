using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class NetworkView : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputfield;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private Button disconnectButton;

    private bool isActiveConnect;
    private bool isActiveDisconnect;

    private NetworkPresenter networkPresenter;

    void Awake()
    {
        networkPresenter = new NetworkPresenter();
        connectButton.interactable = true;
        disconnectButton.interactable = false;
    }

    void Start()
    {
        connectButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                networkPresenter.ConnectWebSocket(inputfield.text);
                connectButton.interactable = false;
                disconnectButton.interactable = true;
            })
            .AddTo(this);

        disconnectButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                networkPresenter.CloseWebSocket();
                connectButton.interactable = true;
                disconnectButton.interactable = false;
            })
            .AddTo(this);
    }

    
}
