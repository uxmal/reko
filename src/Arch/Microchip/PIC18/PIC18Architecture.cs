#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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
using System.Linq;
using System.Text;

using Microchip.Crownking;
using Reko.Arch.Microchip.Common;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// PIC18 processor architecture.
    /// </summary>
    public class PIC18Architecture : ProcessorArchitecture
    {
        private List<FlagGroupStorage> flagGroups;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC18Architecture()
        {
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 16;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        public PIC18Architecture(PIC picDescr) : this()
        {
            LoadConfiguration(picDescr);
        }

        /// <summary>
        /// Loads the PICa configuration.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        public void LoadConfiguration(PIC picDescr)
        {
            PICDescriptor = picDescr;
            Name = picDescr.Name;
            Description = picDescr.Desc;
            PIC18Registers.Create(picDescr).LoadRegisters(); ;
            StackRegister = PIC18Registers.STKPTR;
        }

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor { get; private set; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public PICExecMode ExecMode
        {
            get { return MemoryMapper.ExecMode; }
            set { MemoryMapper.ExecMode = value; }
        }

        /// <summary>
        /// Gets the PIC memory mapper.
        /// </summary>
        public PIC18MemoryMapper MemoryMapper
        {
            get
            {
                if (_memmapper == null)
                    _memmapper = PIC18MemoryMapper.Create(PICDescriptor);
                return _memmapper;
            }
        }
        PIC18MemoryMapper _memmapper;

        public PIC18Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
            => new PIC18Disassembler(this, imageReader);

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
            => new LeImageReader(image, addr);

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
            => new LeImageReader(image, addrBegin, addrEnd);

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
            => new LeImageReader(image, offset);

        public override ImageWriter CreateImageWriter()
            =>new LeImageWriter();

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            => new LeImageWriter(mem, addr);

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            => new PIC18InstructionComparer(norm);

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
            Opcode result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            foreach (FlagGroupStorage f in flagGroups)
            {
                if (f.FlagGroupBits == grf)
                    return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(PIC18Registers.STATUS, grf, GrfToString(grf), dt);
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
                    case 'C': grf |= FlagM.C; break;
                    case 'Z': grf |= FlagM.Z; break;
                    case 'D': grf |= FlagM.DC; break;
                    case 'O': grf |= FlagM.OV; break;
                    case 'N': grf |= FlagM.N; break;
                    default: return null;
                }
            }
            return GetFlagGroup((uint)grf);
        }

        public override RegisterStorage GetRegister(int i)
            => PIC18Registers.GetCoreRegister(i);

        public override RegisterStorage GetRegister(string name)
        {
            var r = PIC18Registers.GetRegister(name);
            if (r == RegisterStorage.None)
                throw new ArgumentException($"'{name}' is not a register name.");
            return r;
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield break;
        }

        public override RegisterStorage[] GetRegisters()
            => PIC18Registers.GetRegisters;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => new PIC18State(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => new PIC18Rewriter(this, rdr, (PIC18State)state, frame, host);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            //TODO: CreatePointerScanner - understand purpose, implement
            throw new NotImplementedException($"{nameof(CreatePointerScanner)} not implemented.");
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("PIC18 has no explicit argument stack");
        }

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
        {
            //TODO: ReadCodeAddress - understand purpose, implement
            throw new NotImplementedException($"{nameof(ReadCodeAddress)} not implemented.");
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = PIC18Registers.GetRegister(name);
            return (reg != RegisterStorage.None);
        }

        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            uint bitPos = 0;
            while (grf != 0)
            {
                if ((grf & 1) != 0)
                {
                    var f = PIC18Registers.GetBitField(PIC18Registers.STATUS, bitPos, 1);
                    if (f != null)
                        s.Append(f.Name);
                }
                grf >>= 1;
                bitPos++;
            }
            return s.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
            => Address.TryParse32(txtAddress, out addr);

    }

}
