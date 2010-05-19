/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Base class that abstracts the process of loading the "code", in whatever format it is in,
    /// into a program.
    /// </summary>
    public abstract class LoaderBase
    {
        private List<EntryPoint> entryPoints;

		public LoaderBase()
		{
			this.entryPoints = new List<EntryPoint>();
		}

        public List<EntryPoint> EntryPoints
        {
            get { return entryPoints; }
        }


        public abstract Program Load(byte[] imageFile, Address userSpecifiedAddress);

        /// <summary>
        /// Loads the contents of a file with the specified filename into an array 
        /// of bytes, optionally at the offset <paramref>offset</paramref>.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <param name="offset">The offset into the array into which the file will be loaded.</param>
        /// <returns>An array of bytes with the file contents at the specified offset.</returns>
        public virtual byte[] LoadImageBytes(string fileName, int offset)
        {
            using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new Byte[stm.Length + offset];
                stm.Read(bytes, offset, (int)stm.Length);
                return bytes;
            }
        }

        protected void CopyImportThunks(Dictionary<uint, PseudoProcedure> importThunks, Program prog)
        {
            if (importThunks == null)
                return;

            foreach (KeyValuePair<uint, PseudoProcedure> item in importThunks)
            {
                prog.ImportThunks.Add(item.Key, item.Value);
            }
        }
    }
}
