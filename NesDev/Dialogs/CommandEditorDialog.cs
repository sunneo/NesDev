using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev.Dialogs
{
    public partial class CommandEditorDialog : Form
    {
        OpenFileDialog open = new OpenFileDialog() { Filter = "Supported Files|*.js;*.bat;*.exe|Any Files|*.*", FilterIndex = 1, Title = "Open Executable File" };
        public Config.CustomCommand Command = new Config.CustomCommand();
        public void LoadConfig(Config.CustomCommand cmd)
        {
            this.Command = cmd;
            this.textBoxName.Text = Command.Name;
            this.textBoxPath.Text = Command.FileName;
            this.textBoxArguments.Text = Command.Arg;
            this.textBoxDescription.Text = Command.Description;
            this.checkBoxHoldConsole.Checked = Command.HoldConsole;
            this.checkBoxGrabOutput.Checked = Command.GrabOutput;
            this.checkBoxHideWindow.Checked = Command.HideWindow;
        }
        public CommandEditorDialog()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            if (this.Command == null)
            {
                this.Command = new Config.CustomCommand();
            }
            this.Command.Name = textBoxName.Text;
            this.Command.FileName = textBoxPath.Text;
            this.Command.Arg = textBoxArguments.Text;
            this.Command.Description = textBoxDescription.Text;
            this.Command.HoldConsole = checkBoxHoldConsole.Checked;
            this.Command.GrabOutput = checkBoxGrabOutput.Checked;
            this.Command.HideWindow = checkBoxHideWindow.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                String filename = open.FileName;
                if (Command == null)
                {
                    Command = new Config.CustomCommand();
                }
                textBoxPath.Text = filename;
                String ext = Path.GetExtension(filename);
                String name=Path.GetFileNameWithoutExtension(filename);
                
                if (ext.Equals(".exe", StringComparison.CurrentCultureIgnoreCase))
                {
                    FileVersionInfo fi = FileVersionInfo.GetVersionInfo(filename);
                    if (String.IsNullOrEmpty(textBoxDescription.Text))
                    {
                        textBoxDescription.Text = fi.FileDescription;
                    }
                    name += "("+fi.ProductVersion+")";
                }
                if (String.IsNullOrEmpty(textBoxName.Text))
                {
                    textBoxName.Text = name;
                }
            }
        }

        private void buttonMacros_Click(object sender, EventArgs e)
        {
            MacroValueWindow macro = new MacroValueWindow();
            macro.SetTryText(textBoxArguments.Text);
            if(macro.ShowDialog(this)== System.Windows.Forms.DialogResult.OK)
            {
                textBoxArguments.Text = macro.Result;
            }
        }
    }
}
