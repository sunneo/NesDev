﻿using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using SharpDX.Mathematics.Interop;
using Resource = SharpDX.Direct3D11.Resource;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpNes
{

    public class SharpDXControl : UserControl, IRenderer
    {
        Device device;
        SwapChain swapChain;
        RenderTarget d2dRenderTarget;
        RawRectangleF clientArea;
        public IRendererDelegates.OnMouseButtonAction onMouseClickHandler { get; set; }
        public IRendererDelegates.OnMouseMoveAction onMouseMoveHandler { get; set; }
        public IRendererDelegates.OnKeyboardAction onKeyboard { get; set; }
        public IRendererDelegates.OnMouseWhellAction onMouseWhell { get; set; }
        public static readonly int MOUSE_LEFT = 0;
        public static readonly int MOUSE_MIDDLE = 1;
        public static readonly int MOUSE_RIGHT = 2;
        public System.Drawing.Drawing2D.SmoothingMode SmoothMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        public System.Drawing.Text.TextRenderingHint TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
        bool useAlpha = true;
        public SharpDXControl()
        {
            //this.DoubleBuffered = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        }
        public void setUseAlpha(bool buse)
        {
            useAlpha = buse;
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
        }
        private readonly Object _drawLock = new Object();
        private readonly Object _drawReqLock = new Object();
        public string RendererName
        {
            get
            {
                return "Direct3D";
            }
        }

        public bool IsValid
        {
            get
            {
                return d2dRenderTarget != null;
            }
        }
        public void InitRendering()
        {
            try
            {
                lock (_drawLock)
                {
                    m_Ready = false;
                    ResizeRedraw = true;
                    var desc = new SwapChainDescription
                    {
                        BufferCount = 2,
                        ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height, new Rational(60, 1),
                            Format.R8G8B8A8_UNorm),
                        IsWindowed = true,
                        OutputHandle = Handle,
                        SampleDescription = new SampleDescription(1, 0),
                        SwapEffect = SwapEffect.Discard,
                        Usage = Usage.RenderTargetOutput
                    };

                    Device.CreateWithSwapChain(DriverType.Hardware,
                        DeviceCreationFlags.BgraSupport,
                        new[] { SharpDX.Direct3D.FeatureLevel.Level_9_3 },
                        desc,
                        out device,
                        out swapChain);

                    var d2dFactory = new SharpDX.Direct2D1.Factory();

                    Factory factory = swapChain.GetParent<Factory>();
                    factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAll);

                    Texture2D backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);

                    Surface surface = backBuffer.QueryInterface<Surface>();

                    d2dRenderTarget = new RenderTarget(d2dFactory, surface,
                        new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));

                    var bitmapProperties = new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore));

                    clientArea = new RawRectangleF
                    {
                        Left = 0,
                        Top = 0,
                        Bottom = ClientSize.Height,
                        Right = ClientSize.Width
                    };

                    factory.Dispose();
                    surface.Dispose();
                    backBuffer.Dispose();
                    m_Ready = true;
                }
            }
            catch (Exception ee)
            {

            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            InitRendering();
        }

        public void EndRendering()
        {
            DisposeDirect3D();
        }

        private void DisposeDirect3D()
        {
            lock (_drawLock)
            {
                if (d2dRenderTarget != null)
                    d2dRenderTarget.Dispose();
                if (swapChain != null)
                    swapChain.Dispose();
                if (device != null)
                    device.Dispose();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeDirect3D();
            }
            base.Dispose(disposing);
        }
        protected override void OnResize(EventArgs e)
        {
            try
            {
                DisposeDirect3D();
                InitRendering();
                base.OnResize(e);
            }
            catch
            {
                // This is pretty stupid, but Mono will send a resize event to this component
                // even when it's not added to a frame, so this will fail horribly
                // during the renderer self-test procedure, which detects this type of failure...
                // on different thread.
            }
        }
        SharpDX.DirectWrite.TextFormat m_TextFormat;
        SharpDX.DirectWrite.Factory FactoryDWrite = new SharpDX.DirectWrite.Factory();
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (m_TextFormat != null)
                m_TextFormat.Dispose();
            m_TextFormat = new SharpDX.DirectWrite.TextFormat(FactoryDWrite, this.Font.FontFamily.Name, this.Font.Size);
        }
        static RawColor4 ToRawColor4(System.Drawing.Color c)
        {
            RawColor4 ret = default(RawColor4);
            ret.A = c.A / 255.0f;
            ret.B = c.B / 255.0f;
            ret.R = c.R / 255.0f;
            ret.G = c.G / 255.0f;
            return ret;
        }
        volatile bool m_Ready = false;
        void Draw()
        {
            lock (_drawLock)
            {
                if (!m_Ready) return;
                d2dRenderTarget.BeginDraw();

                lock (_drawReqLock)
                {
                    if (DrawRequests.Count > 0)
                    {
                        foreach (Action<RenderTarget> req in DrawRequests)
                        {
                            req(d2dRenderTarget);
                        }
                        DrawRequests.Clear();
                    }

                }
                this.d2dRenderTarget.Flush();
                d2dRenderTarget.EndDraw();
                swapChain.Present(0, PresentFlags.None);
            }
        }

        List<Action<RenderTarget>> DrawRequests = new List<Action<RenderTarget>>();
        private RawColor4 coveredColor(int color)
        {
            if (!useAlpha)
            {
                color = unchecked((int)(color | 0xff000000));
            }
            return ToRawColor4(System.Drawing.Color.FromArgb(color));
        }
        public void Clear(int color)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    target.Clear(coveredColor(color));
                });
            }

        }
        public System.Drawing.SizeF MeasureString(String s, System.Drawing.Font font = null, int maxsize = -1)
        {
            if (font == null) font = this.Font;
            using (SharpDX.DirectWrite.TextFormat format = new SharpDX.DirectWrite.TextFormat(FactoryDWrite, font.FontFamily.Name, WeightFromFontStyle(font.Style), StyleFromFontStyle(font.Style), font.Size))
            {
                if (maxsize == -1)
                {
                    maxsize = this.Width;
                }
                using (SharpDX.DirectWrite.TextLayout layout =
            new SharpDX.DirectWrite.TextLayout(FactoryDWrite, s, format, maxsize, font.Height))
                {
                    return new System.Drawing.SizeF(layout.Metrics.Width, layout.Metrics.Height);
                }

            }

        }

        public void drawRoundRect(int x, int y, int w, int h, float rad, int color, int penWidth = 1)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        RoundedRectangle rect = new RoundedRectangle();
                        rect.Rect = new RawRectangleF(x, y, x + w, y + h);
                        rect.RadiusX = rad;
                        rect.RadiusY = rad;
                        target.DrawRoundedRectangle(rect, brush, penWidth);
                    }
                });
            }
        }
        public void fillRoundRect(int x, int y, int w, int h, float radx, float rady, int color)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        dxFillRoundRect(target, x, y, w, h, radx, rady, brush);
                    }
                });
            }
        }
        public void fillRoundRect(int x, int y, int w, int h, float rad, int color)
        {
            fillRoundRect(x, y, w, h, rad, rad, color);
        }
        private void dxFillRoundRect(RenderTarget target, int x, int y, int w, int h, float radx, float rady, Brush brush)
        {
            RoundedRectangle rect = new RoundedRectangle();
            rect.Rect = new RawRectangleF(x, y, x + w, y + h);
            rect.RadiusX = radx;
            rect.RadiusY = rady;
            target.FillRoundedRectangle(rect, brush);
        }
        public void fillRoundRect(int x, int y, int w, int h, float radx, float rady, System.Drawing.Brush gdibrush)
        {
            lock (_drawReqLock)
            {
                var brush = BrushFromGDIBrush(this.d2dRenderTarget, gdibrush);
                DrawRequests.Add((target) =>
                {
                    dxFillRoundRect(target, x, y, w, h, radx, rady, brush);
                    brush.Dispose();
                });
            }
        }
        public void fillRoundRect(int x, int y, int w, int h, float rad, System.Drawing.Brush gdibrush)
        {
            fillRoundRect(x, y, w, h, rad, rad, gdibrush);
        }
        public void fillRoundRect(System.Drawing.Rectangle r, float rad, int color)
        {
            fillRoundRect(r.X, r.Y, r.Width, r.Height, rad, rad, color);
        }
        public void fillRoundRect(System.Drawing.Rectangle r, float rad, System.Drawing.Brush brush)
        {
            fillRoundRect(r.X, r.Y, r.Width, r.Height, rad, rad, brush);
        }
        public void fillRoundRect(System.Drawing.Point position, System.Drawing.Size size, float rad, int color)
        {
            fillRoundRect(position.X, position.Y, size.Width, size.Height, rad, rad, color);
        }
        public void fillRoundRect(System.Drawing.Point position, System.Drawing.Size size, float rad, System.Drawing.Brush brush)
        {
            fillRoundRect(position.X, position.Y, size.Width, size.Height, rad, rad, brush);
        }
        public void drawImage(System.Drawing.Bitmap bmp, int x, int y, int w, int h, float alpha)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    var mem = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                    var bitmapProperties = new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore));
                    using (Bitmap gameBitmap = new Bitmap(d2dRenderTarget, new Size2(bmp.Width, bmp.Height), bitmapProperties))
                    {
                        gameBitmap.CopyFromMemory(mem.Scan0, mem.Stride);
                        d2dRenderTarget.DrawBitmap(gameBitmap, new RawRectangleF(x, y, x + w, y + h), alpha, BitmapInterpolationMode.NearestNeighbor);
                    }
                    bmp.UnlockBits(mem);
                });
            }
        }
        public void drawImage(System.Drawing.Bitmap bmp, int x, int y, int w, int h)
        {
            drawImage(bmp, x, y, w, h, 1.0f);
        }
        private SharpDX.DirectWrite.FontStyle StyleFromFontStyle(System.Drawing.FontStyle style)
        {
            SharpDX.DirectWrite.FontStyle ret = SharpDX.DirectWrite.FontStyle.Normal;
            if (style.HasFlag(System.Drawing.FontStyle.Italic))
            {
                ret |= SharpDX.DirectWrite.FontStyle.Italic;
            }
            return ret;
        }
        private SharpDX.DirectWrite.FontWeight WeightFromFontStyle(System.Drawing.FontStyle style)
        {
            SharpDX.DirectWrite.FontWeight ret = SharpDX.DirectWrite.FontWeight.Normal;
            if (style.HasFlag(System.Drawing.FontStyle.Bold))
            {
                ret = SharpDX.DirectWrite.FontWeight.Bold;
            }
            return ret;
        }
        private SharpDX.Direct2D1.Brush GetBrushFromColor(RenderTarget target, int color)
        {
            SharpDX.Direct2D1.SolidColorBrush brush = new SolidColorBrush(target, coveredColor(color));
            return brush;
        }
        public void drawString(String str, System.Drawing.Rectangle rect, int color, System.Drawing.Font font = null)
        {

            lock (_drawReqLock)
            {
                if (font == null)
                {
                    font = this.Font;
                }
                SharpDX.DirectWrite.FontWeight weight = WeightFromFontStyle(font.Style);
                SharpDX.DirectWrite.FontStyle style = StyleFromFontStyle(font.Style);
                SharpDX.DirectWrite.TextFormat stringFormat = new SharpDX.DirectWrite.TextFormat(this.FactoryDWrite, font.FontFamily.Name, weight, style, font.Size);
                this.DrawRequests.Add((target) =>
                {
                    if (!String.IsNullOrEmpty(str))
                    {
                        using (Brush brush = GetBrushFromColor(target, color))
                        {
                            stringFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                            stringFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
                            target.DrawText(str, stringFormat, new RawRectangleF(rect.X, rect.Y, rect.Right, rect.Bottom), brush);
                            stringFormat.Dispose();
                        }
                    }
                });

            }
        }
        public void drawLine(int x0, int y0, int x1, int y1, int color, int width)
        {
            lock (_drawReqLock)
            {

                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        target.DrawLine(new RawVector2(x0, y0), new RawVector2(x1, y1), brush, width);
                    }
                });
            }
        }
        public void drawEllipse(int x, int y, int w, int h, int color)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        Ellipse ell = new Ellipse(new RawVector2(x + w / 2, y + h / 2), w, h);
                        target.DrawEllipse(ell, brush);
                    }
                });
            }
        }
        private Brush BrushFromGDIBrush(RenderTarget target, System.Drawing.Brush brush)
        {

            if (brush is System.Drawing.Drawing2D.LinearGradientBrush)
            {
                System.Drawing.Drawing2D.LinearGradientBrush linear = (System.Drawing.Drawing2D.LinearGradientBrush)brush;
                LinearGradientBrushProperties prop = new LinearGradientBrushProperties();
                prop.StartPoint = new RawVector2(linear.Rectangle.Left, linear.Rectangle.Top);
                prop.EndPoint = new RawVector2(linear.Rectangle.Right, linear.Rectangle.Bottom);

                GradientStop[] stops = null;
                stops = new GradientStop[linear.InterpolationColors.Colors.Length];
                for (int i = 0; i < stops.Length; ++i)
                {
                    stops[i].Color = ToRawColor4(linear.InterpolationColors.Colors[i]);
                    stops[i].Position = linear.InterpolationColors.Positions[i];
                }
                GradientStopCollection stopCollection = new GradientStopCollection(target, stops, ExtendMode.Clamp);
                LinearGradientBrush ret = new LinearGradientBrush(target, prop, stopCollection);
                return ret;
            }
            else if (brush is System.Drawing.SolidBrush)
            {
                System.Drawing.SolidBrush solid = (System.Drawing.SolidBrush)brush;
                return GetBrushFromColor(target, solid.Color.ToArgb());
            }
            else
            {
                return GetBrushFromColor(target, 0);
            }
        }
        public void fillEllipse(int x, int y, int w, int h, int color)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        dxFillEllipse(target, x, y, w, h, brush);
                    }
                });
            }

        }
        public void fillEllipse(int x, int y, int w, int h, System.Drawing.Brush gdibrush)
        {
            lock (_drawReqLock)
            {
                Brush brush = BrushFromGDIBrush(this.d2dRenderTarget, gdibrush);
                DrawRequests.Add((target) =>
                {
                    dxFillEllipse(target, x, y, w, h, brush);
                    brush.Dispose();
                });
            }
        }
        private void dxFillEllipse(SharpDX.Direct2D1.RenderTarget target, int x, int y, int w, int h, Brush brush)
        {
            Ellipse ell = new Ellipse(new RawVector2(x + w / 2, y + h / 2), w, h);
            target.FillEllipse(ell, brush);
        }
        public void drawCircle(int n_cx, int n_cy, int radius, int pixel)
        {
            int halfRadius = radius / 2;
            drawEllipse(n_cx - halfRadius, n_cy - halfRadius, radius / 2, radius / 2, pixel);
        }
        public void fillCircle(int n_cx, int n_cy, int radius, int pixel)
        {
            int halfRadius = radius / 2;
            fillEllipse(n_cx - halfRadius, n_cy - halfRadius, radius, radius, pixel);
        }
        public void fillCircle(int n_cx, int n_cy, int radius, System.Drawing.Brush gdibrush)
        {
            int halfRadius = radius / 2;
            fillEllipse(n_cx - halfRadius, n_cy - halfRadius, radius, radius, gdibrush);
        }
        public void drawPixel(int x, int y, int color)
        {
            fillRect(x, y, 1, 1, color);
        }
        StrokeStyle mDashedStyle;
        StrokeStyle GetDashedStyle(RenderTarget target)
        {
            if (mDashedStyle == null)
            {
                StrokeStyleProperties prop = new StrokeStyleProperties();
                prop.DashStyle = DashStyle.DashDotDot;
                prop.DashOffset = 2.0f;
                mDashedStyle = new StrokeStyle(target.Factory, prop);
            }
            return mDashedStyle;
        }
        public void drawRect(int x, int y, int w, int h, int color, bool dashed = false, int width = 1)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (Brush brush = GetBrushFromColor(target, color))
                    {
                        if (dashed)
                        {
                            target.DrawRectangle(new RawRectangleF(x, y, x + w, y + h), brush, width, GetDashedStyle(target));
                        }
                        else
                        {
                            target.DrawRectangle(new RawRectangleF(x, y, x + w, y + h), brush, width);
                        }
                    }
                });
            }
        }
        public void fillRect(int x, int y, int w, int h, int color)
        {
            lock (_drawReqLock)
            {
                DrawRequests.Add((target) =>
                {
                    using (var brush = GetBrushFromColor(target, color))
                    {
                        dxFillRect(target, x, y, w, h, brush);
                    }
                });
            }
        }
        public void fillRect(System.Drawing.Rectangle r, int color)
        {
            fillRect(r.X, r.Y, r.Width, r.Height, color);
        }
        public void fillRect(System.Drawing.Point position, System.Drawing.Size size, int color)
        {
            fillRect(position.X, position.Y, size.Width, size.Height, color);
        }
        public void fillRect(System.Drawing.Rectangle r, System.Drawing.Brush linearGradient)
        {
            fillRect(r.X, r.Y, r.Width, r.Height, ((System.Drawing.SolidBrush)linearGradient).Color.ToArgb());
        }
        private void dxFillRect(SharpDX.Direct2D1.RenderTarget target, int x, int y, int w, int h, Brush brush)
        {
            target.FillRectangle(new RawRectangleF(x, y, x + w, y + h), brush);
        }
        public void fillRect(System.Drawing.Point position, System.Drawing.Size size, System.Drawing.Brush gdibrush)
        {
            fillRect(position.X, position.Y, size.Width, size.Height, gdibrush);
        }
        public void fillRect(int x, int y, int w, int h, System.Drawing.Brush gdibrush)
        {
            lock (_drawReqLock)
            {
                Brush brush = BrushFromGDIBrush(d2dRenderTarget, gdibrush);
                DrawRequests.Add((target) =>
                {
                    dxFillRect(target, x, y, w, h, brush);
                    brush.Dispose();
                });
            }
        }
        public void drawString(String str, int x, int y, int color, System.Drawing.Font font = null)
        {
            drawString(str, new System.Drawing.Rectangle(x, y, this.Width, this.Height), color, font);
        }
        public void drawPixels(int[] pixels, int x, int y, int w, int h)
        {
            var bitmapProperties = new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore));
            using (Bitmap gameBitmap = new Bitmap(d2dRenderTarget, new Size2(w, h), bitmapProperties))
            {
                gameBitmap.CopyFromMemory(pixels, w * 4);
                d2dRenderTarget.DrawBitmap(gameBitmap, clientArea, 1.0f, BitmapInterpolationMode.NearestNeighbor);
            }
        }
        public System.Drawing.Bitmap flushToBMP()
        {
            this.Invalidate();
            this.Update();
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bmp, this.ClientRectangle);
            return bmp;
        }
        public System.Drawing.Bitmap flushToBMP(int left, int top, int w, int h)
        {
            using (var bmp = flushToBMP())
            {
                return bmp.Clone(new System.Drawing.Rectangle(left, top, w, h), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
        }
        public void flush()
        {

            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            //base.OnPaint(e);
            Draw();

            e.Graphics.Flush();
        }

    }
}