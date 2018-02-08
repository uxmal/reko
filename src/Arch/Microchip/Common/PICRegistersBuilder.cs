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

using Microchip.Crownking;
using Reko.Core;
using Reko.Core.Types;
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
        #region Locals

        private int _byteAddr = 0;
        private int _bitAddr = 0;
        private int _regIndex = 0;
        private SFRDef _currentSFRDef = null;
        private PICRegisterStorage _currentSFRRegister = null;
        private IPICRegisterSymTable _symTable;

        #endregion

        #region Helpers

        private static PrimitiveType _size2type(uint nzsize)
        {
            if (nzsize == 0)
                throw new ArgumentOutOfRangeException(nameof(nzsize));
            if (nzsize == 1)
                return PrimitiveType.Bool;
            if (nzsize <= 8)
                return PrimitiveType.Byte;
            if (nzsize <= 16)
                return PrimitiveType.Word16;
            if (nzsize <= 32)
                return PrimitiveType.Word32;
            if (nzsize <= 64)
                return PrimitiveType.Word64;
            if (nzsize <= 128)
                return PrimitiveType.Word128;
            if (nzsize <= 256)
                return PrimitiveType.Word256;
            throw new ArgumentOutOfRangeException(nameof(nzsize));
        }

        #endregion

        #region Properties/Fields

        public readonly PIC PIC;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        protected PICRegistersBuilder(PIC pic)
        {
            PIC = pic;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the PIC registers, as found in the PIC definition, into the registers symbol table.
        /// </summary>
        /// <param name="registersSymTable">The registers symbol table interface.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registersSymTable"/> is null.</exception>
        public virtual void LoadRegisters(IPICRegisterSymTable registersSymTable)
        {
            _symTable = registersSymTable ?? throw new ArgumentNullException(nameof(registersSymTable));

            foreach (var e in PIC.DataSpace.RegardlessOfMode.Regions)
            {
                if (e is SFRDataSector sfrsect)
                {
                    sfrsect.Accept(this);
                    continue;
                }
                if (e is NMMRPlace nmmrs)
                {
                    nmmrs.Accept(this);
                    continue;
                }
            }
        }

        #endregion

        #region Visit the PIC definition and create registers

        #region IMemDataRegionVisitor

        public void Visit(SFRDataSector xmlRegion)
        {
            foreach (var e in xmlRegion.SFRs)
            {
                switch (e)
                {
                    case SFRDef sfr:
                        sfr.Accept(this);
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
            if (xmlRegion.RegionID != "peripheralnmmrs")  // Do not consider internal peripherals (for debuggers only).
                xmlRegion.SFRDefs.ForEach(e => e.Accept(this));
        }

        public void Visit(LinearDataSector xmlRegion)
        {
            // Do nothing        
        }

        #endregion

        #region IMemDataSymbolVisitor

        public void Visit(DataBitAdjustPoint xmlSymb)
        {
            _bitAddr += xmlSymb.Offset;
        }

        public void Visit(DataByteAdjustPoint xmlSymb)
        {
            _byteAddr += xmlSymb.Offset;
        }

        public void Visit(JoinedSFRDef xmlSymb)
        {
            SortedList<PICDataAddress, PICRegisterStorage> subregs = new SortedList<PICDataAddress, PICRegisterStorage>();
            xmlSymb.SFRs.ForEach(e =>
            {
                e.Accept(this);
                subregs.Add(_currentSFRRegister.Address, _currentSFRRegister);
            });
            if (subregs.Count > 0)
            {
                PICRegisterStorage joined = new PICRegisterStorage(xmlSymb, subregs.Values);
                _symTable.AddRegister(joined);
            }
        }

        public void Visit(MuxedSFRDef xmlSymb)
        {
            foreach (var selsfr in xmlSymb.SelectSFRs)
            {
                if (String.IsNullOrEmpty(selsfr.When))
                {
                    var sfr = selsfr.SFR;
                    sfr.Accept(this);
                    return;
                }
                selsfr.Accept(this);
            }
        }

        public void Visit(SelectSFR xmlSymb)
        {
            xmlSymb.SFR.Accept(this);
        }

        public void Visit(SFRDef xmlSymb)
        {
            _currentSFRRegister = new PICRegisterStorage(xmlSymb, _regIndex);
            if (_symTable.AddRegister(_currentSFRRegister) != null)
                _regIndex++;
            _currentSFRDef = xmlSymb;

            xmlSymb.SFRModes.ForEach(e => e.Accept(this));
        }

        public void Visit(SFRModeList xmlSymb)
        {
            xmlSymb.SFRModes.ForEach(e => e.Accept(this));
        }

        public void Visit(SFRMode xmlSymb)
        {
            _bitAddr = 0;
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
            if ((xmlSymb.Name != _currentSFRDef.Name) && (xmlSymb.NzWidth != _currentSFRDef.NzWidth))
            {
                var fld = new PICBitFieldStorage(_currentSFRRegister, xmlSymb, (byte)_bitAddr, xmlSymb.Mask);
                _symTable.AddRegisterField(_currentSFRRegister, fld);
            }
            _bitAddr += (int)xmlSymb.NzWidth;
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

        #endregion

    }

}
