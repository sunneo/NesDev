using HalfNes.Com.GrapeShot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes
{
    public class NES
    {
        public void messageBox(String s)
        {
            System.Windows.Forms.MessageBox.Show(s);
        }
        internal ControllerInterface getcontroller1()
        {
            throw new NotImplementedException();
        }

        internal ControllerInterface getcontroller2()
        {
            throw new NotImplementedException();
        }

        internal bool isFrameLimiterOn()
        {
            throw new NotImplementedException();
        }

        public static string VERSION { get; set; }
    }
}
