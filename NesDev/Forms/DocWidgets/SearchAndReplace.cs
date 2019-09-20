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
    public partial class SearchAndReplace : Form
    {
        public class FindContext
        {
            public String SearchPattern;
            public int StartIndex;
            public bool Started;
            public int LastResult;
        }
        public FindContext Context = new FindContext();
        bool bReplaceMode = false;
        public class SearchFlags
        {
            public bool MatchCase;
            public bool WholeMatch;
        }
        public class FindEventArgs : EventArgs
        {
            public SearchFlags Flags = new SearchFlags();
            public String Pattern;
        }
        public class FindReplaceEventArgs : FindEventArgs
        {
            public String ReplaceTo;
        }
        public event EventHandler<FindEventArgs> OnFindClick;
        public event EventHandler<FindReplaceEventArgs> OnReplaceClick;
        public event EventHandler<FindReplaceEventArgs> OnReplaceAllClick;
        public SearchAndReplace()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ContainerControl, true);
            this.textBoxFindText.KeyDown += textBoxFindText_KeyDown;
            this.textBoxReplaceText.KeyDown += textBoxFindText_KeyDown;
        }

        void textBoxFindText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                PerformFind();
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        public void SetFocus()
        {
            ActiveControl = this.textBoxFindText;
        }
        public void SetSearchText(String txt)
        {
            this.textBoxFindText.Text = txt;
        }
        public void PerformFind()
        {
            if (OnFindClick != null)
            {
                var findArg = CreateFindArgs();

                OnFindClick(this, CreateFindArgs());
            }
        }
        FindEventArgs CreateFindArgs()
        {
            FindEventArgs ret = new FindEventArgs();
            ret.Pattern = textBoxFindText.Text;
            ret.Flags.MatchCase = false;
            ret.Flags.WholeMatch = false;

            if (ret.Pattern != this.Context.SearchPattern)
            {
                this.Context.SearchPattern = ret.Pattern;
                this.Context.Started = false;
                this.Context.LastResult = -1;
                this.Context.StartIndex = -1;
            }
            return ret;
        }
        FindReplaceEventArgs CreateReplaceArgs()
        {
            FindReplaceEventArgs ret = new FindReplaceEventArgs();
            ret.Pattern = textBoxFindText.Text;
            ret.ReplaceTo = textBoxReplaceText.Text;
            ret.Flags.MatchCase = false;
            ret.Flags.WholeMatch = false;
            if (ret.Pattern != this.Context.SearchPattern)
            {
                this.Context.SearchPattern = ret.Pattern;
                this.Context.Started = false;
                this.Context.LastResult = -1;
                this.Context.StartIndex = -1;
            }
            return ret;
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            ActiveControl = textBoxFindText;
        }
        private void buttonFind_Click(object sender, EventArgs e)
        {
            PerformFind();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (OnReplaceClick != null)
            {
                OnReplaceClick(this, CreateReplaceArgs());
            }
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            if (OnReplaceAllClick != null)
            {
                OnReplaceAllClick(this, CreateReplaceArgs());
            }
        }

        private void buttonSwitchMode_Click(object sender, EventArgs e)
        {
            SetMode(!bReplaceMode);
        }
        public void SetMode(bool replaceMode)
        {
            if (replaceMode)
            {
                splitContainer1.Panel2Collapsed = false;
                buttonSwitchMode.Text = "▲";
                this.splitContainer1.SplitterWidth = 1;
                this.Height = 90;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
                this.splitContainer1.SplitterWidth = 1;
                buttonSwitchMode.Text = "▼";
                this.Height = 60;
            }
            bReplaceMode = replaceMode;
        }
    }
}
