using UnityEngine;

public class DisplayPresenter
{
    public DisplayPresenter()
    {
        DisplayModel.windowsAPI.GetDisplayPixel();
    }


    /// <summary>
    /// デスクトップマスコット用のセットアップ
    /// 1. ウィンドウ上部のバーの消去
    /// 2. 背景のクロマキー透過
    /// 3. 最前面表示
    /// </summary>
    public void SetUp(Color chromakeyColor)
    {
        DisplayModel.windowsAPI.ClearWindowFrame();
        DisplayModel.windowsAPI.MakeWindowTransparent(chromakeyColor);
        DisplayModel.windowsAPI.BringWindowTop();
    }


    /// <summary>
    /// メニュー、通知アイコンの位置を初期位置等に戻す(緊急用)
    /// </summary>
    public void PositionInitialization()
    {
        MenuModel.externalInteration.InitializeMenuPosition();
        NotificationModel.notificationPosition.InitializeMenuPosition();
    }
}
