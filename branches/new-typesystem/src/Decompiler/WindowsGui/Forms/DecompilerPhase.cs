/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using System;

namespace Decompiler.WindowsGui.Forms
{
	/// <summary>
	/// A state in the state machine that describes the current state of 
	/// the decompiler application.
	/// </summary>
	public abstract class DecompilerPhase
	{
		private PhasePage page;
		private DecompilerPhase next;

		public DecompilerPhase(PhasePage page)
		{
			this.page = page;
		}

		public abstract void Execute(DecompilerDriver decompiler);


		public DecompilerPhase NextPhase
		{
			get { return next; } 
			set { next = value; }
		}

		/// <summary>
		/// The user interface associated with this page.
		/// </summary>
		public PhasePage Page
		{
			get { return page; }
		}
	}

	public class InitialPhase : DecompilerPhase
	{
		public InitialPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.LoadProgram();
		}
	}

	public class LoadedPhase : DecompilerPhase
	{
		public LoadedPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.ScanProgram();
		}

	}

	public class ScannedPhase : DecompilerPhase
	{
		public ScannedPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.LoadProgram();
		}

	}


	public class MachineCodeRewrittenPhase : DecompilerPhase
	{
		public MachineCodeRewrittenPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.AnalyzeDataFlow();
		}
	}


	public class DataFlowPhase : DecompilerPhase
	{
		public DataFlowPhase(PhasePage page) : base (page)
		{
		}
	

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.ReconstructTypes();
		}

	}


	public class TypeReconstructedPhase : DecompilerPhase
	{
		public TypeReconstructedPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
			decompiler.LoadProgram();
		}

	}

	public class CodeStructuredPhase : DecompilerPhase
	{
		public CodeStructuredPhase(PhasePage page) : base (page)
		{
		}

		public override void Execute(DecompilerDriver decompiler)
		{
		}
	}
}

