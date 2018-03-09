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
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public class PIC16Architecture : PICArchitecture
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC16Architecture() : this("pic16") { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        public PIC16Architecture(string archID) : base(archID)
        {
        }

        /// <summary>
        /// Constructor. Used for tests purpose.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        public PIC16Architecture(PIC picDescr) : base("pic16", picDescr)
        {
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads the PIC configuration. Creates memory mapper and registers.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        protected override void LoadConfiguration()
        {
            Description = picDescriptor.Desc;
            PIC16Registers.Create(picDescriptor).LoadRegisters();
            StackRegister = PIC16Registers.STKPTR;
        }

        #endregion

        #region Public Methods/Properties

        /// <summary>
        /// Gets the PIC memory mapper.
        /// </summary>
        public override IPICMemoryDescriptor MemoryDescriptor
        {
            get
            {
                if (memDescr is null)
                    memDescr = PIC16MemoryDescriptor.Create(PICDescriptor);
                return memDescr;
            }
        }
        private PIC16MemoryDescriptor memDescr;

        public PIC16Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
            => new PIC16Disassembler(this, imageReader);

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            => new PIC16InstructionComparer(norm);

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
            var fl = new FlagGroupStorage(PIC16Registers.STATUS, grpFlags, GrfToString(grpFlags), dt);
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
                    default:
                        return null;
                }
            }
            return GetFlagGroup((uint)grf);
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield break;
        }

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
            throw new NotImplementedException("There is no wider register for PIC16");
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
                return preg.SubRegs.FirstOrDefault(r => ((r.BitAddress == (ulong)offset) && (r.BitSize == (ulong)width)));
            return null;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => new PIC16State(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => new PIC16Rewriter(this, rdr, (PIC16State)state, frame, host);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new PIC16PointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("PIC16 has no explicit argument stack");
        }

        public override string GrfToString(uint grpFlags)
        {
            var sb = new StringBuilder();
            uint bitPos = 0;
            while (grpFlags != 0)
            {
                if ((grpFlags & 1) != 0)
                {
                    var f = PICRegisters.GetBitFieldByReg(PIC16Registers.STATUS, bitPos, 1);
                    if (f != null)
                        sb.Append(f.Name);
                }
                grpFlags >>= 1;
                bitPos++;
            }
            return sb.ToString();
        }

        #endregion

    }

}
