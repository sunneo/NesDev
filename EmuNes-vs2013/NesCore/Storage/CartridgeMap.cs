﻿using NesCore.Utility;
using NesCore.Video;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NesCore.Audio;

namespace NesCore.Storage
{
    public abstract class CartridgeMap
    {
        public delegate void BankSwitchHandler(ushort address, ushort size);

        public CartridgeMap(Cartridge cartridge)
        {
            Cartridge = cartridge;

            // set initial mirror mode
            MirrorMode = cartridge.MirrorMode;

            // basic nametable ram supports 2 actual pages
            nameTableRam = new byte[0x800];

            // default implementation provides no samples
            WriteAudioSample = (sample) => { }; 
        }

        public abstract string Name { get;  }

        public virtual Action TriggerInterruptRequest { get; set; }

        public virtual Action CancelInterruptRequest { get; set; }

        public BankSwitchHandler ProgramBankSwitch { get; set; }

        public BankSwitchHandler CharacterBankSwitch { get; set; }

        public NesCore.Audio.Apu.WriteSampleHandler WriteAudioSample { get; set; }

        public Cartridge Cartridge { get; private set; }

        public virtual byte this[ushort address]
        {
            get
            {
                Debug.WriteLine(Name + ": Unexpected read from address " + Hex.Format(address));
                return (byte)(address >> 8); // open bus
            }
            set
            {
                Debug.WriteLine(Name + ": Unexpected write of value " + Hex.Format(value) + " at address " + Hex.Format(address));
            }
        }

        public virtual void StepVideo(int scanLine, int cycle, bool showBackground, bool showSprites) { }

        public SpriteSize SpriteSize { get; set; }

        public MirrorMode MirrorMode { get; protected set; }

        public bool AccessingSpriteCharacters { get; set; }

        public virtual void Reset() { }

        public virtual byte ReadNameTableByte(ushort address)
        {
            ushort mirroredAddress = MirrorAddress(MirrorMode, address);

            // default implementation mirrors nametables C and D ($2800 - $2FFF)
            // to A and B - this can be overridden by MMC5 etc.
            mirroredAddress %= 0x800;

            return nameTableRam[mirroredAddress];
        }

        public virtual void WriteNameTableByte(ushort address, byte value)
        {
            ushort mirroredAddress = MirrorAddress(MirrorMode, address);

            // default implementation mirrors nametables C and D ($2800 - $2FFF)
            // to A and B - this can be overridden by MMC5 etc.
            mirroredAddress %= 0x800;

            nameTableRam[mirroredAddress] = value;
        }

        protected ushort MirrorAddress(MirrorMode mirrorMode, ushort address)
        {
            address %= 0x1000;
            int table = address / 0x0400;
            int offset = address % 0x0400;

            int nameTableIndex = ((int)mirrorMode >> (table * 2)) & 0x03;

            return (ushort)(nameTableIndex * 0x0400 + offset);
        }

        protected byte[] nameTableRam;
    }
}
