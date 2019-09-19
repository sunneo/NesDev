using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesDev
{
    public class GlobalEvents
    {
        public class AdditionaSaveFileArg
        {
            public DialogResult Result;
        }
        public class SaveFileRequestEventArgs : EventArgs
        {
            public String FileName;
            public bool Cancel;
            public AdditionaSaveFileArg AdditionalArg;
        }
        public static event EventHandler SearchBoxRequired;
        public static event EventHandler ReplaceBoxRequired;
        public static event EventHandler<Interfaces.ICustomizeCommand> CommandLaunchOccurred;
        public static event EventHandler<SaveFileRequestEventArgs> SaveFileOccurred;
        public static event EventHandler<SaveFileRequestEventArgs> CloseFileOccurred;
        public static event EventHandler<Exception> ExceptionOccurred;
        public static void NotifySearchBoxRequired(object sender)
        {
            try
            {
                if (SearchBoxRequired != null)
                {
                    SearchBoxRequired(sender, EventArgs.Empty);
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(sender, ee);
            }
        }
        public static void NotifyReplaceBoxRequired(object sender)
        {
            try
            {
                if (ReplaceBoxRequired != null)
                {
                    ReplaceBoxRequired(sender, EventArgs.Empty);
                }
            }
            catch (Exception ee)
            {
                GlobalEvents.NotifyException(sender, ee);
            }
        }
        public static void NotifyLaunchCommand(object sender, Interfaces.ICustomizeCommand cmd)
        {
            try
            {
                if (CommandLaunchOccurred != null)
                {
                    CommandLaunchOccurred(sender, cmd);
                }
            }
            catch (Exception ex)
            {
                NotifyException(sender, ex);
            }
        }
        public static void NotifyException(object sender, Exception ee)
        {
            try
            {
                if (ExceptionOccurred != null)
                {
                    ExceptionOccurred(sender, ee);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void NotifySaveFile(object sender, SaveFileRequestEventArgs ee, bool showAdditionalArg=false)
        {
            try
            {
                if (SaveFileOccurred != null)
                {
                    if (showAdditionalArg)
                    {
                        if (ee != null)
                        {
                            ee.AdditionalArg = new AdditionaSaveFileArg();
                        }
                    }
                    SaveFileOccurred(sender, ee);
                }
            }
            catch (Exception ex)
            {
                NotifyException(sender ,ex);
            }
        }
        public static void NotifyCloseFile(object sender, SaveFileRequestEventArgs ee, bool showAdditionalArg = false)
        {
            try
            {
                if (CloseFileOccurred != null)
                {
                    if (showAdditionalArg)
                    {
                        if (ee != null)
                        {
                            ee.AdditionalArg = new AdditionaSaveFileArg();
                        }
                    }
                    CloseFileOccurred(sender, ee);
                }
            }
            catch (Exception ex)
            {
                NotifyException(sender, ex);
            }
        }
    }
}
