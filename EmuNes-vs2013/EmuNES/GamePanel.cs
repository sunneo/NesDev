using SharpNes.Audio;
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


namespace SharpNes
{
    public class GamePanel:Panel
    {
        public NesCore.Console Console { get; private set; }

        private Cartridge cartridge;
        private string cartridgeRomFilename;
        
        
        private bool cartridgeUsesGun;

        // execution state
        private GameState gameState;

        // video system
        private FastBitmap bitmapBuffer;
        private DateTime gameTickDateTime;
        

        // view size
        private Size applicationMargin;
        private Size bufferSize;
        private byte screenSize;
        
        private bool tvAspect;
        
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
        BackgroundWorker mWorker = new BackgroundWorker();
        public event EventHandler<CodeDisassemblyForm> OnDisassemblerShown;
        public void ShowDisassembler()
        {
            OnDiagnosticsCodeDisassembly(this, EventArgs.Empty);
        }
        // debug
        private CodeDisassemblyForm codeDisassemblyForm;
        private bool traceEnabled;

        // window message processing
        
        private static readonly int WM_SYSCOMMAND = 0x0112;
        private static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

        // NST database
        private NstDatabase nstDatabase;

        
        // cheat system
        private CheatSystem cheatSystem;
        
        public GamePanel()
        {
            this.SetStyle(ControlStyles.Selectable , true);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DoubleBuffered = true;

            this.BackColor = Color.Black;
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
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
           // int leftCenteredMargin = (this.Width - bufferSize.Width) / 2;

            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            
            switch (gameState)
            {
                case GameState.Stopped:
                    graphics.DrawImage(Properties.Resources.Background, 0, 0, this.Width, this.Height);
                    break;
                case GameState.Paused:
                    graphics.DrawImage(bitmapBuffer.Bitmap, 0, 0, bufferSize.Width, bufferSize.Height);
                    break;
                case GameState.Running:
                    graphics.DrawImage(bitmapBuffer.Bitmap, 0, 0, bufferSize.Width, bufferSize.Height);
                    break;
            }
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
        private void OnGameStop(object sender, EventArgs eventArgs)
        {
            if (gameState == GameState.Stopped)
                return;

            this.Invalidate();
            gameState = GameState.Stopped;
            waveOut.Stop();
            
        }
        public void Open(String filename)
        {
            LoadCartridgeRom(filename);
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
        public void Pause()
        {
            OnGameRunPause(this, EventArgs.Empty);
        }
        public void StepOver()
        {
            Console.StepOver();
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

            this.Invalidate();
        }
        private bool LoadCartridgeRom(string cartridgeRomPath)
        {
            try
            {

                Stream cartridgeRomStream = GetCartridgeRomStream(cartridgeRomPath);

                BinaryReader romBinaryReader = new BinaryReader(cartridgeRomStream);
                Cartridge newCartridge = new Cartridge(romBinaryReader);
                romBinaryReader.Close();

                Console.LoadCartridge(newCartridge);

                this.cartridge = newCartridge;

                this.cartridgeRomFilename = Path.GetFileNameWithoutExtension(cartridgeRomPath);

                this.Text = cartridgeRomFilename + " - " + Application.ProductName;


                if (traceEnabled)
                {
                    Console.Cartridge.Map.ProgramBankSwitch =
                        (address, size) => codeDisassemblyForm.InvalidateMemoryRange(address, size);
                }

                OnGameStop(this, EventArgs.Empty);
                OnGameRunPause(this, EventArgs.Empty);



                // set cursor to crosshairs if it is a gun-based game
                cartridgeUsesGun = false;
                NstDatabaseEntry nstDatabaseEntry = nstDatabase[cartridge.Crc];
                if (nstDatabaseEntry != null && nstDatabaseEntry.Peripherals.Contains(Peripheral.Zapper))
                {
                    cartridgeUsesGun = true;
                }
                this.Cursor = cartridgeUsesGun ? Cursors.Cross : Cursors.Default;

                // load cheat file if present
                cheatSystem.Clear();
                string cheatFilePath = ReplaceRomExtension(cartridgeRomPath, ".cht");
                if (File.Exists(cheatFilePath))
                {
                    cheatSystem.Load(cheatFilePath);
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

        private void OnDiagnosticsCodeDisassembly(object sender, EventArgs eventArgs)
        {
            traceEnabled = !traceEnabled;
            
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
        public void SendKeyDown(Keys KeyCode)
        {
            keyboardState[KeyCode] = true;
        }
        public void SendKeyUp(Keys KeyCode)
        {
            keyboardState[KeyCode] = false;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            keyboardState[e.KeyCode] = true;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyboardState[e.KeyCode] = false;
            base.OnKeyUp(e);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 0x0100)
            {
                keyboardState[keyData] = true;
            }
            else if (msg.Msg == 0x0101)
            {
                keyboardState[keyData] = false;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
            
            screenSize = newScreenSize;
            tvAspect = newTvAspect;

            bufferSize.Width = tvAspect ? 282 * screenSize : 256 * screenSize;
            bufferSize.Height = 240 * screenSize;

            Width = bufferSize.Width + applicationMargin.Width;
            Height = bufferSize.Height + applicationMargin.Height;

           
            this.Invalidate();
        }
        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            applicationMargin = new Size(Width - this.ClientSize.Width, Height - this.ClientSize.Height);
            SetScreen(1, true);
            

            codeDisassemblyForm = new CodeDisassemblyForm(Console);

            //Application.Idle += TickWhileIdle;
            mWorker = new BackgroundWorker();
            mWorker.WorkerSupportsCancellation = true;
            mWorker.DoWork += TickWhileIdle;
            mWorker.RunWorkerAsync();
            
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (this.Console != null)
            {
                this.Console.Halt();

                if (Console.Audio != null)
                {
                    Console.Audio.WriteSample = null;
                }
                if (apuAudioProvider != null)
                {
                    apuAudioProvider.Enabled = false;
                    apuAudioProvider = null;
                }
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }


            }
            try
            {
                if (codeDisassemblyForm != null)
                {
                    codeDisassemblyForm.Close();
                    codeDisassemblyForm = null;
                }
                if (this.mWorker != null)
                {
                    this.mWorker.CancelAsync();
                }
            }
            catch (Exception ee)
            {

            }
        }
        
        protected override void OnSizeChanged(EventArgs e)
        {
            int wratio = (int)Math.Floor(((double)this.Width) / 256.0);
            int hratio = (int)Math.Floor(((double)this.Height) / 240.0);
            int ratio = Math.Min(wratio, hratio);
            if (ratio <= 0) ratio = 1;
            SetScreenSize((byte)ratio);
            base.OnSizeChanged(e);
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
        event EventHandler<int> m_MeasureFPSHandler;
        public event EventHandler<int> OnMeasureFPS
        {
            add
            {
                m_MeasureFPSHandler += value;
            }
            remove
            {
                m_MeasureFPSHandler -= value;
            }
        }

        private void ConfigureVideo()
        {
            bitmapBuffer = new FastBitmap(256, 240);

            Console.Video.WritePixel = (x, y, colour, pixelType) =>
            {
                // do not render top and bottom 8 rows
                if (y < 8 || y > 231)
                {
                    return;
                }

                int offset = (y * 256 + x) * 4;

                bitmapBuffer.Bits[offset++] = colour.Blue;
                bitmapBuffer.Bits[offset++] = colour.Green;
                bitmapBuffer.Bits[offset] = colour.Red;
            };
            Console.Video.ShowFrame = () =>
            {
                this.Invalidate();
                if (m_MeasureFPSHandler != null)
                {
                    // frame rate
                    DateTime currentDateTime = DateTime.Now;
                    double currentDeltaTime = (currentDateTime - frameDateTime).TotalSeconds;
                    frameDateTime = currentDateTime;
                    averageDeltaTime = averageDeltaTime * 0.9 + currentDeltaTime * 0.1;
                    int frameRate = (int)(1.0 / averageDeltaTime);
                    m_MeasureFPSHandler(this, frameRate);
                }
            };
        }

        private void ConfigureAudio()
        {
            waveOut = new WaveOut();
            apuAudioProvider = new ApuAudioProvider();

            waveOut.DesiredLatency = 100;
            waveOut.Init(apuAudioProvider);
            Console.Audio.SampleRate = waveOut.OutputWaveFormat.SampleRate;
            float[] outputBuffer = new float[waveOut.OutputWaveFormat.SampleRate/4];
            
            int writeIndex = 0;

            
            Console.Audio.WriteSample = (sampleValue) =>
            {
                // fill buffer
                outputBuffer[writeIndex++] = sampleValue;
                if (writeIndex >= outputBuffer.Length)
                {
                    float[] buf = outputBuffer;
                    outputBuffer = new float[waveOut.OutputWaveFormat.SampleRate / 4];
                    writeIndex = 0;
                    ThreadPool.QueueUserWorkItem((s) => {
                        apuAudioProvider.Queue(buf);
                    });
                    
                    // when buffer full, send to wave provider
                    writeIndex = 0;
                        
                }
            };

            
            
            

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
        private byte DetermineCartridgeMapperId(uint romCrc, byte romMapperId)
        {
            NstDatabaseEntry nstDatabaseEntry = nstDatabase[romCrc];

            if (nstDatabaseEntry == null)
                return romMapperId;

            return nstDatabaseEntry.MapperId;
        }
    }
}
