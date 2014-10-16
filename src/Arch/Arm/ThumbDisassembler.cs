#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ThumbDisassembler : DisassemblerBase<MachineInstruction>
    {
        private ThumbInstruction thumb;
        private IProcessorArchitecture arch;
        private ImageReader rdr;
        private ushort lsw;
        private ushort msw;

        public ThumbDisassembler(IProcessorArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override MachineInstruction Current { get { return thumb; } }

        public override bool MoveNext()
        {
            if (!rdr.IsValid)
                return false;

            thumb = new ThumbInstruction
            {
                Address = rdr.Address,
            };
            Disassemble();
            thumb.Length = rdr.Address - thumb.Address;
            return true;
        }

        public ThumbInstruction Disassemble()
        {
            this.lsw = rdr.ReadLeUInt16();
            switch (this.lsw>> 11)
            {
            case 0x1D:
            case 0x1E:
            case 0x1F:
                this.msw = rdr.ReadLeUInt16();
                Disassemble32bitInstruction();
                break;
            default:
                Disassemble16bitInstruction();
                break;
            }
            return thumb;
        }

        private void Disassemble16bitInstruction()
        {

        }

        private bool IsBitSet(uint w, int bit) { return ((w >> bit) & 1) != 0; }

        private void Disassemble32bitInstruction() 
        {
            switch ((lsw >> 11) & 3)
            {
            case 1:
                switch ((lsw >> 9) & 3)
                {
                case 0: LdmStmMultiDouble(); break;
                case 1: DataProcessing(); break;
                case 2:
                case 3: Coprocessor(); break;
                }
                break;
            case 2:
                if (IsBitSet(msw, 15))
                {
                    Control();
                }
                else
                {
                    DataProcessing();
                }
                break;
            case 3:
                switch ((lsw >> 8) & 7)
                {
                case 0: StoreSingleDataItem(); break;
                case 1: AdvancedSimdStructLdSt(); break;
                case 2: LdbMemHints(); break;
                }
                break;
            }
        }

        private void LdbMemHints()
        {
            throw new NotImplementedException();
        }

        private void AdvancedSimdStructLdSt()
        {
            throw new NotImplementedException();
        }

        private void StoreSingleDataItem()
        {
            throw new NotImplementedException();
        }

        private void Control()
        {
            throw new NotImplementedException();
        }

        private void Coprocessor()
        {
            throw new NotImplementedException();
        }

        private void DataProcessing()
        {
            throw new NotImplementedException();
        }

        private void LdmStmMultiDouble()
        {
            throw new NotImplementedException();
        }
    }
}
