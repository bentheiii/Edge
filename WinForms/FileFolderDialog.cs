using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Edge.FileFolderDialog
{
    //credit to http://www.codeproject.com/Articles/44914/Select-file-or-folder-from-the-same-dialog
    public class FileFolderDialog : CommonDialog
    {
        private OpenFileDialog _dialog = new OpenFileDialog();

        public OpenFileDialog Dialog
        {
            get { return _dialog; }
            set { _dialog = value; }
        }
        public new DialogResult ShowDialog(IWin32Window owner = null)
        {
            // Set validate names to false otherwise windows will not let you select "Folder Selection."
            _dialog.ValidateNames = false;
            _dialog.CheckFileExists = false;
            _dialog.CheckPathExists = true;

            try
            {
                // Set initial directory (used when dialog.FileName is set from outside)
                if (!string.IsNullOrEmpty(_dialog.FileName))
                {
                    _dialog.InitialDirectory = Directory.Exists(_dialog.FileName) ? _dialog.FileName : System.IO.Path.GetDirectoryName(_dialog.FileName);
                }
            }
            catch 
            {
                // Do nothing
            }

            // Always default to Folder Selection.
            _dialog.FileName = "Folder Selection.";

            return owner == null ? _dialog.ShowDialog() : _dialog.ShowDialog(owner);
        }

        /// <summary>
        // Helper property. Parses FilePath into either folder path (if Folder Selection. is set)
        // or returns file path
        /// </summary>
        public string SelectedPath
        {
            get
            {
                try
                {
                    if (_dialog.FileName != null &&
                        (_dialog.FileName.EndsWith("Folder Selection.") || !File.Exists(_dialog.FileName)) &&
                        !Directory.Exists(_dialog.FileName))
                    {
                        return System.IO.Path.GetDirectoryName(_dialog.FileName);
                    }
                    return _dialog.FileName;
                }
                catch
                {
                    return _dialog.FileName;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _dialog.FileName = value;
                }
            }
        }

        /// <summary>
        /// When multiple files are selected returns them as semi-colon seprated string
        /// </summary>
        public string SelectedPaths
        {
            get
            {
                if (_dialog.FileNames != null && _dialog.FileNames.Length > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string fileName in _dialog.FileNames)
                    {
                        try
                        {
                            if (File.Exists(fileName))
                                sb.Append(fileName + ";");
                        }
                        catch
                        {
                            // Go to next
                        }
                    }
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public override void Reset()
        {
            _dialog.Reset();
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            return true;
        }
    }
}
