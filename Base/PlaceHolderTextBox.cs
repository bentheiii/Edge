using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edge.LabeledTextBox
{
    public class PlaceHolderTextBox : TextBox
    {
        public string PlaceHolderText { get; set; }
        public bool PlaceHeld { get; set; }
        public PlaceHolderTextBox()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            base.Text = PlaceHolderText;
            PlaceHeld = true;
        }
        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            if (base.Text == "" && !PlaceHeld)
            {
                PlaceHeld = true;
                base.Text = PlaceHolderText;
            }
        }
        private void OnGotFocus(object sender, EventArgs eventArgs)
        {
            if (PlaceHeld)
            {
                PlaceHeld = false;
                base.Text = "";
            }
        }
        public override string Text
        {
            get
            {
                return PlaceHeld ? "" : base.Text;
            }
            set
            {
                PlaceHeld = false;
                base.Text = value;
                OnLostFocus(this,new EventArgs());
            }
        }
    }
}
