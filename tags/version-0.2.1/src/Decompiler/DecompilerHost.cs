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

using Decompiler.Configuration;
using Decompiler.Core;
using System;
using System.IO;

namespace Decompiler
{
	/// <summary>
	/// Interface used by the decompiler's components to talk to the outside world.
	/// </summary>
	public interface DecompilerHost
	{
        void WriteDisassembly(Action<TextWriter> writer);
        void WriteIntermediateCode(Action<TextWriter> writer);
        void WriteTypes(Action<TextWriter> writer);
        void WriteDecompiledCode(Action<TextWriter> writer);

        IDecompilerConfigurationService Configuration { get; }
	}

	/// <summary>
	/// Used when no host is required.
	/// </summary>
	public class NullDecompilerHost : DecompilerHost
	{
        public static readonly DecompilerHost Instance = new NullDecompilerHost();

		#region DecompilerHost Members

        public IDecompilerConfigurationService Configuration
        {
            get { throw new NotImplementedException(); }
        }

		#endregion

        #region DecompilerHost Members

        public void WriteDisassembly(Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteIntermediateCode(Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteTypes(Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        public void WriteDecompiledCode(Action<TextWriter> writer)
        {
            writer(TextWriter.Null);
        }

        #endregion
    }
}
