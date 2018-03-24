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
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// A PIC registers symbol table builder. Must be inherited.
    /// </summary>
    public abstract class PICRegistersBuilder : IMemDataRegionVisitor, IMemDataSymbolVisitor
    {
        private int byteAddr = 0;
        private int bitFieldAddr = 0;
        private ulong bitRegAddr = 0;
        private int regNumber = 0;
        private SFRDef currSFRDef = null;
        private PICRegisterStorage currSFRReg = null;
        private IPICRegisterSymTable symTable;
        private PIC pic;

        private static PrimitiveType Size2Type(uint nzsize)
        {
            if (nzsize == 0)
                throw new ArgumentOutOfRangeException(nameof(nzsize));
            if (nzsize == 1)
                return PrimitiveType.Bool;
            if (nzsize <= 8)
                return PrimitiveType.Byte;
            if (nzsize <= 16)
                return PrimitiveType.UInt16;
            if (nzsize <= 32)
                return PrimitiveType.UInt32;
            if (nzsize <= 64)
                return PrimitiveType.UInt64;
            if (nzsize <= 128)
                return PrimitiveType.UInt128;
            if (nzsize <= 256)
                return PrimitiveType.Word256;
            throw new ArgumentOutOfRangeException(nameof(nzsize));
        }


        /// <summary>
        /// Loads the PIC registers, as found in the PIC definition, into the registers symbol table.
        /// </summary>
        /// <param name="registersSymTable">The registers symbol table interface.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registersSymTable"/> is null.</exception>
        protected void LoadRegistersInTable(IPICRegisterSymTable registersSymTable, PIC thePIC)
        {
            symTable = registersSymTable ?? throw new ArgumentNullException(nameof(registersSymTable));
            pic = thePIC ?? throw new ArgumentNullException(nameof(thePIC));

            pic.DataSpace.RegardlessOfMode.Regions.
                OfType<IMemDataRegionAcceptor>().
                ToList().
                ForEach(r => r.Accept(this));
        }


        #region Visit the PIC definition and create registers

        public void Visit(SFRDataSector xmlRegion)
        {
            foreach (var e in xmlRegion.SFRs)
            {
                bitRegAddr = 0;
                switch (e)
                {
                    case SFRDef sfr:
                        sfr.Accept(this);
                        regNumber++;
                        continue;

                    case JoinedSFRDef join:
                        join.Accept(this);
                        continue;

                    case MuxedSFRDef mux:
                        mux.Accept(this);
                        continue;
                }
            }
        }

        public void Visit(GPRDataSector xmlRegion)
        {
            // Do nothing        
        }

        public void Visit(DPRDataSector xmlRegion)
        {
            // Do nothing        
        }

        public void Visit(EmulatorZone xmlRegion)
        {
            // Do nothing        
        }

        public void Visit(NMMRPlace xmlRegion)
        {
            if (xmlRegion.RegionID != "peripheralnmmrs")  // Do not consider internal peripherals (for hardware debuggers only).
                xmlRegion.SFRDefs.ForEach(e => e.Accept(this));
        }

        public void Visit(LinearDataSector xmlRegion)
        {
            // Do nothing        
        }


        public void Visit(DataBitAdjustPoint xmlSymb)
        {
        }

        public void Visit(DataByteAdjustPoint xmlSymb)
        {
            byteAddr += xmlSymb.Offset;
        }

        public void Visit(JoinedSFRDef xmlSymb)
        {
            var subregs = new SortedList<PICDataAddress, PICRegisterStorage>();
            bitRegAddr = 0;
            xmlSymb.SFRs.ForEach(e =>
            {
                e.Accept(this);
                currSFRReg.BitAddress = bitRegAddr;
                bitRegAddr += currSFRDef.NzWidth;
                subregs.Add(currSFRReg.Address, currSFRReg);
            });
            if (subregs.Count > 0)
            {
                var sfr = new PICRegisterStorage(xmlSymb, regNumber, subregs.Values);
                symTable.AddRegister(sfr);
            }
            regNumber++;
        }

        public void Visit(MuxedSFRDef xmlSymb)
        {
            foreach (var selsfr in xmlSymb.SelectSFRs)
            {
                if (String.IsNullOrEmpty(selsfr.When))
                {
                    var sfr = selsfr.SFR;
                    sfr.Accept(this);
                    regNumber++;
                    return;
                }
                selsfr.Accept(this);
                regNumber++;
            }
        }

        public void Visit(SelectSFR xmlSymb)
        {
            xmlSymb.SFR.Accept(this);
        }

        public void Visit(SFRDef xmlSymb)
        {
            currSFRReg = new PICRegisterStorage(xmlSymb, regNumber) { BitAddress = 0UL };
            symTable.AddRegister(currSFRReg);
            currSFRDef = xmlSymb;

            xmlSymb.SFRModes.ForEach(e => e.Accept(this));
        }

        public void Visit(SFRModeList xmlSymb)
        {
            xmlSymb.SFRModes.ForEach(e => e.Accept(this));
        }

        public void Visit(SFRMode xmlSymb)
        {
            foreach (var e in xmlSymb.Fields)
            {
                switch (e)
                {
                    case DataBitAdjustPoint adj:
                        adj.Accept(this);
                        continue;

                    case SFRFieldDef sfrfield:
                        sfrfield.Accept(this);
                        continue;
                }

            }
        }

        public void Visit(SFRFieldDef xmlSymb)
        {
            // We do not add SFR Fields which are duplicating the parent SFR register definition (same name or same bit width)
            if ((xmlSymb.CName != currSFRDef.CName) && (xmlSymb.NzWidth != currSFRDef.NzWidth))
            {
                var fld = new PICBitFieldStorage(currSFRReg, xmlSymb, xmlSymb.BitPos, xmlSymb.Mask);
                symTable.AddRegisterField(currSFRReg, fld);
            }
        }

        public void Visit(SFRFieldSemantic xmlSymb)
        {
            //TODO? SFR Field semantics
        }

        public void Visit(Mirror xmlSymb)
        {
            // Do nothing
        }

        public void Visit(DMARegisterMirror xmlSymb)
        {
            // Do nothing
        }

        #endregion

    }

}
