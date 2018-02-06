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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// PIC register traits.
    /// </summary>
    public class PICRegisterTraits
    {

        #region Properties

        /// <summary>
        /// Gets the PIC register name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the PIC register description.
        /// </summary>
        public string Desc {get;}

        /// <summary>
        /// Gets the PIC register bit width.
        /// </summary>
        public int BitWidth { get; }

        /// <summary>
        /// Gets the register memory address or null if not memory-mapped.
        /// </summary>
        public PICDataAddress Address { get; }

        /// <summary>
        /// Gets the Non-Memory-Mapped ID of the register or null if memory mapped.
        /// </summary>
        public string NMMRID { get; }

        /// <summary>
        /// Gets the individual bits access modes.
        /// </summary>
        public string Access { get; }

        /// <summary>
        /// Gets the individual bits state after a Master Clear.
        /// </summary>
        public string MCLR { get; }

        /// <summary>
        /// Gets the individual bits state after a Power-On reset.
        /// </summary>
        public string POR { get; }

        /// <summary>
        /// Gets the PIC register implementation mask.
        /// </summary>
        public ulong Impl { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is volatile.
        /// </summary>
        public bool IsVolatile { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is indirect.
        /// </summary>
        public bool IsIndirect { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC register is memory mapped.
        /// </summary>
        public bool IsMemoryMapped => !(Address is null);

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PICRegisterTraits()
        {
            Name = "None";
            Desc = "";
            BitWidth = 8;
            Impl = 0xFF;
            Access = "nnnnnnnn";
            MCLR = "uuuuuuuu";
            POR = "uuuuuuuu";
            IsVolatile = false;
            IsIndirect = false;
            NMMRID = String.Empty;
            Address = null;

        }

        public PICRegisterTraits(SFRDef sfr)
        {
            if (sfr == null) throw new ArgumentNullException(nameof(sfr));
            Name = sfr.Name;
            Desc = sfr.Desc;
            BitWidth = (int)sfr.NzWidth;
            Impl = sfr.Impl;
            Access = sfr.Access;
            MCLR = sfr.MCLR;
            POR = sfr.POR;
            IsVolatile = sfr.IsVolatile;
            IsIndirect = sfr.IsIndirect;
            NMMRID = sfr.NMMRID;
            Address = null;
            if (string.IsNullOrEmpty(NMMRID) || sfr.Addr != 0)
            {
                Address = PICDataAddress.Ptr(sfr.Addr);
                NMMRID = String.Empty;
            }

        }

        public PICRegisterTraits(JoinedSFRDef joinedsfr, ICollection<PICRegisterStorage> subregs)
        {
            if (joinedsfr == null) throw new ArgumentNullException(nameof(joinedsfr));
            if (subregs == null) throw new ArgumentNullException(nameof(subregs));
            Name = joinedsfr.Name;
            Desc = joinedsfr.Desc;
            BitWidth = (int)joinedsfr.NzWidth;
            Address = PICDataAddress.Ptr(joinedsfr.Addr);
            NMMRID = String.Empty;
            Access = String.Join("", subregs.Reverse().Select(e => e.Access));
            MCLR = String.Join("", subregs.Reverse().Select(e => e.MCLR));
            POR = String.Join("", subregs.Reverse().Select(e => e.POR));
            Impl = subregs.Reverse().Aggregate(0UL, (total, reg) => total = total * 256 + reg.Impl);
            IsVolatile = subregs.Any(e => e.IsVolatile == true);
            IsIndirect = subregs.Any(e => e.IsIndirect == true);
        }

        #endregion

    }

}
