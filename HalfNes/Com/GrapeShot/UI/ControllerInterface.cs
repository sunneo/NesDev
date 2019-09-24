using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.UI
{
    public interface ControllerInterface
    {

        void strobe();

        void output(bool state);

        int peekOutput();

        int getbyte();
    }

}
