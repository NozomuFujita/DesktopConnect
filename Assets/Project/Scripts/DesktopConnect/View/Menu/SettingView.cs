using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class SettingView : MonoBehaviour
{
    [Space(10)]
    [Header("Notification")]
    private float notificationSize;
    private TMP_Text notificationSizeText;
    [SerializeField]
    private Button notificationSizeMagnification;
    [SerializeField]
    private Button notificationSizeApply;
    private TMP_Text notificationVolumeText;
    private float notificationVolume;
    [SerializeField]
    private Button notificationVolumeMagnification;
    [SerializeField]
    private Button notificationVolumeApply;
    //[SerializeField]
    //private Button notificationResetApply;
    [SerializeField]
    private Button notificationDefaultApply;

    void Awake()
    {
        NotificationAwake();
    }


    void Start()
    {
        NotificationStart();
    }

    #region Notification

    private void NotificationAwake()
    {
        notificationSizeText = notificationSizeMagnification.GetComponentInChildren<TMP_Text>();
        notificationVolumeText = notificationVolumeMagnification.GetComponentInChildren<TMP_Text>();
        notificationSize = PlayerPrefs.GetFloat("NotificationSize", 1.0f);
        notificationSizeText.text = notificationSize.ToString("0.##" + "x");
        notificationVolume = PlayerPrefs.GetFloat("NotificationVolume", 1.0f);
        notificationVolumeText.text = notificationVolume.ToString("0.##" + "x");
    }

    private void NotificationStart()
    {
        notificationSizeMagnification.OnClickAsObservable()
            .Subscribe(_ =>
            {
                notificationSize = SizeMagnification(notificationSize);
                notificationSizeText.text = notificationSize.ToString("0.##" + "x");
            })
            .AddTo(this);

        notificationSizeApply.OnClickAsObservable()
           .Subscribe(_ =>
           {
               NotificatiobSizeApply();
           })
           .AddTo(this);

        notificationVolumeMagnification.OnClickAsObservable()
            .Subscribe(_ =>
            {
                notificationVolume = VolumeMagnification(notificationVolume);
                notificationVolumeText.text = notificationVolume.ToString("0.##" + "x");
            })
            .AddTo(this);

        notificationVolumeApply.OnClickAsObservable()
           .Subscribe(_ =>
           {
               NotificatiobVolumeApply();
           })
           .AddTo(this);

        notificationDefaultApply.OnClickAsObservable()
            .Subscribe(_ =>
            {
                notificationSize = 1.0f;
                notificationSizeText.text = notificationSize.ToString("0.##" + "x");
                NotificatiobSizeApply();

                notificationVolume = 1.0f;
                notificationVolumeText.text = notificationVolume.ToString("0.##" + "x");
                NotificatiobVolumeApply();
            })
            .AddTo(this);
    }

    private void NotificatiobSizeApply()
    {
        PlayerPrefs.SetFloat("NotificationSize", notificationSize);
    }

    private void NotificatiobVolumeApply()
    {
        PlayerPrefs.SetFloat("NotificationVolume", notificationVolume);
    }

    #endregion

    #region General

    private float SizeMagnification(float magnification)
    {
        magnification += 0.25f;
        if (magnification > 1.5f)
        {
            magnification = 0.5f;
        }
        return magnification;
    }

    private float VolumeMagnification(float magnification)
    {
        magnification += 0.25f;
        if (magnification > 1.0f)
        {
            magnification = 0.0f;
        }
        return magnification;
    }

    #endregion
}
