using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CCDefault.Annotations;
using Edge.Units.DigitalInfo;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Edge.Enviroment
{
    
    public static class DotNetFramework
    {
        //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
        static private double getversionbyregstring(string regstring)
        {
            Regex r = new Regex(@"^v[0-9]+((\.)?[0-9])*$");
            if (r.IsMatch(regstring))
            {
                string t = regstring.Remove(0, 1);
                if (t.IndexOf('.') != t.LastIndexOf('.'))
                {
                    t = t.Substring(0,t.LastIndexOf('.'));
                }
                return Convert.ToDouble(t, CultureInfo.InvariantCulture);
            }
            return -1;
        }
        static public double latestdotnetversion()
        {
            RegistryKey installed_Versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
// ReSharper disable once PossibleNullReferenceException
            string[] version_Names = installed_Versions.GetSubKeyNames();
            double v = getversionbyregstring(version_Names[version_Names.Length - 1]);
            return v;
        }
        static public double[] alldotnetversions()
        {
            RegistryKey installed_Versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
// ReSharper disable once PossibleNullReferenceException
            string[] version_Names = installed_Versions.GetSubKeyNames();
            List<double> ret = new List<double>();
            foreach (string s in version_Names)
            {
                double t = getversionbyregstring(s);
                if (t != -1)
                    ret.Add(t);
            }
            return ret.Distinct().ToArray();
        }
        static public bool dotnetversioninstalled(double version)
        {
            foreach (double d in alldotnetversions())
            {
                if (d.Equals(version))
                    return true;
            }
            return false;
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
                ret._Freespace = new BinaryData(f, BinaryData.Bit);
                ret._TotalSpace = new BinaryData(t, BinaryData.Bit);
                ret._TotalFreePpace = new BinaryData(ft, BinaryData.Bit);
                return ret;
            }
            return null;
        }
        public class DiskSpaceData
        {
            internal BinaryData _Freespace;
            internal BinaryData _TotalSpace;
            internal BinaryData _TotalFreePpace;
            /// <summary>
            /// measures in bytes
            /// </summary>
            public BinaryData freespace
            {
                get
                {
                    return this._Freespace;
                }
            }
            /// <summary>
            /// measures in bytes
            /// </summary>
            public BinaryData totalspace
            {
                get
                {
                    return this._TotalSpace;
                }
            }
            /// <summary>
            /// measures in bytes
            /// </summary>
            public BinaryData totalfreespace
            {
                get
                {
                    return this._TotalFreePpace;
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
