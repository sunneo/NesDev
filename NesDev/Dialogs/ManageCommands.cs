using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev.Dialogs
{
    public partial class ManageCommands : Form
    {
        Config.CustomCommandList CommandList = new Config.CustomCommandList();
        public ManageCommands()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.listView1.ShowItemToolTips = true;
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback((stat) => {
                try
                {
                    this.CommandList = Utility.Deserialize<Config.CustomCommandList>(Program.config.LauncherListPath);
                    if (this.CommandList == null)
                    {
                        this.CommandList = new Config.CustomCommandList();
                    }
                    this.BeginInvoke(new Action(RefreshList));
                }
                catch(Exception ee)
                {
                    GlobalEvents.NotifyException(this, ee);
                }
            }));
        }
        private void RefreshList()
        {
            try
            {
                this.listView1.VirtualListSize = CommandList.List.Count;
                this.listView1.Invalidate();
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        private void listView1_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            try
            {
                if (e.IsSelected)
                {
                    if (e.EndIndex >= 0 && e.EndIndex < this.CommandList.List.Count)
                    {
                        UIUpdateSelectedIndex(e.EndIndex);
                    }
                }
                else
                {
                    UIUpdateSelectedIndex(-1);
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex < 0) return;
                Config.CustomCommand cmd = this.CommandList.List[e.ItemIndex];
                ListViewItem item = new ListViewItem(new String[]{
                cmd.Name,
                cmd.FileName,
                cmd.Arg
            });
                e.Item = item;
                item.ToolTipText = String.Join(Environment.NewLine, cmd.Name, cmd.FileName, cmd.Arg);
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }
        private void UIUpdateSelectedIndex(int idx)
        {
            if (idx == -1)
            {
                label2.Text = "";
            }
            else
            {
                Config.CustomCommand cmd = this.CommandList.List[idx];
                label2.Text = cmd.Description;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedIndices.Count == 0)
                {
                    UIUpdateSelectedIndex(-1);
                    return;
                }
                UIUpdateSelectedIndex(listView1.SelectedIndices[0]);
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CommandEditorDialog dialog = new CommandEditorDialog();
                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.CommandList.List.Add(dialog.Command);
                    this.BeginInvoke(new Action(RefreshList));
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedIndices.Count == 0) return;
                CommandEditorDialog dialog = new CommandEditorDialog();
                int idx = listView1.SelectedIndices[0];
                dialog.LoadConfig(this.CommandList.List[idx]);
                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.CommandList.List.RemoveAt(idx);
                    this.CommandList.List.Insert(idx, dialog.Command);
                    this.BeginInvoke(new Action(RefreshList));
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;
            int idx = listView1.SelectedIndices[0];
            this.CommandList.List.RemoveAt(idx);
            this.BeginInvoke(new Action(RefreshList));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Utility.Serialize(Program.config.LauncherListPath, this.CommandList);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
