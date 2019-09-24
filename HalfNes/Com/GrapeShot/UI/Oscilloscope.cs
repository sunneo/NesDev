using HalfNes.Com.GrapeShot.Audio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.UI
{
    public class Oscilloscope : AudioOutInterface
    {
        private static int width = 400, length = 640;
        private static int scf = 65536 / width / 2;
        DebugUI d;
        Bitmap b;
        Graphics g;
        AudioOutInterface iface;
        int[] buffer = new int[length];
        int buf_ptr = 0;
        int prevsample = 0;

        Color bgclr = Color.Black;
        Color clr = Color.Green;
        public Oscilloscope(AudioOutInterface i)
        {
            this.iface = i;
            d = new DebugUI(length, width);
            b = new Bitmap(length, width,  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(b);
            bgclr = Color.Black;
            clr = Color.Green;
            d.run();
        }

        public Oscilloscope()
        {
            this.iface = null;
            d = new DebugUI(length, width);
            b = new Bitmap(length, width, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(b);
            bgclr = Color.Black;
            clr = Color.Red;
            d.run();
        }


        public void outputSample(int sample)
        {
            if (buf_ptr > 0
                    || (prevsample <= 0 && sample >= 0))
            {
                //start cap @ zero crossing
                if (buf_ptr < buffer.Length)
                {
                    buffer[buf_ptr++] = sample;
                }
            }
            prevsample = sample;
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
            using (var brush = new SolidBrush(this.bgclr))
            {
                g.FillRectangle(brush, 0, 0, length, width);
            }
            using (var pen = new Pen(this.clr))
            {
                for (int i = 1; i < buf_ptr; ++i)
                {
                    g.DrawLine(pen, i - 1, (buffer[i - 1] / scf) + width / 2, i, (buffer[i] / scf) + width / 2);
                }
                g.DrawLine(pen, 0, width / 2, length, width / 2);
            }
            d.setFrame(b);
            buf_ptr = 0;

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
            d.Hide();
            d.Dispose();
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
