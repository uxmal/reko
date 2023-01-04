#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Z80
{
    public class Z80ProcessorArchitecture : ProcessorArchitecture
    {
        public Z80ProcessorArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.PointerType = PrimitiveType.Ptr16;
            this.WordWidth = PrimitiveType.Word16;
            this.StackRegister = Registers.sp;
            this.CarryFlagMask = (uint)FlagM.CF;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Z80Disassembler(this, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Z80InstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Z80ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt16()).ToHashSet();
            return new Z80PointerScanner(map, rdr, knownLinAddresses, flags).Select(li => Address.Ptr16(li));
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Z80Rewriter(this, rdr, state, binder, host);
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v == Mnemonic.ex_af ? "ex" : Enum.GetName(typeof(Mnemonic), v)!,
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (string.Compare(name, "ex", StringComparison.InvariantCultureIgnoreCase) == 0)
                return (int)Mnemonic.ex_af;
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override RegisterStorage? GetRegister(string name)
        {
            return Registers.GetRegister(name);
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.GetRegister(domain, range.BitMask());
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All!;
        }

        public override FlagGroupStorage[] GetFlags()
        {
            return Registers.Flags;
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
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

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.f, grf, GrfToString(flagRegister, "", grf), dt);
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

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }
    }

    public static class Registers
    {
        public static readonly RegisterStorage b = RegisterStorage.Reg8("b", 1, 8);
        public static readonly RegisterStorage c = RegisterStorage.Reg8("c", 1);
        public static readonly RegisterStorage d = RegisterStorage.Reg8("d", 2, 8);
        public static readonly RegisterStorage e = RegisterStorage.Reg8("e", 2);
        public static readonly RegisterStorage h = RegisterStorage.Reg8("h", 3, 8);
        public static readonly RegisterStorage l = RegisterStorage.Reg8("l", 3);
        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0, 8);

        public static readonly RegisterStorage bc = RegisterStorage.Reg16("bc", 1);
        public static readonly RegisterStorage de = RegisterStorage.Reg16("de", 2);
        public static readonly RegisterStorage hl = RegisterStorage.Reg16("hl", 3);
        public static readonly RegisterStorage sp = RegisterStorage.Reg16("sp", 4);
        public static readonly RegisterStorage ix = RegisterStorage.Reg16("ix", 5);
        public static readonly RegisterStorage iy = RegisterStorage.Reg16("iy", 6);
        public static readonly RegisterStorage af = RegisterStorage.Reg16("af", 0);

        public static readonly RegisterStorage f = RegisterStorage.Reg8("f", 0);

        public static readonly RegisterStorage i = RegisterStorage.Reg8("i", 8);
        public static readonly RegisterStorage r = RegisterStorage.Reg8("r", 9);

        public static readonly RegisterStorage bc_ = RegisterStorage.Reg16("bc'", 12);
        public static readonly RegisterStorage de_ = RegisterStorage.Reg16("de'", 13);
        public static readonly RegisterStorage hl_ = RegisterStorage.Reg16("hl'", 14);
        public static readonly RegisterStorage af_ = RegisterStorage.Reg16("af'", 15);

        public static readonly FlagGroupStorage S = new FlagGroupStorage(f, (uint)FlagM.SF, "S", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(f, (uint)FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage P = new FlagGroupStorage(f, (uint)FlagM.PF, "P", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(f, (uint)FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(f, (uint)FlagM.CF, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage SZ = new FlagGroupStorage(f, (uint) (FlagM.SF|FlagM.ZF), "SZ", PrimitiveType.Byte);
        public static readonly FlagGroupStorage SZP = new FlagGroupStorage(f, (uint) (FlagM.ZF | FlagM.SF | FlagM.PF), "SZP", PrimitiveType.Byte);
        public static readonly FlagGroupStorage SZPC = new FlagGroupStorage(f, (uint) (FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF), "SZPC", PrimitiveType.Byte);

        internal static RegisterStorage?[] All;
        internal static FlagGroupStorage[] Flags = new[] { S, Z, P, N, C };
        internal static Dictionary<StorageDomain, RegisterStorage[]> SubRegisters;
        internal static Dictionary<string, RegisterStorage> regsByName;
        private readonly static RegisterStorage?[] regsByStorage;

        static Registers()
        {
            All = new RegisterStorage?[] {
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

            Registers.regsByName = All.Where(reg => reg != null).ToDictionary(reg => reg!.Name, reg => reg!);
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

        internal static RegisterStorage? GetRegister(int r)
        {
            return All[r];
        }

        internal static RegisterStorage? GetRegister(string name)
        {
            return regsByName.TryGetValue(name, out var reg) ? reg : null;
        }

        internal static RegisterStorage? GetRegister(StorageDomain domain, ulong mask)
        {
            var iReg = domain - StorageDomain.Register;
            if (iReg < 0 || iReg >= regsByStorage.Length)
                return null;
            RegisterStorage? regBest = regsByStorage[iReg];
            if (regBest is null)
                return null;
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