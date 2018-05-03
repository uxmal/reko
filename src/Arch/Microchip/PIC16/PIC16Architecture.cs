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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    public class PIC16Architecture : PICArchitecture
    {

        /// <summary>
        /// Instantiates a new PIC16 architecture.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        /// <param name="mode">The PIC mode. Contains details and creators for target PIC16.</param>
        public PIC16Architecture(string archID)
            : base(archID)
        {
        }

        /// <summary>
        /// Loads the PIC16 configuration. Creates memory mapper and registers.
        /// </summary>
        protected override void LoadConfiguration()
        {
            Description = PICDescriptor.Desc;
            ProcessorMode.CreateMemoryDescriptor();
            ProcessorMode.CreateRegisters();
            StackRegister = PICRegisters.STKPTR;
        }

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
                return preg.AttachedRegs.FirstOrDefault(r => ((r.BitAddress == (ulong)offset) && (r.BitSize == (ulong)width)));
            return null;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
            => CreateDisassemblerImpl(imageReader);

        public override ProcessorState CreateProcessorState()
            => ProcessorMode.CreateProcessorState(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
            => ProcessorMode.CreateRewriter(this, ProcessorMode.CreateDisassembler(this, rdr), (PICProcessorState)state, frame, host);

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

    }

}
