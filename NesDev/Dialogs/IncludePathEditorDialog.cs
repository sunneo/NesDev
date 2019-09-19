using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev.Dialogs
{
    public partial class IncludePathEditorDialog : Form
    {
        FolderBrowserDialog open = new FolderBrowserDialog();
        public IncludePathEditorDialog()
        {
            InitializeComponent();
        }
        
        public void SetIncludePath(IEnumerable<String> s)
        {
            this.textBox1.Text = String.Join(Environment.NewLine, s);
        }
        public List<String> GetIncludePath()
        {
            return new List<string>(this.textBox1.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.None));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.AppendText(open.SelectedPath + Environment.NewLine);
            }
        }
    }
}
