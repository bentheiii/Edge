using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edge
{
    public class ReturnForm<T> : Form
    {
        private T _ret = default(T);
        public new Tuple<DialogResult, T> ShowDialog(IWin32Window owner = null)
        {
            var res = owner == null ? base.ShowDialog() : base.ShowDialog(owner);
            return Tuple.Create(res, _ret);
        }
        public new void Close()
        {
            // ReSharper disable once IntroduceOptionalParameters.Global
            Close(DialogResult.OK);
        }
        public void Close(T ret, DialogResult res = DialogResult.OK)
        {
            DialogResult = res;
            _ret = ret;
            base.Close();
        }
        public void Close(DialogResult res, T ret = default(T))
        {
            Close(ret, res);
        }
    }
}
