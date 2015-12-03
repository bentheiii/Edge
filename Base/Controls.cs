using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CCDefault.Annotations;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Edge.Controls
{
    public class ControlMouseDragger
    {
        private Point _cursoradjust = new Point(-1, -1), _formadjust;
        private Control _handler = null;
        private Control _displacer = null;
        public Control displacer
        {
            get
            {
                return _displacer;
            }
            set
            {
                _displacer = value;
                if (_cursoradjust.X != -1)
                {
                    _formadjust = displacer.Location;
                }
            }
        }
        public Control Handler
        {
            get
            {
                return _handler;
            }
            set
            {
                if (_handler != null)
                {
                    _handler.MouseDown -= handlerMouseDown;
                    _handler.MouseMove -= handlerMouseMove;
                    _handler.MouseUp -= handlerMouseUp;
                }
                _handler = value;
                _handler.MouseDown += handlerMouseDown;
                _handler.MouseUp += handlerMouseUp;
                _handler.MouseMove += handlerMouseMove;
            }
        }
        public ControlMouseDragger(Control displacer, Control handle)
        {
            this.displacer = displacer;
            this.Handler = handle;
        }
        private void handlerMouseDown(object sender, MouseEventArgs e)
        {
            _cursoradjust = Cursor.Position;
            _formadjust = displacer.Location;
        }
        private void handlerMouseMove(object sender, MouseEventArgs e)
        {
            if (_cursoradjust.X != -1)
            {
                displacer.Location = new Point(_formadjust.X + Cursor.Position.X - _cursoradjust.X, _formadjust.Y + Cursor.Position.Y - _cursoradjust.Y);
            }
        }
        private void handlerMouseUp(object sender, MouseEventArgs e)
        {
            _formadjust = new Point(-1, -1);
        }
    }
    public static class ContontrolExtensions
    {
        public static void BindEnum<T>(this ComboBox c, T selectedVal) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            c.DataSource = Enum.GetValues(typeof(T));
            c.SelectedValue = selectedVal;
        }
        public static void BindEnum<T>(this ComboBox c) where T : struct, IConvertible
        {
            c.BindEnum(default(T));
        }
    }
}
namespace Edge.Window
{
    public interface IFormBound
    {
        void bindtoprogress(Form f, string appid);
    }
    /// <summary>
    /// thanks to http://bobpowell.net/
    /// </summary>
    public static class Flasher
    {

        [DllImport("user32.dll")]
        private static extern int FlashWindowEx(ref Flashwinfo pfwi);

        [StructLayout(LayoutKind.Sequential)]
        private struct Flashwinfo
        {
            public uint cbSize;
            public IntPtr hwnd;
            public int dwFlags;
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            public uint uCount;
            public int dwTimeout;
        }

