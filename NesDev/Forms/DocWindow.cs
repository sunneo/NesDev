using NesDev.Forms.DocWidgets;
using NesDev.Interfaces;
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
    public partial class DocWindow : DockContent, IEditor,IEditorController,IEditorLocator, IEditorUndoRedo
    {
        public const String CppKeywords="__int32 __int64 _tchar bool BOOLEAN byte char CHAR16 CHAR8 decimal double float fpos_t int INT INT16 INT32 INT64 INT8 INTN long LONG sbyte SHELL_STATUS short signed string TCHAR time_t tm uint UINT16 UINT32 UINT64 UINT8 UINTN ulong ULONG unsigned ushort void VOID   WCHAR wchar_t abstract as auto base bool break case catch checked class Complex const continue default delegate do else en  um event explicit extern false finally fixed for foreach goto hello if Imaginary implicit in inline interface internal is lock long namespace   new null object operator out override params private protected public readonly ref register restrict return sealed short sizeof sizeof stackal  loc static STATIC string struct switch this throw true try typedef typeof unchecked union unsafe using virtual volatile while while";
        public const String AsmKeywords = ".export .define .import .include .segment .incbin .endif .if .word .byte .res .fopt .setcpu .smart .autoimport .case .debuginfo on off .importzp .macpack .forceimport .proc .endproc .repeat .endrepeat brk bpl jsr bmi rti bvc rts bvs bcc ldy bcs cpy bne cpx beq ora and eor adc sta lda cmp sbc ldx bit asl rol lsr ror stx ldx dec inc php clc plp sec pha cli pla sei dey tya tay clv iny cld inx sed txa txs tax tsx dex nop jmp sty ldy cpy cpx a b x y sp";

        public String FileName { get; private set; }
        protected virtual void OnUpdateFileName(String filename)
        {
            String ext = Path.GetExtension(filename);
            bool styleWasSet = false;
            if (ext.EndsWith(".bat", StringComparison.CurrentCultureIgnoreCase))
            {
                this.scintilla1.Lexer = ScintillaNET.Lexer.Batch;
                SetupBatchStyle();
                styleWasSet = true;
            }
            else if (ext.Equals(".asm", StringComparison.CurrentCultureIgnoreCase) || ext.Equals(".s", StringComparison.CurrentCultureIgnoreCase))
            {
                this.scintilla1.Lexer = ScintillaNET.Lexer.Asm;
                SetupAsmStyle();
                styleWasSet = true;
            }
            else if ( ext.Equals(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                this.scintilla1.Lexer = ScintillaNET.Lexer.Json;
            }
            else if (ext.Equals(".js", StringComparison.CurrentCultureIgnoreCase))
            {
                this.scintilla1.Lexer = ScintillaNET.Lexer.Cpp;
            }
            if (!styleWasSet)
            {
                SetupCppStyle();
            }
            this.TabText = this.Text = Path.GetFileName(filename);
            this.ToolTipText = filename;
        }
        public void Open(String filename)
        {
            try
            {
                OnUpdateFileName(filename);
                this.scintilla1.Text = File.ReadAllText(filename);
                this.scintilla1.EmptyUndoBuffer();
                this.scintilla1.ReadOnly = false;
                this.IsModified = false;
                
                this.FileName = filename;
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.OnLocationChanged(e);
        }
        public DocWindow()
        {
            InitializeComponent();
            this.Icon = Icon.FromHandle(global::NesDev.Properties.Resources.x_office_document_icon.GetHicon());
        }
        
        private void SetupCppStyle()
        {
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.String].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Regex].ForeColor = Color.Brown;
            
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = Color.MediumPurple;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.UserLiteral].ForeColor = Color.MediumPurple;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.CommentDoc].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.CommentDocKeyword].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.CommentDocKeywordError].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.PreprocessorComment].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.PreprocessorCommentDoc].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = Color.DarkRed;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.StringRaw].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Cpp.Verbatim].ForeColor = Color.BlueViolet;
            this.scintilla1.SetKeywords(0, CppKeywords);
        }
        private void SetupAsmStyle()
        {
            this.scintilla1.Styles[ScintillaNET.Style.Asm.Character].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.String].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.Number].ForeColor = Color.MediumPurple;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.CpuInstruction].ForeColor = Color.DarkBlue;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.ExtInstruction].ForeColor = Color.DarkBlue;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.MathInstruction].ForeColor = Color.DarkBlue;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.Comment].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.CommentBlock].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.CommentDirective].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.Operator].ForeColor = Color.DarkRed;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.DirectiveOperand].ForeColor = Color.DarkBlue;
            this.scintilla1.Styles[ScintillaNET.Style.Asm.Directive].ForeColor = Color.Purple;
            this.scintilla1.SetKeywords(0, AsmKeywords);
        }
        private void SetupBatchStyle()
        {
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Command].ForeColor = Color.Brown;
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Label].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Hide].ForeColor = Color.Pink;
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Word].ForeColor = Color.Blue;
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Comment].ForeColor = Color.DarkGreen;
            this.scintilla1.Styles[ScintillaNET.Style.Batch.Operator].ForeColor = Color.DarkRed;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                this.scintilla1.Margins[0].BackColor = Color.DarkBlue;
                this.scintilla1.Margins[0].Width = 10;
                this.scintilla1.Margins[0].Type = ScintillaNET.MarginType.Symbol;
                this.scintilla1.Margins[0].Sensitive = true;
                

                this.scintilla1.Margins[1].BackColor = Color.LightGray;
                this.scintilla1.Margins[1].Type = ScintillaNET.MarginType.Number;
                this.scintilla1.Margins[1].Width = 50;
                this.scintilla1.Margins[1].Sensitive = false;
                this.scintilla1.Styles[ScintillaNET.Style.LineNumber].BackColor = Color.LightGray;
                this.scintilla1.MarginClick += scintilla1_MarginClick;
                

                this.scintilla1.Styles[ScintillaNET.Style.Default].Font = "Consolas";
                this.scintilla1.Styles[ScintillaNET.Style.Default].Size = 11;
                

                this.scintilla1.Markers[(int)ScintillaNET.MarkerSymbol.Circle].SetForeColor(Color.Red);
                this.scintilla1.Markers[(int)ScintillaNET.MarkerSymbol.Circle].SetBackColor(Color.Yellow);
                this.scintilla1.KeyPress += scintilla1_KeyPress;
                this.scintilla1.KeyUp += scintilla1_KeyUp;
                this.scintilla1.TextChanged += scintilla1_TextChanged;
                this.scintilla1.Technology = ScintillaNET.Technology.DirectWrite;
                
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        void scintilla1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                ShowSearchBox(false);
                if(!String.IsNullOrEmpty(mSearchReplaceDlg.Context.SearchPattern))
                    mSearchReplaceDlg.PerformFind();
            }
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (mSearchReplaceDlg != null)
            {
                mSearchReplaceDlg.Location = PointToScreen(new Point(this.Right - mSearchReplaceDlg.Width, this.Top));
                mSearchReplaceDlg.BringToFront();
            }

        }
        void scintilla1_TextChanged(object sender, EventArgs e)
        {
            this.IsModified = true;
            if (!this.scintilla1.CanUndo)
            {
                this.IsModified = false;
            }
        }
        public void HideSearchDlg()
        {
            if (mSearchReplaceDlg != null && !mSearchReplaceDlg.IsDisposed)
            {
                mSearchReplaceDlg.Hide();
            }
        }
        public void ShowSearchDlg()
        {
            if (mSearchReplaceDlg != null && !mSearchReplaceDlg.IsDisposed)
            {
                mSearchReplaceDlg.Show();
            }
        }
        SearchAndReplace mSearchReplaceDlg = null;
        private void InitSearchWindow()
        {
            mSearchReplaceDlg.OnFindClick += mSearchReplaceDlg_OnFindClick;
            mSearchReplaceDlg.OnReplaceAllClick += mSearchReplaceDlg_OnReplaceAllClick;
            mSearchReplaceDlg.OnReplaceClick += mSearchReplaceDlg_OnReplaceClick;
           
            
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (this.mSearchReplaceDlg != null && !this.mSearchReplaceDlg.IsDisposed)
            {
                this.mSearchReplaceDlg.Close();
            }
        }


        void mSearchReplaceDlg_OnReplaceClick(object sender, SearchAndReplace.FindReplaceEventArgs e)
        {
            
        }

        void mSearchReplaceDlg_OnReplaceAllClick(object sender, SearchAndReplace.FindReplaceEventArgs e)
        {
            
        }

        
        void mSearchReplaceDlg_OnFindClick(object sender, SearchAndReplace.FindEventArgs e)
        {
            this.scintilla1.SearchFlags = ScintillaNET.SearchFlags.None;
            if(e.Flags.MatchCase){
                this.scintilla1.SearchFlags |= ScintillaNET.SearchFlags.MatchCase;
            }
            if(e.Flags.WholeMatch){
                this.scintilla1.SearchFlags |= ScintillaNET.SearchFlags.WholeWord;
            }
            this.scintilla1.SearchFlags |= ScintillaNET.SearchFlags.WordStart;
            this.scintilla1.TargetStart = scintilla1.CurrentPosition;
            if (mSearchReplaceDlg.Context.Started && mSearchReplaceDlg.Context.LastResult == -1)
            {
                this.scintilla1.TargetStart = 0;
            }
            this.scintilla1.TargetEnd = scintilla1.TextLength;

            int idx = this.scintilla1.SearchInTarget(e.Pattern);
            this.mSearchReplaceDlg.Context.Started = true;
            this.mSearchReplaceDlg.Context.StartIndex = this.scintilla1.TargetStart;
            this.mSearchReplaceDlg.Context.LastResult = idx;
            if (idx != -1)
            {
                this.GotoLine(this.scintilla1.LineFromPosition(idx));
                this.scintilla1.SelectionStart = idx;
                this.scintilla1.SelectionEnd = idx + e.Pattern.Length;
            }
            else
            {
                this.scintilla1.SelectionEnd = this.scintilla1.SelectionStart;
            }
        }
        private void ShowSearchBox(bool breplace)
        {
            if (mSearchReplaceDlg == null || mSearchReplaceDlg.IsDisposed)
            {
                mSearchReplaceDlg = new SearchAndReplace();
                InitSearchWindow();
            }
            mSearchReplaceDlg.StartPosition = FormStartPosition.Manual;
            mSearchReplaceDlg.Location = PointToScreen(new Point(this.Right - mSearchReplaceDlg.Width, this.Top));
            if (!mSearchReplaceDlg.Visible)
                mSearchReplaceDlg.Show(this);
            mSearchReplaceDlg.SetMode(breplace);
            if (!String.IsNullOrEmpty(scintilla1.SelectedText))
            {
                mSearchReplaceDlg.SetSearchText(scintilla1.SelectedText);
            }
            mSearchReplaceDlg.BringToFront();
            mSearchReplaceDlg.Focus();
            mSearchReplaceDlg.Select();
            mSearchReplaceDlg.SetFocus();
        }

        void scintilla1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!ModifierKeys.HasFlag(Keys.Control)) return;
            if (e.KeyChar== 0x06) // ctrl+f
            {
                ShowSearchBox(false);
                e.Handled = true;
            }
            else if (e.KeyChar == 0x7) // ctrl+g
            {
                DocWidgets.GotoLine gotoLine = new DocWidgets.GotoLine();
                int max = scintilla1.Lines.Count;
                int cur = scintilla1.CurrentLine + 1;
                gotoLine.SetInfo(cur, max);
                gotoLine.StartPosition = FormStartPosition.CenterParent;
                if (gotoLine.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this.GotoLine(gotoLine.Line - 1);
                }
                e.Handled = true;
            }
            else if (e.KeyChar == 0x8) // ctrl+h
            {
                ShowSearchBox(true);
                e.Handled = true;
            }
            else if(e.KeyChar == 0x13) // ctrl+s
            {
                Save();
                e.Handled = true;
            }
            if (e.KeyChar < 0x20)
            {
                e.Handled = true;
            }
        }



        
        void scintilla1_MarginClick(object sender, ScintillaNET.MarginClickEventArgs e)
        {
            if (e.Margin == 0)
            {
                int line = scintilla1.LineFromPosition(e.Position);

                var marker = this.scintilla1.Lines[line].MarkerGet();
                if (marker == 0)
                {
                    this.scintilla1.Lines[line].MarkerAdd((int)ScintillaNET.MarkerSymbol.Circle);
                }
                else
                {
                    this.scintilla1.Lines[line].MarkerDelete((int)ScintillaNET.MarkerSymbol.Circle);
                }
            }
            
        }

        public void Undo()
        {
            scintilla1.Undo();
        }

        public void Redo()
        {
            scintilla1.Redo();
        }

        public void GotoLine(int line)
        {
            if (line >= scintilla1.Lines.Count) return;
            scintilla1.GotoPosition(scintilla1.Lines[line].Position);
        }

        public void GotoPos(int pos)
        {
            scintilla1.GotoPosition(pos);
        }

        public bool IsModified
        {
            get;
            private set;
        }

        public void Save()
        {
            ScintillaNET.ScintillaReader reader = new ScintillaNET.ScintillaReader(this.scintilla1);
            if (String.IsNullOrEmpty(this.FileName))
            {
                GlobalEvents.SaveFileRequestEventArgs saveFile = new GlobalEvents.SaveFileRequestEventArgs();
                GlobalEvents.NotifySaveFile(this, saveFile);
                if (saveFile.Cancel) return;
                this.FileName = saveFile.FileName;

            }
            File.WriteAllText(this.FileName, reader.ReadToEnd());
            this.IsModified = false;
            OnUpdateFileName(this.FileName);
        }

        public void Reload()
        {
            if (!String.IsNullOrEmpty(this.FileName))
            {
                this.Open(this.FileName);
            }
        }

        public IEditorController Controller
        {
            get { return this; }
        }

        public IEditorUndoRedo UndoRedo
        {
            get { return this; }
        }

        public IEditorLocator Locator
        {
            get { return this; }
        }
    }
}
