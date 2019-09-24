using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    public abstract class Timer
    {
        protected int period;
        protected int position;

        public int getperiod()
        {
            return period;
        }

        public abstract void setperiod(int newperiod);

        public abstract void setduty(int duty);

        public abstract void setduty(int[] duty);

        public abstract void reset();

        public abstract void clock();

        public abstract void clock(int cycles);

        public abstract int getval();
    }
}
