#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Arch.Pdp.Pdp11;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.Pdp
{
    public class Pdp11Architecture : ProcessorArchitecture
    {
        private readonly RegisterStorage[] regs;
        private readonly RegisterStorage[] fpuRegs;
        private readonly FlagGroupStorage[] flagRegs;

        public Pdp11Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            regs = new RegisterStorage[] { 
                Registers.r0, Registers.r1, Registers.r2, Registers.r3, 
                Registers.r4, Registers.r5, Registers.sp, Registers.pc,
            };
            fpuRegs = new RegisterStorage[]
            {
                Registers.ac0, Registers.ac1, Registers.ac2,Registers.ac3,
                Registers.ac4, Registers.ac5, 
            };
            flagRegs = new FlagGroupStorage[] 
            {
                Registers.N, Registers.Z, Registers.V, Registers.C
            };

            Endianness = EndianServices.Little;
            InstructionBitSize = 16;
            StackRegister = Registers.sp;
            CarryFlagMask = Registers.C.FlagGroupBits;
            FramePointerType = PrimitiveType.Ptr16;
            PointerType = PrimitiveType.Ptr16;
            WordWidth = PrimitiveType.Word16;
        }

        #region IProcessorArchitecture Members

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Pdp11Disassembler(rdr, this);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Pdp11InstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Pdp11ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            int i = domain - StorageDomain.Register;
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public RegisterStorage? GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        internal RegisterStorage? GetFpuRegister(int i)
        {
            return (0 <= i && i < fpuRegs.Length)
                ? fpuRegs[i]
                : null;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage? GetRegister(string name)
        {
            foreach (RegisterStorage reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return reg;
            }
            return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs;
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage result)
        {
            result = null;
            foreach (RegisterStorage reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    result = reg;
                    return true;
                }
            }
            return false;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
		{
			PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf), dt);
			return fl;
		}

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            uint grf = 0;
            foreach (var c in name)
            {
                switch (c)
                {
                case 'N': grf |= Registers.N.FlagGroupBits; break;
                case 'Z': grf |= Registers.Z.FlagGroupBits; break;
                case 'V': grf |= Registers.V.FlagGroupBits; break;
                case 'C': grf |= Registers.C.FlagGroupBits; break;
                }
            }
            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            return new FlagGroupStorage(Registers.psw, grf, name, dt);
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
			var s = new StringBuilder();
			foreach (var r in flagRegs)
			{
				if ((grf & r.FlagGroupBits) != 0)
					s.Append(r.Name);
			}
			return s.ToString();
		}

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Pdp11Rewriter(this, new Pdp11Disassembler(rdr, this), binder, host);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt16();
            if (codeAlign)
                uAddr &= unchecked((ushort)~1u);
            return Address.Ptr16(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            if (rdr.TryReadUInt16(out var uaddr))
            {
                return Address.Ptr16(uaddr);
            }
            else
            {
                return null;
            }
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }
        #endregion
    }
}
