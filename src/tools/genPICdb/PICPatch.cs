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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Libraries.Microchip;

namespace Reko.Tools.genPICdb
{
    public class PICPatch : IMemDataRegionVisitor, IMemDataSymbolVisitor
    {
        private PIC pic;
        private int byteAddr = 0;
        private uint bitFieldAddr = 0;

        private PICPatch(PIC pic)
        {
            this.pic = pic;
        }

        public static void Patch(PIC pic)
        {
            new PICPatch(pic ?? throw new ArgumentNullException(nameof(pic))).PerformPatch();
        }

        private void PerformPatch()
        {
            pic.DataSpace.RegardlessOfMode.Regions.
                OfType<IMemDataRegionAcceptor>().
                ToList().
                ForEach(r => r.Accept(this));
        }

        void IMemDataRegionVisitor.Visit(SFRDataSector xmlRegion)
        {
            xmlRegion.SFRs.
                OfType<IMemDataSymbolAcceptor>().
                ToList().
                ForEach(r => r.Accept(this));
        }

        void IMemDataRegionVisitor.Visit(DPRDataSector xmlRegion)
        {
            xmlRegion.SFRs.
                OfType<IMemDataSymbolAcceptor>().
                ToList().
                ForEach(r => r.Accept(this));
        }

        void IMemDataRegionVisitor.Visit(GPRDataSector xmlRegion) { }
        void IMemDataRegionVisitor.Visit(EmulatorZone xmlRegion) { }
        void IMemDataRegionVisitor.Visit(NMMRPlace xmlRegion) { }
        void IMemDataRegionVisitor.Visit(LinearDataSector xmlRegion) { }

        void IMemDataSymbolVisitor.Visit(DataBitAdjustPoint xmlSymb)
        {
            bitFieldAddr = (uint)(bitFieldAddr + xmlSymb.Offset);
        }

        void IMemDataSymbolVisitor.Visit(DataByteAdjustPoint xmlSymb)
        {
            byteAddr += xmlSymb.Offset;
        }

        void IMemDataSymbolVisitor.Visit(SFRDef xmlSymb)
        {
            xmlSymb.SFRModes.ForEach(sfr => {
                sfr.Accept(this);
            });
        }

        void IMemDataSymbolVisitor.Visit(SFRFieldDef xmlSymb)
        {
            xmlSymb.SFRFieldSemantics.ForEach(s => s.Accept(this));
            xmlSymb.BitPos = (byte)bitFieldAddr;
            bitFieldAddr += xmlSymb.NzWidth;
        }

        void IMemDataSymbolVisitor.Visit(SFRFieldSemantic xmlSymb) { }

        void IMemDataSymbolVisitor.Visit(SFRModeList xmlSymb)
        {
            xmlSymb.SFRModes.ForEach(mode => mode.Accept(this));
        }

        void IMemDataSymbolVisitor.Visit(SFRMode xmlSymb)
        {
            bitFieldAddr = 0;
            xmlSymb.Fields.OfType<IMemDataSymbolAcceptor>().ToList().ForEach(fld => fld.Accept(this));
        }

        void IMemDataSymbolVisitor.Visit(Mirror xmlSymb) { }

        void IMemDataSymbolVisitor.Visit(JoinedSFRDef xmlSymb)
        {
            xmlSymb.SFRs.ForEach(sfr => sfr.Accept(this));
            if (xmlSymb.NzWidth == 0)
            {
                uint jsize = 0;
                xmlSymb.SFRs.ForEach(s => jsize += s.NzWidth );
                xmlSymb.NzWidth = jsize;
            }
        }

        void IMemDataSymbolVisitor.Visit(MuxedSFRDef xmlSymb) { }

        void IMemDataSymbolVisitor.Visit(SelectSFR xmlSymb) { }

        void IMemDataSymbolVisitor.Visit(DMARegisterMirror xmlSymb) { }
    }

}
