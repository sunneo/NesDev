using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot
{
    public class utils
    {
        
        private utils() { }

        public static int BIT0 = 1, BIT1 = 2, BIT2 = 4, BIT3 = 8, BIT4 = 16,
                BIT5 = 32, BIT6 = 64, BIT7 = 128, BIT8 = 256, BIT9 = 512,
                BIT10 = 1024, BIT11 = 2048, BIT12 = 4096, BIT13 = 8192,
                BIT14 = 16384, BIT15 = 32768;

        public static int setbit(int num, int bitnum, bool state)
        {
            return (state) ? (num | (1 << bitnum)) : (num & ~(1 << bitnum));
        }

        public static String hex(int num)
        {
            String s = num.ToString("X").ToUpper();
            if ((s.Length & 1) == 1)
            {
                s = "0" + s;
            }
            return s;
        }

        public static String hex(long num)
        {
            String s = num.ToString("X").ToUpper();
            if ((s.Length & 1) == 1)
            {
                s = "0" + s;
            }
            return s;
        }
        public static uint ReverseBits(uint n)
        {
            n = (n >> 1) & 0x55555555 | (n << 1) & 0xaaaaaaaa;
            n = (n >> 2) & 0x33333333 | (n << 2) & 0xcccccccc;
            n = (n >> 4) & 0x0f0f0f0f | (n << 4) & 0xf0f0f0f0;
            n = (n >> 8) & 0x00ff00ff | (n << 8) & 0xff00ff00;
            n = (n >> 16) & 0x0000ffff | (n << 16) & 0xffff0000;
            return n;
        }
        public static int reverseByte(int nibble)
        {
            //reverses 8 bits packed into int.
            
            return (int)((ReverseBits(unchecked((uint)nibble)) >> 24) & 0xff);
        }

        public static void printarray<T>(T[] a)
        {
            StringBuilder s = new StringBuilder();
            foreach (T i in a)
            {
                s.Append(i);
                s.Append(", ");
            }
            if (s.Length >= 1)
            {
                s.Remove(s.Length - 1, 1);
            }
            s.Append("\n");
            Console.Error.WriteLine(s.ToString());
        }



        public static int max(int[] array)
        {
            int m = array[0];
            foreach (int i in array)
            {
                if (i > m)
                {
                    m = i;
                }
            }
            return m;
        }
    }
    public class Arrays
    {
        public static void fill<T>(T[] arr, T val)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = val;
            }
        }
    }
}
