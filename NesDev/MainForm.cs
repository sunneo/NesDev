using Interfaces;
using Jint.Native;
using NesDev.Dialogs;
using NesDev.Forms;
using Interfaces;
using Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NesDev
{
    public partial class MainForm : Form
    {
        OpenFileDialog open = new OpenFileDialog() { Filter = "Supported Files|*.c;*.h;*.s;*.asm;*.bat;makefile;*.txt;*.mk;*.js;*.nes|Any File|*.*", FilterIndex = 1, Title = "Open File" };
        SaveFileDialog save = new SaveFileDialog() { Filter = "Supported Files|*.c;*.h;*.s;*.asm;*.bat;makefile;*.txt;*.mk;*.js|Any File|*.*", FilterIndex = 1, Title = "Save File" };
        WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme theme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
        public Dictionary<String, DockContent> FileNameMap = new Dictionary<string, DockContent>();
        OutputWindow outputWindow = new OutputWindow();
        public DocWindow CurrentDoc
        {
            get
            {
                if (dockPanel1.ActiveContent != null && dockPanel1.ActiveContent is DocWindow)
                {
                    return dockPanel1.ActiveContent as DocWindow;
                }
                if (dockPanel1.ActiveDocument != null && dockPanel1.ActiveDocument is DocWindow)
                {
                    return dockPanel1.ActiveDocument as DocWindow;
                }
                return null;
            }
        }
        

        public MainForm()
        {
            InitializeComponent();
            this.dockPanel1.Theme = theme;
            this.dockPanel1.ShowDocumentIcon = true;
            this.Icon = Icon.FromHandle(global::NesDev.Properties.Resources.Nintendo_Famicom.GetHicon());
            GlobalEvents.SaveFileOccurred += GlobalEvents_SaveFileOccurred;
            GlobalEvents.ExceptionOccurred += GlobalEvents_ExceptionOccurred;
            GlobalEvents.CommandLaunchOccurred += GlobalEvents_CommandLaunchOccurred;
        }
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UIRefreshLauncherMenuItems();
        }

        void GlobalEvents_CommandLaunchOccurred(object sender, ICustomizeCommand e)
        {
            if (String.IsNullOrEmpty(e.FileName)) return;
            String ext = Path.GetExtension(e.FileName);
            if (ext.Equals(".js", StringComparison.CurrentCultureIgnoreCase))
            {
                JSLauncherCustomizeCommands(e);
            }
            else
            {
                ProcessLaunchICustomizeCommand(e);
            }
        }

        void consoleHolderLauncherProcess_Exited(object sender, EventArgs e)
        {
            
        }
        private void AppendToConsole(String msg, bool forceShow=false, OutputWindow outputWindow=null)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<String, bool, OutputWindow>(AppendToConsole), msg, forceShow, outputWindow);
                return;
            }
            if (outputWindow == null)
            {
                outputWindow = this.outputWindow;
            }
            try
            {
                if (outputWindow == null || outputWindow.IsDisposed)
                {
                    outputWindow = new OutputWindow();
                    outputWindow.CreateControl();
                }
                outputWindow.AppendLine(msg);
                if (forceShow)
                {
                    outputWindow.Show(this.dockPanel1,  DockState.DockBottom);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }
        void GlobalEvents_ExceptionOccurred(object sender, Exception e)
        {
            try
            {
                if (outputWindow != null && !outputWindow.IsDisposed)
                {
                    outputWindow.AppendLine(e.ToString());
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        void GlobalEvents_SaveFileOccurred(object sender, GlobalEvents.SaveFileRequestEventArgs e)
        {
            if (sender is IEditor)
            {
                IEditor editor = sender as IEditor;
                if (editor == null) return;
                if (editor.Controller == null) return;
                if (String.IsNullOrEmpty(editor.Controller.FileName))
                {
                    if (save.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.FileName = save.FileName;
                    }
                }
            }
        }
        public void UIOpenFile(String filename)
        {
            try
            {
                DockContent window = null;
                if (FileNameMap.ContainsKey(filename))
                {
                    window = FileNameMap[filename];
                    if (!window.IsDisposed)
                    {
                        window.Show();
                        return;
                    }
                }
                String ext = Path.GetExtension(filename);
                if (ext.Equals(".nes", StringComparison.CurrentCultureIgnoreCase))
                {
                    window = new DockContent();
                    
                    window.TabText = window.Text = Path.GetFileName(filename);
                    window.ToolTipText = filename;
                    window.BackColor = Color.Black;
                    SharpNes.GamePanel gamePanel = new SharpNes.GamePanel();
                    window.KeyPreview =  true;
                    gamePanel.Dock = DockStyle.Fill;
                    window.Controls.Add(gamePanel);
                    gamePanel.OnDisassemblerShown += MainForm_OnDisassemblerShown;
                    window.Tag = gamePanel;
                    gamePanel.Tag = filename;
                    window.FormClosed += docwindow_FormClosed;
                    this.BeginInvoke(new Action(() =>
                    {
                        gamePanel.Open(filename);
                        gamePanel.Focus();
                    }));
                }
                else
                {
                    window = new Forms.DocWindow();
                    window.Tag = filename;
                    window.FormClosed += docwindow_FormClosed;
                    window.FormClosing += window_FormClosing;
                    this.BeginInvoke(new Action(() =>
                    {
                        (window as Forms.DocWindow).Open(filename);
                    }));
                }
                FileNameMap[filename] = window;
                window.Show(this.dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }

        void MainForm_OnDisassemblerShown(object sender, SharpNes.Diagnostics.CodeDisassemblyForm e)
        {
            e.Show(this.dockPanel1, DockState.DockRight);
        }

        void window_FormClosing(object sender, FormClosingEventArgs e)
        {
            Forms.DocWindow doc = sender as Forms.DocWindow;
            try
            {
                if (doc == null) return;
                if (!BeforeClose(doc))
                {
                    e.Cancel = true;
                    return;
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
           
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                UIOpenFile(open.FileName);
            }
        }

        void docwindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            DockContent doc = sender as DockContent;
            try
            {
                if (doc.Tag == null) return;

                if (doc.Tag != null && !String.IsNullOrEmpty(doc.Tag.ToString()))
                {
                    FileNameMap.Remove(doc.Tag.ToString());
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
            
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dockPanel1.ActiveContent != null)
                {
                    dockPanel1.ActiveContent.DockHandler.Close();
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(this, ee);
            }
        }
        public bool BeforeClose(DockContent content)
        {
            if (content is IEditor)
            {
                IEditor editor = content as IEditor;
                if (editor.Controller != null)
                {
                    if (editor.Controller.IsModified)
                    {
                        DialogResult res = AskToSave(content);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            editor.Controller.Save();
                            return true;
                        }
                        else if (res == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            return true;
        }
        DialogResult AskToSave(DockContent content)
        {
            if (content is IEditor)
            {
                IEditor editor = content as IEditor;
                if (editor.Controller != null)
                {
                    String filename = "*UNNAMED*";
                    if (!String.IsNullOrEmpty(editor.Controller.FileName))
                    {
                        filename = editor.Controller.FileName;
                    }
                    return MessageBox.Show(this, "File " + filename + " Has been modified, save change before closing?", "Closing File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }
            }
            return System.Windows.Forms.DialogResult.No;
        }
        public bool BeforeClose()
        {
            foreach (DockContent content in dockPanel1.Contents)
            {
                if (content.IsDisposed) continue;
                if (!BeforeClose(content)) return false;
            }
            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!BeforeClose())
            {
                e.Cancel = true;
                return;
            }
            Utility.Serialize(Program.AppConfigName, Program.config);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void viewOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outputWindow.IsDisposed)
            {
                outputWindow = new OutputWindow();
            }
            if (outputWindow.DockState == DockState.Unknown)
            {
                outputWindow.Show(this.dockPanel1, DockState.DockBottom);
            }
            else
            {
                outputWindow.Show();
            }
        }

        private void memoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void debugRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void debugDisassemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel1.ActiveDocument != null && ((DockContent)dockPanel1.ActiveDocument).Tag is SharpNes.GamePanel)
            {
                (((DockContent)dockPanel1.ActiveDocument).Tag as SharpNes.GamePanel).ShowDisassembler();
            }
        }

        private void startDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PerformBuild())
            {
                String nes = GuessCurrentNesFileName();
                if (!String.IsNullOrEmpty(nes))
                {
                    UIOpenFile(nes);
                }
            }
        }

        public String GuessCurrentNesFileName()
        {
            if (dockPanel1.ActiveDocument != null && dockPanel1.ActiveDocument is DocWindow)
            {
                DocWindow window = dockPanel1.ActiveDocument as DocWindow;
                String filename = window.Tag as String;
                String fileNameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                String ext = Path.GetExtension(filename);
                String dir = Path.GetDirectoryName(filename);
                String outputFileName = Path.Combine(dir, fileNameWithoutExt + ".nes");
                if (File.Exists(outputFileName))
                {
                    return outputFileName;
                }
            }
            return "";
        }

        private void startWithoutDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String nes = GuessCurrentNesFileName();
            if (!String.IsNullOrEmpty(nes))
            {
                UIOpenFile(nes);
            }
        }

        private void debugStepOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel1.ActiveDocument != null && ((DockContent)dockPanel1.ActiveDocument).Tag is SharpNes.GamePanel)
            {
                (((DockContent)dockPanel1.ActiveDocument).Tag as SharpNes.GamePanel).StepOver();
            }
        }

        private void debugPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel1.ActiveDocument != null && ((DockContent)dockPanel1.ActiveDocument).Tag is SharpNes.GamePanel)
            {
                (((DockContent)dockPanel1.ActiveDocument).Tag as SharpNes.GamePanel).Pause();
            }
        }

        private void fileSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingDialog setting = new SettingDialog();
            setting.Load(Program.config);
            if (setting.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setting.Save(Program.config);
                Utility.Serialize(Program.AppConfigName, Program.config);
            }
        }

        private void manageCommandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageCommands dialog = new ManageCommands();
            if(dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // update menu items
                UIRefreshLauncherMenuItems();
            }
        }
        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocWindow doc = new DocWindow();
            doc.TabText = "UNNAMED";
            doc.Show(this.dockPanel1, DockState.Document);
        }

        public bool PerformBuild()
        {
            if (dockPanel1.ActiveDocument != null && dockPanel1.ActiveDocument is DocWindow)
            {

                DocWindow window = dockPanel1.ActiveDocument as DocWindow;
                String filename = window.Tag as String;

                AppendToConsole("Building " + filename, true);
                String fileNameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                String ext = Path.GetExtension(filename);
                String dir = Path.GetDirectoryName(filename);
                StringBuilder asmincBuilder = new StringBuilder();
                StringBuilder ccincBuilder = new StringBuilder();
                String outputFileName = Path.Combine(dir, fileNameWithoutExt + ".nes");
                foreach (String s in Program.config.Configure.ASMINC)
                {
                    if (String.IsNullOrEmpty(s)) continue;
                    asmincBuilder.Append("-I " + "\"" + s + "\" ");
                }
                asmincBuilder.Append("-I " + "\"" + dir + "\" ");
                foreach (String s in Program.config.Configure.CCINC)
                {
                    if (String.IsNullOrEmpty(s)) continue;
                    ccincBuilder.Append("-I " + "\"" + s + "\" ");
                }
                ccincBuilder.Append("-I " + "\"" + dir + "\" ");
                String asminc = asmincBuilder.ToString();
                String ccinc = ccincBuilder.ToString();
                String assemblyOutput = "";
                String crt0Src = "";
                if (!String.IsNullOrEmpty(Program.config.Configure.CRTAssembly))
                {
                    crt0Src = "\"" + Program.config.Configure.CRTAssembly + "\"";
                }
                else
                {
                    crt0Src = "\"" + Path.Combine(dir, "crt0.s") + "\"";
                }
                String crt0 = "\"" + Path.Combine(dir, "crt0.o") + "\"";
                String objectOutput = "\"" + Path.Combine(dir, fileNameWithoutExt + ".o") + "\"";
                String runtimeLib = "";
                if (!String.IsNullOrEmpty(Program.config.Configure.RuntimeLib))
                {
                    runtimeLib = "\"" + Program.config.Configure.RuntimeLib + "\"";
                }
                else
                {
                    runtimeLib = "\"" + Path.Combine(dir, "runtime.lib") + "\"";
                }

                ProcessExecResult res = null;

                // Compile Crt0
                res = Exec(true, Program.config.Configure.CA, asminc, Program.config.Configure.ASMFLAGS, crt0Src, "-o", "\"" + Path.Combine(dir, "crt0.o") + "\"");

                if (!res.Success)
                {
                    AppendToConsole(res.Error);
                    AppendToConsole("Program Exit With Code " + res.ExitCode);
                    return false;
                }

                if (ext.Equals(".c", StringComparison.CurrentCultureIgnoreCase))
                {
                    // Compile C
                    {
                        assemblyOutput = Path.Combine(dir, fileNameWithoutExt + ".s");
                        res = Exec(true, Program.config.Configure.CC, ccinc, Program.config.Configure.CCFLAGS, "\"" + filename + "\"", "-o", "\"" + assemblyOutput + "\"");

                        if (!res.Success)
                        {
                            AppendToConsole(res.Error);
                            AppendToConsole("Program Exit With Code " + res.ExitCode);
                            return false;
                        }
                    }
                }
                else
                {
                    assemblyOutput = filename;
                }

                // Compile asm
                res = Exec(true, Program.config.Configure.CA, asminc, Program.config.Configure.ASMFLAGS, "\"" + assemblyOutput + "\"", "-o", objectOutput);
                if (!res.Success)
                {
                    AppendToConsole(res.Error);
                    AppendToConsole("Program Exit With Code " + res.ExitCode);
                    return false;
                }

                // Link
                res = Exec(true, Program.config.Configure.LD, "-C", "\"" + Program.config.Configure.LDCFG + "\"", crt0, objectOutput, runtimeLib, "-o", outputFileName);
                if (!res.Success)
                {
                    AppendToConsole(res.Error);
                    AppendToConsole("Program Exit With Code " + res.ExitCode);
                    return false;
                }
                AppendToConsole("Building " + filename + " Success", true);
                AppendToConsole("", true);
            }
            return true;
        }
        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformBuild();
        }
    }
}
