﻿using SharpNes.Audio;
using SharpNes.Diagnostics;
using SharpNes.Input;
using SharpNes.Settings;
using NAudio.Wave;
using NesCore.Input;
using NesCore.Storage;
using NesCore.Video;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NesCore.Utility;
using System.Globalization;
using BumpKit;
using AForge.Video.FFMPEG;
using SharpNes.Cheats;
using SharpNes.Database;
using System.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpNes
{
    public partial class MainForm : DockContent
    {
        public MainForm(bool closeWithApp=true,bool hideMenu=false,bool hideStatusBar=false)
        {
            InitializeComponent();
            if (closeWithApp)
            {
                this.FormClosing += OnApplicationClosing;
            }
            else
            {
                this.FormClosed += MainForm_FormClosed;
                this.SizeChanged += MainForm_SizeChanged;
            }
            if (hideMenu)
            {
                this.viewScreenSizeFullScreenMenuItem.Enabled = false;
                this.MainMenuStrip.Hide();
            }
            if (hideStatusBar)
            {
                this.statusStrip.Hide();
            }
            videoPanel.Paint += OnVideoPanelPaint;

            // enabled DoubleBuffered property (protected)
            System.Reflection.PropertyInfo aProp =
               typeof(System.Windows.Forms.Control).GetProperty(
                 "DoubleBuffered",
                 System.Reflection.BindingFlags.NonPublic |
                 System.Reflection.BindingFlags.Instance);
            aProp.SetValue(videoPanel, true, null);

            ConfigureInput();

            Console = new NesCore.Console();

            ConfigureProcessor();
            ConfigureVideo();
            ConfigureAudio();
            ConfigureControllers();
            ConfigureNstDatabase();

            gameState = GameState.Stopped;

            // cheat system
            cheatSystem = new CheatSystem(Console.Processor);
        }

        void MainForm_SizeChanged(object sender, EventArgs e)
        {
            int ratio = (int)Math.Floor(((double)this.Width) / 256.0);
            if (ratio <= 0) ratio = 1;
            SetScreenSize((byte)ratio);
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.mWorker != null)
                {
                    this.mWorker.CancelAsync();
                }
            }
            catch (Exception ee)
            {

            }
        }
#if false
        /// <summary>
        /// Windows message handler
        /// </summary>
        /// <param name="message">Windows message to process</param>
        protected override void WndProc(ref Message message)
        {
            // check if screen saver message dispatched
            if (message.Msg == WM_SYSCOMMAND && (int)message.WParam == SC_SCREENSAVE)
            {
                // if game is running, suppress screen saver request
                if (gameState == GameState.Running)
                {
                    message.Result = INVALID_HANDLE_VALUE;
                    return;
                }
            }

            // fall back to default message processor
            base.WndProc(ref message);
        }
