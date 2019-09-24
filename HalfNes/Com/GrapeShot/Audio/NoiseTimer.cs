using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    class NoiseTimer : Timer
    {
        private int divider = 0;
        private int[] values = genvalues(1, 1);
        private int prevduty = 1;
        private static int periodadd = 0;

        public NoiseTimer()
        {
            period = 0;
        }


        public override void setduty(int duty)
        {
            if (duty != prevduty)
            {
                values = genvalues(duty, values[position]);
                position = 0;
            }
            prevduty = duty;
        }


        public override void clock()
        {
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


        public override int getval()
        {
            return (values[position] & 1);
        }


        public override void reset()
        {
            position = 0;
        }


        public override void clock(int cycles)
        {
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


        public override void setperiod(int newperiod)
        {
            period = newperiod;
        }

        public static int[] genvalues(int whichbit, int seed)
        {
            int[] tehsuck = new int[(whichbit == 1) ? 32767 : 93];
            for (int i = 0; i < tehsuck.Length; ++i)
            {
                seed = (seed >> 1)
                        | ((((seed & (1 << whichbit)) != 0)
                        ^ ((seed & (utils.BIT0)) != 0))
                        ? 16384 : 0);
                tehsuck[i] = seed;
            }
            return tehsuck;

        }


        public override void setduty(int[] duty)
        {
            throw new Exception("Not supported on noise channel.");
        }
    }
}
