#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// A PIC registers symbol table builder. Must be inherited.
    /// </summary>
    public abstract class PICRegistersBuilder
    {
        private ulong bitRegAddr = 0;
        private static int regNumber = 0;
        private IPICRegisterSymTable? symTable;

        /// <summary>
        /// Loads the PIC registers, as found in the PIC definition, into the registers symbol table.
        /// </summary>
        /// <param name="registersSymTable">The registers symbol table interface.</param>
        /// <param name="pic">the PIC definition.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registersSymTable"/> is
        ///                                         null.</exception>
        protected void LoadRegistersInTable(IPICRegisterSymTable registersSymTable, IPICDescriptor pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            symTable = registersSymTable ?? throw new ArgumentNullException(nameof(registersSymTable));
            regNumber = 0;

            foreach (var sfr in pic.SFRs)
            {
                var reg = AddSFRRegister(sfr, regNumber);
                regNumber++;
                symTable.AddRegister(reg);
            }
            foreach (var jsfr in pic.JoinedRegisters)
            {
                var subregs = new List<PICRegisterStorage>();
                bitRegAddr = 0;
                foreach (var sfr in jsfr.ChildSFRs)
                {
                    if (!PICRegisters.TryGetRegister(sfr.Name, out PICRegisterStorage? creg))
                    {
                        creg = AddSFRRegister(sfr, regNumber++);
                    }
                    creg.BitAddress = bitRegAddr;
                    subregs.Add(creg);
                    bitRegAddr += 8;
                }
                if (subregs.Count > 0)
                {
                    var sfr = new PICRegisterStorage(jsfr, regNumber, subregs);
                    if (symTable.AddRegister(sfr))
                        regNumber++;
                }
            }
        }


        private PICRegisterStorage AddSFRRegister(ISFRRegister sfr, int regnum)
        {
            var reg = new PICRegisterStorage(sfr, regNumber) { BitAddress = 0UL };
            foreach (var sfld in sfr.BitFields)
            {
                if ((sfld.Name != sfr.Name) && (sfld.BitWidth != sfr.BitWidth))
                {
                    var fld = new PICRegisterBitFieldStorage(reg, sfld);
                    if (!reg.BitFields!.ContainsKey(fld.BitFieldSortKey))
                    {
                        if (symTable!.AddRegisterBitField(fld))
                        {
                            reg.BitFields.Add(fld.BitFieldSortKey, fld);
                            continue;
                        }
                    }
                }
            }
            return reg;
        }

    }

}
