/* 
 * Copyright (C) 1999-2009 John Källén.
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

        public abstract void Load(Address userSpecifiedAddress);

        public abstract DecompilerProject Project { get; }

        public abstract Program Program { get; }

        protected void SetDefaultFilenames(string inputFilename, DecompilerProject project)
        {
            project.Input.Filename = inputFilename;

            project.Output.DisassemblyFilename = Path.ChangeExtension(inputFilename, ".asm");
            project.Output.IntermediateFilename = Path.ChangeExtension(inputFilename, ".dis");
            project.Output.OutputFilename = Path.ChangeExtension(inputFilename, ".c");
            project.Output.TypesFilename = Path.ChangeExtension(inputFilename, ".h");
        }

    }
}
