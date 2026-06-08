using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace DisplaySystem
{
    public class WindowsAPI
    {
        // アクティブウィンドウのウィンドウハンドルを取得
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        // 指定したウィンドウとの関係を持つウィンドウへのハンドルを取得
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        // 指定したシステム メトリックまたはシステム構成設定を取得
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        // 指定したウィンドウの外接する四角形の寸法を取得(寸法は画面の左上隅を基準にした画面座標で指定)
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RectInt lpRect);
        // 指定したウィンドウに関する情報を取得
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        // 指定したウィンドウのタイトル バー テキストの長さを文字数で取得
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        // 指定したウィンドウのタイトル バーのテキスト (ある場合) をバッファーにコピー
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        // 指定したウィンドウが属するクラスの名前を取得
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        // 指定したウィンドウの属性を変更
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        // レイヤード ウィンドウの不透明度および透明度のカラー キーを設定
        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);
        // 子ウィンドウ、ポップアップ ウィンドウ、またはトップレベル ウィンドウのサイズ、位置、Z の順序を変更
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        // ウィンドウをアクティブにする(バッググラウンドにある場合はアクティブにならない)
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);
        // 操作が完了するまで待たずにウィンドウの表示状態を設定
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        // 指定したウィンドウの表示状態を決定(取得)
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        // 指定したウィンドウを最小化 (アイコン) するかどうかを決定
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        // 指定したウィンドウを最大化 (アイコン) するかどうかを決定
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);
        // 指定したウィンドウでマウス入力およびキーボード入力が有効かどうかを判別
        [DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);
        // 画面上のすべての最上位ウィンドウを列挙(子ウィンドウ(...ポップアップのようなもの)は列挙されない)
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        // 指定したウィンドウの表示状態を設定
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // 指定したメッセージをウィンドウまたはウィンドウに送信
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref MINMAXINFO lParam);

        // 指定したウィンドウを作成したスレッドをフォアグラウンドに移動し、ウィンドウをアクティブにする
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        // GetWindowRectだと見た目とRectで誤差が生じるため、正確に取得するならこっち
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")]
        private static extern long DwmGetWindowAttributeRect(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttribute, out RectInt lpRect, int cbAttribute);
        // 不明
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")]
        private static extern bool DwmGetWindowAttributeBool(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttribute, out bool lpBool, int cbAttribute);
        // 指定したウィンドウの位置と寸法を変更
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        const int MaxLength = 256;
        const int GW_OWNER = 4;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;
        const int SW_RESTORE = 9;
        const int WM_GETMINMAXINFO = 0x0024;
        const int WS_CAPTION = 0x00C00000;
        const int WS_VISIBLE = 0x10000000;
        const int WS_EX_LAYERD = 0x00080000;
        const int WS_EX_TOPMOST = 0x00000008;
        const int HWND_TOP = 0;
        const int HWND_TOPMOST = -1;
        const int LWA_COLORKEY = 1;
        const int SWP_SHOWWINDOW = 0x00000040;

        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        };

        private List<string> exclusionList;
        private List<WindowInfo> windowList;
        private UseWindowManagment useWindowManagment;


        public WindowsAPI()
        {
            exclusionList = new List<string>();
            windowList = new List<WindowInfo>();
            useWindowManagment = new UseWindowManagment();
        }



        private IntPtr ZeroChecker(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                hWnd = GetActiveWindow();
                UnityEngine.Debug.LogError("WindowAPI : hWndが0です");
            }

            return hWnd;
        }


        /// <summary>
        /// メインディスプレイの解像度を取得
        /// </summary>
        public Vector2Int GetDisplayPixel()
        {
            var displayPixel = new Vector2Int();
            displayPixel.x = GetSystemMetrics(SM_CXSCREEN);
            displayPixel.y = GetSystemMetrics(SM_CYSCREEN);
            return displayPixel;
        }


        /// <summary>
        /// ウィンドウのフレームを削除
        /// </summary>
        public void ClearWindowFrame()
        {
            IntPtr hWnd = GetActiveWindow();
            int style = GetWindowLong(hWnd, GWL_STYLE);
            style &= ~WS_CAPTION;
            SetWindowLong(hWnd, GWL_STYLE, style);
        }


        /// <summary>
        /// 指定色をクロマキーとして透過
        /// </summary>
        public void MakeWindowTransparent(Color chromakey)
        {
            int r = Mathf.RoundToInt(chromakey.r * 255);
            int g = Mathf.RoundToInt(chromakey.g * 255);
            int b = Mathf.RoundToInt(chromakey.b * 255);
            int chromakeyBGR = (b << 16) | (g << 8) | (r << 0);

            IntPtr hWnd = GetActiveWindow();
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            exStyle |= WS_EX_LAYERD;
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
            SetLayeredWindowAttributes(hWnd, chromakeyBGR, 255, LWA_COLORKEY);
        }


        /// <summary>
        /// ウィンドウを最前面に持っていく
        /// </summary>
        public void BringWindowTop()
        {
            IntPtr hWnd = GetActiveWindow();
            var displayPixel = GetDisplayPixel();
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOPMOST;
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, displayPixel.x, displayPixel.y, SWP_SHOWWINDOW);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
        }


        /// <summary>
        /// ウィンドウの位置や大きさを変える
        /// </summary>
        public void SetWindowPos(IntPtr hWnd, int x, int y, int cx, int cy)
        {
            ZeroChecker(hWnd);

            //SetWindowActive(hWnd);
            SetWindowPos(hWnd, HWND_TOP, x, y, cx, cy, SWP_SHOWWINDOW);
        }


        /// <summary>
        /// 指定したウィンドウのシステム的な大きさ、幅などの情報を取得 (可視領域より少し大きい)
        /// </summary>
        public RectInt GetWindowSystemRect(IntPtr hWnd)
        {
            ZeroChecker(hWnd);

            var rect = new RectInt();
            GetWindowRect(hWnd, out rect);
            return rect;
        }


        /// <summary>
        /// 指定したウィンドウの見えている大きさ、幅などの情報を取得 (システムな大きさより少し小さい)
        /// </summary>
        public RectInt GetWindowVisualRect(IntPtr hWnd)
        {
            ZeroChecker(hWnd);

            var rect = new RectInt();
            DwmGetWindowAttributeRect(hWnd, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RectInt)));
            return rect;
        }


        /// <summary>
        /// 指定したウィンドウの最小サイズを取得 (大きさはシステムの大きさ)
        /// </summary>
        public MINMAXINFO GetWindowMinSize(IntPtr hWnd)
        {
            ZeroChecker(hWnd);

            MINMAXINFO mmi = new MINMAXINFO();
            SendMessage(hWnd, WM_GETMINMAXINFO, IntPtr.Zero, ref mmi);
            return mmi;
        }


        /// <summary>
        /// 指定したウィンドウをアクティブにする
        /// </summary>
        public void SetWindowActive(IntPtr hWnd)
        {
            if(hWnd == IntPtr.Zero)
            {
                return;
            }

            if(IsIconic(hWnd))
            {
                ShowWindowAsync(hWnd, SW_RESTORE);
            }
            SetForegroundWindow(hWnd);
        }


        /// <summary>
        /// 指定したウィンドウの移動
        /// </summary>
        public void TranslateWindowPosition(IntPtr hWnd, RectInt rect)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            MoveWindow(hWnd, rect.left, rect.top, rect.width, rect.height, true);
        }


        /// <summary>
        /// 画面に表示されているウィンドウを取得
        /// </summary>
        public List<WindowInfo> GetEnumWindows()
        {
            windowList.Clear();
            exclusionList.Clear();

            exclusionList = useWindowManagment.ReadExclusionList();
            EnumWindows(EnumrateWindows, IntPtr.Zero);
            return windowList;
        }

        private bool EnumrateWindows(IntPtr hWnd, IntPtr lParam)
        {
            // ウィンドウが非表示なら除外
            if (!IsWindowVisible(hWnd))
                return true;

            // ウィンドウが無効状態なら除外
            if (!IsWindowEnabled(hWnd))
                return true;

            // ウィンドウが最小化されているなら除外
            if (IsIconic(hWnd))
                return true;

            // ウィンドウハンドルが0なら除外
            if (GetWindow(hWnd, GW_OWNER) != IntPtr.Zero)
                return true;

            // ウィンドウのタイトルバーテキストの長さが0なら除外
            int textLength = GetWindowTextLength(hWnd);
            if (textLength == 0)
                return true;

            // 見えないウィンドウの除外(?) 当時の資料を探すも行方不明
            DwmGetWindowAttributeBool(hWnd, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out bool cLock, Marshal.SizeOf(typeof(bool)));
            if (cLock)
                return true;

            // ウィンドウのクラス名による個別除外(自アプリや、上記で選抜出来なかった環境ごとのアプリを覗くため)
            var className = new StringBuilder(MaxLength);
            GetClassName(hWnd, className, className.Capacity);
            if (exclusionList.Exists(s => s == className.ToString()))
                return true;

            // ウィンドウリストへの登録
            var title = new StringBuilder(textLength + 1);
            GetWindowText(hWnd, title, title.Capacity);
            windowList.Add(new WindowInfo(hWnd, title.ToString(), className.ToString(), GetWindowVisualRect(hWnd)));
            return true;
        }


        /// <summary>
        /// 除外リストの更新
        /// </summary>
        public void AddExclusionWindow(string className)
        {
            useWindowManagment.WriteExclusionList(className);
        }
    }

}