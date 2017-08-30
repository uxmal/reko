#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
    /// 80186/7, 80286/7, 80386/7, 80486, and Pentium,  x86-64
    /// </summary>
    [Designer("Reko.Arch.X86.Design.X86ArchitectureDesigner,Reko.Arch.X86.Design")]
	public class IntelArchitecture : ProcessorArchitecture
	{
		private ProcessorMode mode;
        private List<FlagGroupStorage> flagGroups;

        public IntelArchitecture(ProcessorMode mode)
        {
            this.mode = mode;
            this.flagGroups = new List<FlagGroupStorage>();
            this.InstructionBitSize = 8;
            this.CarryFlagMask = (uint)FlagM.CF;
            this.PointerType = mode.PointerType;
            this.WordWidth = mode.WordWidth;
            this.FramePointerType = mode.FramePointerType;
            this.StackRegister = mode.StackRegister;
            this.FpuStackRegister = Registers.Top;
            this.Options = new X86Options();
        }

		public Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
		{
			if (mode == ProcessorMode.Protected32)
			{
				return Address.Ptr32(offset);
			}
			else
			{
				return state.AddressFromSegOffset(seg, offset);
			}
		}

        public X86Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
        {
            return mode.CreateDisassembler(imageReader, Options);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new X86InstructionComparer(norm);
        }

        public override FrameApplicationBuilder CreateFrameApplicationBuilder(IStorageBinder binder, CallSite site, Expression callee)
        {
            return new X86FrameApplicationBuilder(this, binder, site, callee, false);
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            Opcode result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
        {
            ulong mask = bits.Where(b => b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            Dictionary<uint, RegisterStorage> subregs;
            if ((mask & reg.BitMask) == reg.BitMask)
                return reg;
            RegisterStorage rMax = null;
            if (Registers.SubRegisters.TryGetValue(reg, out subregs))
            {
                foreach (var subreg in subregs.Values)
                {
                    if ((subreg.BitMask & mask) == subreg.BitMask &&
                        (rMax == null || subreg.BitSize > rMax.BitSize))
                    {
                        rMax = subreg;
                    }
                }
            }
            return rMax;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
		{
            return CreateDisassemblerImpl(imageReader);
		}

		public override ProcessorState CreateProcessorState()
		{
			return new X86State(this);
		}

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            return new X86Rewriter(this, host, (X86State) state, rdr, frame);
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

        public override Address MakeAddressFromConstant(Constant c)
        {
            return mode.MakeAddressFromConstant(c);
        }

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
        {
            return mode.CreateSegmentedAddress(seg.ToUInt16(), offset.ToUInt32());
        }

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
        {
            return mode.ReadCodeAddress(byteSize, rdr, state);
        }

		public override FlagGroupStorage GetFlagGroup(uint grf)
		{
			foreach (FlagGroupStorage f in flagGroups)
			{
				if (f.FlagGroupBits == grf)
					return f;
			}

			PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.eflags, grf, GrfToString(grf), dt);
			flagGroups.Add(fl);
			return fl;
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
			return GetFlagGroup((uint) grf);
		}

		public override RegisterStorage GetRegister(int i)
		{
			return Registers.GetRegister(i);
		}

        public override RegisterStorage GetRegister(string name)
		{
			var r = Registers.GetRegister(name);
			if (r == RegisterStorage.None)
				throw new ArgumentException(string.Format("'{0}' is not a register name.", name));
			return r;
		}

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            return Registers.All.Where(r => r != null && r.OverlapsWith(reg));
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0 && reg.BitSize == (ulong) width)
                return reg;
            Dictionary<uint, RegisterStorage> dict;
            if (!Registers.SubRegisters.TryGetValue(reg, out dict))
                return null;
            RegisterStorage subReg;
            if (!dict.TryGetValue((uint)(offset * 256 + width), out subReg))
                return null;
            return subReg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All.Where(a => a != null).ToArray();
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


		public override string GrfToString(uint grf)
		{
			StringBuilder s = new StringBuilder();
			for (int r = Registers.S.Number; grf != 0; ++r, grf >>= 1)
			{
				if ((grf & 1) != 0)
					s.Append(Registers.GetRegister(r).Name);
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
        public X86ArchitectureReal()
            : base(ProcessorMode.Real)
        {
            this.Name = "x86-real-16";
        }
    }

    public class X86ArchitectureProtected16 : IntelArchitecture
    {
        public X86ArchitectureProtected16()
            : base(ProcessorMode.ProtectedSegmented)
        {
            this.Name = "x86-protected-16";
        }
    }

    public class X86ArchitectureFlat32 : IntelArchitecture
    {
        public X86ArchitectureFlat32()
            : base(ProcessorMode.Protected32)
        {
            this.Name = "x86-protected-32";
        }
    }

    public class X86ArchitectureFlat64 : IntelArchitecture
    {
        public X86ArchitectureFlat64()
            : base(ProcessorMode.Protected64)
        {
            this.Name = "x86-protected-64";
        }
    }
}
