#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class implements a policy that partitions <see cref="Procedure"/>s and 
    /// data objects into separate output files.
    /// </summary>
    public abstract class OutputFilePolicy
    {
        protected readonly Program program;

        public OutputFilePolicy(Program program)
        {
            this.program = program;
        }

        /// <summary>
        /// Returns a placement mapping for rendering high-level items.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public abstract Dictionary<string, IDictionary<Address, object>> GetObjectPlacements(
            string fileExtension,
            DecompilerEventListener listener);

        /// <summary>
        /// Returns a placement mapping for rendering low-level items.
        /// </summary>
        /// <param name="fileExtension">File extension to use on the files in the mapping.</param>
        /// <returns></returns>
        public abstract Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>> GetItemPlacements(
            string fileExtension);
    }
}