        [Flags]
        private enum Flashwinfoflags
        {
            [UsedImplicitly]
            FLASHW_STOP = 0,
            FLASHW_CAPTION = 0x00000001,
            FLASHW_TRAY = 0x00000002,
            FLASHW_ALL = (FLASHW_CAPTION | FLASHW_TRAY),
            [UsedImplicitly]
            FLASHW_TIMER = 0x00000004,
            FLASHW_TIMERNOFG = 0x0000000C
        }
        public static void flash(Form f)
        {
            Flashwinfo fw = new Flashwinfo
            {
                cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(Flashwinfo))),
                hwnd = f.Handle,
                dwFlags = (int)(Flashwinfoflags.FLASHW_ALL | Flashwinfoflags.FLASHW_TIMERNOFG),
                dwTimeout = 0
            };
            FlashWindowEx(ref fw);
        }
        public static void stopflash(Form f)
        {
            f.Activate();
        }
    }
    public class IconOverlayManager : IFormBound
    {
        private readonly TaskbarManager _tbm;
        public IconOverlayManager()
        {
            this._tbm = TaskbarManager.Instance;
        }
        public IconOverlayManager(Form f, string appid)
        {
            this._tbm = TaskbarManager.Instance;
            this.bindtoprogress(f, appid);
        }
        public void bindtoprogress(Form f, string appid)
        {
            this._tbm.SetApplicationIdForSpecificWindow(f.Handle, appid);
        }
        public void changeoverlay(Icon icon, string alttext = "")
        {
            this._tbm.SetOverlayIcon(icon, alttext);
        }
    }
    public class Taskbarprogresser : IFormBound
    {
        public Taskbarprogresser()
        {
            this._tbm = TaskbarManager.Instance;
        }
        public Taskbarprogresser(Form f, string appid)
        {
            this._tbm = TaskbarManager.Instance;
            this.bindtoprogress(f, appid);
        }
        public void bindtoprogress(Form f, string appid)
        {
            this._tbm.SetApplicationIdForSpecificWindow(f.Handle, appid);
        }
        private readonly TaskbarManager _tbm;
        public void setprogress(double i)
        {
            if (i != 0)
            {
                this._tbm.SetProgressState(TaskbarProgressBarState.Normal);
                this._tbm.SetProgressValue((int)(i * 1000), 1000);
                return;
            }
            this._tbm.SetProgressState(TaskbarProgressBarState.NoProgress);
        }
        public enum ProgressBarState { Regular = TaskbarProgressBarState.Normal, DisableProgressBar = TaskbarProgressBarState.NoProgress, Red = TaskbarProgressBarState.Error, Yellow = TaskbarProgressBarState.Paused, GreenSpinny = TaskbarProgressBarState.Indeterminate };
        public void changeprogressbarstate(ProgressBarState s)
        {
            this._tbm.SetProgressState((TaskbarProgressBarState)s);
        }
    }
    public class Thumbnailbuttons
    {
        public static void addbutton(Form f, Icon buttonicon, string tooltip, Action<object, IntPtr> onclick, out ThumbnailButtonProxy proxy, string appid)
        {
            TaskbarManager tbm = TaskbarManager.Instance;
            tbm.SetApplicationIdForSpecificWindow(f.Handle, appid);
            using (ThumbnailToolBarButton tntbb = new ThumbnailToolBarButton(buttonicon, tooltip))
            {
                proxy = new ThumbnailButtonProxy(onclick);
                tntbb.Click += proxy.proxyaction;
                ThumbnailToolBarManager ttbm = tbm.ThumbnailToolBars;
                ttbm.AddButtons(f.Handle, tntbb);
            }
        }
        public static void addbutton(Form f, Icon buttonicon, string tooltip, Action<object, IntPtr> onclick, string appid)
        {
            ThumbnailButtonProxy p;
            addbutton(f, buttonicon, tooltip, onclick, out p, appid);
        }
        private Thumbnailbuttons()
        {

        }
        public class ThumbnailButtonProxy
        {
            public ThumbnailButtonProxy()
            {
            }
            public ThumbnailButtonProxy(Action<object, IntPtr> a)
            {
                this.RealAction = a;
            }
            public readonly Action<object, IntPtr> RealAction;
            public void proxyaction(object s, ThumbnailButtonClickedEventArgs e)
            {
                this.RealAction.Invoke(s, e.WindowHandle);
            }
        }
    }
    public static class TrayIcons
    {
        private const int DEFAULTTIMEOUT = 500;
        public static void showBalloon(string title, string body, int timeout = DEFAULTTIMEOUT, Action<object, EventArgs> onclick = null)
        {
            showBalloon(title, body, SystemIcons.Application, timeout, onclick);
        }
        public static void showBalloon(string title, string body, Icon c, int timeout = DEFAULTTIMEOUT, Action<object, EventArgs> onclick = null)
        {
            using (NotifyIcon i = new NotifyIcon { Icon = c, Visible = true })
            {
                i.BalloonTipClosed += disposebaloon;
                showBalloon(title, body, i, timeout, onclick);
            }
        }
        public static void showBalloon(string title, string body, NotifyIcon i, int timeout = DEFAULTTIMEOUT, Action<object, EventArgs> onclick = null)
        {
            showBalloon(title, body, i, ToolTipIcon.None, timeout, onclick);
        }
        public static void showBalloon(string title, string body, NotifyIcon i, ToolTipIcon c, int timeout = DEFAULTTIMEOUT, Action<object, EventArgs> onclick = null)
        {
            if (title != null)
            {
                i.BalloonTipTitle = title;
            }
            if (body != null)
            {
                i.BalloonTipText = body;
            }
            if (c != ToolTipIcon.None)
            {
                i.BalloonTipIcon = c;
            }
            i.ShowBalloonTip(timeout);
            if (onclick != null)
                i.BalloonTipClicked += new EventHandler(onclick);
        }
        private static void disposebaloon(object sender, EventArgs e)
        {
            ((NotifyIcon)(sender)).Dispose();
        }
    }
    public abstract class Keyboard
    {
        [Flags]
        private enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
        }
    }
}
