using Jint.Native;
using NesDev.Dialogs;
using NesDev.Forms;
using NesDev.Interfaces;
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
        private void ProcessLaunchICustomizeCommand(ICustomizeCommand e)
        {
            Process process = new Process();
            process.StartInfo.FileName = e.FileName;
            process.EnableRaisingEvents = true;
            if (!String.IsNullOrEmpty(e.Arg))
            {
                process.StartInfo.Arguments = e.Arg;
            }
            if (e.HideWindow)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
            }
            if (e.GrabOutput)
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                if (e.HoldConsole)
                {
                    process.Exited += consoleHolderLauncherProcess_Exited;
                    OutputWindow output = GetOutputWindowForCmd(e);
                    if (output == null)
                    {
                        output = new OutputWindow("Output Window - " + e.Name);
                        output.Tag = e;
                    }
                    output.Show(this.dockPanel1, DockState.DockBottom);
                    output.FormClosed += (o, ee) =>
                    {
                        try
                        {
                            if (process != null && !process.HasExited)
                            {
                                process.Kill();
                                process.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalEvents.NotifyException(this, ex);
                        }
                    };
                    process.OutputDataReceived += (o, ee) =>
                    {
                        AppendToConsole(ee.Data, true, output);
                    };
                    process.ErrorDataReceived += (o, ee) =>
                    {
                        AppendToConsole(ee.Data, true, output);
                    };
                }
                else
                {
                    process.OutputDataReceived += (o, ee) =>
                    {
                        AppendToConsole(ee.Data, true);
                    };
                    process.ErrorDataReceived += (o, ee) =>
                    {
                        AppendToConsole(ee.Data, true);
                    };
                }
            }
            else
            {
                process.StartInfo.UseShellExecute = true;
            }
            process.Start();
            if (e.GrabOutput)
            {
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }
        }

        private void JSLauncherCustomizeCommands(ICustomizeCommand cmd)
        {
            Jint.Engine Engine = new Jint.Engine();

            Engine.SetValue("$IDE", this);
            Engine.SetValue("$IDE.file", new JSEngineFile());
            BaseJSEngine engineCtx = new BaseJSEngine();
            JSEngineConsole console = new JSEngineConsole();
            Engine.SetValue("console", console);
            Engine.SetValue("setInterval", new Func<JsValue, int, int>(engineCtx.setInterval));
            Engine.SetValue("clearInterval", new Action<int>(engineCtx.clearInterval));
            Engine.SetValue("setTimeout", new Func<JsValue, int, int>(engineCtx.setInterval));
            Engine.SetValue("clearTimeout", new Action<int>(engineCtx.clearInterval));
            OutputWindow output = GetOutputWindowForCmd(cmd);
            if (output == null)
            {
                output = new OutputWindow("Output Window(js) - " + cmd.Name);
                output.Tag = cmd;
            }
            if (cmd.GrabOutput)
            {
                if (cmd.HoldConsole)
                {

                    output.Show(this.dockPanel1, DockState.DockBottom);
                    console.log = new Action<String>((x) => { AppendToConsole(x, false, output); });
                    console.log = new Action<String>((x) => { AppendToConsole(x, false, output); });
                }
                else
                {
                    console.log = new Action<String>((x) => { AppendToConsole(x, false, null); });
                    console.log = new Action<String>((x) => { AppendToConsole(x, false, null); });
                }
            }
            else if (!cmd.HideWindow)
            {
                output.Show(this);
                console.log = new Action<String>((x) => { AppendToConsole(x, false, output); });
                console.log = new Action<String>((x) => { AppendToConsole(x, false, output); });
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                try
                {
                    Engine.Execute(File.ReadAllText(cmd.FileName));
                }
                catch (Exception ee)
                {
                    GlobalEvents.NotifyException(this, ee);
                }
            }));

        }
        private void UIRefreshLauncherMenuItems()
        {
            Config.CustomCommandList list = Utility.Deserialize<Config.CustomCommandList>(Program.config.LauncherListPath);
            if (list != null && list.List.Count > 0)
            {
                while (commandsToolStripMenuItem.DropDownItems.Count > 2)
                {
                    commandsToolStripMenuItem.DropDownItems.RemoveAt(2);
                }
                foreach (Config.CustomCommand cmd in list.List)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(cmd.Name);
                    item.Click += launcherItem_Click;
                    item.Tag = cmd;
                    commandsToolStripMenuItem.DropDownItems.Add(item);
                }
            }
        }

        void launcherItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item.Tag == null) return;
                if (item.Tag is Config.CustomCommand)
                {
                    Config.CustomCommand cmd = item.Tag as Config.CustomCommand;
                    cmd.Launch();
                }
            }
        }

    }
}
