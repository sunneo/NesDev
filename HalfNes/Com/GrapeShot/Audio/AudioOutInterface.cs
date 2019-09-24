using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    public interface AudioOutInterface
    {

        void outputSample(int sample);

        void flushFrame(bool waitIfBufferFull);

        void pause();

        void resume();

        void destroy();

        bool bufferHasLessThan(int samples);
    }
}
