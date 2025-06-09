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

using Reko.Arch.X86.Analysis;
using Reko.Arch.X86.Rewriter;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Assemblers;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

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
        private readonly ProcessorMode mode;
        private Decoder[]? rootDecoders;

        public IntelArchitecture(IServiceProvider services, string archId, ProcessorMode mode, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.mode = mode;
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.CarryFlag = Registers.C;
            this.PointerType = mode.PointerType;
            this.WordWidth = mode.WordWidth;
            this.FramePointerType = mode.FramePointerType;
            this.StackRegister = mode.StackRegister;
            this.FpuStackRegister = Registers.Top;
            this.Options = options;
            this.LoadUserOptions(options);
        }

        public override IAssembler CreateAssembler(string? asmDialect)
        {
            return new Assembler.X86TextAssembler(this);
        }

        public X86Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
        {
            return mode.CreateDisassembler(this.Services, EnsureRootDecoders(), imageReader, Options);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return mode.CreateEmulator(this, segmentMap, envEmulator);
        }

        public override T? CreateExtension<T>() where T : class
        {
            if (typeof(IAnalysisFactory).IsAssignableFrom(typeof(T)))
                return (T) (object) new X86AnalysisFactory();
            return default;
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new X86InstructionComparer(norm);
        }

        public override FrameApplicationBuilder CreateFrameApplicationBuilder(IStorageBinder binder, CallSite site)
        {
            return new X86FrameApplicationBuilder(this, binder, site);
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

        private Decoder[] EnsureRootDecoders()
        {
            if (this.rootDecoders is null)
            {
                rootDecoders = mode.CreateRootDecoders(Options);
            }
            return rootDecoders;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return mode.MakeAddressFromConstant(c);
        }

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
        {
            return mode.CreateSegmentedAddress(seg.ToUInt16(), offset.ToUInt32())!.Value;
        }

        public override Address? ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state)
        {
            if (!mode.TryReadCodeAddress(byteSize, rdr, state, out var addr))
                return null;
            return addr;
        }

        public RegisterStorage? GetControlRegister(int v)
        {
            return mode.GetControlRegister(v);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
		{
            var f = new FlagGroupStorage(Registers.eflags, grf, GrfToString(flagRegister, "", grf));
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
                default: throw new ArgumentException($"Unknown x86 flag bit '{name[i]}'.");
				}
			}
			return GetFlagGroup(Registers.eflags, (uint) grf);
		}

        public override RegisterStorage? GetRegister(string name)
		{
			var r = Registers.GetRegister(name);
            if (r == RegisterStorage.None)
                return null;
			return r;
		}

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return GetSubregister(domain, range);
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

        internal static RegisterStorage? GetSubregister(StorageDomain domain, BitRange range)
        {
            if (range.IsEmpty)
                return null;
            RegisterStorage? reg = null;
            if (Registers.SubRegisters.TryGetValue(domain, out RegisterStorage[]? subregs))
            {
                for (int i = 0; i < subregs.Length; ++i)
                {
                    var subreg = subregs[i];
                    var subRange = new BitRange((int) subreg.BitAddress, (int) (subreg.BitAddress + subreg.BitSize));
                    if (subRange.Covers(range))
                        reg = subreg;
                }
            }
            return reg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All.Where(a => a is not null).ToArray();
        }

        public override FlagGroupStorage[] GetFlags()
        {
            return Registers.EflagsBits;
        }

        public override List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            var dasm = mode.CreateDisassembler(Services, EnsureRootDecoders(), rdr, this.Options);
            return this.mode.InlineCall(this.Services, dasm, addrCallee, addrContinuation, binder);
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options is not null)
            {
                this.rootDecoders = null;
                this.Options = options;
            }
        }

        public override Dictionary<string, object>? SaveUserOptions()
        {
            if (Options is null)
                return null;
            var dict = new Dictionary<string, object>(Options);
            return dict;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = Registers.GetRegister(name);
            return (reg != RegisterStorage.None);
        }

		public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
		{
			StringBuilder s = new StringBuilder();
            if (flagregister == Registers.eflags)
            {
                foreach (var fr in Registers.EflagsBits)
                {
                    if ((fr.FlagGroupBits & grf) != 0) s.Append(fr.Name);
                }
            }
            else if (flagregister == Registers.FPUF)
            {
                foreach (var fr in Registers.FpuFlagsBits)
                {
                    if ((fr.FlagGroupBits & grf) != 0) s.Append(fr.Name);
                }
            }
            return s.ToString();
		}

		public ProcessorMode ProcessorMode
		{
			get { return mode; }
		}

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return mode.TryParseAddress(txtAddress, out addr);
        }
    }

    public class X86ArchitectureReal : IntelArchitecture
    {
        public X86ArchitectureReal(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, ProcessorMode.Real, options)
        {
        }
    }

    public class X86ArchitectureProtected16 : IntelArchitecture
    {
        public X86ArchitectureProtected16(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, ProcessorMode.ProtectedSegmented, options)
        {
        }
    }

    public class X86ArchitectureFlat32 : IntelArchitecture
    {
        public X86ArchitectureFlat32(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, ProcessorMode.Protected32, options)
        {
        }
    }

    public class X86ArchitectureFlat64 : IntelArchitecture
    {
        public X86ArchitectureFlat64(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, ProcessorMode.Protected64, options)
        {
        }
    }
}
