using NesDev.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesDev.Config
{
    public class CustomCommand:ICustomizeCommand
    {
        public string FileName
        {
            get;
            set;
        }

        public string Arg
        {
            get;
            set;
        }

        public virtual void Launch()
        {
            GlobalEvents.NotifyLaunchCommand(this, this);
        }


        public string Description
        {
            get;
            set;
        }


        public bool HoldConsole
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }


        public bool GrabOutput
        {
            get;
            set;
        }


        public bool HideWindow
        {
            get;
            set;
        }
    }
}
