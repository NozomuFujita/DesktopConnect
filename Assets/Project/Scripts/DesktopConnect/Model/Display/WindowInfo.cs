using System;
using System.Runtime.InteropServices;

namespace DisplaySystem
{
    public class WindowInfo
    {
        public IntPtr hWnd;
        public string title;
        public string className;
        public RectInt rectInt;


        public WindowInfo(IntPtr hWnd, string title, string className, RectInt rectInt)
        {
            this.hWnd = hWnd;
            this.title = title;
            this.className = className;
            this.rectInt = rectInt;
        }
    }


    /// <summary>
    /// ディスプレイに表示されるウィンドウのピクセル値
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RectInt
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int width { get { return right - left; } }
        public int height { get { return bottom - top; } }
    }


    /// <summary>
    /// ディスプレイに表示されるウィンドウのfloat(Unityワールド上での)値
    /// </summary>
    public struct RectFloat
    {
        public float left;
        public float top;
        public float right;
        public float bottom;
        public float width { get { return right - left; } }
        public float height { get { return top - bottom; } }
    }


    /// <summary>
    /// 座標を保存するためのPOINT構造体
    /// </summary>
    public struct POINT
    {
        public int x; 
        public int y;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
}