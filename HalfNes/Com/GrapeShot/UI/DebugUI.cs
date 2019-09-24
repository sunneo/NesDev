using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HalfNes.Com.GrapeShot.UI
{
    public class DebugUI : Form
    {
        // StrokeInformer aStrokeInformer = new StrokeInformer();

        private ShowFrame fbuf;
        private int xsize, ysize;
        
        public DebugUI(int height, int width)
        {
            this.xsize = height;
            this.ysize = width;
            fbuf = new ShowFrame(xsize, ysize);
        }

        public void run()
        {
            this.Text = ("HalfNES  Debug " + NES.VERSION);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            fbuf.Dock = DockStyle.Fill;
            this.Controls.Add(fbuf);
            this.Show();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void messageBox(String s)
        {
            MessageBox.Show(s);
        }

        public void setFrame(Bitmap b)
        {
            fbuf.nextFrame = b;

            fbuf.Invalidate();
        }


        public class ShowFrame : Panel
        {
            int xsize, ysize;
            public Bitmap nextFrame;

            /**
             *
             */
            public ShowFrame(int xsize, int ysize)
            {
                this.xsize = xsize;
                this.ysize = ysize;
                this.DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.DrawImage(nextFrame, 0, 0, xsize, ysize);
            }

        }
    }


}
