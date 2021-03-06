﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpNes
{
    public partial class SDLMMControl : UserControl, IRenderer
    {
        public interface SDLMMEventListener
        {
            event EventHandler<SDLMMControl> ScreenPainted;
            event EventHandler<Point> ScreenMouseDown;
            event EventHandler<Point> ScreenMouseUp;
            event EventHandler<Point> ScreenMouseMove;
            Size Resolution { get; set; }
            void Invalidate();
        }
        public class PointHitTestClass
        {
            Point currentPt;
            public PointHitTestClass(Point pt)
            {
                this.currentPt = pt;
            }
            public bool IsHitCircle(Point center, int r, bool onlyBorder = false)
            {
                double deltaX = currentPt.X - center.X;
                double deltaY = currentPt.Y - center.Y;
                if (onlyBorder)
                {
                    return deltaX * deltaX + deltaY * deltaY == r * r;
                }
                else
                {
                    return deltaX * deltaX + deltaY * deltaY <= r * r;
                }
            }
            public bool IsHitRectangle(Rectangle rect)
            {
                return rect.Contains(this.currentPt);
            }
            public bool IsHitLine(Point p1, Point p2)
            {
                int x = Math.Min(p1.X, p2.X);
                int y = Math.Min(p1.Y, p2.Y);
                int w = Math.Abs(p1.X - p2.X);
                int h = Math.Abs(p1.X - p2.X);
                Rectangle rect = new Rectangle(x, y, w, h);
                if (!IsHitRectangle(rect))
                {
                    return false;
                }
                double P2SubstractP1X = (double)p2.X - (double)p1.X;
                double P2SubstractP1Y = (double)p2.Y - (double)p1.Y;
                double PtSubstractP1X = (double)currentPt.X - (double)p1.X;
                double PtSubstractP1Y = (double)currentPt.Y - (double)p1.Y;
                double slope0 = (P2SubstractP1X) / P2SubstractP1Y;
                double slope1 = (PtSubstractP1X) / PtSubstractP1Y;
                if (slope1 != slope0)
                {
                    return false;
                }
                double squarePtP1 = PtSubstractP1X * PtSubstractP1X + PtSubstractP1Y * PtSubstractP1Y;
                double squareP2P1 = P2SubstractP1X * P2SubstractP1X + P2SubstractP1Y * P2SubstractP1Y;
                return squarePtP1 > squareP2P1;
            }
        }
        public class RadialGradientBrushBuilder
        {
            PointF origin = new PointF(.5f, .5f);
            PointF focusScales = new PointF(0, 0);
            Color? centerColor;
            List<Color> surroundColor = new List<Color>();
            List<float> triangularPositions = new List<float>();
            bool HasFocusScales = false;
            bool HasBlendTriangularShape = false;
            float blendTriangularFocus;
            float blendTriangularScale;

            public RadialGradientBrushBuilder SetCenter(PointF pos)
            {
                origin = pos;
                return this;
            }
            public RadialGradientBrushBuilder SetCenter(double x, double y)
            {
                origin = new PointF((float)x, (float)y);
                return this;
            }
            public RadialGradientBrushBuilder SetFocusScale(PointF pt)
            {
                HasFocusScales = true;
                focusScales = pt;
                return this;
            }
            public RadialGradientBrushBuilder SetFocusScale(double x, double y)
            {
                HasFocusScales = true;
                focusScales = new PointF((float)x, (float)y);
                return this;
            }
            public RadialGradientBrushBuilder SetBlendTriangularShape(double focus, double scale)
            {
                HasBlendTriangularShape = true;
                blendTriangularFocus = (float)focus;
                blendTriangularScale = (float)scale;
                return this;
            }
            public RadialGradientBrushBuilder AddColor(Color c)
            {
                if (centerColor == null)
                {
                    centerColor = c;
                }
                else
                {
                    surroundColor.Add(c);
                }
                return this;
            }
            public PathGradientBrush Build(int x, int y, int r)
            {
                GraphicsPath gp = new GraphicsPath();
                Rectangle rect = Rectangle.FromLTRB(x - r, y - r, x + r, y + r);
                gp.AddEllipse(rect);
                PathGradientBrush pgb = new PathGradientBrush(gp);

                pgb.CenterPoint = new PointF(x + rect.Width * origin.X,
                                             y + rect.Height * origin.Y);
                if (centerColor == null)
                {
                    centerColor = Color.White;
                }
                pgb.CenterColor = centerColor.Value;
                if (surroundColor.Count == 0)
                {
                    pgb.SurroundColors = new Color[] { Color.Black };
                }
                else
                {
                    pgb.SurroundColors = surroundColor.ToArray();
                }
                if (HasBlendTriangularShape)
                {
                    pgb.SetBlendTriangularShape(blendTriangularFocus, blendTriangularScale);
                }
                if (HasFocusScales)
                {
                    pgb.FocusScales = focusScales;
                }
                return pgb;
            }
        }
        public class LinearGradientBrushBuilder
        {
            private Point p1, p2;
            private Color c1, c2;
            private List<Color> _colors = new List<Color>();
            private List<float> _positions = new List<float>();
            private int _RotateDegree;
            public LinearGradientBrushBuilder(Point p1, Point p2, Color c1, Color c2)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.c1 = c1;
                this.c2 = c2;
            }
            public LinearGradientBrushBuilder Rotate(int degree)
            {
                _RotateDegree = degree;
                return this;
            }
            public LinearGradientBrushBuilder AddColor(Color c)
            {
                _colors.Add(c);
                return this;
            }
            public LinearGradientBrushBuilder AddColor(UInt32 c)
            {
                _colors.Add(Color.FromArgb(unchecked((int)c)));
                return this;
            }
            public LinearGradientBrushBuilder AddPosition(double position)
            {
                _positions.Add((float)position);
                return this;
            }
            public LinearGradientBrush Build()
            {
                if (p2.Y == p1.Y && p1.X == p1.X)
                {
                    p2.Y = p1.Y + 1;
                }
                LinearGradientBrush ret = new LinearGradientBrush(p1, p2, c1, c2);
                if (_positions.Count > 1 && _colors.Count > 1)
                {
                    ColorBlend _colorBlend = new ColorBlend();
                    _colorBlend.Positions = _positions.ToArray();
                    _colorBlend.Colors = _colors.ToArray();
                    ret.InterpolationColors = _colorBlend;
                    if (_RotateDegree != 0)
                    {
                        ret.RotateTransform(_RotateDegree);
                    }
                }
                return ret;
            }
        }
        Bitmap canvas;
        Graphics graphic;
        public static readonly uint alphaMask = 0xff000000u;
        bool useAlpha = false;
        volatile bool hasDrawRequest = false;
        volatile bool drawFncInvoked = false;

        public IRendererDelegates.OnMouseButtonAction onMouseClickHandler { get; set; }
        public IRendererDelegates.OnMouseMoveAction onMouseMoveHandler { get; set; }
        public IRendererDelegates.OnKeyboardAction onKeyboard { get; set; }
        public IRendererDelegates.OnMouseWhellAction onMouseWhell { get; set; }
        public static readonly int MOUSE_LEFT = 0;
        public static readonly int MOUSE_MIDDLE = 1;
        public static readonly int MOUSE_RIGHT = 2;
        SolidBrushDictionary<int, SolidBrush> brushDic = new SolidBrushDictionary<int, SolidBrush>(128);
        SolidBrushDictionary<int, Pen> penDic = new SolidBrushDictionary<int, Pen>(128);
        Graphics grabGraphic = null;
        Rectangle? grabRectangle;
        public void SetOverrideDrawingTarget(Graphics g, Rectangle r)
        {
            this.grabGraphic = g;
            this.grabRectangle = r;
        }
        private int mouseIdx(MouseButtons btn)
        {
            switch (btn)
            {
                default:
                case System.Windows.Forms.MouseButtons.Left:
                    return MOUSE_LEFT;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MOUSE_MIDDLE;
                case System.Windows.Forms.MouseButtons.Right:
                    return MOUSE_RIGHT;
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (onMouseWhell != null)
            {
                onMouseWhell(e.X, e.Y, e.Delta * SystemInformation.MouseWheelScrollLines / 120);
            }
            base.OnMouseWheel(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (onMouseClickHandler != null)
            {
                onMouseClickHandler(e.X, e.Y, mouseIdx(e.Button), true);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (onMouseClickHandler != null)
            {
                onMouseClickHandler(e.X, e.Y, mouseIdx(e.Button), false);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (onMouseMoveHandler != null)
            {
                onMouseMoveHandler(e.X, e.Y, mouseIdx(e.Button), e.Button != System.Windows.Forms.MouseButtons.None);
            }
            base.OnMouseMove(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (onKeyboard != null)
            {
                onKeyboard((int)e.KeyData, e.Control, true);
            }
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (onKeyboard != null)
            {
                onKeyboard((int)e.KeyData, e.Control, false);
            }
            base.OnKeyUp(e);
        }
        public bool Selectable
        {
            get
            {
                return GetStyle(ControlStyles.Selectable);
            }
            set
            {
                SetStyle(ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, value);
            }
        }        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SDLMMControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CausesValidation = false;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SDLMMControl";
            this.Size = new System.Drawing.Size(471, 484);
            this.SizeChanged += new System.EventHandler(this.SDLMMControl_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
        public SDLMMControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            canvas = new Bitmap(this.Width, this.Height);

            try
            {
                if (grabGraphic != null)
                {
                    graphic = grabGraphic;
                }
                else
                {
                    graphic = Graphics.FromImage(canvas);
                }
                graphic.InterpolationMode = setInterpolationMode;
                graphic.SmoothingMode = setSmoothMode;
                graphic.TextRenderingHint = setTextRenderHint;
            }
            catch (Exception)
            {

            }
        }
        public bool IsValid
        {
            get
            {
                return graphic != null;
            }
        }
        SmoothingMode setSmoothMode = SmoothingMode.None;
        InterpolationMode setInterpolationMode = InterpolationMode.Low;
        System.Drawing.Text.TextRenderingHint setTextRenderHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
        public InterpolationMode InterpolationMode
        {
            get
            {
                return setInterpolationMode;
            }
            set
            {
                setInterpolationMode = value;
                if (graphic != null)
                {
                    graphic.InterpolationMode = value;
                }
            }
        }

        public SmoothingMode SmoothMode
        {
            get
            {
                return setSmoothMode;
            }
            set
            {
                setSmoothMode = value;
                if (graphic != null)
                {
                    graphic.SmoothingMode = value;

                }
            }
        }
        public System.Drawing.Text.TextRenderingHint TextRenderingHint
        {
            get
            {
                return setTextRenderHint;
            }
            set
            {
                setTextRenderHint = value;
                if (graphic != null)
                {
                    graphic.TextRenderingHint = value;

                }
            }
        }
        public SizeF MeasureString(String s, Font font = null, int maxsize = -1)
        {
            if (font == null) font = this.Font;
            if (maxsize < 0)
            {
                return graphic.MeasureString(s, font);
            }
            return graphic.MeasureString(s, font, maxsize);
        }

        public void setUseAlpha(Boolean buse)
        {
            useAlpha = buse;
        }
        private Color coveredColor(int color)
        {
            if (!useAlpha)
            {
                color = (int)((uint)color | alphaMask);
            }
            return Color.FromArgb(color);
        }
        public void getScreen(out int[] outpixels, out int w, out int h)
        {

            int[] outputArray = new int[canvas.Width * canvas.Height];
            BitmapData bmpData = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, canvas.PixelFormat);
            unsafe
            {
                int* pixelsMap = (int*)bmpData.Scan0;

                Parallel.For(0, outputArray.Length, (i) =>
                {
                    outputArray[i] = pixelsMap[i];
                });
            }
            canvas.UnlockBits(bmpData);
            outpixels = outputArray;
            w = canvas.Width;
            h = canvas.Height;

        }
        public void getScreen(out Bitmap bmp)
        {
            bmp = new Bitmap(canvas);
        }
        public void drawPixel(int x, int y, int color)
        {
            hasDrawRequest = true;
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            //lock (canvas)
            {
                canvas.SetPixel(x, y, coveredColor(color));
            }
        }
        public void drawEllipse(int x, int y, int w, int h, int color)
        {
            hasDrawRequest = true;
            Pen pen = GetPenFromColor(color);
            graphic.DrawEllipse(pen, x, y, w, h);
        }
        public void fillEllipse(int x, int y, int w, int h, int color)
        {
            hasDrawRequest = true;
            Brush brush = GetBrushFromColor(color);
            graphic.FillEllipse(brush, x, y, w, h);
        }
        public void fillEllipse(int x, int y, int w, int h, Brush brush)
        {
            hasDrawRequest = true;
            graphic.FillEllipse(brush, x, y, w, h);
        }
        public void drawCircle(int n_cx, int n_cy, int radius, int pixel)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                Pen pen = GetPenFromColor(pixel);
                graphic.DrawEllipse(pen, n_cx - radius, n_cy - radius, radius * 2, radius * 2);
            }
        }
        private static bool UseDictionaryPrefetch = true;
        SolidBrush GetBrushFromColor(int color)
        {
            if (!UseDictionaryPrefetch)
            {
                return new SolidBrush(coveredColor(color));
            }
            SolidBrush ret = brushDic.Get(color);
            if (ret == null)
            {
                ret = new SolidBrush(coveredColor(color));
                brushDic.Put(color, ret);
            }
            return ret;
        }
        Pen GetPenFromColor(int color, int penWidth = 1)
        {
            if (!UseDictionaryPrefetch)
            {
                return new Pen(coveredColor(color));
            }
            Pen ret = penDic.Get(color);
            if (ret == null)
            {
                ret = new Pen(coveredColor(color));
                penDic.Put(color, ret);
            }
            ret.Width = penWidth;
            return ret;
        }
        public void fillCircle(int x, int y, int r, Brush brush)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                graphic.FillEllipse(brush, x - r, y - r, r * 2, r * 2);
            }
        }
        public void fillCircle(int x, int y, int r, int color)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                Brush brush = GetBrushFromColor(color);
                graphic.FillEllipse(brush, x - r, y - r, r * 2, r * 2);
            }
        }

        float[] drawRectDefaultDashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
        public void drawRect(int x, int y, int w, int h, int color, bool dashed = false, int width = 1)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                Pen pen = GetPenFromColor(color, width);
                if (dashed)
                {
                    pen.DashStyle = DashStyle.DashDotDot;
                    pen.DashPattern = drawRectDefaultDashPattern;
                }
                graphic.DrawRectangle(pen, x, y, w, h);
            }
        }
        // Draw a rectangle in the indicated Rectangle
        // rounding the indicated corners.
        public static GraphicsPath MakeRoundedRect(
            RectangleF rect, float xradius = 5, float yradius = 5,
            bool round_ul = true, bool round_ur = true, bool round_lr = true, bool round_ll = true)
        {

            // Make a GraphicsPath to draw the rectangle.
            PointF point1, point2;
            GraphicsPath path = new GraphicsPath();

            // Upper left corner.
            if (round_ul)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 180, 90);
                point1 = new PointF(rect.X + xradius, rect.Y);
            }
            else point1 = new PointF(rect.X, rect.Y);

            // Top side.
            if (round_ur)
                point2 = new PointF(rect.Right - xradius, rect.Y);
            else
                point2 = new PointF(rect.Right, rect.Y);
            path.AddLine(point1, point2);

            // Upper right corner.
            if (round_ur)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 270, 90);
                point1 = new PointF(rect.Right, rect.Y + yradius);
            }
            else point1 = new PointF(rect.Right, rect.Y);

            // Right side.
            if (round_lr)
                point2 = new PointF(rect.Right, rect.Bottom - yradius);
            else
                point2 = new PointF(rect.Right, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower right corner.
            if (round_lr)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius,
                    rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 0, 90);
                point1 = new PointF(rect.Right - xradius, rect.Bottom);
            }
            else point1 = new PointF(rect.Right, rect.Bottom);

            // Bottom side.
            if (round_ll)
                point2 = new PointF(rect.X + xradius, rect.Bottom);
            else
                point2 = new PointF(rect.X, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower left corner.
            if (round_ll)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 90, 90);
                point1 = new PointF(rect.X, rect.Bottom - yradius);
            }
            else point1 = new PointF(rect.X, rect.Bottom);

            // Left side.
            if (round_ul)
                point2 = new PointF(rect.X, rect.Y + yradius);
            else
                point2 = new PointF(rect.X, rect.Y);
            path.AddLine(point1, point2);

            // Join with the start point.
            path.CloseFigure();

            return path;
        }
        public void drawRoundRect(int x, int y, int w, int h, float rad, int color, int penWidth = 1)
        {
            hasDrawRequest = true;
            if (rad <= 0)
            {
                drawRect(x, y, w, h, color);
                return;
            }
            GraphicsPath path = MakeRoundedRect(new RectangleF(x, y, w, h), rad, rad);

            graphic.DrawPath(GetPenFromColor(color, penWidth), path);
        }
        public static void RoundRectangle(Graphics graphic, int x, int y, int w, int h, float rad, Color color, int penWidth = 1)
        {
            Pen pen = new Pen(new SolidBrush(color));
            pen.Width = penWidth;
            if (rad <= 0)
            {
                graphic.DrawRectangle(pen, x, y, w, h);
                return;
            }
            GraphicsPath path = MakeRoundedRect(new RectangleF(x, y, w, h), rad, rad);

            graphic.DrawPath(pen, path);
        }
        public void fillRoundRect(int x, int y, int w, int h, float radx, float rady, int color)
        {
            hasDrawRequest = true;
            if (radx <= 0 || rady <= 0)
            {
                fillRect(x, y, w, h, color);
                return;
            }

            GraphicsPath path = MakeRoundedRect(new RectangleF(x, y, w, h), radx, rady);
            graphic.FillPath(GetBrushFromColor(color), path);
        }
        public void fillRoundRect(int x, int y, int w, int h, float rad, int color)
        {
            fillRoundRect(x, y, w, h, rad, rad, color);
        }
        public void fillRoundRect(int x, int y, int w, int h, float radx, float rady, Brush brush)
        {
            hasDrawRequest = true;
            if (radx <= 0 || rady <= 0)
            {
                fillRect(x, y, w, h, brush);
                return;
            }
            GraphicsPath path = MakeRoundedRect(new RectangleF(x, y, w, h), radx, rady);
            graphic.FillPath(brush, path);
        }
        public void fillRoundRect(int x, int y, int w, int h, float rad, Brush brush)
        {
            fillRoundRect(x, y, w, h, rad, rad, brush);
        }
        public void fillRoundRect(Rectangle r, float rad, int color)
        {
            fillRoundRect(r.X, r.Y, r.Width, r.Height, rad, rad, color);
        }
        public void fillRoundRect(Rectangle r, float rad, Brush brush)
        {
            fillRoundRect(r.X, r.Y, r.Width, r.Height, rad, rad, brush);
        }
        public void fillRoundRect(Point position, Size size, float rad, int color)
        {
            fillRoundRect(position.X, position.Y, size.Width, size.Height, rad, rad, color);
        }
        public void fillRoundRect(Point position, Size size, float rad, Brush brush)
        {
            fillRoundRect(position.X, position.Y, size.Width, size.Height, rad, rad, brush);
        }

        public void fillRect(int x, int y, int w, int h, int color)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                Brush brush = GetBrushFromColor(color);
                graphic.FillRectangle(brush, x, y, w, h);
            }
        }
        public void fillRect(int x, int y, int w, int h, Brush linearGradient)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                graphic.FillRectangle(linearGradient, x, y, w, h);
            }
        }
        public void fillRect(Rectangle r, int color)
        {
            fillRect(r.X, r.Y, r.Width, r.Height, color);
        }
        public void fillRect(Point position, Size size, int color)
        {
            fillRect(position.X, position.Y, size.Width, size.Height, color);
        }
        public void fillRect(Rectangle r, Brush linearGradient)
        {
            fillRect(r.X, r.Y, r.Width, r.Height, linearGradient);
        }
        public void fillRect(Point position, Size size, Brush linearGradient)
        {
            fillRect(position.X, position.Y, size.Width, size.Height, linearGradient);
        }

        void drawQuadBezierSeg(Point p0, Point p1, Point p2, int color)
        {
            drawQuadBezierSeg(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, color);
        }
        void drawQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2, int color)
        {
            hasDrawRequest = true;
            BitmapData data = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);
            IntPtr ptr = data.Scan0;
            unsafe
            {
                int maxlen = canvas.Width * canvas.Height;
                int* intptr = (int*)ptr;
                int width = canvas.Width;
                int sx = x2 - x1, sy = y2 - y1;
                long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
                double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

                if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
                { /* begin with longer part */
                    x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
                }
                if (cur != 0)
                {                                    /* no straight line */
                    xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
                    yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
                    xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
                    if (cur * sx * sy < 0)
                    {                           /* negated curvature? */
                        xx = -xx; yy = -yy; xy = -xy; cur = -cur;
                    }
                    dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
                    dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
                    xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
                    do
                    {
                        int targetIdx = (int)y0 * width + (int)x0;
                        if (targetIdx >= 0 && targetIdx < maxlen)
                        {
                            intptr[targetIdx] = color;
                        }                                 /* plot curve */
                        if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
                        y1 = ((2 * err) < dx) ? 0 : 1;                  /* save value for test of y step */
                        if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
                        if (y1 != 0) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
                    } while (dy < dx);           /* gradient negates -> algorithm fails */
                }
            }
            canvas.UnlockBits(data);
            drawLine(x0, y0, x2, y2, color);                  /* plot remaining part to end */
        }
        public void drawLine(int x0, int y0, int x1, int y1, int color, bool maskForEscape, params Rectangle[] mask)
        {
            _drawLine(x0, y0, x1, y1, color, maskForEscape, mask);
        }
        public void drawLine(int x0, int y0, int x1, int y1, int color, int stride, bool maskForEscape, params Rectangle[] mask)
        {
            if (stride == 1)
            {
                _drawLine(x0, y0, x1, y1, color, maskForEscape, mask);
            }
            else if (stride > 1)
            {
                if (y0 == y1)
                {
                    for (int i = -stride / 2; i <= stride / 2; ++i)
                    {
                        _drawLine(x0, y0 + i, x1, y1 + i, color, maskForEscape, mask);
                    }
                    return;
                }
                else
                {
                    for (int i = -stride / 2; i <= stride / 2; ++i)
                    {
                        _drawLine(x0 + i, y0, x1 + i, y1, color, maskForEscape, mask);
                    }
                }
            }


        }
        private void _drawLine(int x0, int y0, int x1, int y1, int color, bool maskForEscape, params Rectangle[] mask)
        {
            hasDrawRequest = true;
            BitmapData data = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);
            IntPtr ptr = data.Scan0;
            unsafe
            {
                int maxlen = canvas.Width * canvas.Height;
                int* intptr = (int*)ptr;
                double x = x1 - x0;
                double y = y1 - y0;
                double length = Math.Sqrt(x * x + y * y);
                double addx = x / length;
                double addy = y / length;
                x = x0;
                y = y0;
                int width = canvas.Width;
                int height = canvas.Height;
                if (maskForEscape)
                {
                    for (int i = 0; i < length; i += 1)
                    {
                        bool masked = false;
                        for (int j = 0; j < mask.Length; ++j)
                        {
                            if (mask[j].Contains((int)x, (int)y))
                            {
                                masked = true;
                                break;
                            }
                        }
                        if (!masked && y >= 0 && y < height && x >= 0 && x < width)
                        {

                            int targetIdx = (int)y * width + (int)x;
                            if (targetIdx >= 0 && targetIdx < maxlen)
                            {
                                intptr[targetIdx] = color;
                            }
                        }
                        x += addx;
                        y += addy;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i += 1)
                    {
                        bool masked = false;
                        for (int j = 0; j < mask.Length; ++j)
                        {
                            if (mask[j].Contains((int)x, (int)y))
                            {
                                masked = true;
                                break;
                            }
                        }
                        if (masked && y >= 0 && y < height && x >= 0 && x < width)
                        {
                            int targetIdx = (int)y * width + (int)x;
                            if (targetIdx >= 0 && targetIdx < maxlen)
                            {
                                intptr[targetIdx] = color;
                            }
                        }
                        x += addx;
                        y += addy;
                    }
                }
            }
            canvas.UnlockBits(data);
        }
        public void drawRectMasked(int x, int y, int w, int h, int color, int stride, bool maskForEscape, params Rectangle[] mask)
        {
            drawLine(x, y, x + w, y, color, stride, maskForEscape, mask);
            drawLine(x, y, x, y + h, color, stride, maskForEscape, mask);
            drawLine(x, y + h, x + w, y + h, color, stride, maskForEscape, mask);
            drawLine(x + w, y, x + w, y + h, color, stride, maskForEscape, mask);
        }
        public void drawRectMasked(int x, int y, int w, int h, int color, bool maskForEscape, params Rectangle[] mask)
        {
            drawRectMasked(x, y, w, h, color, 1, maskForEscape, mask);
        }
        public void drawLine(int x, int y, int x2, int y2, int color, int stride = 1)
        {
            hasDrawRequest = true;
            //lock(canvas)
            {
                Pen pen = GetPenFromColor(color, stride);
                graphic.DrawLine(pen, x, y, x2, y2);
            }
        }
        public void drawPixels(int[] pixels, int x, int y, int w, int h)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                int origh = pixels.Length / w;
                GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                Bitmap bmp = new Bitmap(w, origh, w * 4, PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject());
                graphic.DrawImageUnscaledAndClipped(bmp, new Rectangle(x, y, w, h));
                handle.Free();
            }
        }
        public void drawPixels(int[] pixels, int x, int y, int w, int h, int transkey)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {
                int origh = pixels.Length / w;
                Bitmap bmp = new Bitmap(w, origh);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                unsafe
                {
                    int* pixelsMap = (int*)bmpData.Scan0;
#if false
                for(int i=0;i<pixels.Length; ++i){
                   Color c;
                   if ((pixels[i]&0xffffff) == transkey)
                   {
                       c = Color.FromArgb(0);
                   }
                   else
                   {
                       c = coveredColor(pixels[i]);
                   }
                   bmp.SetPixel(i % w, i / w, c);      
                }
#else
                    Parallel.For(0, pixels.Length, (i) =>
                    {
                        Color c;
                        if ((pixels[i] & 0xffffff) == transkey)
                        {
                            c = Color.FromArgb(0);
                        }
                        else
                        {
                            c = coveredColor(pixels[i]);
                            pixelsMap[i] = c.ToArgb();
                        }

                        //bmp.SetPixel(i % w, i / w, c);
                    });
                }
#endif
                bmp.UnlockBits(bmpData);
                graphic.DrawImageUnscaledAndClipped(bmp, new Rectangle(x, y, w, h));
            }
        }
        public void stretchpixels(int[] pixels, int w, int h, int[] output, int w2, int h2)
        {
            float dw = ((float)w2) / w;
            float dh = ((float)h2) / h;
            int ii, e;
            e = h2 * w2;
            //#pragma omp parallel for firstprivate(output,pixels,dw,dh)
            for (ii = 0; ii < e; ii += 1)
            {
                int origi, origj;
                float i = ii % w2;
                float j = ii / w2;
                //for(j=0; j<h2; j+=1){
                //for(i=0; i<w2; i+=1){
                origi = (int)(i / dw);
                origj = (int)(j / dh);
                output[(int)(j * w2 + i)] = pixels[(int)(origj * w + origi)];
                //}
            }
        }
        public void stretchpixels2(ref int[] pixels, int w, int h, int w2, int h2)
        {
            int[] newbg = new int[w2 * h2];
            int[] org = pixels;
            stretchpixels(org, w, h, newbg, w2, h2);
            pixels = newbg;
        }
        public void drawPixelsSingleThread(int[] pixels, int x, int y, int w, int h)
        {

            if (w <= 0 || h <= 0) return;
            hasDrawRequest = true;
            //lock (canvas)
            {
                int origh = pixels.Length / w;
                Bitmap bmp = new Bitmap(w, origh);
                if (h > origh)
                {
                    h = origh;
                }
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                unsafe
                {
                    int* pixelsMap = (int*)bmpData.Scan0;
                    for (int i = 0; i < pixels.Length; ++i)
                    {
                        Color c = coveredColor(pixels[i]);
                        pixelsMap[i] = c.ToArgb();
                        //bmp.SetPixel(i % w, i / w, c);
                    }
                }
                bmp.UnlockBits(bmpData);
                graphic.DrawImageUnscaledAndClipped(bmp, new Rectangle(x, y, w, h));
            }
        }
        public void loadimage(String name, out int[] pixels, out int w, out int h)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(name);

            w = bmp.Width;
            h = bmp.Height;
