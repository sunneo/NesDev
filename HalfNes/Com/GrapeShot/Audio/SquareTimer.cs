using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    class SquareTimer : Timer
    {
        protected int[] values;
        private int periodadd;
        private int divider = 0;


        public override void clock()
        {
            if (period + periodadd <= 0)
            {
                return;
            }
            ++divider;
            // note: stay away from negative division to avoid rounding problems
            int periods = (divider + period + periodadd) / (period + periodadd);
            if (periods < 0)
            {
                periods = 0; // can happen if period or periodadd were made smaller
            }
            position = (position + periods) % values.Length;
            divider -= (period + periodadd) * periods;
        }


        public override void clock(int cycles)
        {
            if (period < 8)
            {
                return;
            }
            divider += cycles;
            // note: stay away from negative division to avoid rounding problems
            int periods = (divider + period + periodadd) / (period + periodadd);
            if (periods < 0)
            {
                periods = 0; // can happen if period or periodadd were made smaller
            }
            position = (position + periods) % values.Length;
            divider -= (period + periodadd) * periods;
        }

        public SquareTimer(int ctrlen, int periodadd)
        {
            this.periodadd = periodadd;
            values = new int[ctrlen];
            period = 0;
            position = 0;
            setduty(ctrlen / 2);
        }

        public SquareTimer(int ctrlen)
        {
            this.periodadd = 0;
            values = new int[ctrlen];
            period = 0;
            position = 0;
            setduty(ctrlen / 2);
        }


        public override void reset()
        {
            position = 0;
        }


        public override void setduty(int duty)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = (i < duty) ? 1 : 0;
            }
        }


        public override void setduty(int[] dutyarray)
        {
            values = dutyarray;
        }


        public override int getval()
        {
            return values[position];
        }


        public override void setperiod(int newperiod)
        {
            period = newperiod;
        }
    }
}
