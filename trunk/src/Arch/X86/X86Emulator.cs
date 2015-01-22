#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    using Decompiler.Core.Machine;
    using TWord = System.UInt32;

    public class X86Emulator
    {
        public event EventHandler BeforeStart;
        public event EventHandler BreakpointHit;
        public event EventHandler ExceptionRaised;

        private IntelArchitecture arch;
        private LoadedImage img;
        IEnumerator<IntelInstruction> dasm;

        public readonly ulong[] Registers;
        public readonly bool[] Valid;
        public TWord Flags;

        public X86Emulator(IntelArchitecture arch, LoadedImage loadedImage)
        {
            this.arch = arch;
            this.img = loadedImage;
            this.Registers = new ulong[40];
            this.Valid = new bool[40];
        }

        public Core.Address InstructionPointer
        {
            get { return ip; }
            set
            {
                ip = value;
                var rdr = arch.CreateImageReader(img, value);
                dasm = arch.CreateDisassembler(rdr).GetEnumerator();
            }
        }
        private Address ip;


        public void Run()
        {
            CreateStack();
            BeforeStart.Fire(this);
            try
            {
                while (dasm.MoveNext())
                {
                    CheckBreakPoints();
                    Execute(dasm.Current);
                }
            }
            catch (Exception)
            {
                ExceptionRaised.Fire(this);
            }
        }

        public void CreateStack()
        {

        }

        public void CheckBreakPoints()
        {
            BreakpointHit.Fire(this);
        }

        public void Execute(IntelInstruction instr)
        {
            TWord w;
            switch (instr.code)
            {
            default:
                throw new NotImplementedException();
            case Opcode.add: Add(instr.op1, instr.op2); return;
            case Opcode.mov: Write(instr.op1, Read(instr.op2)); break;
            case Opcode.sub: Sub(instr.op1, instr.op2); return;
            }
        }

        private void Add(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            TWord sum = l + r;
            Write(dst, sum);
            uint ov = ((~(l ^ r) & (l ^ sum)) & 0x80000000u) >> 20;
            Flags =
                (r > sum ? 1u : 0u) |     // Carry
                (sum == 0 ? 1u << 6: 0u)  | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Sub(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            r = ~r + 1u;
            TWord diff = l + r;
            Write(dst, diff);
            uint ov = ((~(l ^ r) & (l ^ diff)) & 0x80000000u) >> 20;
            Flags =
                (l > diff ? 1u : 0u) |     // Carry
                (diff == 0 ? 1u << 6 : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }

        private TWord Read(MachineOperand op)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                return ReadRegister(r.Register);
            }
            var i = op as ImmediateOperand;
            if (i != null)
                return i.Value.ToUInt32();
            var m = op as MemoryOperand;
            if (m != null)
            {
                TWord ea = GetEffectiveAddress(m);
                return img.ReadLeUInt32(new Address(ea));
            }
            throw new NotImplementedException();
        }

        private TWord GetEffectiveAddress(MemoryOperand m)
        {
            TWord ea = 0;
            if (m.Offset != null)
            {
                if (m.Base == null || m.Base == RegisterStorage.None)
                {
                    ea += m.Offset.ToUInt32();
                }
                else if (m.Index == RegisterStorage.None)
                {
                    ea += ReadRegister(m.Base) + m.Offset.ToUInt32();
                }
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();

            return ea;
        }

        private TWord ReadRegister(RegisterStorage r)
        {
            return (TWord) Registers[r.Number];
        }

        private void Write(MachineOperand op, TWord w)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                WriteRegister(r.Register, w);
                return;
            }
            var m = op as MemoryOperand;
            if (m != null)
            {
                var ea = GetEffectiveAddress(m);
                switch (op.Width.Size)
                {
                case 1: img.WriteByte(new Address(ea), (byte)w); return;
                }
            }
            throw new NotImplementedException();
        }

        public void WriteRegister(RegisterStorage r, TWord value)
        {
            ((IntelRegister)r).SetRegisterFileValues(Registers, value, Valid);
        }
    }
}