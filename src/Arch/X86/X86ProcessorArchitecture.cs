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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Reko.Core.Code;
using Reko.Core.Operators;

namespace Reko.Arch.X86
{
	// X86 flag masks.

	[Flags]
	public enum FlagM : byte
	{
		SF = 1,             // sign
		CF = 2,             // carry
		ZF = 4,             // zero
		DF = 8,             // direction
		
        OF = 16,            // overflow
        PF = 32,            // parity
	}

    /// <summary>
    /// Processor architecture definition for the Intel x86 family. Currently supported processors are 8086/7,
    /// 80186/7, 80286/7, 80386/7, 80486, Pentium, and x86-64.
    /// </summary>
    [Designer("Reko.Arch.X86.Design.X86ArchitectureDesigner,Reko.Arch.X86.Design")]
	public class IntelArchitecture : ProcessorArchitecture
	{
		private ProcessorMode mode;
        private Dictionary<uint, FlagGroupStorage> flagGroupCache;

        public IntelArchitecture(string archId, ProcessorMode mode) : base(archId)
        {
            this.mode = mode;
            this.flagGroupCache = new Dictionary<uint, FlagGroupStorage>();
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.CarryFlagMask = (uint)FlagM.CF;
            this.PointerType = mode.PointerType;
            this.WordWidth = mode.WordWidth;
            this.FramePointerType = mode.FramePointerType;
            this.StackRegister = mode.StackRegister;
            this.FpuStackRegister = Registers.Top;
            this.Options = new X86Options();
        }

        public X86Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
        {
            return mode.CreateDisassembler(imageReader, Options);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return mode.CreateEmulator(this, segmentMap, envEmulator);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new X86InstructionComparer(norm);
        }

        public override FrameApplicationBuilder CreateFrameApplicationBuilder(IStorageBinder binder, CallSite site, Expression callee)
        {
            return new X86FrameApplicationBuilder(this, binder, site, callee, false);
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

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
		{
            return CreateDisassemblerImpl(imageReader);
		}

		public override ProcessorState CreateProcessorState()
		{
			return new X86State(this);
		}

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new X86Rewriter(this, host, (X86State) state, rdr, binder);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            return mode.CreateInstructionScanner(map, rdr, knownAddresses, flags);
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            return mode.CreateStackAccess(binder, offset, dataType);
        }

        //$REFACTOR: this probably should live in X86FrameApplicationBuilder
        public override Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            Expression e = binder.EnsureRegister(Registers.Top);
            if (offset != 0)
            {
                BinaryOperator op;
                if (offset < 0)
                {
                    offset = -offset;
                    op = Operator.ISub;
                }
                else
                {
                    op = Operator.IAdd;
                }
                e = new BinaryExpression(op, e.DataType, e, Constant.Create(e.DataType, offset));
            }
            return new MemoryAccess(Registers.ST, e, dataType);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return mode.MakeAddressFromConstant(c);
        }

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
        {
            return mode.CreateSegmentedAddress(seg.ToUInt16(), offset.ToUInt32());
        }

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
        {
            if (!mode.TryReadCodeAddress(byteSize, rdr, state, out var addr))
                addr = null;
            return addr;
        }

