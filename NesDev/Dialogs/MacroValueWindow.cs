using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using Serializers;
namespace NesDev.Dialogs
{
    public partial class MacroValueWindow : Form
    {
        public String Result = "";
        public MacroValueWindow()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach(ListViewItem item in this.listView1.Items)
            {
                item.SubItems[1].Text = Utility.Expander.Expand(item.SubItems[0].Text);
            }
        }
        public void SetTryText(String txt)
        {
            textBoxTry.Text = txt;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var info = listView1.HitTest(e.Location);
            if (info != null && info.Item != null)
            {
                textBoxTry.Text += info.Item.Text + " ";
            }
        }

        private void textBoxTry_TextChanged(object sender, EventArgs e)
        {
            String txt = textBoxTry.Text;
            txt = Utility.Expander.Expand(txt);
            textBoxPreView.Text = txt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Result = textBoxTry.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
