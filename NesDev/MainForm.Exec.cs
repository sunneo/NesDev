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
        public class ProcessExecResult
        {
            public int ExitCode;
            public String Error;
            public String Output;
            public bool Success
            {
                get
                {
                    return ExitCode == 0;
                }
            }
        }
        public ProcessExecResult Exec(bool verbose, String filename, params String[] argv)
        {
            ProcessExecResult res = new ProcessExecResult();
            using (Process process = new Process())
            {
                if (verbose)
                {
                    AppendToConsole(String.Join(" ", filename, String.Join(" ", argv)), true);
                }
                StringBuilder outbuf = new StringBuilder();
                StringBuilder errbuf = new StringBuilder();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.FileName = Utility.Expander.Expand(filename);
                process.OutputDataReceived += (s, e) =>
                {
                    outbuf.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    errbuf.AppendLine(e.Data);
                };
                if (argv != null && argv.Length > 0)
                {
                    process.StartInfo.Arguments = Utility.Expander.Expand(String.Join(" ", argv));
                }
                if (process.Start())
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                process.WaitForExit();
                res.ExitCode = process.ExitCode;
                res.Output = outbuf.ToString();
                res.Error = errbuf.ToString();
            }
            return res;
        }
    }
}
