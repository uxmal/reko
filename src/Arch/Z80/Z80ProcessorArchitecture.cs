#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Z80
{
    public class Z80ProcessorArchitecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Z80ProcessorArchitecture(string archId) : base(archId)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.PointerType = PrimitiveType.Ptr16;
            this.WordWidth = PrimitiveType.Word16;
            this.StackRegister = Registers.sp;
            this.CarryFlagMask = (uint)FlagM.CF;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Z80Disassembler(imageReader);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Z80InstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Z80ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Z80Rewriter(this, rdr, state, binder, host);
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v == Mnemonic.ex_af ? "ex" : Enum.GetName(typeof(Mnemonic), v),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            if (string.Compare(name, "ex", StringComparison.InvariantCultureIgnoreCase) == 0)
                return (int)Mnemonic.ex_af;
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(string name)
        {
            return Registers.GetRegister(name);
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.GetRegister(domain, range.BitMask());
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.regsByName.TryGetValue(name, out reg);
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.P.FlagGroupBits) != 0) yield return Registers.P;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0 && reg.BitSize == (ulong)width)
                return reg;
            var subregs = Registers.SubRegisters[reg.Domain];
            var mask = ((1ul << width) - 1) << offset;
            var result = reg;
            foreach (var subreg in subregs)
            {
                if ((mask & ~subreg.BitMask) == 0)
                    result = subreg;
            }
            return result;
        }

        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs)
        {
            ulong mask = regs.Where(b => b != null && b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            RegisterStorage[]  subregs;
            if ((mask & reg.BitMask) == reg.BitMask)
                return reg;
            RegisterStorage rMax = null;
            if (Registers.SubRegisters.TryGetValue(reg.Domain, out subregs))
            {
                throw new NotImplementedException();
                //foreach (var subreg in subregs.Values)
                //{
                //    if ((subreg.BitMask & mask) == subreg.BitMask &&
                //        (rMax == null || subreg.BitSize > rMax.BitSize))
                //    {
                //        rMax = subreg;
                //    }
                //}
            }
            return rMax;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            FlagGroupStorage f;
            if (flagGroups.TryGetValue(grf, out f))
            {
                return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.f, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            FlagM flags = 0;
            foreach (char c in name)
            {
                switch (c)
                {
                case 'Z': flags |= FlagM.ZF; break;
                case 'S': flags |= FlagM.SF; break;
                case 'C': flags |= FlagM.CF; break;
                case 'P': flags |= FlagM.PF; break;
                default: throw new ArgumentException("name");
                }
            }
            if (flags == 0)
                throw new ArgumentException("name");
            return GetFlagGroup(Registers.f, (uint)flags);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
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

		public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
		{
			StringBuilder sb = new StringBuilder();
            if ((grf & Registers.S.FlagGroupBits) != 0) sb.Append(Registers.S.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) sb.Append(Registers.Z.Name);
            if ((grf & Registers.P.FlagGroupBits) != 0) sb.Append(Registers.P.Name);
            if ((grf & Registers.N.FlagGroupBits) != 0) sb.Append(Registers.N.Name);
            if ((grf & Registers.C.FlagGroupBits) != 0) sb.Append(Registers.C.Name);
			return sb.ToString();
		}

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }
    }

    public static class Registers
    {
        public static readonly RegisterStorage b = new RegisterStorage("b", 1, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage c = new RegisterStorage("c", 1, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage d = new RegisterStorage("d", 2, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage e = new RegisterStorage("e", 2, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage h = new RegisterStorage("h", 3, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage l = new RegisterStorage("l", 3, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage a = new RegisterStorage("a", 0, 8, PrimitiveType.Byte);

        public static readonly RegisterStorage bc = new RegisterStorage("bc", 1, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage de = new RegisterStorage("de", 2, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage hl = new RegisterStorage("hl", 3, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage sp = new RegisterStorage("sp", 4, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage ix = new RegisterStorage("ix", 5, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage iy = new RegisterStorage("iy", 6, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage af = new RegisterStorage("af", 0, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage f = new RegisterStorage("f", 0, 0, PrimitiveType.Byte);

        public static readonly RegisterStorage i = new RegisterStorage("i", 8, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage r = new RegisterStorage("r", 9, 0, PrimitiveType.Byte);

        public static readonly RegisterStorage bc_ = new RegisterStorage("bc'", 12, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage de_ = new RegisterStorage("de'", 13, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage hl_ = new RegisterStorage("hl'", 14, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage af_ = new RegisterStorage("af'", 15, 0, PrimitiveType.Word16);

        public static readonly FlagGroupStorage S = new FlagGroupStorage(f, (uint)FlagM.SF, "S", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(f, (uint)FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage P = new FlagGroupStorage(f, (uint)FlagM.PF, "P", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(f, (uint)FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(f, (uint)FlagM.CF, "C", PrimitiveType.Bool);

        internal static RegisterStorage[] All;
        internal static Dictionary<StorageDomain, RegisterStorage[]> SubRegisters;
        internal static Dictionary<string, RegisterStorage> regsByName;
        private static RegisterStorage[] regsByStorage;

        static Registers()
        {
            All = new RegisterStorage[] {
             b ,
             c ,
             d ,
             e ,

             h ,
             l ,
             a ,
             f,
               
             bc,
             de,
             hl,
             sp,
             ix,
             iy,
             af,
             null,

             i ,
             r ,
             null,
             null,

             bc_,
             de_,
             hl_,
             null,

            };

            Registers.regsByName = All.Where(reg => reg != null).ToDictionary(reg => reg.Name);
            regsByStorage = new[]
            {
                af, bc,  de, hl, sp, ix, iy, null,
                i,  r,   null, null, bc_, de_, hl_, af_,
            };

            SubRegisters = new Dictionary<StorageDomain, RegisterStorage[]>
            {
                {
                    af.Domain, new [] { Registers.a, Registers.f }
                },
                {
                    bc.Domain, new [] { Registers.c, Registers.b }
                },
                {
                    de.Domain, new []  { Registers.e, Registers.d }
                },
                {
                    hl.Domain, new[] { Registers.l, Registers.h }
                },
            };
        }

        internal static RegisterStorage GetRegister(int r)
        {
            return All[r];
        }

        internal static RegisterStorage GetRegister(string name)
        {
            return regsByName[name];
        }

        internal static RegisterStorage GetRegister(StorageDomain domain, ulong mask)
        {
            var iReg = domain - StorageDomain.Register;
            if (iReg < 0 || iReg >= regsByStorage.Length)
                return null;
            RegisterStorage regBest = regsByStorage[iReg];
            if (SubRegisters.TryGetValue(domain, out var subregs))
            {
                for (int i = 0; i < subregs.Length; ++i)
                {
                    var reg = subregs[i];
                    var regMask = reg.BitMask;
                    if ((mask & (~regMask)) == 0)
                        regBest = reg;
                }
            }
            return regBest;
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        SF = 0x80,             // sign
        ZF = 0x40,             // zero
        PF = 0x04,             // overflow / parity
        NF = 0x02,             // carry
        CF = 0x01,             // carry
    }

    public enum CondCode
    {
        nz,
        z,
        nc,
        c,
        po,
        pe,
        p,
        m,
    }
}