        public RegisterStorage GetControlRegister(int v)
        {
            return mode.GetControlRegister(v);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
		{
            FlagGroupStorage f;
            if (flagGroupCache.TryGetValue(grf, out f))
			{
				return f;
			}

			var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            f = new FlagGroupStorage(Registers.eflags, grf, GrfToString(flagRegister, "", grf), dt);
			flagGroupCache.Add(grf, f);
			return f;
		}

        public override FlagGroupStorage GetFlagGroup(string name)
		{
			FlagM grf = 0;
			for (int i = 0; i < name.Length; ++i)
			{
				switch (name[i])
				{
				case 'S': grf |= FlagM.SF; break;
				case 'C': grf |= FlagM.CF; break;
				case 'Z': grf |= FlagM.ZF; break;
				case 'D': grf |= FlagM.DF; break;
				case 'O': grf |= FlagM.OF; break;
				case 'P': grf |= FlagM.PF; break;
                default: return null;
				}
			}
			return GetFlagGroup(Registers.eflags, (uint) grf);
		}

        public override RegisterStorage GetRegister(string name)
		{
			var r = Registers.GetRegister(name);
			if (r == RegisterStorage.None)
				throw new ArgumentException(string.Format("'{0}' is not a register name.", name));
			return r;
		}

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return GetSubregisterUsingMask(domain, range.BitMask());
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            return Registers.All.Where(r => r != null && r.OverlapsWith(reg));
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            var mask = reg.BitMask & new BitRange(offset, offset + width).BitMask();
            var subreg = GetSubregisterUsingMask(reg.Domain, mask) ?? reg;
            return subreg;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.D.FlagGroupBits) != 0) yield return Registers.D;
            if ((grf & Registers.O.FlagGroupBits) != 0) yield return Registers.O;
            if ((grf & Registers.P.FlagGroupBits) != 0) yield return Registers.P;
        }

        private static RegisterStorage GetSubregisterUsingMask(StorageDomain domain, ulong mask)
        {
            RegisterStorage[] subregs;
            if (mask == 0)
                return null;
            RegisterStorage reg = null;
            if (Registers.SubRegisters.TryGetValue(domain, out subregs))
            {
                for (int i = 0; i < subregs.Length; ++i)
                {
                    var subreg = subregs[i];
                    var regMask = subreg.BitMask;
                    if ((mask & (~regMask)) == 0)
                        reg = subreg;
                }
            }
            return reg;
        }

        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
        {
            ulong mask = bits.Where(b => b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            if (mask == 0)
                return null;
            mask &= reg.BitMask;
            RegisterStorage[] subregs;
            if (Registers.SubRegisters.TryGetValue(reg.Domain, out subregs))
            {
                for (int i = 0; i < subregs.Length; ++i)
                {
                    var subreg = subregs[i];
                    var regMask = subreg.BitMask;
                    if ((mask & (~regMask)) == 0)
                        reg = subreg;
                }
            }
            return reg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All.Where(a => a != null).ToArray();
        }

        public override List<RtlInstruction> InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return this.mode.InlineCall(addrCallee, addrContinuation, rdr, binder);
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            if (options != null)
            {
                this.Options = new X86Options
                {
                    Emulate8087 = options.ContainsKey("Emulate8087") && (string)options["Emulate8087"] == "true"
                };
            }
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            if (Options == null)
                return null;
            var dict = new Dictionary<string, object>();
            if (Options.Emulate8087)
            {
                dict["Emulate8087"] = "true";
            }
            return dict;
        }

        public override void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg)
        {
            foreach (var rAlias in GetAliases(reg))
            {
                ids.Remove(rAlias);
                if (reg.BitAddress > 0 && rAlias.BitSize == 16)
                {
                    ids.Add(GetSubregister(rAlias, 0, 8));
                }
            }
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = Registers.GetRegister(name);
            return (reg != RegisterStorage.None);
        }

		public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
		{
			StringBuilder s = new StringBuilder();
            foreach (var fr in Registers.EflagsBits)
            {
                if ((fr.FlagGroupBits & grf) != 0) s.Append(fr.Name);
			}
			return s.ToString();
		}

		public ProcessorMode ProcessorMode
		{
			get { return mode; }
		}

        public X86Options Options { get; set; }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return mode.TryParseAddress(txtAddress, out addr);
        }
    }

    public class X86ArchitectureReal : IntelArchitecture
    {
        public X86ArchitectureReal(string archId)
            : base(archId, ProcessorMode.Real)
        {
        }
    }

    public class X86ArchitectureProtected16 : IntelArchitecture
    {
        public X86ArchitectureProtected16(string archId)
            : base(archId, ProcessorMode.ProtectedSegmented)
        {
        }
    }

    public class X86ArchitectureFlat32 : IntelArchitecture
    {
        public X86ArchitectureFlat32(string archId)
            : base(archId, ProcessorMode.Protected32)
        {
        }
    }

    public class X86ArchitectureFlat64 : IntelArchitecture
    {
        public X86ArchitectureFlat64(string archId)
            : base(archId, ProcessorMode.Protected64)
        {
        }
    }
}
