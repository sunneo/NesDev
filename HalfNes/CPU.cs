using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes
{
    public class CPU
    {
        public int clocks { get; set; }

        public int interrupt { get; set; }

        internal void stealcycles(int p)
        {
            throw new NotImplementedException();
        }
    }
}
