#region License
/* 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    /// <summary>
    /// Scanner heuristics are used to guide the scanning process.
    /// </summary>
    public static class ScannerHeuristics
    {
        /// <summary>
        /// Use the shingle scanner to heuristically identify procedures 
        /// that couldn't be reached using the recursive scanner.
        /// </summary>
        public const string Shingle = "shingle";

        /// <summary>
        /// Accept instructions that the processor architecture considers
        /// "unlinkely" as valid.
        /// </summary>
        public const string Unlikely = "unlikely";

        /// <summary>
        /// Accept only unpriviliged instructions.
        /// </summary>
        public const string UserMode = "userMode";
    }
}
