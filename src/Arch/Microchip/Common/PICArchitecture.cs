#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.MicrochipPIC.Common
{

    public class PICArchitecture : ProcessorArchitecture
    {
        protected List<FlagGroupStorage> flagGroups;


        /// <summary>
        /// Instantiates a new PIC architecture for the specified PIC generic family.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        public PICArchitecture(string archID) : base("pic")
        {
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 8;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
        }

        /// <summary>
        /// Gets or sets the PIC architecture options.
        /// </summary>
        public PICArchitectureOptions Options { get; set; }

        /// <summary>
        /// Gets the processor mode builders.
        /// </summary>
        public IPICProcessorMode ProcessorMode => Options?.ProcessorMode;

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor => ProcessorMode?.PICDescriptor;

        /// <summary>
        /// Gets the PIC processor state.
        /// </summary>
        public PICProcessorState State { get; private set; }

        /// <summary>
        /// Creates the PIC processor model.
        /// Creates the memory mapper, running state and registers for the targeted PIC.
        /// </summary>
        public void CreatePICProcessorModel()
        {
            if (Options == null || Options.ProcessorMode == null)
                throw new InvalidOperationException($"Needs to set architecture's {nameof(Options)} before calling {nameof(CreatePICProcessorModel)} method.");
            Description = PICDescriptor.Desc;
            ProcessorMode.CreateMemoryDescriptor();
            ProcessorMode.CreateRegisters();
            State = ProcessorMode.CreateProcessorState(this);
            StackRegister = PICRegisters.STKPTR;
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            => new PICInstructionComparer(norm);

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString().ToUpper(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Opcode result))
                return null;
            return (int)result;
        }

        public override FlagGroupStorage GetFlagGroup(uint grpFlags)
        {
            foreach (FlagGroupStorage f in flagGroups)
            {
                if (f.FlagGroupBits == grpFlags)
                    return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grpFlags) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(PICRegisters.STATUS, grpFlags, GrfToString(grpFlags), dt);
            flagGroups.Add(fl);
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
                        return null;
                }
            }
            return GetFlagGroup((uint)grf);
        }

        public override string GrfToString(uint grpFlags)
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
        public override RegisterStorage GetRegister(int i)
            => PICRegisters.PeekRegisterByIdx(i);

        /// <summary>
        /// Gets a register given its name.
        /// </summary>
        /// <param name="regName">Name of the register.</param>
        /// <returns>
        /// The register.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///                                     illegal values.</exception>
        public override RegisterStorage GetRegister(string regName)
            => PICRegisters.GetRegister(regName);

        /// <summary>
        /// Get the proper sub-register of <paramref name="reg" /> that starts at offset
        /// <paramref name="offset" /> and is of size <paramref name="width"/>.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="offset">The bit offset of the sub-register.</param>
        /// <param name="width">The bit width of the sub-register.</param>
        /// <returns>
        /// The sub-register.
        /// </returns>
        /// <remarks>
        /// Most architectures not have sub-registers, and will use this default implementation. This
        /// method is overridden for architectures like x86 and Z80, where sub-registers <code>(ah, al,
        /// etc)</code>
        /// do exist.
        /// </remarks>
        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
            => PICRegisters.GetSubregister(reg, offset, width);

        /// <summary>
        /// Find the widest sub-register that covers the register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">The target register.</param>
        /// <param name="regs">.</param>
        /// <returns>
        /// The widest subregister(s).
        /// </returns>
        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs)
            => PICRegisters.GetWidestSubregister(reg, regs);

        /// <summary>
        /// Find the parent (joined) register of the register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">The child (joinee) register.</param>
        /// <returns>
        /// The parent register or null.
        /// </returns>
        public RegisterStorage GetParentRegister(RegisterStorage reg)
            => PICRegisters.GetParentRegister(reg);

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
        public override bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            var res = PICRegisters.TryGetRegister(regName, out var preg);
            reg = preg;
            return res;
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield break;
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            if (options == null)
                throw new InvalidOperationException("Missing PIC model user options.");

            if (options.TryGetValue("Model", out var model))
            {
                switch (model)
                {
                    case string spicname:
                        var eExecMode = PICExecMode.Extended;
                        if (options.TryGetValue("ExecuteMode", out var execmode) && (execmode is PICExecMode eexecmod))
                        {
                            eExecMode = eexecmod;
                        }
                        Options = new PICArchitectureOptions(spicname, eExecMode);
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

        public override Dictionary<string, object> SaveUserOptions()
        {
            if (Options == null)
                return null;
            var dict = new Dictionary<string, object>
            {
                ["Model"] = Options.ProcessorMode.PICName,
                ["ExecuteMode"] = Options.PICExecutionMode.ToString()
            };
            return dict;
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
            => new LeImageReader(image, addr);

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
            => new LeImageReader(image, addrBegin, addrEnd);

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
            => new LeImageReader(image, offset);

        public override ImageWriter CreateImageWriter()
            => new LeImageWriter();

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            => new LeImageWriter(mem, addr);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => ProcessorMode.CreateRewriter(this, ProcessorMode.CreateDisassembler(this, rdr), (PICProcessorState)state, frame, host);

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => ProcessorMode.CreateProcessorState(this);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
            => ProcessorMode.CreateBankedAddress(seg.ToByte(), offset.ToByte());

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("Microchip PIC has no explicit argument stack");
        }

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
            => PICProgAddress.Ptr(rdr.ReadLeUInt32());

        public override bool TryParseAddress(string txtAddress, out Address addr)
            => Address.TryParse32(txtAddress, out addr);

        public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
            => mem.TryReadLe(addr, dt, out value);

        private PICDisassemblerBase CreateDisassemblerImpl(EndianImageReader imageReader)
            => ProcessorMode.CreateDisassembler(this, imageReader);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return ProcessorMode.CreatePointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }
    }

}
