#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Tlcs.Tlcs90;
using Reko.Core;
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

namespace Reko.Arch.Tlcs
{
    /// <summary>
    /// Architecture definition for the 32-bit Toshiba TLCS-900
    /// processor.
    /// </summary>
    public class Tlcs90Architecture : ProcessorArchitecture
    {
        internal static Dictionary<StorageDomain, Dictionary<ulong, RegisterStorage>> Subregisters = new Dictionary<StorageDomain, Dictionary<ulong, RegisterStorage>>
        {
            {
                Registers.af.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.af.BitMask, Registers.af },
                    { Registers.a.BitMask, Registers.a },
                    { Registers.f.BitMask, Registers.f },
                }
            },
            {
                Registers.bc.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.bc.BitMask, Registers.bc },
                    { Registers.b.BitMask, Registers.b },
                    { Registers.c.BitMask, Registers.c },
                }
            },
            {
                Registers.de.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.de.BitMask, Registers.de },
                    { Registers.d.BitMask, Registers.d },
                    { Registers.e.BitMask, Registers.e },
                }
            },
            {
                Registers.hl.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.hl.BitMask, Registers.hl },
                    { Registers.h.BitMask, Registers.h },
                    { Registers.l.BitMask, Registers.l },
                }
            },
            {
                Registers.af_.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.af_.BitMask, Registers.af_ },
                }
            },
            {
                Registers.bc_.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.bc_.BitMask, Registers.bc_ },
                }
            },
            {
                Registers.de_.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.de_.BitMask, Registers.de_ },
                }
            },
            {
                Registers.hl_.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.hl_.BitMask, Registers.hl_ },
                }
            },
            {
                Registers.bx.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.bx.BitMask, Registers.bx },
                }
            },
            {
                Registers.by.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.by.BitMask, Registers.by },
                }
            },
            {
                Registers.sp.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.sp.BitMask, Registers.sp },
                }
            },
            {
                Registers.pc.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.pc.BitMask, Registers.pc },
                }
            },
            {
                Registers.f.Domain,
                new Dictionary<ulong, RegisterStorage>
                {
                    { Registers.f.BitMask, Registers.f },
                }
            }
        };

        public Tlcs90Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            base.CarryFlag = Registers.C;
            base.Endianness = EndianServices.Little;
            base.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.StackRegister = Registers.sp;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Tlcs90Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Tlcs90InstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Tlcs90State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Tlcs90Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            uint grf = 0;
            foreach (var c in name)
            {
                switch (c)
                {
                case 'S': grf |= Registers.S.FlagGroupBits; break;
                case 'Z': grf |= Registers.Z.FlagGroupBits; break;
                case 'I': grf |= Registers.I.FlagGroupBits; break;
                case 'H': grf |= Registers.H.FlagGroupBits; break;
                case 'V': grf |= Registers.V.FlagGroupBits; break;
                case 'N': grf |= Registers.N.FlagGroupBits; break;
                case 'C': grf |= Registers.C.FlagGroupBits; break;
                }
            }
            return GetFlagGroup(Registers.f, grf);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            foreach (FlagGroupStorage f in Registers.flagBits)
            {
                if (f.FlagGroupBits == grf)
                    return f;
            }

            var fl = new FlagGroupStorage(Registers.f, grf, GrfToString(Registers.f, "", grf));
            return fl;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(string name)
        {
            return Registers.allRegs.FirstOrDefault(r => r.Name == name);
        }

        public override RegisterStorage? GetRegister(StorageDomain regDomain, BitRange range)
        {
            if (!Subregisters.TryGetValue(regDomain, out var subs))
                return null;
            var key = range.BitMask();
            if (!subs.TryGetValue(key, out var subreg))
                return null;
            return subreg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.allRegs;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.I.FlagGroupBits) != 0) yield return Registers.I;
            if ((grf & Registers.H.FlagGroupBits) != 0) yield return Registers.H;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            // We ignore the flagRegister, as the architecture only has one flag register.
            var s = new StringBuilder();
            foreach (var freg in Registers.flagBits)
            {
                if ((freg.FlagGroupBits & grf) != 0)
                    s.Append(freg.Name);
            }
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            reg = Registers.allRegs.FirstOrDefault(r => string.Compare(r.Name ,name, StringComparison.OrdinalIgnoreCase) == 0);
            return reg is not null;
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }
    }
}
