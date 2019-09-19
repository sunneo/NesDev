using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev.Forms.DocWidgets
{
    public partial class GotoLine : Form
    {
        public int Line = 0;
        public GotoLine()
        {
            InitializeComponent();
        }
        public void SetInfo(int curline, int maxline)
        {
            labelMaxLine.Text = maxline.ToString();
            labelCurLine.Text = curline.ToString();
            Line = curline;
            numericUpDown1.Maximum = maxline;
            numericUpDown1.Value = curline;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Line = (int)numericUpDown1.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
