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
        public class BaseJSEngine
        {
            public Dictionary<int, BackgroundWorker> intervaljobs = new Dictionary<int, BackgroundWorker>();
            public Dictionary<int, BackgroundWorker> timeoutjobs = new Dictionary<int, BackgroundWorker>();
            public int intervalVal = 0;
            public int timeoutVal = 0;
            public int setInterval(JsValue fnc, int interval)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker.WorkerReportsProgress = true;
                worker.DoWork += (s, e) =>
                {
                    while (!worker.CancellationPending)
                    {
                        Thread.Sleep(interval);
                        fnc.Invoke();
                    }
                };
                worker.RunWorkerAsync();
                int ret = intervalVal;
                ++intervalVal;
                intervaljobs[ret] = worker;
                return ret;
            }
            public void clearInterval(int handle)
            {
                if (intervaljobs.ContainsKey(handle))
                {
                    BackgroundWorker worker = intervaljobs[handle];
                    worker.CancelAsync();
                    intervaljobs.Remove(handle);
                }
            }
            public int setTimeout(JsValue fnc, int interval)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker.WorkerReportsProgress = true;
                worker.DoWork += (s, e) =>
                {
                    Thread.Sleep(interval);
                    if (!worker.CancellationPending)
                    {
                        fnc.Invoke();
                    }
                };
                worker.RunWorkerAsync();
                int ret = timeoutVal;
                ++timeoutVal;
                timeoutjobs[ret] = worker;
                return ret;
            }
            public void clearTimeout(int handle)
            {
                if (timeoutjobs.ContainsKey(handle))
                {
                    BackgroundWorker worker = timeoutjobs[handle];
                    worker.CancelAsync();
                    timeoutjobs.Remove(handle);
                }
            }
        }
        public class JSEngineConsole
        {
            public Action<String> log;
            public Action<String> dir;
        }
        public class JSEngineFile
        {
            public void write(String path, String content)
            {
                File.WriteAllText(path, content);
            }
            public String read(String path)
            {
                return File.ReadAllText(path);
            }
        }
        public String ENV(String env)
        {
            return Utility.Expander.Expand("$(" + env + ")");
        }

        private OutputWindow GetOutputWindowForCmd(ICustomizeCommand cmd)
        {
            foreach (DockContent content in dockPanel1.Contents)
            {
                if (content is OutputWindow && !content.IsDisposed)
                {
                    if (content.Tag == cmd)
                    {
                        return (OutputWindow)content;
                    }
                }
            }
            return null;
        }
    }
}
