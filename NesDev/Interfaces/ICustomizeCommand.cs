using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesDev.Interfaces
{
    public interface ICustomizeCommand
    {
        String Name { get; }
        String FileName { get; }
        String Arg { get;  }
        String Description { get;  }
        bool GrabOutput { get;  }
        bool HoldConsole { get; }
        bool HideWindow { get; }
        void Launch();
    }
}
