#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Assemblers.M68k
{
    /// <summary>
    /// Handy little assembler that lets you write C# code that assembles 
    /// directly to 68k opcodes.
    /// </summary>
    /// <remarks>
    /// //$TODO: many more instructions, symbols.
    /// </remarks>
    public class M68kAssembler
    {
        private M68kArchitecture arch;
        private Platform platform;
        private Address addrBase;
        private Emitter emitter;
        private SymbolTable symtab;
        private List<EntryPoint> entryPoints;
        private SortedDictionary<string, TypeLibrary> importLibraries;
        private Dictionary<uint, PseudoProcedure> importThunks;
        private List<ushort> constants;

        public M68kAssembler(M68kArchitecture arch, Address addrBase, List<EntryPoint> entryPoints)
        {
            BaseAddress = addrBase;
            emitter = new Emitter();
            constants = new List<ushort>();
        }

        public Address BaseAddress { get; private set; }

        public RegisterOperand d0 { get { return new RegisterOperand(Registers.d0); } }
        public RegisterOperand d1 { get { return new RegisterOperand(Registers.d1); } }
        public RegisterOperand d2 { get { return new RegisterOperand(Registers.d2); } }
        public RegisterOperand d3 { get { return new RegisterOperand(Registers.d3); } }
        public RegisterOperand d4 { get { return new RegisterOperand(Registers.d4); } }
        public RegisterOperand d5 { get { return new RegisterOperand(Registers.d5); } }
        public RegisterOperand d6 { get { return new RegisterOperand(Registers.d6); } }
        public RegisterOperand d7 { get { return new RegisterOperand(Registers.d7); } }

        public RegisterOperand a0 { get { return new RegisterOperand(Registers.a0); } }
        public RegisterOperand a1 { get { return new RegisterOperand(Registers.a1); } }
        public RegisterOperand a2 { get { return new RegisterOperand(Registers.a2); } }
        public RegisterOperand a3 { get { return new RegisterOperand(Registers.a3); } }
        public RegisterOperand a4 { get { return new RegisterOperand(Registers.a4); } }
        public RegisterOperand a5 { get { return new RegisterOperand(Registers.a5); } }
        public RegisterOperand a6 { get { return new RegisterOperand(Registers.a6); } }
        public RegisterOperand a7 { get { return new RegisterOperand(Registers.a7); } }

        public void Nop()
        {
            emitter.EmitBeUint16(0x4E71);
        }

        public LoadedImage GetImage()
        {
            return new LoadedImage(BaseAddress, emitter.Bytes);
        }

        private int Ea(MachineOperand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                var dReg = rop.Register as DataRegister;
                if (dReg != null)
                    return dReg.Number & 7;
            }
            var mop = op as MemoryOperand;
            if (mop != null)
            {
                var aReg = mop.Base;
                if (mop.Offset == null || mop.Offset.ToInt32() == 0)
                    return aReg.Number & 7 | 0x10;
            }

            var preOp = op as PredecrementMemoryOperand;
            if (preOp != null)
            {
                return preOp.Register.Number & 7 | 0x20;
            }
            throw new NotImplementedException(op.ToString());
        }

        public MachineOperand Mem(RegisterOperand rop)
        {
            var a = (AddressRegister) rop.Register;
            return new MemoryOperand(null, a);
        }

        public MemoryOperand Mem(int offset, RegisterOperand rop)
        {
            var a = (AddressRegister) rop.Register;
            return new MemoryOperand(null, a, Constant.Int16((short)offset));
            throw new NotImplementedException();
        }

        private int SmallQ(int c)
        {
            if (c <= 0 || c > 8)
                throw new ArgumentOutOfRangeException("c");
            if (c == 8)
                c = 0;
            return c;
        }

        private int SwapEa(MachineOperand op)
        {
            int ea = Ea(op) & 37;
            return (ea >> 3) & 7 | (ea & 7) << 3;
        }

        private int AReg(MachineOperand op)
        {
            var rop = (RegisterOperand) op;
            var addr = (AddressRegister) rop.Register;
            return addr.Number & 7;
        }

        private int DReg(MachineOperand op)
        {
            var rop = (RegisterOperand) op;
            var data = (DataRegister) rop.Register;
            return data.Number & 7;
        }

        public PredecrementMemoryOperand Pre(RegisterOperand a)
        {
            return new PredecrementMemoryOperand(null, (AddressRegister) a.Register);
        }

        private void EmitConstants()
        {
            foreach (ushort c in constants)
            {
                emitter.EmitBeUint16(c);
            }
            constants.Clear();
        }


        public void Adda_l(MachineOperand eaSrc, RegisterOperand aDst)
        {
            Emit(0xD1C0 | Ea(eaSrc) | AReg(aDst) << 9);
        }

        public void Addi_w(int c, MachineOperand eaDst)
        {
            constants.Add((ushort) c);
            Emit(0x0640 | Ea(eaDst));
        }

        public void Addq_l(int c, MachineOperand eaDst)
        {
            Emit(0x5080 | Ea(eaDst) & 0x3F | (SmallQ(c) << 9));
        }

        public void Move_b(MachineOperand src, MachineOperand dst)
        {
            var eaSrc = Ea(src);
            var eaDst = SwapEa(dst);
            Emit(0x1000 | eaSrc | (eaDst << 6));
        }

        public void Lsl_l(int c, RegisterOperand dDst)
        {
            Emit(0xE188 | SmallQ(c) << 9 | DReg(dDst));
        }

        private void Emit(int opcode)
        {
            emitter.EmitBeUint16(opcode);
            EmitConstants();
        }
    }
}
