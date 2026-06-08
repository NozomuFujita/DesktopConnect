using DisplaySystem;
using System.Collections.Generic;

public class DisplayModel
{
    // デフォルト値
    public static readonly int displayWidthPixel = 1680;
    public static readonly int displayHeightPixel = 1050;
    public static readonly float displayWidthFloat = 8.0f; //(中央から端までの距離)
    public static readonly float displayHeightFloat = 10.0f;

    private static WindowsAPI _windowsAPI;
    public static WindowsAPI windowsAPI => _windowsAPI;
    private static PixelFloatConverter _pixelFloatConverter;
    public static PixelFloatConverter pixelFloatConverter => _pixelFloatConverter;
    private static DisplayChecker _displayChecker;
    public static DisplayChecker displayChecker => _displayChecker;

    static DisplayModel()
    {
        _windowsAPI = new WindowsAPI();

        var displayPixel = windowsAPI.GetDisplayPixel();
        displayWidthPixel = displayPixel.x;
        displayHeightPixel = displayPixel.y;
        _pixelFloatConverter = new PixelFloatConverter(displayWidthPixel, displayHeightPixel, ref displayWidthFloat, ref displayHeightFloat);

        _displayChecker = new DisplayChecker(displayWidthFloat, displayHeightFloat);
    }
}