#endif
        private void OnFormLoad(object sender, EventArgs eventArgs)
        {
            recentFileManager = new RecentFileManager(
                fileRecentFilesMenuItem,
                LoadRecentRom, null,
                Properties.Resources.FileOpen);

            applicationMargin = new Size(Width - videoPanel.ClientSize.Width, Height - videoPanel.ClientSize.Height);
            SetScreen(1, true);
            SetScreenFilter(ScreenFilter.None);
            SetMotionBlur(false);
            viewScreenSizeFullScreenMenuItem.ShortcutKeys = Keys.Alt | Keys.Enter;

            // make space for checkboxes (disabled due to icons)
            ((ToolStripDropDownMenu)viewMenuItem.DropDown).ShowCheckMargin = true;

            codeDisassemblyForm = new CodeDisassemblyForm(Console);

            //Application.Idle += TickWhileIdle;
            mWorker = new BackgroundWorker();
            mWorker.WorkerSupportsCancellation = true;
            mWorker.DoWork += TickWhileIdle;
            mWorker.RunWorkerAsync();
        }
        BackgroundWorker mWorker = new BackgroundWorker();
        private void OnFormDeactivate(object sender, EventArgs eventArgs)
        {
           // if (gameState == GameState.Running)
           //     OnGameRunPause(sender, eventArgs);
        }

        private void OnFormMove(object sender, EventArgs eventArgs)
        {
           // if (gameState == GameState.Running)
           //     OnGameRunPause(sender, eventArgs);
        }
        public void Pause()
        {
            OnGameRunPause(this, EventArgs.Empty);
        }
        private void OnApplicationClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = MessageBox.Show(
                this, "Are you sure?", "Exit " + Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;

            if (cancelEventArgs.Cancel)
                return;

            // save previous SRAM if applicable
            if (this.cartridge != null && this.cartridge.SaveRam.Modified)
                StoreSaveRam();
        }

        private void OnDragEnter(object sender, DragEventArgs dragEventArgs)
        {
            if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                dragEventArgs.Effect = DragDropEffects.Copy;
        }

        private void OnDragDrop(object sender, DragEventArgs dragEventArgs)
        {
            string[] files = (string[])dragEventArgs.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 1)
            {
                MessageBox.Show(this, "Please drop only one ROM file at a time", "Open ROM file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string cartridgeRomPath = files[0];
            LoadCartridgeRom(cartridgeRomPath);
        }

        private void OnFileOpen(object sender, EventArgs eventArgs)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.ExecutablePath;
            openFileDialog.Filter = "NES ROM files (*.nes, *.zip)|*.nes;*.zip|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Open Game ROM";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            LoadCartridgeRom(openFileDialog.FileName);
        }

        private void OnFileProperties(object sender, EventArgs eventArgs)
        {
            if (cartridge == null)
                return;

            string cartridgeProperties
                = "Program ROM Size: " + KiloBytes.Format(cartridge.ProgramRom.Count) + "\r\n"
                + "Character ROM Size: " + KiloBytes.Format(cartridge.CharacterRom.Length) + "\r\n"
                + "Mapper ID: " + cartridge.MapperId + " (" + cartridge.Map.Name + ")\r\n"
                + "Initial Mirroring Mode: " + cartridge.MirrorMode + "\r\n"
                + "Battery Present: " + (cartridge.BatteryPresent ? "Yes" : "No")
                + "\r\nCRC: " + Hex.Format(cartridge.Crc).Replace("$", "");

            MessageBox.Show(this, cartridgeProperties, this.cartridgeRomFilename + " Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnFileExit(object sender, EventArgs eventArgs)
        {
            Application.Exit();
        }

        private void OnGameRunPause(object sender, EventArgs eventArgs)
        {
            switch (gameState)
            {
                case GameState.Stopped:
                    // run
                    Console.Reset();
                    gameTickDateTime = DateTime.Now;
                    averageDeltaTime = 1.0 / 60.0;
                    apuAudioProvider.Enabled = true;
                    gameState = GameState.Running;
                    waveOut.Play();
                    break;
                case GameState.Running:
                    // pause
                    apuAudioProvider.Enabled = false;
                    gameState = GameState.Paused;
                    Console.Halt();
                    break;
                case GameState.Paused:
                    // resume;
                    gameTickDateTime = DateTime.Now;
                    averageDeltaTime = 1.0 / 60.0;
                    apuAudioProvider.Enabled = true;
                    gameState = GameState.Running;
                    Console.Continue();
                    break;
            }

            videoPanel.Invalidate();
            UpdateGameMenuItems();
            UpdateRecordMenuItems();
        }

        private void OnGameReset(object sender, EventArgs eventArgs)
        {
            OnGameStop(sender, eventArgs);
            Console.Reset();
            OnGameRunPause(sender, eventArgs);
        }

        private void OnGameStop(object sender, EventArgs eventArgs)
        {
            if (gameState == GameState.Stopped)
                return;

            videoPanel.Invalidate();
            gameState = GameState.Stopped;
            waveOut.Stop();

            // save previous SRAM if applicable
            if (this.cartridge.SaveRam.Modified)
                StoreSaveRam();

            // stop recording if active
            if (videoRecording)
                OnRecordVideoMp4StartStop(this, EventArgs.Empty);

            UpdateGameMenuItems();
            UpdateRecordMenuItems();
        }

        private void OnViewScreenSizeX1(object sender, EventArgs eventArgs)
        {
            SetScreenSize(1);
        }

        private void OnViewScreenSizeX2(object sender, EventArgs eventArgs)
        {
            SetScreenSize(2);
        }

        private void OnViewScreenSizeX3(object sender, EventArgs eventArgse)
        {
            SetScreenSize(3);
        }

        private void OnViewScreenSizeX4(object sender, EventArgs eventArgs)
        {
            SetScreenSize(4);
        }

        private void OnViewScreenSizeFullScreen(object sender, EventArgs e)
        {
            if(this.viewScreenSizeFullScreenMenuItem.Enabled)
                SetFullScreen(!fullScreen);
        }

        private void OnViewTvAspect(object sender, EventArgs eventArgs)
        {
            SetTvAspect(!tvAspect);
        }

        private void OnViewScreenFilter(object sender, EventArgs e)
        {
            int filterCount = Enum.GetValues(typeof(ScreenFilter)).Length;
            screenFilter = (ScreenFilter)(((int)screenFilter + 1) % filterCount);
            SetScreenFilter(screenFilter);
        }

        private void OnVewScreenFilterItem(object sender, EventArgs eventArgs)
        {
            int filterIndex = 0;
            foreach (ToolStripMenuItem filterMenuItem in viewScreenFilterMenuItem.DropDownItems)
            {
                if (filterMenuItem == sender)
                {
                    SetScreenFilter((ScreenFilter)filterIndex);
                    return;
                }
                ++filterIndex;
            }
        }

        private void OnViewMotionBlur(object sender, EventArgs eventArgs)
        {
            SetMotionBlur(!motionBlur);
        }

        private void OnViewNoSpriteOverflow(object sender, EventArgs eventArgs)
        {
            Console.Video.NoSpriteOverflow = !Console.Video.NoSpriteOverflow;
            viewNoSpriteOverflowMenuItem.Checked = Console.Video.NoSpriteOverflow;
        }

        private void OnOptionsInput(object sender, EventArgs eventArgs)
        {
            InputOptionsForm inputOptionsForm = new InputOptionsForm(Console, keyboardState, mouseState, gameControllerManager);
            inputOptionsForm.ShowDialog(this);
        }

        private void OnOptionsCheats(object sender, EventArgs eventArgs)
        {
            CheatsForm cheatsForm = new CheatsForm(Console.Memory,  this.cheatSystem);
            cheatsForm.ShowDialog(this);
        }
        public void StepOver()
        {
            Console.StepOver();
        }

        private void OnRecordVideoMp4StartStop(object sender, EventArgs eventArgs)
        {
            if (videoRecording)
            {
                // stop
                videoRecording = false;
                videoRecordingMp4 = false;

                mp4VideoEncoder.Close();
                mp4VideoEncoder = null;

                recordVideoMp4StartStopMenuItem.Text = "&Start";
                recordVideoGifMenuItem.Enabled = true;
            }
            else // not recording
            {
                // start
                if (gameState == GameState.Stopped)
                    return;

                if (File.Exists(videoPathMp4))
                    File.Delete(videoPathMp4);

                videoRecording = true;
                videoRecordingMp4 = true;

                mp4VideoEncoder = new VideoFileWriter();
                mp4VideoEncoder.Open(videoPathMp4, bitmapBuffer.Width, bitmapBuffer.Height, 60, VideoCodec.MPEG4);

                recordVideoMp4StartStopMenuItem.Text = "&Stop";
                recordVideoGifMenuItem.Enabled = false;
            }
        }

        private void OnRecordVideoGifStartStop(object sender, EventArgs eventArgs)
        {
            if (videoRecording)
            {
                // stop
                videoRecording = false;
                videoRecordingGif = false;

                gifVideoEncoder.Dispose();
                gifVideoEncoder = null;
        
                recordVideoGifStartStopMenuItem.Text = "&Start";
                recordVideoMp4MenuItem.Enabled = true;
            }
            else // not recording
            {
                // start
                if (gameState == GameState.Stopped)
                    return;

                if (File.Exists(videoPathGif))
                    File.Delete(videoPathGif);

                videoRecording = true;
                videoRecordingGif = true;

                gifVideoEncoder = new GifEncoder(File.OpenWrite(videoPathGif));
                
                recordVideoGifStartStopMenuItem.Text = "&Stop";
                recordVideoMp4MenuItem.Enabled = false;
            }
        }

        private void OnDiagnosticsCodeDisassembly(object sender, EventArgs eventArgs)
        {
            traceEnabled = !traceEnabled;
            diagnosticsCodeDisassemblyMenuItem.Checked = traceEnabled;

            if (traceEnabled)
            {
                if (OnDisassemblerShown != null)
                {
                    OnDisassemblerShown(this, codeDisassemblyForm);
                }
                else
                {
                    codeDisassemblyForm.Show(this);
                }
                Console.Processor.Trace = () => codeDisassemblyForm.Trace();
                if (Console.Cartridge != null)
                {
                    Console.Cartridge.Map.ProgramBankSwitch
                        = (address, size) => codeDisassemblyForm.InvalidateMemoryRange(address, size);
                }
            }
            else
            {
                codeDisassemblyForm.Hide();
                Console.Processor.Trace = null;
                if (Console.Cartridge != null)
                {
                    Console.Cartridge.Map.ProgramBankSwitch = null;
                }
            }
            this.Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            keyboardState[keyEventArgs.KeyCode] = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            keyboardState[keyEventArgs.KeyCode] = false;

            // custom shurtcut code as menu item has dropdown items
            if (keyEventArgs.Control && keyEventArgs.KeyCode == Keys.F)
                OnViewScreenFilter(this, EventArgs.Empty);
        }

        void TickWhileIdle(object sender, EventArgs eventArgs)
        {
            while (!this.IsDisposed && mWorker != null && !mWorker.CancellationPending)
            {
                try
                {
                    OnGameTick(sender, eventArgs);
                }
                catch (Exception ee)
                {
                    System.Console.WriteLine(ee.ToString());
                }
                Thread.Sleep(1);
            }
        }

        private void OnGameTick(object sender, EventArgs eventArgs)
        {
            gameControllerManager.UpdateState();

            if (gameState != GameState.Running)
                return;

#if false
            Console.Run(0.020);
#else
            DateTime currentTickDateTime = DateTime.Now;
            double tickDelta = (currentTickDateTime - gameTickDateTime).TotalSeconds;
            gameTickDateTime = currentTickDateTime;

            Console.Run(tickDelta, mWorker);
#endif
        }

        private void OnIconTick(object sender, EventArgs eventArgs)
        {
            /*
            if (gameState != GameState.Running)
            {
                if (this.Icon != Properties.Resources.Nes)
                    this.Icon = Properties.Resources.Nes;
                return;
            }

            // update application icon with running game
            if (gameIcon != null)
                DestroyIcon(gameIcon.Handle);

            Bitmap bitmapIcon = ResizeImage(bitmapBuffer.Bitmap, 32, 32);
            gameIcon = Icon.FromHandle(bitmapIcon.GetHicon());
            bitmapIcon.Dispose();
            this.Icon = gameIcon;
             */
        }

        private void OnVideoPanelMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (gameState == GameState.Paused)
                OnGameRunPause(sender, EventArgs.Empty);

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                mouseState.LeftButtonPressed = true;
            }
        }

        private void OnVideoPanelMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                mouseState.LeftButtonPressed = false;
            }
        }

        private void OnVideoPanelMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            mouseState.Position = mouseEventArgs.Location;
            int x = mouseState.Position.X;
            int y = mouseState.Position.Y;

            x = x * 256 / bufferSize.Width;
            y = y * 240 / bufferSize.Height;

            if (x < 0 && x >= 256 || y < 0 || y >= 240)
            {
                mouseState.SensePixel = false;
                return;
            }

            int off = y * 256 + x;
            off *= 4;
            if (off + 2 < bitmapBuffer.Bits.Length)
            {
                byte r = bitmapBuffer.Bits[off];
                byte g = bitmapBuffer.Bits[off + 1];
                byte b = bitmapBuffer.Bits[off + 2];
                mouseState.SensePixel = r + g + b > 384;
            }
        }

        private void OnVideoPanelPaint(object sender, PaintEventArgs paintEventArgs)
        {
            Graphics graphics = paintEventArgs.Graphics;
            int leftCenteredMargin = (videoPanel.Width - bufferSize.Width) / 2;

            graphics.InterpolationMode = InterpolationMode.Low;
            switch (gameState)
            {
                case GameState.Stopped:
                    graphics.InterpolationMode = InterpolationMode.Low;
                    graphics.DrawImage(Properties.Resources.Background, 0, 0, videoPanel.Width, videoPanel.Height);
                    break;
                case GameState.Paused:
                    graphics.DrawImage(bitmapBuffer.Bitmap, leftCenteredMargin, 0, bufferSize.Width, bufferSize.Height);
                    /*graphics.DrawImage(Properties.Resources.FilterPause, 0, 0, videoPanel.Width, videoPanel.Height);
                    DrawCenteredText(graphics, "Paused");*/
                    break;
                case GameState.Running:
                    if (screenFilter != ScreenFilter.None)
                    {
                        graphics.DrawImage(bitmapBuffer.Bitmap, leftCenteredMargin, 0, bufferSize.Width, bufferSize.Height);
                        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        graphics.DrawImage(resizedScreenFilter, leftCenteredMargin, 0);
                    }
                    else
                    {
                        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        graphics.DrawImage(bitmapBuffer.Bitmap, leftCenteredMargin, 0, bufferSize.Width, bufferSize.Height);
                    }

                    // video recording indicator
                    if (videoRecording && gameTickDateTime.Second % 2 == 0)
                    {
                        graphics.FillEllipse(Brushes.Red, leftCenteredMargin + bufferSize.Width - 24, bufferSize.Height - 24, 16, 16);
                    }

                    break;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void DrawCenteredText(Graphics graphics, string text)
        {
            if (resizedCaptionFont == null)
                resizedCaptionFont = new Font(this.Font.FontFamily, videoPanel.Height / 10, FontStyle.Italic | FontStyle.Bold, GraphicsUnit.Pixel);

            SizeF textSize = graphics.MeasureString(text, resizedCaptionFont);

            int outlineThickness = videoPanel.Height / 120;
            textSize.Width += outlineThickness;
            textSize.Height += outlineThickness;

            float textX = (videoPanel.Width - textSize.Width) / 2;
            float textY = (videoPanel.Height - textSize.Height) / 2;

            Rectangle pathRectangle = new Rectangle((int)textX, (int)textY, (int)textSize.Width, (int)textSize.Height);

            using (GraphicsPath graphicsPath = new GraphicsPath())
            using (Pen outlinePen = new Pen(Color.Black, outlineThickness) { LineJoin = LineJoin.Round })
            using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(pathRectangle, Color.White, Color.LightSkyBlue, 90f))
            {
                graphicsPath.AddString("Paused", resizedCaptionFont.FontFamily, (int)resizedCaptionFont.Style,
                    resizedCaptionFont.Size, pathRectangle, StringFormat.GenericDefault);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawPath(outlinePen, graphicsPath);
                graphics.FillPath(linearGradientBrush, graphicsPath);
            }
        }

        private void UpdateGameMenuItems()
        {
            switch (gameState)
            {
                case GameState.Stopped:
                    gameRunMenuItem.Text = "&Run";
                    gameRunMenuItem.Image = Properties.Resources.GameRun;
                    gameRunMenuItem.ShortcutKeys = Keys.Control | Keys.R;
                    break;
                case GameState.Running:
                    gameRunMenuItem.Text = "&Pause";
                    gameRunMenuItem.Image = Properties.Resources.GamePause;
                    gameRunMenuItem.ShortcutKeys = Keys.Control | Keys.P;
                    break;
                case GameState.Paused:
                    gameRunMenuItem.Text = "&Resume";
                    gameRunMenuItem.Image = Properties.Resources.GameRun;
                    gameRunMenuItem.ShortcutKeys = Keys.Control | Keys.R;
                    break;
            }
            gameStopMenuItem.Enabled = gameState != GameState.Stopped;
        }

        private void UpdateRecordMenuItems()
        {
            recordMenuItem.Enabled = gameState == GameState.Running;
        }

        private void ConfigureInput()
        {
            // map for key states to help controller mapping
            this.keyboardState = new KeyboardState();

            // map for mouse states to help controller mapping
            this.mouseState = new MouseState();

            // set up joystick object before console
            this.gameControllerManager = new GameControllerManager();
        }
        public const bool SkipIlligalOpCode = true;
        private void ConfigureProcessor()
        {
            Console.Processor.Lockup = () =>
            {
                if (SkipIlligalOpCode)
                {
                    System.Console.Error.WriteLine("Processor Error");
                    System.Console.Error.WriteLine("KIL instruction enountered at address " +
                        Hex.Format(Console.Processor.State.ProgramCounter) +
                        ". There may be a problem with the ROM or a software bug in the emulator. ROM Details: " + cartridge.ToString());
                }
                else
                {
                    Console.Halt();
                    OnGameStop(this, EventArgs.Empty);
                    MessageBox.Show(this,
                        "KIL instruction enountered at address " +
                        Hex.Format(Console.Processor.State.ProgramCounter) +
                        ". There may be a problem with the ROM or a software bug in the emulator. ROM Details: " + cartridge.ToString(),
                        "Processor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void ConfigureVideo()
        {
            bitmapBuffer = new FastBitmap(256, 240);

            Console.Video.WritePixel = (x, y, colour, pixelType) =>
            {
                // do not render top and bottom 8 rows
                if (y < 8 || y > 231)
                    colour = Colour.Black;

                int offset = (y * 256 + x) * 4;

                if (motionBlur)
                {
                    float delta = (colour.Blue - bitmapBuffer.Bits[offset]) / 2f;
                    if (Math.Abs(delta) < 1) delta = Math.Sign(delta);
                    bitmapBuffer.Bits[offset++] += (byte)delta;
                    delta = (colour.Green - bitmapBuffer.Bits[offset]) / 2f;
                    if (Math.Abs(delta) < 1) delta = Math.Sign(delta);
                    bitmapBuffer.Bits[offset++] += (byte)delta;
                    delta = (colour.Red - bitmapBuffer.Bits[offset]) / 2f;
                    if (Math.Abs(delta) < 1) delta = Math.Sign(delta);
                    bitmapBuffer.Bits[offset] += (byte)delta;
                }
                else
                {
                    bitmapBuffer.Bits[offset++] = colour.Blue;
                    bitmapBuffer.Bits[offset++] = colour.Green;
                    bitmapBuffer.Bits[offset] = colour.Red;
                }
            };

            Console.Video.ShowFrame = () =>
            {
                videoPanel.Invalidate();

                // frame rate
                DateTime currentDateTime = DateTime.Now;
                double currentDeltaTime = (currentDateTime - frameDateTime).TotalSeconds;
                frameDateTime = currentDateTime;
                averageDeltaTime = averageDeltaTime * 0.9 + currentDeltaTime * 0.1;
                int frameRate = (int)(1.0 / averageDeltaTime);
                frameRateStatusLabel.Text = frameRate + " FPS";

                // MP4 recording
                if (videoRecordingMp4)
                    mp4VideoEncoder.WriteVideoFrame(bitmapBuffer.Bitmap);

                // GIF recording
                if (videoRecordingGif)
                    gifVideoEncoder.AddFrame(bitmapBuffer.Bitmap, 0, 0, TimeSpan.FromMilliseconds(16));

            };
        }

        private void ConfigureAudio()
        {
            Console.Audio.SampleRate = 44100;

            float[] outputBuffer = new float[256];
            int writeIndex = 0;

            apuAudioProvider = new ApuAudioProvider();

            Console.Audio.WriteSample = (sampleValue) =>
            {
                // fill buffer
                outputBuffer[writeIndex++] = sampleValue;
                writeIndex %= outputBuffer.Length;

                // when buffer full, send to wave provider
                if (writeIndex == 0)
                    apuAudioProvider.Queue(outputBuffer);
            };

            waveOut = new WaveOut();
            waveOut.DesiredLatency = 100;

            waveOut.Init(apuAudioProvider);

        }

        private void ConfigureControllers()
        {
            // load input settings
            InputSettings inputSettings = Properties.Settings.Default.InputSettings;

            // if never set, create and save
            if (inputSettings == null)
            {
                inputSettings = new InputSettings();
                Properties.Settings.Default.InputSettings = inputSettings;
                Properties.Settings.Default.Save();
            }

            // if no joypad set, configure default keyboard controls
            if (inputSettings.Joypads.Count == 0)
            {
                inputSettings.BuildDefaultSettings();
                Properties.Settings.Default.InputSettings = inputSettings;
                Properties.Settings.Default.Save();
            }

            // assign joypads
            foreach (JoypadSettings joyPadSettings in inputSettings.Joypads)
                Console.ConnectController(joyPadSettings.Port,
                    joyPadSettings.ConfigureJoypad(keyboardState, gameControllerManager));

            // assign zappers
            foreach (ZapperSettings zapperSettings in inputSettings.Zappers)
                Console.ConnectController(zapperSettings.Port,
                    zapperSettings.ConfigureZapper(mouseState));
        }

        private void ConfigureNstDatabase()
        {
            nstDatabase = new NstDatabase();

            Cartridge.DetermineMapperId = DetermineCartridgeMapperId;
        }

        private void LoadRecentRom(object sender, EventArgs eventArgs)
        {
            string cartridgeRomPath = ((ToolStripMenuItem)sender).Text;
            if (!LoadCartridgeRom(cartridgeRomPath))
                recentFileManager.RemoveRecentFile(cartridgeRomPath);
        }
        public void Open(String filename)
        {
            LoadCartridgeRom(filename);
        }
        private bool LoadCartridgeRom(string cartridgeRomPath)
        {
            try
            {
                // save previous SRAM if applicable
                if (this.cartridge != null && this.cartridge.SaveRam.Modified)
                    StoreSaveRam();

                Stream cartridgeRomStream = GetCartridgeRomStream(cartridgeRomPath);

                BinaryReader romBinaryReader = new BinaryReader(cartridgeRomStream);
                Cartridge newCartridge = new Cartridge(romBinaryReader);
                romBinaryReader.Close();

                // load SRAM if file exists
                this.cartridgeSaveRamFilename = ReplaceRomExtension(cartridgeRomPath, ".sram");

                if (File.Exists(this.cartridgeSaveRamFilename))
                {
                    BinaryReader saveRamBinaryReader = new BinaryReader(new FileStream(this.cartridgeSaveRamFilename, FileMode.Open));
                    newCartridge.SaveRam.Load(saveRamBinaryReader);
                    saveRamBinaryReader.Close();
                    statusHistoryLabel.Text = "SaveRam file detected and loaded";
                }

                Console.LoadCartridge(newCartridge);

                this.cartridge = newCartridge;

                this.cartridgeRomFilename = Path.GetFileNameWithoutExtension(cartridgeRomPath);

                this.Text = cartridgeRomFilename + " - " + Application.ProductName;

                // enable some menu items dependent on a loaded cartridge
                filePropertiesMenuItem.Enabled = true;
                gameMenuItem.Enabled = true;
                optionsCheatsMenuItem.Enabled = true;

                if (traceEnabled)
                {
                    Console.Cartridge.Map.ProgramBankSwitch = 
                        (address, size) => codeDisassemblyForm.InvalidateMemoryRange(address, size);
                }

                OnGameStop(this, EventArgs.Empty);
                OnGameRunPause(this, EventArgs.Empty);

                recentFileManager.AddRecentFile(cartridgeRomPath);

                // set video file paths
                this.videoPathMp4 = ReplaceRomExtension(cartridgeRomPath, ".mp4");
                this.videoPathGif = ReplaceRomExtension(cartridgeRomPath, ".gif");

                // set cursor to crosshairs if it is a gun-based game
                cartridgeUsesGun = false;
                NstDatabaseEntry nstDatabaseEntry = nstDatabase[cartridge.Crc];
                if (nstDatabaseEntry != null && nstDatabaseEntry.Peripherals.Contains(Peripheral.Zapper))
                {
                    cartridgeUsesGun = true;
                }
                videoPanel.Cursor = cartridgeUsesGun ? Cursors.Cross : Cursors.Default;

                // load cheat file if present
                cheatSystem.Clear();
                string cheatFilePath = ReplaceRomExtension(cartridgeRomPath, ".cht");
                if (File.Exists(cheatFilePath))
                {
                    cheatSystem.Load(cheatFilePath);
                    statusHistoryLabel.Text = "Cheat file loaded";
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Unable to load cartridge rom. Reason: " + exception.Message, "Open Game ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string ReplaceRomExtension(string cartridgeRomPath, string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            return cartridgeRomPath.Replace(".nes", extension).Replace(".zip", extension);
        }

        private void StoreSaveRam()
        {
            BinaryWriter saveRamBinaryWriter = new BinaryWriter(new FileStream(this.cartridgeSaveRamFilename, FileMode.OpenOrCreate, FileAccess.Write));
            this.cartridge.SaveRam.Save(saveRamBinaryWriter);
            saveRamBinaryWriter.Close();
            statusHistoryLabel.Text = "SaveRam saved to disk";

        }

        /// <summary>
        /// Tries to get a stream to a cartridge rom file using the given path.
        /// If the file is a ZIP archive, the a stream to the first NES file is
        /// returned.
        /// </summary>
        /// <param name="cartridgeRomPath">Path to NES or ZIP file</param>
        /// <returns></returns>
        private Stream GetCartridgeRomStream(string cartridgeRomPath)
        {
            Stream cartridgeRomStream = null;

            if (Path.GetExtension(cartridgeRomPath).ToLower() == ".zip")
            {
                Stream zipStream = new FileStream(cartridgeRomPath, FileMode.Open);
                ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

                // find first nes file
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    if (entry.FullName.EndsWith(".nes", StringComparison.OrdinalIgnoreCase))
                    {
                        cartridgeRomStream = entry.Open();
                        break;
                    }
                }
            }

            if (cartridgeRomStream == null)
                cartridgeRomStream = new FileStream(cartridgeRomPath, FileMode.Open);

            return cartridgeRomStream;
        }

        private void SetScreenSize(byte newScreenSize)
        {
            SetScreen(newScreenSize, tvAspect);
        }

        private void SetTvAspect(bool newTvAspect)
        {
            SetScreen(screenSize, newTvAspect);
        }
        private void SetScreen(byte newScreenSize, bool newTvAspect)
        {
            if (fullScreen)
                SetFullScreen(false);

            resizedCaptionFont = null;

            screenSize = newScreenSize;
            tvAspect = newTvAspect;

            bufferSize.Width = tvAspect ? 282 * screenSize : 256 * screenSize;
            bufferSize.Height = 240 * screenSize;

            Width = bufferSize.Width + applicationMargin.Width;
            Height = bufferSize.Height + applicationMargin.Height;

            // create resized version of raster filter to optimise rendering
            UpdateResizedScreenFilter();

            viewScreenSizeX1MenuItem.Checked = newScreenSize == 1;
            viewScreenSizeX2MenuItem.Checked = newScreenSize == 2;
            viewScreenSizeX3MenuItem.Checked = newScreenSize == 3;
            viewScreenSizeX4MenuItem.Checked = newScreenSize == 4;
            viewTvAspectMenuItem.Checked = tvAspect;
            videoPanel.Invalidate();
        }

        private void SetFullScreen(bool fullScreen)
        {
            if (this.fullScreen == fullScreen)
                return;

            resizedCaptionFont = null;

            this.fullScreen = fullScreen;

            this.TopMost = fullScreen;

            if (fullScreen)
            {
                if (!cartridgeUsesGun)
                    Cursor.Hide();

                this.windowModePosition = new Point(this.Left, this.Top);
                this.FormBorderStyle = FormBorderStyle.None;
                this.MainMenuStrip.Hide();
                this.statusStrip.Hide();
                this.Left = this.Top = 0;
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                this.Width = screenWidth;
                this.Height = screenHeight;
                bufferSize.Width = screenHeight * 282 / 256;
                bufferSize.Height = screenHeight + 1;

                UpdateResizedScreenFilter();
            }
            else
            {
                Cursor.Show();

                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MainMenuStrip.Show();
                this.statusStrip.Show();
                SetScreen(screenSize, tvAspect);
                this.Left = windowModePosition.X;
                this.Top = windowModePosition.Y;
            }
        }

        private void UpdateResizedScreenFilter()
        {
            Image sourceFilter = null;
            switch (screenFilter)
            {
                case ScreenFilter.Raster: sourceFilter = Properties.Resources.FilterRaster; break;
                case ScreenFilter.Lcd: sourceFilter = Properties.Resources.FilterLcd; break;
            }

            if (sourceFilter != null)
                this.resizedScreenFilter = ResizeImage(sourceFilter,
                    bufferSize.Width, bufferSize.Height);
        }

        private void SetScreenFilter(ScreenFilter newScreenFilter)
        {
            screenFilter = newScreenFilter;
            UpdateResizedScreenFilter();

            viewScreenFilterNoneMenuItem.Checked = screenFilter == ScreenFilter.None;
            viewScreenFilterRasterMenuItem.Checked = screenFilter == ScreenFilter.Raster;
            viewScreenFilterLcdMenuItem.Checked = screenFilter == ScreenFilter.Lcd;
        }

        private void SetMotionBlur(bool newMotionBlur)
        {
            motionBlur = newMotionBlur;
            viewMotionBlurMenuItem.Checked = motionBlur;
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.Low;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private byte DetermineCartridgeMapperId(uint romCrc, byte romMapperId)
        {
            NstDatabaseEntry nstDatabaseEntry = nstDatabase[romCrc];

            if (nstDatabaseEntry == null)
                return romMapperId;

            if (nstDatabaseEntry.MapperId != romMapperId)
                statusHistoryLabel.Text = "Incorrect Mapper ID detected (" + romMapperId + "), should be " + nstDatabaseEntry.MapperId;

            return nstDatabaseEntry.MapperId;
        }

        public NesCore.Console Console { get; private set; }

        private Cartridge cartridge;
        private string cartridgeRomFilename;
        private string cartridgeSaveRamFilename;
        private RecentFileManager recentFileManager;
        private bool cartridgeUsesGun;

        // execution state
        private GameState gameState;

        // video system
        private FastBitmap bitmapBuffer;
        private DateTime gameTickDateTime;
        private Icon gameIcon;

        // view size
        private Size applicationMargin;
        private Size bufferSize;
        private byte screenSize;
        private bool fullScreen;
        private Point windowModePosition;
        private bool tvAspect;
        private ScreenFilter screenFilter;
        private bool motionBlur;

        private Image resizedScreenFilter;
        private Font resizedCaptionFont;

        // frame rate handling
        private DateTime frameDateTime;
        private double averageDeltaTime;

        // audio system
        private WaveOut waveOut;
        private ApuAudioProvider apuAudioProvider;

        // input system
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private GameControllerManager gameControllerManager;

        public event EventHandler<CodeDisassemblyForm> OnDisassemblerShown;
        public void ShowDisassembler()
        {
            OnDiagnosticsCodeDisassembly(this, EventArgs.Empty);
        }
        // debug
        private CodeDisassemblyForm codeDisassemblyForm;
        private bool traceEnabled;

        // window message processing
        private static readonly int SC_SCREENSAVE = 0xF140;
        private static readonly int WM_SYSCOMMAND = 0x0112;
        private static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

        // NST database
        private NstDatabase nstDatabase;

        // video capture
        private VideoFileWriter mp4VideoEncoder;
        private string videoPathMp4;
        private GifEncoder gifVideoEncoder;
        private string videoPathGif;
        private bool videoRecording;
        private bool videoRecordingMp4;
        private bool videoRecordingGif;

        // cheat system
        private CheatSystem cheatSystem;

    }
}
