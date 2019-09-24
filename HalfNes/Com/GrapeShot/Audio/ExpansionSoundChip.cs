using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    public interface ExpansionSoundChip
    {

         void clock(int cycles);

         void write(int register, int data);

         int getval();
    }

}
