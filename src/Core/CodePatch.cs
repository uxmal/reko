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

using Reko.Core.Rtl;

namespace Reko.Core
{
    /// <summary>
    /// This class represents a region of memory which, when encountered
    /// by the Scanner, should be replaced by the statements in the 
    /// specified RTL cluster.
    /// </summary>
    public class CodePatch
    {
        /// <summary>
        /// Constructs a new <see cref="CodePatch"/> instance.
        /// </summary>
        /// <param name="cluster">Cluster of RTL instructions used as replacement.
        /// </param>
        public CodePatch(RtlInstructionCluster cluster)
        {
            this.Code = cluster;
        }

        /// <summary>
        /// The address at which to perform the patch.
        /// </summary>
        public Address Address => Code.Address;

        /// <summary>
        /// RTL instructions to patch, at the address specified 
        /// by the Code.Address property.
        /// </summary>
        public RtlInstructionCluster Code { get; }

        /// <summary>
        /// Number of bytes to patch.
        /// </summary>
        public int Length => Code.Length;
    }
}
