using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    public class Reverberator : AudioOutInterface
    {
        AudioOutInterface iface;
        CircularBuffer cb;
        double echo, lp_coef, hp_coef;

        public Reverberator(AudioOutInterface i, int length, double echo_gain, double lp_coef, double hp_coef)
        {
            this.echo = echo_gain;
            this.lp_coef = lp_coef;
            this.hp_coef = hp_coef;
            iface = i;
            cb = new CircularBuffer(length);
        }
        int lpaccum = 0;

        private int lowpass_filter(int sample)
        {
            sample += lpaccum;
            lpaccum -= (int)(sample * lp_coef);
            return lpaccum;
        }
        private int dckiller = 0;

        private int highpass_filter(int sample)
        {
            //for killing the dc in the signal
            sample += dckiller;
            dckiller -= (int)(sample * hp_coef);//the actual high pass part
            dckiller += (sample > 0 ? -1 : 1);//guarantees the signal decays to exactly zero
            return sample;
        }


        public void outputSample(int sample)
        {
            sample -= (int)(cb.read() * echo);
            if (sample < -32768)
            {
                sample = -32768;
                //System.err.println("clip");
            }
            if (sample > 32767)
            {
                sample = 32767;
                //System.err.println("clop");
            }
            cb.write(lowpass_filter(highpass_filter(sample)));
            if (!(iface == null))
            {
                iface.outputSample(sample);
            }
        }


        public void flushFrame(bool waitIfBufferFull)
        {
            if (!(iface == null))
            {
                iface.flushFrame(waitIfBufferFull);
            }

        }


        public void pause()
        {
            if (!(iface == null))
            {
                iface.pause();
            }
        }


        public void resume()
        {
            if (!(iface == null))
            {
                iface.resume();
            }
        }


        public void destroy()
        {
            if (!(iface == null))
            {
                iface.destroy();
            }
        }


        public bool bufferHasLessThan(int samples)
        {
            if (!(iface == null))
            {
                return iface.bufferHasLessThan(samples);
            }
            else
            {
                return false;
            }
        }
    }
}
