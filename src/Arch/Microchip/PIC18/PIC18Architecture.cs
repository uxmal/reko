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

using Reko.Libraries.Microchip;
using Reko.Arch.Microchip.Common;

namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// PIC18 processor architecture.
    /// </summary>
    public class PIC18Architecture : ProcessorArchitecture
    {
        private List<FlagGroupStorage> flagGroups;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Interpreted as the name of the PIC.</param>
        public PIC18Architecture(string archID) : base(archID)
        {
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 16;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
            LoadConfiguration(PICDescriptor);
        }

        /// <summary>
        /// Constructor. Used for tests purpose.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        public PIC18Architecture(PIC picDescr) : base(picDescr.Name)
        {
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 16;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
            LoadConfiguration(picDescr);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads the PIC configuration. Creates memory mapper and registers.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        private void LoadConfiguration(PIC picDescr)
        {
            PICDescriptor = picDescr;
            Description = picDescr.Desc;
            PIC18Registers.Create(picDescr).LoadRegisters(); 
            StackRegister = PIC18Registers.STKPTR;
        }

        #endregion

        #region Public Methods/Properties

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor
        {
            get
            {
                if (_picDescriptor is null)
                {
                    try
                    {
                        PICCrownking db = PICCrownking.GetDB();
                        if (db is null)
                            throw new InvalidOperationException("Cannot get access to PIC database");
                        PIC pic = db.GetPIC(Name);
                        if (pic is null)
                            throw new InvalidOperationException("No such PIC");
                        PICDescriptor = pic;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Unable to retrieve PIC definition for PIC name '{Name}'", ex);
                    }
                }
                return _picDescriptor;
            }
            private set
            {
                if (_picDescriptor != value)
                {
                    _picDescriptor = value;
                    if (!(_picDescriptor is null))
                        LoadConfiguration(_picDescriptor);
                }
            }
        }
        private PIC _picDescriptor;

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
                if (_memmapper is null)
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
            if (!Enum.TryParse(name, true, out Opcode result))
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
            => PIC18Registers.GetCoreRegisterByIdx(i);

        public override RegisterStorage GetRegister(string name)
        {
            var r = PIC18Registers.GetRegisterByName(name);
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

        /// <summary>
        /// Find the widest sub-register that covers the register reg. Not implemented.
        /// </summary>
        /// <param name="reg">.</param>
        /// <param name="bits">.</param>
        /// <returns>
        /// Not implemented.
        /// </returns>
        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
        {
            throw new NotImplementedException("There is no wider register for PIC18");
        }

        /// <summary>
        /// Get the improper sub-register of <paramref name="reg" /> that starts at offset
        /// <paramref name="offset" /> and is of size <paramref name="width"/>.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="offset">The bit offset.</param>
        /// <param name="width">The bit width.</param>
        /// <returns>
        /// The sub-register or null.
        /// </returns>
        /// <remarks>
        /// Most architectures do not have sub-registers, and will use a default implementation. This
        /// method is overridden for architectures like x86 and Z80, where sub-registers (<code>"ah", "al"</code>, etc)
        /// do exist.
        /// </remarks>
        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0 && reg.BitSize == (ulong)width)
                return reg;
            if (reg is PICRegisterStorage preg)
            {
                foreach (var r in preg.SubRegs)
                {
                    if ((r.BitAddress == (ulong)offset) && (r.BitSize == (ulong)width))
                        return r;
                }
            }
            return null;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => new PIC18State(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => new PIC18Rewriter(this, rdr, (PIC18State)state, frame, host);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new PIC18PointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li)); ;
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("PIC18 has no explicit argument stack");
        }

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
            => PICProgAddress.Ptr(rdr.ReadLeUInt32());

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = PIC18Registers.GetRegisterByName(name);
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
                    var f = PIC18Registers.GetBitFieldByReg(PIC18Registers.STATUS, bitPos, 1);
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

        #endregion

    }

}
