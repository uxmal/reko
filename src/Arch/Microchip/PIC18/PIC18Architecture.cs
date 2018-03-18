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

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    /// <summary>
    /// PIC18 processor architecture.
    /// </summary>
    public class PIC18Architecture : PICArchitecture
    {

        /// <summary>
        /// Instantiates a new PIC18 architecture.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        /// <param name="mode">The PIC mode. Contains details and creators for target PIC18.</param>
        public PIC18Architecture(string archID, PICProcessorMode mode)
            : base(archID, mode)
        {
        }


        /// <summary>
        /// Loads the PIC configuration. Creates memory mapper and registers.
        /// </summary>
        protected override void LoadConfiguration()
        {
            Description = PICDescriptor.Desc;
            ProcessorMode.CreateRegisters();
            StackRegister = PIC18Registers.STKPTR;
        }


        #region Public Methods/Properties

        /// <summary>
        /// Gets the PIC18 memory mapper.
        /// </summary>
        public override IPICMemoryDescriptor MemoryDescriptor
        {
            get
            {
                if (memDescr is null)
                    memDescr = PIC18MemoryDescriptor.Create(PICDescriptor);
                return memDescr;
            }
        }
        private PIC18MemoryDescriptor memDescr;

        public PICDisassemblerBase CreateDisassemblerImpl(EndianImageReader imageReader)
            => ProcessorMode.CreateDisassembler(this, imageReader);

        public override FlagGroupStorage GetFlagGroup(uint grpFlags)
        {
            foreach (FlagGroupStorage f in flagGroups)
            {
                if (f.FlagGroupBits == grpFlags)
                    return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grpFlags) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(PIC18Registers.STATUS, grpFlags, GrfToString(grpFlags), dt);
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
                return preg.SubRegs.FirstOrDefault(r => ((r.BitAddress == (ulong)offset) && (r.BitSize == (ulong)width)));
            return null;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => ProcessorMode.CreateProcessorState(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => ProcessorMode.CreateRewriter(this, ProcessorMode.CreateDisassembler(this, rdr), (PIC18State)state, frame, host);

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new PIC18PointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("PIC18 has no explicit argument stack");
        }

        public override string GrfToString(uint grpFlags)
        {
            var sb = new StringBuilder();
            uint bitPos = 0;
            while (grpFlags != 0)
            {
                if ((grpFlags & 1) != 0)
                {
                    var f = PICRegisters.PeekBitFieldFromRegister(PIC18Registers.STATUS, bitPos, 1);
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
