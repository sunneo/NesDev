using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesCore.Video
{
    public class Colour
    {
        public byte[] bits=new byte[3];
        public Colour(byte red, byte green, byte blue)
        {
            bits[0] = blue;
            bits[1] = green;
            bits[2] = red;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red
        {
            get
            {
                return bits[2];
            }
            private set
            {
                bits[2] = value;
            }
        }
        public byte Green
        {
            get
            {
                return bits[1];
            }
            private set
            {
                bits[1] = value;
            }
        }
        public byte Blue
        {
            get
            {
                return bits[0];
            }
            private set
            {
                bits[0] = value;
            }
        }

        public static readonly Colour Black = new Colour(0, 0, 0);
    }
}
