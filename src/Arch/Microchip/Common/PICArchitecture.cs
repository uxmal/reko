#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.MicrochipPIC.Common
{

    public class PICArchitecture : ProcessorArchitecture
    {
        private const string opt_model = "model";
        private const string opt_execmode = "execution_mode";
        private const string opt_loadertype = "loader_type";

        protected Dictionary<uint, FlagGroupStorage> flagGroups;


        /// <summary>
        /// Instantiates a new PIC architecture for the specified PIC generic family.
        /// </summary>
        public PICArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            flagGroups = new Dictionary<uint, FlagGroupStorage>();
            Endianness = EndianServices.Little;
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 8;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
            this.Options = null!;
        }

        public PICArchitecture(IServiceProvider services, Dictionary<string, object> options)
            : this(services, "pic", options) { }

        /// <summary>
        /// Gets or sets the PIC architecture options.
        /// </summary>
        public new PICArchitectureOptions Options { get; set; }

        /// <summary>
        /// Gets the processor mode builders.
        /// </summary>
        public IPICProcessorModel ProcessorModel => Options?.ProcessorModel!;

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public IPICDescriptor PICDescriptor => ProcessorModel?.PICDescriptor!;

        /// <summary>
        /// Creates the PIC processor model.
        /// Creates the memory mapper, running state and registers for the targeted PIC.
        /// </summary>
        public void CreatePICProcessorModel()
        {
            if (Options is null || Options.ProcessorModel is null)
                throw new InvalidOperationException($"Needs to set architecture's {nameof(Options)} before calling {nameof(CreatePICProcessorModel)} method.");
            Description = Options.ProcessorModel.PICName;
            ProcessorModel.CreateMemoryDescriptor();
            ProcessorModel.CreateRegisters();
            StackRegister = PICRegisters.STKPTR;
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            => new PICInstructionComparer(norm);

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v.ToString().ToUpper(),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grpFlags)
        {
            if (flagGroups.TryGetValue(grpFlags, out var f))
                return f;

            var fl = new FlagGroupStorage(flagRegister, grpFlags, GrfToString(PICRegisters.STATUS, "", grpFlags));
            flagGroups.Add(grpFlags, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string flgsName)
        {
            FlagM grf = 0;
            for (int i = 0; i < flgsName.Length; ++i)
            {
                switch (flgsName[i])
                {
                    case 'C':
                        grf |= FlagM.C;
                        break;
                    case 'Z':
                        grf |= FlagM.Z;
                        break;
                    case 'D':
                        grf |= FlagM.DC;
                        break;
                    case 'O':
                        grf |= FlagM.OV;
                        break;
                    case 'N':
                        grf |= FlagM.N;
                        break;
                    default:
                        return null!;
                }
            }
            return GetFlagGroup(PICRegisters.STATUS, (uint)grf);
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            var grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.C) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint)FlagM.C);
            if ((grf & (uint) FlagM.Z) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint)FlagM.Z);
            if ((grf & (uint) FlagM.DC) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint)FlagM.DC);
            if ((grf & (uint) FlagM.OV) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint)FlagM.OV);
            if ((grf & (uint) FlagM.N) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint)FlagM.N);
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grpFlags)
        {
            var sb = new StringBuilder();
            byte bitPos = 0;
            while (grpFlags != 0)
            {
                if ((grpFlags & 1) != 0)
                {
                    if (PICRegisters.TryGetBitField(PICRegisters.STATUS, out var fld, bitPos, 1))
                    {
                        sb.Append(fld.Name);
                    }
                }
                grpFlags >>= 1;
                bitPos++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets a register given its index number.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register instance or null.
        /// </returns>
        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
            => PICRegisters.GetSubregister(
                    PICRegisters.PeekRegisterByIdx(domain - StorageDomain.Register),
                    range.Lsb,
                    range.Extent)!;

        /// <summary>
        /// Gets a register given its name.
        /// </summary>
        /// <param name="regName">Name of the register.</param>
        /// <returns>
        /// The register or null if there is no such register.
        /// </returns>
        public override RegisterStorage? GetRegister(string regName)
            => PICRegisters.GetRegister(regName);

        /// <summary>
        /// Find the parent (joined) register of the register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">The child (joinee) register.</param>
        /// <returns>
        /// The parent register or null.
        /// </returns>
        public RegisterStorage GetParentRegister(RegisterStorage reg)
            => PICRegisters.GetParentRegister(reg)!;

        /// <summary>
        /// Gets the registers.
        /// </summary>
        /// <returns>
        /// An array of register storage.
        /// </returns>
        public override RegisterStorage[] GetRegisters()
            => PICRegisters.GetRegisters;

        /// <summary>
        /// Attempts to get a <see cref="RegisterStorage"/> from its name.
        /// </summary>
        /// <param name="regName">Name of the register.</param>
        /// <param name="reg">[out] The register.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public override bool TryGetRegister(string regName, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            var res = PICRegisters.TryGetRegister(regName, out var preg);
            reg = preg;
            return res;
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            //TODO: throw exception instead of tinkering the options, when dcproject loading will be effective.
            if (options is null)
            {
                options = new Dictionary<string, object>()
                {
                    { opt_model, PICProcessorModel.DefaultPICName },
                    { opt_execmode, PICExecMode.Traditional },
                    { opt_loadertype, "raw" }
                };
            }

            if (options.TryGetValue(opt_model, out var model))
            {
                switch (model)
                {
                    case string spicname:
                        PeekOption(options, opt_execmode, out PICExecMode eExecMode, PICExecMode.Traditional);
                        PeekOption(options, opt_loadertype, out string loaderType, "raw");
                        Options = new PICArchitectureOptions(spicname, eExecMode, loaderType);
                        break;

                    case PICArchitectureOptionsPicker picker:
                        Options = new PICArchitectureOptions(picker);
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid PIC model user options.");
                }
                CreatePICProcessorModel();
            }
            else
                throw new InvalidOperationException($"Missing PIC model user options.");
        }

        public override Dictionary<string, object>? SaveUserOptions()
        {
            if (Options is null)
                return null;
            var dict = new Dictionary<string, object>
            {
                [opt_model] = Options.ProcessorModel.PICName,
                [opt_execmode] = Options.PICExecutionMode.ToString(),
                [opt_loadertype] = Options.LoaderType
            };
            return dict;
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => ProcessorModel.CreateRewriter(this, ProcessorModel.CreateDisassembler(this, rdr), (PICProcessorState)state, frame, host);

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => ProcessorModel.CreateProcessorState(this);

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
            => Address.Ptr32(c.ToUInt32());

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
            => ProcessorModel.CreateBankedAddress(seg.ToByte(), offset.ToByte());

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
            => throw new NotSupportedException("Microchip PIC has no explicit argument stack.");

        public override Address? ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state)
            => Address.Ptr32(rdr.ReadLeUInt32());

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
            => Address.TryParse32(txtAddress, out addr);

        public override void PostprocessProgram(Program program)
            => ProcessorModel.PostprocessProgram(program, this);

        private PICDisassemblerBase CreateDisassemblerImpl(EndianImageReader imageReader)
            => ProcessorModel.CreateDisassembler(this, imageReader);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return ProcessorModel.CreatePointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        private void PeekOption<T>(Dictionary<string, object> options, string name, out T optvalue, T defaultValue)
        {
            optvalue = defaultValue;
            if (options.TryGetValue(name.ToLower(), out var obj) && (obj is T tobj))
            {
                optvalue = tobj;
            }
        }

    }

}
