﻿using NesCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesCore.Storage
{
    class CartridgeMapMmc3 : CartridgeMap
    {
        public CartridgeMapMmc3(Cartridge cartridge, bool mmc3AVariant = false)
            : base(cartridge)
        {
            this.mmc3AVariant = mmc3AVariant;

            registers = new byte[8];
            programBankOffsets = new int[4];
            characterBankOffsets = new int[8];

            programBankOffsets[0] = GetProgramBankOffset(0);
            programBankOffsets[1] = GetProgramBankOffset(1);
            programBankOffsets[2] = GetProgramBankOffset(-2);
            programBankOffsets[3] = GetProgramBankOffset(-1);
        }

        public override string Name { get { return mmc3AVariant ? "MMC3A" : "MMC3"; } }

        public override byte this[ushort address]
        {
            get
            {
                if (address < 0x2000)
                {
                    int bank = address / 0x0400;
                    int offset = address % 0x0400;

                    int flatAddress = characterBankOffsets[bank] + offset;

                    // MMC3A variant
                    if (mmc3AVariant)
                    {
                        if (bank < 4)
                            flatAddress += leftUpperChr;
                        else
                            flatAddress += rightUpperChr;
                    }

                    return Cartridge.CharacterRom[flatAddress];
                }
                else if (address >= 0x8000)
                {
                    address -= 0x8000;
                    int bank = address / 0x2000;
                    int offset = address % 0x2000;

                    return Cartridge.ProgramRom[programBankOffsets[bank] + offset];
                }
                else if (address >= 0x6000)
                    return Cartridge.SaveRam[(ushort)(address - 0x6000)];
                else
                {
                    Debug.WriteLine(Name + ": Unexpected read from address " + Hex.Format(address));
                    return (byte)(address >> 8); // return open bus
                }
            }

            set
            {
                if (address < 0x2000)
                {
                    int bank = address / 0x0400;
                    int offset = address % 0x0400;

                    int flatAddress = characterBankOffsets[bank] + offset;

                    // MMC3A variant
                    if (mmc3AVariant)
                    {
                        if (bank < 4)
                            flatAddress += leftUpperChr;
                        else
                            flatAddress += rightUpperChr;
                    }

                    Cartridge.CharacterRom[flatAddress] = value;
                }
                else if (mmc3AVariant && address >= 0x4100 && address < 0x6000)
                {
                    // MMC3 variant assigned to mapper 12
                    if (mmc3AVariant)
                    {
                        // ---R ---L
                        leftUpperChr = value & 0x01;
                        leftUpperChr *= 0x100 * 0x400;

                        rightUpperChr = (value >> 4) & 0x01;
                        rightUpperChr *= 0x100 * 0x400;
                    }
                }
                else if (address >= 0x8000)
                    WriteRegister(address, value);
                else if (address >= 0x6000)
                    Cartridge.SaveRam[(ushort)(address - 0x6000)] = value;
                else
                {
                    // Somari homebrew ROM writes to $4100 as part mapper 116 to switch between VRC2/MMC3/MMc1
                    Debug.WriteLine(Name + ": Unexpected write of value " + Hex.Format(value) + " at address " + Hex.Format(address));
                }
            }
        }

        public override void StepVideo(int scanLine, int cycle, bool showBackground, bool showSprites)
        {
            if (cycle != 260)
                return;

            if (scanLine > 239 && scanLine < 261)
                return;

            if (!showBackground && !showSprites)
                return;

            HandleScanLine();
        }

        public void SaveState(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(registerIndex);
            binaryWriter.Write(registers);
            binaryWriter.Write(programBankMode);
            binaryWriter.Write(characterBankMode);
            for (int index = 0; index < 4; index++)
                binaryWriter.Write(programBankOffsets[index]);
            for (int index = 0; index < 8; index++)
                binaryWriter.Write(characterBankOffsets[index]);
            binaryWriter.Write(irqReload);
            binaryWriter.Write(irqCounter);
            binaryWriter.Write(irqEnable);
        }

        public void LoadState(BinaryReader binaryReader)
        {
            registerIndex = binaryReader.ReadByte();
            registers = binaryReader.ReadBytes(8);
            programBankMode = binaryReader.ReadByte();
            characterBankMode = binaryReader.ReadByte();
            for (int index = 0; index < 4; index++)
                programBankOffsets[index] = binaryReader.ReadInt32();
            for (int index = 0; index < 8; index++)
                characterBankOffsets[index] = binaryReader.ReadInt32();
            irqReload = binaryReader.ReadByte();
            irqCounter = binaryReader.ReadByte();
            irqEnable = binaryReader.ReadBoolean();
        }

        private void HandleScanLine()
        {
            if (irqCounter == 0)
                irqCounter = irqReload;
            else
            {
                --irqCounter;
                if (irqCounter == 0 && irqEnable)
                    if(TriggerInterruptRequest!=null) TriggerInterruptRequest.Invoke();
            }
        }

        private void WriteRegister(ushort address, byte value)
        {
            if (address <= 0x9FFF && address % 2 == 0)
                WriteBankSelect(value);
            else if (address <= 0x9FFF && address % 2 == 1)
                WriteBankData(value);
            else if (address <= 0xBFFF && address % 2 == 0)
                WriteMirror(value);
            else if (address <= 0xBFFF && address % 2 == 1)
                WriteProtect(value);
            else if (address <= 0xDFFF && address % 2 == 0)
                WriteIRQLatch(value);
            else if (address <= 0xDFFF && address % 2 == 1)
                WriteIRQReload(value);
            else if (address <= 0xFFFF && address % 2 == 0)
                WriteIRQDisable(value);
            else if (address <= 0xFFFF && address % 2 == 1)
                WriteIRQEnable(value);        
        }

        private void WriteBankSelect(byte value)
        {
            programBankMode = (byte)((value >> 6) & 1);
            characterBankMode = (byte)((value >> 7) & 1);
            registerIndex = (byte)(value & 7);
            UpdateOffsets();

            // invalidate address regions
            if(CharacterBankSwitch!=null) CharacterBankSwitch.Invoke(0x0000, 0x2000);
            if(ProgramBankSwitch!=null) ProgramBankSwitch.Invoke(0x8000, 0x8000);
        }

        private void WriteBankData(byte value)
        {
            registers[registerIndex] = value;
            UpdateOffsets();
        }

        private void WriteMirror(byte value)
        {
            MirrorMode = ((value & 1) == 0) ? MirrorMode.Vertical : MirrorMode.Horizontal;
        }

        private void WriteProtect(byte value)
        {
        }

        private void WriteIRQLatch(byte value)
        {
            irqReload = value;
        }

        private void WriteIRQReload(byte value)
        {
            irqCounter = 0;
        }

        private void WriteIRQDisable(byte value)
        {
            irqEnable = false;
        }

        private void WriteIRQEnable(byte value)
        {
            irqEnable = true;
        }

        private int GetProgramBankOffset(int index)
        {
            if (index >= 0x80)
                index -= 0x100;

            index %= Cartridge.ProgramRom.Count / 0x2000;
            int offset = index * 0x2000;
            if (offset < 0)
                offset += Cartridge.ProgramRom.Count;

            return offset;
        }

        private int GetCharacterBankOffset(int index)
        {
            if (index >= 0x80)
                index -= 0x100;

            index %= Cartridge.CharacterRom.Length / 0x0400;

            int offset = index * 0x0400;
            if (offset < 0)
                offset += Cartridge.CharacterRom.Length;

            return offset;
        }
	
        private void UpdateOffsets()
        {
            if (programBankMode == 0)
            {
                programBankOffsets[0] = GetProgramBankOffset(registers[6]);
                programBankOffsets[1] = GetProgramBankOffset(registers[7]);
                programBankOffsets[2] = GetProgramBankOffset(-2);
                programBankOffsets[3] = GetProgramBankOffset(-1);
            }
            else // == 1
            {
                programBankOffsets[0] = GetProgramBankOffset(-2);
                programBankOffsets[1] = GetProgramBankOffset(registers[7]);
                programBankOffsets[2] = GetProgramBankOffset(registers[6]);
                programBankOffsets[3] = GetProgramBankOffset(-1);
            }

            if (characterBankMode == 0)
            {
                characterBankOffsets[0] = GetCharacterBankOffset(registers[0] & 0xFE);
                characterBankOffsets[1] = GetCharacterBankOffset(registers[0] | 0x01);
                characterBankOffsets[2] = GetCharacterBankOffset(registers[1] & 0xFE);
                characterBankOffsets[3] = GetCharacterBankOffset(registers[1] | 0x01);
                characterBankOffsets[4] = GetCharacterBankOffset(registers[2]);
                characterBankOffsets[5] = GetCharacterBankOffset(registers[3]);
                characterBankOffsets[6] = GetCharacterBankOffset(registers[4]);
                characterBankOffsets[7] = GetCharacterBankOffset(registers[5]);
            }
            else // == 1
            {
                characterBankOffsets[0] = GetCharacterBankOffset(registers[2]);
                characterBankOffsets[1] = GetCharacterBankOffset(registers[3]);
                characterBankOffsets[2] = GetCharacterBankOffset(registers[4]);
                characterBankOffsets[3] = GetCharacterBankOffset(registers[5]);
                characterBankOffsets[4] = GetCharacterBankOffset(registers[0] & 0xFE);
                characterBankOffsets[5] = GetCharacterBankOffset(registers[0] | 0x01);
                characterBankOffsets[6] = GetCharacterBankOffset(registers[1] & 0xFE);
                characterBankOffsets[7] = GetCharacterBankOffset(registers[1] | 0x01);
            }
        }

        protected int[] programBankOffsets;
        protected int[] characterBankOffsets;

        private bool mmc3AVariant;
        private int leftUpperChr;
        private int rightUpperChr;
        private byte registerIndex;
        private byte[] registers;
        private byte programBankMode;
        private byte characterBankMode;
        private byte irqReload;
        private byte irqCounter;
        private bool irqEnable;
    }
}
