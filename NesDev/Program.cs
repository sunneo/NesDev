using NesDev.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev
{
    public static class Program
    {
        public const String AppConfigName = "appconf.json";
        public static MainForm mainform;
        public static AppConfig config;
        public class ExpanderImplement:Interfaces.IMacroExpander
        {
            public string Expand(string input)
            {
                StringBuilder strb = new StringBuilder();
                int i = 0;
                while (i <= input.Length - 1)
                {
                    int pos = input.IndexOf("$(", i);
                    if (pos == -1)
                    {
                        String substr = input.Substring(i);
                        strb.Append(substr);
                        break;
                    }
                    else
                    {
                        strb.Append(input.Substring(i,pos-i));
                        i = pos + 2;
                        int epos = input.IndexOf(")", pos);
                        if (epos >= 0)
                        {
                            String macro = input.Substring(pos, epos - pos + 1).Trim('$', '(', ')');
                            switch (macro)
                            {
                                case "CC":
                                    strb.Append(config.Configure.CC);
                                    break;
                                case "CA":
                                    strb.Append(config.Configure.CA);
                                    break;
                                case "ASMINC":
                                    strb.Append(String.Join(" -I", config.Configure.ASMINC));
                                    break;
                                case "CCINC":
                                    strb.Append(String.Join(" -I", config.Configure.CCINC));
                                    break;
                                case "CCFLAGS":
                                    strb.Append(config.Configure.CCFLAGS);
                                    break;
                                case "ASMFLAGS":
                                    strb.Append(config.Configure.ASMFLAGS);
                                    break;
                                case "LD":
                                    strb.Append(config.Configure.LD);
                                    break;
                                case "LDCFG":
                                    strb.Append(config.Configure.LDCFG);
                                    break;
                                case "CURDOC":
                                    {
                                        var doc = mainform.CurrentDoc;
                                        if (doc != null)
                                        {
                                            strb.Append(doc.FileName);
                                        }
                                    }
                                    break;
                                case "CURDOCNAME":
                                    {
                                        var doc = mainform.CurrentDoc;
                                        if (doc != null)
                                        {
                                            strb.Append(Path.GetFileNameWithoutExtension(doc.FileName));
                                        }
                                    }
                                    break;
                            }
                            i = epos + 1;
                        }
                       
                    }
                    
                }
                return strb.ToString();
            }
        }
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Control.CheckForIllegalCrossThreadCalls = false;
            config = Utility.Deserialize<AppConfig>(AppConfigName);
            if (config == null)
            {
                config = new AppConfig();
            }
            String nesSdk = Path.Combine(Application.StartupPath, "sdk");
            if (Directory.Exists(nesSdk))
            {
                if (String.IsNullOrEmpty(config.Configure.CC))
                {
                    config.Configure.CC = Path.Combine(nesSdk, "tools", "bin", "cc65.exe");
                }
                if (String.IsNullOrEmpty(config.Configure.CA))
                {
                    config.Configure.CA = Path.Combine(nesSdk, "tools", "bin", "ca65.exe");
                }
                if (String.IsNullOrEmpty(config.Configure.LD))
                {
                    config.Configure.LD = Path.Combine(nesSdk, "tools", "bin", "ld65.exe");
                }
                if (config.Configure.ASMINC.Count == 0)
                {
                    config.Configure.ASMINC.Add( Path.Combine(nesSdk, "tools","asminc"));
                    config.Configure.ASMINC.Add(Path.Combine(nesSdk, "tools", "crt"));
                }
                if (config.Configure.CCINC.Count == 0)
                {
                    config.Configure.CCINC.Add(Path.Combine(nesSdk, "tools", "include"));
                    config.Configure.CCINC.Add(Path.Combine(nesSdk, "tools", "crt"));
                }
                if (String.IsNullOrEmpty(config.Configure.CCFLAGS))
                {
                    config.Configure.CCFLAGS = "-Oi --add-source";
                }
                if (String.IsNullOrEmpty(config.Configure.LDCFG))
                {
                    config.Configure.LDCFG = Path.Combine(nesSdk, "tools", "crt", "nrom_128_horz.cfg");
                }
                if (String.IsNullOrEmpty(config.Configure.CRTAssembly))
                {
                    config.Configure.CRTAssembly = Path.Combine(nesSdk, "tools", "crt", "crt0.s");
                }
                if (String.IsNullOrEmpty(config.Configure.RuntimeLib))
                {
                    config.Configure.RuntimeLib = Path.Combine(nesSdk, "tools", "crt", "runtime.lib");
                }
            }
            Utility.Expander = new ExpanderImplement();
            Program.mainform = new MainForm();
            Application.Run(Program.mainform);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            
        }
    }
}
