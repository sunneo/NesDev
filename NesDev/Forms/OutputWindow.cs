using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NesDev.Forms
{
    public partial class OutputWindow : DockContent
    {
        public void AppendLine(String fmt)
        {
            this.richTextBox1.AppendText(fmt + Environment.NewLine);
            this.richTextBox1.Select(this.richTextBox1.TextLength, 0);
            this.richTextBox1.ScrollToCaret();
        }
        public void AppendLine()
        {
            this.AppendLine("");
        }
        public void AppendLine(String fmt, Object arg)
        {
            this.AppendLine(String.Format(fmt, arg));
        }
        public void AppendLine(String fmt, Object arg1, Object arg2)
        {
            this.AppendLine(String.Format(fmt, arg1,arg2));
        }
        public void AppendLine(String fmt, Object arg1, Object arg2, Object arg3)
        {
            this.AppendLine(String.Format(fmt, arg1, arg2, arg3));
        }
        public void AppendLine(String fmt, params Object[] args)
        {
            this.AppendLine(String.Format(fmt, args));
        }
        public OutputWindow(String title="")
        {
            InitializeComponent();
            this.richTextBox1.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            this.Icon = Icon.FromHandle(global::NesDev.Properties.Resources.log_file_1_504262.GetHicon());
            if (!String.IsNullOrEmpty(title))
            {
                this.Text = title;
                this.TabText = title;
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.richTextBox1.ResetText();
        }
    }
}
