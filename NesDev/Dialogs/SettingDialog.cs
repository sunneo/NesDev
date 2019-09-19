using NesDev.Config;
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
    public partial class SettingDialog : Form
    {
        OpenFileDialog open = new OpenFileDialog() { Title = "Open Executable File", Filter = "Executable|*.exe;*.jar;*.bat;*.com|Config File|*.cfg|Any File|*.*", FilterIndex = 1 };
        public SettingDialog()
        {
            InitializeComponent();
        }
        public void Load(AppConfig config)
        {
            textBoxCC.Text = config.Configure.CC;
            textBoxCCINC.Text = String.Join(";", config.Configure.CCINC);
            textBoxCCFLAGS.Text = config.Configure.CCFLAGS;
            textBoxCA.Text = config.Configure.CA;
            textBoxASMINC.Text = String.Join(";", config.Configure.ASMINC);
            textBoxASMFLAGS.Text = config.Configure.ASMFLAGS;
            textBoxLD.Text = config.Configure.LD;
            textBoxLDCFG.Text = config.Configure.LDCFG;
        }
        public void Save(AppConfig config)
        {
            config.Configure.CC = textBoxCC.Text;
            config.Configure.CCINC = new List<string>(textBoxCCINC.Text.Split(';'));
            config.Configure.CCFLAGS = textBoxCCFLAGS.Text;
            config.Configure.CA = textBoxCA.Text;
            config.Configure.ASMINC = new List<string>(textBoxASMINC.Text.Split(';'));
            config.Configure.ASMFLAGS = textBoxASMFLAGS.Text;
            config.Configure.LD = textBoxLD.Text;
            config.Configure.LDCFG = textBoxLDCFG.Text;
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

        private void buttonBrowseCC_Click(object sender, EventArgs e)
        {
            open.FilterIndex = 1;
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxCC.Text = open.FileName;
            }
        }

        private void buttonEditCCINC_Click(object sender, EventArgs e)
        {
            IncludePathEditorDialog dialog = new IncludePathEditorDialog();
            dialog.SetIncludePath(this.textBoxCCINC.Text.Split(';'));
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxCCINC.Text = String.Join(";", dialog.GetIncludePath());
            }
        }

        private void buttonBrowseCA_Click(object sender, EventArgs e)
        {
            open.FilterIndex = 1;
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxCA.Text = open.FileName;
            }
        }

        private void buttonEditASMINC_Click(object sender, EventArgs e)
        {
            IncludePathEditorDialog dialog = new IncludePathEditorDialog();
            dialog.SetIncludePath(this.textBoxASMINC.Text.Split(';'));
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxASMINC.Text = String.Join(";", dialog.GetIncludePath());
            }
        }

        private void buttonBrowseLD_Click(object sender, EventArgs e)
        {
            open.FilterIndex = 1;
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxLD.Text = open.FileName;
            }
        }

        private void buttonBrowseLDCFG_Click(object sender, EventArgs e)
        {
            open.FilterIndex = 2;
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxLDCFG.Text = open.FileName;
            }
        }
    }
}
