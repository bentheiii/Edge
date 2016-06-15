using System;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Edge.Looping;
using Edge.Units.DataSizes;

namespace Edge.Enviroment
{
    public static class Keyboard
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
    public static class Platform
    {
// ReSharper disable InconsistentNaming
        public enum PlatformArcitecture {x86 = 32,bit64 = 64, other = -1 }
// ReSharper restore InconsistentNaming
        public static PlatformArcitecture getmachineplatform()
        {
	        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")) ? PlatformArcitecture.bit64 : PlatformArcitecture.x86;
        }
        public static byte[] getMacAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == string.Empty)// only return MAC Address from first card  
                {
                    //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return Loops.Range(0, sMacAddress.Length).Where(x => x%2 == 0)
                .Select(x => Convert.ToByte(sMacAddress.Substring(x, 2), 16))
                .ToArray();
        }
    }
    public static class Disk
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
        /// <summary>
        /// returns null if disk not found
        /// </summary>
        public static DiskSpaceData getdiskdata(char driveletter)
        {
            DiskSpaceData ret = new DiskSpaceData();
            ulong f, t, ft;
            if (GetDiskFreeSpaceEx(driveletter+":", out f, out t, out ft))
            {
                ret._freespace = new DataSize(f, DataSize.Bit);
                ret._totalSpace = new DataSize(t, DataSize.Bit);
                ret._totalFreePpace = new DataSize(ft, DataSize.Bit);
                return ret;
            }
            return null;
        }
        public class DiskSpaceData
        {
            internal DataSize _freespace;
            internal DataSize _totalSpace;
            internal DataSize _totalFreePpace;
            /// <summary>
            /// measures in bytes
            /// </summary>
            public DataSize freespace
            {
                get
                {
                    return this._freespace;
                }
            }
            /// <summary>
            /// measures in bytes
            /// </summary>
            public DataSize totalspace
            {
                get
                {
                    return this._totalSpace;
                }
            }
            /// <summary>
            /// measures in bytes
            /// </summary>
            public DataSize totalfreespace
            {
                get
                {
                    return this._totalFreePpace;
                }
            }
            internal DiskSpaceData() { }
        }
    }
    public static class ScreenLens
    {
        public static Image capture(Rectangle r, bool showcursor=true)
        {
            Bitmap bitmap = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(r.Location, Point.Empty, r.Size);
            if (showcursor)
            {
                Point p = new Point(Cursor.Position.X - r.Location.X, Cursor.Position.Y - r.Location.Y);
                Rectangle cursorBounds = new Rectangle(p, Cursor.Current.Size);
                Cursors.Default.Draw(g, cursorBounds);
            }
            return bitmap;
        }
        public static Image capture(bool showcursor = true)
        {
            return capture(Screen.GetBounds(Screen.GetBounds(Point.Empty)), showcursor);
        }
        public static Image capture(Form f, bool showcursor = true)
        {
            Rectangle r = new Rectangle(f.Location, f.Size);
            return capture(r, showcursor);
        }
    }
}
