using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HalfNes.Com.GrapeShot.Audio
{
    public class SwingAudioImpl : AudioOutInterface
    {
        public class ApuAudioProvider : WaveProvider32
        {
            public WaveOut Parent;
            public ApuAudioProvider()
            {
                cyclicBuffer = new float[base.WaveFormat.SampleRate];
                readIndex = writeIndex = 0;
                Enabled = true;
            }

            public bool Enabled { get; set; }

            public override int Read(float[] buffer, int offset, int sampleCount)
            {
                lock (queueLock)
                {
                    if (!Enabled || size == 0)
                    {
                        buffer[offset] = 0;
                        return 1;
                    }

                    sampleCount = Math.Min(sampleCount, size);

                    for (int n = 0; n < sampleCount; n++)
                    {
                        buffer[n + offset] = cyclicBuffer[readIndex++];
                        readIndex %= cyclicBuffer.Length;
                        --size;
                    }
                    return sampleCount;
                }
            }
            public int available()
            {
                return size;
            }
            public void flush()
            {
            }
            public void stop()
            {
                Parent.Stop();
            }
            public void write(byte[] buf, int offset, int length)
            {
                MemoryStream ms = new MemoryStream(buf);
                ms.Position = offset;
                BinaryReader reader = new BinaryReader(ms);
                int lengthOfShort = length / 2;
                float[] fbuf = new float[lengthOfShort];
                for (int i = 0; i < lengthOfShort; ++i)
                {
                    short s = reader.ReadInt16();
                    float floatVal = s / 32768.0f;
                    fbuf[i] = floatVal;
                }
                Queue(fbuf);
            }
            public void Queue(float[] sampleValues)
            {
                lock (queueLock)
                {
                    for (int index = 0; index < sampleValues.Length; index++)
                    {
                        if (size >= cyclicBuffer.Length)
                            return;

                        cyclicBuffer[writeIndex] = sampleValues[index];
                        ++writeIndex;
                        writeIndex %= cyclicBuffer.Length;
                        ++size;
                    }
                }
            }

            private float[] cyclicBuffer = new float[22050];
            private int readIndex;
            private int writeIndex;
            private int size;
            private object queueLock = new object();

            public void start()
            {
                Parent.Play();
            }

            public void close()
            {
                this.Parent.Dispose();
            }

            public int getBufferSize()
            {
                return cyclicBuffer.Length;
            }
        }

        private bool soundEnable;
        private byte[] audiobuf;
        private int bufptr = 0;
        private float outputvol;
        ApuAudioProvider sdl;
        WaveOut waveOut;
        public SwingAudioImpl(NES nes, int samplerate, Mapper.TVType tvtype)
        {
            soundEnable = PrefsSingleton.get().getBoolean("soundEnable", true);
            outputvol = (float)(PrefsSingleton.get().getInt("outputvol", 13107) / 16384.0);
            double fps;
            switch (tvtype)
            {
                case NTSC:
                default:
                    fps = 60.0;
                    break;
                case PAL:
                case DENDY:
                    fps = 50.0;
                    break;
            }
            if (soundEnable)
            {
                int samplesperframe = (int)Math.Ceiling((samplerate * 2) / fps);
                audiobuf = new byte[samplesperframe * 2];
                try
                {

                    sdl = new ApuAudioProvider();
                    waveOut = new WaveOut();
                    sdl.Parent = waveOut;
                    waveOut.Init(sdl);
                    sdl.start();
                }
                catch (Exception a)
                {
                    Console.Error.WriteLine(a);
                    nes.messageBox("Unable to inintialize sound.");
                    soundEnable = false;
                }
            }
        }


        public void flushFrame(bool waitIfBufferFull)
        {
            if (soundEnable)
            {

                //            if (sdl.available() == sdl.getBufferSize()) {
                //                System.err.println("Audio is underrun");
                //            }
                if (sdl.available() < bufptr)
                {
                    //                System.err.println("Audio is blocking");
                    if (waitIfBufferFull)
                    {

                        //write to audio buffer and don't worry if it blocks
                        sdl.write(audiobuf, 0, bufptr);
                    }
                    //else don't bother to write if the buffer is full
                }
                else
                {
                    sdl.write(audiobuf, 0, bufptr);
                }
            }
            bufptr = 0;

        }


        public void outputSample(int sample)
        {
            if (soundEnable)
            {
                sample = (int)(sample * outputvol);
                if (sample < -32768)
                {
                    sample = -32768;
                    //System.err.println("clip");
                }
                if (sample > 32767)
                {
                    sample = 32767;
                    //System.err.println("clop");
                }
                //left ch
                int lch = sample;
                audiobuf[bufptr] = (byte)(lch & 0xff);
                audiobuf[bufptr + 1] = (byte)((lch >> 8) & 0xff);
                //right ch
                int rch = sample;
                audiobuf[bufptr + 2] = (byte)(rch & 0xff);
                audiobuf[bufptr + 3] = (byte)((rch >> 8) & 0xff);
                bufptr += 4;
            }
        }


        public void pause()
        {
            if (soundEnable)
            {
                sdl.flush();
                sdl.stop();
            }
        }


        public void resume()
        {
            if (soundEnable)
            {
                sdl.start();
            }
        }


        public void destroy()
        {
            if (soundEnable)
            {
                sdl.stop();
                sdl.close();
            }
        }

        public bool bufferHasLessThan(int samples)
        {
            //returns true if the audio buffer has less than the specified amt of samples remaining in it
            return (sdl == null) ? false : ((sdl.getBufferSize() - sdl.available()) <= samples);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public WaveFormat WaveFormat
        {
            get { throw new NotImplementedException(); }
        }
    }

}
