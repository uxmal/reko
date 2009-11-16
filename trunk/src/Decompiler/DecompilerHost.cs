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
		TextWriter CreateDisassemblyWriter();
		TextWriter GetIntermediateCodeWriter();
		TextWriter CreateTypesWriter(string filename);				// TextWriter into which the reconstructed types are written.
		TextWriter CreateDecompiledCodeWriter(string filename);	// Textwriter into which the Data is written

        DecompilerConfiguration Configuration { get; }
	}

	/// <summary>
	/// Used when no host is required.
	/// </summary>
	public class NullDecompilerHost : DecompilerHost
	{
        public static readonly DecompilerHost Instance = new NullDecompilerHost();

		#region DecompilerHost Members


		public TextWriter CreateDisassemblyWriter()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public TextWriter GetIntermediateCodeWriter()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public TextWriter CreateTypesWriter(string filename)
		{
			throw new Exception("The method or operation is not implemented."); 
		}

		public TextWriter CreateDecompiledCodeWriter(string filename)
		{
			throw new Exception("The method or operation is not implemented."); 
		}

        public DecompilerConfiguration Configuration
        {
            get { throw new NotImplementedException(); }
        }

		public void ProgramLoaded()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ProgramScanned()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MachineCodeRewritten()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void InterproceduralAnalysisComplete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ProceduresTransformed()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void TypeReconstructionComplete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CodeStructuringComplete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DecompilationFinished()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void WriteDiagnostic(DiagnosticOld d, Address address, string format, params object[] args)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ShowProgress(string caption, int numerator, int denominator)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
