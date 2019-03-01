#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Configuration;
using Reko.Core;
using System;
using System.IO;
using System.Threading;
using Reko.Core.Output;

namespace Reko
{
	/// <summary>
	/// Interface used by the decompiler's components to talk to the outside world.
	/// </summary>
	public interface DecompilerHost
	{
        void WriteDisassembly(Program program, Action<Formatter> writer);
        void WriteIntermediateCode(Program program, Action<TextWriter> writer);
        void WriteTypes(Program program, Action<TextWriter> writer);
        void WriteDecompiledCode(Program program, Action<TextWriter> writer);
        void WriteGlobals(Program program, Action<TextWriter> writer);

        IConfigurationService Configuration { get; }
	}

	/// <summary>
	/// Used when no host is required.
	/// </summary>
	public class NullDecompilerHost : DecompilerHost
	{
        public static readonly DecompilerHost Instance = new NullDecompilerHost();

        public NullDecompilerHost()
        {
        }

		#region DecompilerHost Members

        public IConfigurationService Configuration
        {
            get { throw new NotImplementedException(); }
        }

        public void WriteDisassembly(Program program, Action<Formatter> writer)
        {
            writer(new NullFormatter());
        }

        public void WriteIntermediateCode(Program program, Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteTypes(Program program, Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteDecompiledCode(Program program, Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteGlobals(Program program, Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        #endregion
    }
}