#if false
            int W = bmp.Width;
            int H = bmp.Height;
            int totalLength = W * H;
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int[] outputPixels = new int[W * H];
            unsafe
            {
               
                int* pixelData = (int*)bmpData.Scan0;
                for (int i = 0; i < H; ++i)
                {
                    for (int j = 0; j < W; ++j)
                    {
                        outputPixels[i * W + j] = pixelData[i * W + j];
                    }
                }
            }
            bmp.UnlockBits(bmpData);
           
#else
            int W = bmp.Width;
            int H = bmp.Height;
            int[] outputPixels = new int[W * H];
            for (int i = 0; i < bmp.Height; ++i)
            {
                for (int j = 0; j < bmp.Width; ++j)
                {
                    outputPixels[i * bmp.Width + j] = bmp.GetPixel(j, i).ToArgb();
                }
            }
            pixels = outputPixels;
#endif
        }
        public void drawImage(Bitmap bmp, int x, int y, int w, int h)
        {
            lock (drawImageLocker)
            {
                hasDrawRequest = true;
                //lock (canvas)
                {
#if false
                    bool hasAlpha = false;
                    switch(bmp.PixelFormat)
                    {
                        case PixelFormat.PAlpha:
                        case PixelFormat.Format64bppPArgb:
                        case PixelFormat.Format64bppArgb:
                        case PixelFormat.Format32bppPArgb:
                        case PixelFormat.Format32bppArgb:
                        case PixelFormat.Format16bppArgb1555:
                        case PixelFormat.Alpha:
                            hasAlpha = true;
                            break;
                    }
                    if (!hasAlpha)
                    {
                        CompositingMode mode = graphic.CompositingMode;
                        graphic.CompositingMode= CompositingMode.SourceCopy;
                        graphic.DrawImage(bmp, x, y, w, h);
                        graphic.CompositingMode = mode;
                    }
                    else
                    {
                        graphic.DrawImage(bmp, x, y, w, h);
                    }
#endif
                    graphic.DrawImage(bmp, x, y, w, h);
                }
            }
        }
        public static int[] bitmapToPixels(Bitmap bmp)
        {
            int[] outputarr = new int[bmp.Width * bmp.Height];
            int Length = outputarr.Length;
            BitmapData bmpData0 = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            unsafe
            {
                int* pixelsMap0 = (int*)bmpData0.Scan0;
                Parallel.For(0, Length, (i) =>
                {
                    outputarr[i] = pixelsMap0[i];
                });
            }
            bmp.UnlockBits(bmpData0);
            return outputarr;
        }
        public void drawImageSingleThread(Bitmap bmp, int x, int y, int w, int h, float alpha)
        {

            hasDrawRequest = true;
            if (alpha >= 1)
            {
                drawImage(bmp, x, y, w, h);
                return;
            }
            Bitmap newBmp = new Bitmap(bmp);
            lock (drawImageLocker)
            {
                if (alpha < 0) alpha = 0;

                int Length = bmp.Width * bmp.Height;
                Bitmap bmp2 = bmp;
                if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    bmp2 = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
                }
                BitmapData bmpData0 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp2.PixelFormat);
                BitmapData bmpData = newBmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp2.PixelFormat);
                unsafe
                {
                    int* pixelsMap0 = (int*)bmpData0.Scan0;
                    int* pixelsMap = (int*)bmpData.Scan0;
                    for (int i = 0; i < Length; ++i)
                    {
                        Color c = Color.FromArgb(pixelsMap0[i]);
                        int ialpha = (int)(c.A * alpha);
                        if (ialpha < 0) ialpha = 0;
                        if (ialpha > 255) ialpha = 255;
                        pixelsMap[i] = Color.FromArgb(ialpha, c.R, c.G, c.B).ToArgb();
                        //bmp.SetPixel(i % w, i / w, c);
                    }
                }
                UnlockBitmapData(bmp2, bmpData0);
                UnlockBitmapData(newBmp, bmpData);
            }
            drawImage(newBmp, x, y, w, h);
        }

        private void UnlockBitmapData(Bitmap bmp, BitmapData bmpdata)
        {
            try
            {
                bmp.UnlockBits(bmpdata);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }
        object drawImageLocker = new object();
        public void drawImage(Bitmap bmp, int x, int y, int w, int h, float alpha)
        {
            hasDrawRequest = true;
            if (alpha >= 1)
            {
                drawImage(bmp, x, y, w, h);
                return;
            }
            Bitmap newBmp = new Bitmap(bmp);
            lock (drawImageLocker)
            {
                if (alpha < 0) alpha = 0;
                int Length = bmp.Width * bmp.Height;
                Bitmap bmp2 = bmp;
                if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    bmp2 = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
                }
                BitmapData bmpData0 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp2.PixelFormat);
                BitmapData bmpData = newBmp.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp2.PixelFormat);
                unsafe
                {
                    int* pixelsMap0 = (int*)bmpData0.Scan0;
                    int* pixelsMap = (int*)bmpData.Scan0;
                    Parallel.For(0, Length, (i) =>
                    {
                        if (i >= Length) return;
                        Color c = Color.FromArgb(pixelsMap0[i]);
                        int ialpha = (int)(c.A * alpha);
                        if (ialpha < 0) ialpha = 0;
                        if (ialpha > 255) ialpha = 255;
                        pixelsMap[i] = Color.FromArgb(ialpha, c.R, c.G, c.B).ToArgb();
                        //bmp.SetPixel(i % w, i / w, c);
                    });
                }
                UnlockBitmapData(bmp2, bmpData0);
                UnlockBitmapData(newBmp, bmpData);
            }
            drawImage(newBmp, x, y, w, h);
        }
        public void Clear(int color)
        {
            hasDrawRequest = true;
            this.graphic.Clear(coveredColor(color));
        }
        public void drawPie(Rectangle rect, int color, float startAngle, float sweepAngle)
        {
            Pen pen = GetPenFromColor(color);
            this.graphic.DrawPie(pen, rect, startAngle, sweepAngle);
        }
        public void fillPie(Rectangle rect, int color, float startAngle, float sweepAngle)
        {
            Brush brush = GetBrushFromColor(color);
            this.graphic.FillPie(brush, rect, startAngle, sweepAngle);
        }
        public void drawDropdownButton(String txt, int x, int y, int w, int h, System.Windows.Forms.VisualStyles.ComboBoxState state, Font font = null)
        {
            hasDrawRequest = true;
            {

                try
                {
                    if (font == null)
                    {
                        font = this.Font;
                    }
                    Rectangle bound = new Rectangle(x, y, w, h);
                    int maxWidth = 32;
                    // SmoothingMode mode = graphic.SmoothingMode;
                    // System.Drawing.Drawing2D.InterpolationMode interpolation = graphic.InterpolationMode;

                    //graphic.SmoothingMode = SmoothingMode.AntiAlias;
                    //graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    //graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    fillRect(bound, GetBrushFromColor(unchecked((int)0xffffffff)));
                    //System.Windows.Forms.ComboBoxRenderer.DrawTextBox(graphic, bound, txt, font, TextFormatFlags.WordEllipsis, state);
                    drawString(txt, bound.Left, bound.Top, unchecked((int)0xff000000), font);
                    if (state == System.Windows.Forms.VisualStyles.ComboBoxState.Hot)
                    {
                        drawRect(bound.Left, bound.Top, bound.Width, bound.Height, unchecked((int)0xff4488ff));
                    }

                    // graphic.SmoothingMode = mode;

                    //graphic.InterpolationMode = interpolation;
                    bound.Offset(bound.Width - maxWidth, 0);
                    bound.Size = new System.Drawing.Size(maxWidth, h);
                    System.Windows.Forms.ComboBoxRenderer.DrawDropDownButton(graphic, bound, state);
                }
                catch (Exception)
                {

                }
            }
        }
        public void drawButton(String txt, int x, int y, int w, int h, bool focused, System.Windows.Forms.VisualStyles.PushButtonState state, Font font = null)
        {
            hasDrawRequest = true;
            Rectangle bound = new Rectangle(x, y, w, h);
            System.Windows.Forms.ButtonRenderer.DrawButton(graphic, bound, txt, font, focused, state);
        }
        public void drawString(String str, int x, int y, int color, Font font = null)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {

                try
                {
                    if (font == null)
                    {
                        font = System.Drawing.SystemFonts.DefaultFont;
                    }
                    Brush brush = GetBrushFromColor(color);
                    if (!String.IsNullOrEmpty(str))
                        graphic.DrawString(str, font, brush, x, y);
                }
                catch (Exception)
                {

                }
            }
        }
        public void drawString(String str, Rectangle rect, int color, Font font = null)
        {
            hasDrawRequest = true;
            //lock (canvas)
            {

                try
                {
                    if (font == null)
                    {
                        font = System.Drawing.SystemFonts.DefaultFont;
                    }
                    Brush brush = GetBrushFromColor(color);
                    if (!String.IsNullOrEmpty(str))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.FormatFlags = StringFormatFlags.NoClip;
                        graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        graphic.DrawString(str, font, brush, rect, stringFormat);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (this.Controls.Count > 0)
            {
                return;
            }
            if (drawFncInvoked)
            {
                CompositingMode mode = e.Graphics.CompositingMode;
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.DrawImageUnscaled(canvas, 0, 0, canvas.Width, canvas.Height);
                e.Graphics.CompositingMode = mode;
                hasDrawRequest = false;
                drawFncInvoked = false;
            }
            else
            {
                e.Graphics.DrawImageUnscaled(canvas, 0, 0, canvas.Width, canvas.Height);
            }
            if (this.Controls.Count > 0)
                base.OnPaint(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            //lock (canvas)
            if (drawFncInvoked)
            {
                CompositingMode mode = e.Graphics.CompositingMode;
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.DrawImageUnscaled(canvas, 0, 0, canvas.Width, canvas.Height);
                e.Graphics.CompositingMode = mode;
                if (this.grabGraphic != null && this.grabRectangle != null)
                {
                    CompositingMode mode2 = this.grabGraphic.CompositingMode;
                    Rectangle rect = grabRectangle.Value;
                    this.grabGraphic.DrawImage(canvas, rect.X, rect.Y, rect.Width, rect.Height);
                    this.grabGraphic.CompositingMode = mode2;
                }
                hasDrawRequest = false;
                drawFncInvoked = false;
            }
            else
            {
                e.Graphics.DrawImageUnscaled(canvas, 0, 0, canvas.Width, canvas.Height);
            }
            if (this.Controls.Count > 0)
                base.OnPaint(e);
        }

        public Bitmap flushToBMP()
        {
            graphic.Flush();
            return canvas;
        }
        public Bitmap flushToBMP(int left, int top, int w, int h)
        {
            graphic.Flush();
            if (left < 0)
            {
                left = 0;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (left + w > canvas.Width)
            {
                w = canvas.Width - left;
            }
            if (top + h > canvas.Height)
            {
                h = canvas.Height - top;
            }

            return canvas.Clone(new Rectangle(left, top, w, h), canvas.PixelFormat);
        }
        public void flush()
        {

            drawFncInvoked = hasDrawRequest;
            //lock (canvas)
            {
                graphic.Flush();
            }
            this.Invalidate();
        }

        public void mode7Render(double angle, int vx, int vy, int[] pixels, int bw, int bh, int x, int y, int w,
                int h)
        {
            hasDrawRequest = true;

            mode7render_internal(0.5, 1.5, 2, 1, angle, vx, vy, pixels, bw, bh, x, y, w, h);
        }


        int[] mode7ToDraw;
        int mode7ToDrawW, mode7ToDrawH;

        public void mode7render_internal(double groundFactor, double xFac, double yFac,
                 int scanlineJump, double angle, int vx, int vy, int[] bg, int bw,
                 int bh, int tx, int ty, int _w, int _h)
        {
            hasDrawRequest = true;
            if (tx + _w >= this.Width)
            {
                _w = this.Width - tx;
            }
            if (ty + _h > this.Height)
            {
                _h = this.Height - ty;
            }
            int w = _w;
            int h = _h;
            if (mode7ToDraw != null)
            {
                if (w * h > mode7ToDrawW * mode7ToDrawH)
                {
                    mode7ToDraw = new int[w * h];
                }
            }
            else
            {
                mode7ToDraw = new int[w * h];
            }
            if (mode7ToDrawW != w)
            {
                mode7ToDrawW = w;
            }
            if (mode7ToDrawH != h)
            {
                mode7ToDrawH = h;
            }
            int[] toDraw = mode7ToDraw;
            int lev = w / scanlineJump;
            int x;
            double ca = Math.Cos(angle) * 48 * groundFactor * xFac;
            double sa = Math.Sin(angle) * 48 * groundFactor * xFac;
            double can = Math.Cos(angle + 3.1415926 / 2) * 16 * groundFactor * yFac;
            double san = Math.Sin(angle + 3.1415926 / 2) * 16 * groundFactor * yFac;

            for (x = 0; x < lev; ++x)
            {
                int y;
                double xr = -(((double)x / lev) - 0.5);
                double cax = (ca * xr) + can;
                double sax = (sa * xr) + san;
                for (y = 0; y < h; ++y)
                {
                    double zf = ((double)h) / y;
                    int xd = (int)(vx + zf * cax);
                    int yd = (int)(vy + zf * sax);
                    if (yd < bh && xd < bw && yd > 0 && xd > 0)
                    {
                        toDraw[y * w + x] = bg[yd * bw + xd];
                    }
                }
            }

            drawPixels(toDraw, tx, ty, w, h);
        }

        private void SDLMMControl_SizeChanged(object sender, EventArgs e)
        {
            lock (canvas)
            {
                if (graphic != null)
                {
                    hasDrawRequest = true;
                    drawFncInvoked = true;
                    graphic.Flush();
                    int width = this.Width;
                    int height = this.Height;
                    if (width <= 0 || height <= 0)
                    {
                        return;
                    }
                    if (width <= 0) width = 32;
                    if (height <= 0) height = 32;
                    canvas = new Bitmap(canvas, width, height);
                    if (grabGraphic != null)
                    {
                        graphic = grabGraphic;
                    }
                    else
                    {
                        graphic = Graphics.FromImage(canvas);
                    }
                    graphic.InterpolationMode = setInterpolationMode;
                    graphic.SmoothingMode = setSmoothMode;
                    graphic.TextRenderingHint = setTextRenderHint;
                }
            }
        }

    }
    public class SolidBrushDictionary<key, type>
    {
        int bucketSize = 16;
        public SolidBrushDictionary(int bucketSize = 16)
        {
            this.bucketSize = bucketSize;
        }
        Dictionary<key, type> suggestList = new Dictionary<key, type>();
        List<key> LRUStringList = new List<key>();

        public type Get(key word)
        {
            if (suggestList.ContainsKey(word))
            {
                int foundIdx = -1;
                for (int i = 0; i < LRUStringList.Count; ++i)
                {
                    if (LRUStringList[i].Equals(word))
                    {
                        foundIdx = i;
                        break;
                    }
                }
                if (foundIdx != -1 && foundIdx > 0)
                {
                    LRUStringList.RemoveAt(foundIdx);
                    LRUStringList.Insert(0, word);
                }
                return suggestList[word];
            }
            return default(type);
        }
        public void Put(key word, type lstSuggestions) // add or replace
        {
            if (LRUStringList.Count >= bucketSize)
            {
                key victim = LRUStringList[LRUStringList.Count - 1];
                LRUStringList.RemoveAt(LRUStringList.Count - 1);
                LRUStringList.Insert(0, word);
                suggestList.Remove(victim);
            }
            if (suggestList.ContainsKey(word))
            {
                int foundIdx = -1;
                for (int i = 0; i < LRUStringList.Count; ++i)
                {
                    if (LRUStringList[i].Equals(word))
                    {
                        foundIdx = i;
                        break;
                    }
                }
                if (foundIdx != -1 && foundIdx > 0)
                {
                    LRUStringList.RemoveAt(foundIdx);
                    suggestList.Remove(word);
                }
            }
            LRUStringList.Insert(0, word);
            suggestList[word] = lstSuggestions;
        }
    }
}
