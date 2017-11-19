#region License
/* 
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Expression = Reko.Core.Expressions.Expression;
using IProcessorArchitecture = Reko.Core.IProcessorArchitecture;
using SortedList = System.Collections.SortedList;
using Storage = Reko.Core.Storage;
using StringWriter = System.IO.StringWriter;
using TextWriter = System.IO.TextWriter;

namespace Reko.Analysis
{
	/// <summary>
	/// Abstract base class for summary information kept with procedures or blocks.
	/// </summary>
	public abstract class DataFlow
	{
		/// <summary>
		/// Displays the dataflow object as a human-readable stream of text.
		/// </summary>
		/// <param name="arch">current processor architecture (to interpret registers)</param>
		/// <param name="sb">stream into which the data is written</param>
		public abstract void Emit(IProcessorArchitecture arch, TextWriter sb);

		public static void EmitRegisters(IProcessorArchitecture arch, string caption, uint grfFlags, HashSet<RegisterStorage> regs, TextWriter sb)
		{
			sb.Write(caption);
			if (grfFlags != 0)
			{
				sb.Write(" {0}", arch.GrfToString(grfFlags));
			}
			EmitRegistersCore(arch, regs, sb);
		}

		public static void EmitRegisters(IProcessorArchitecture arch, string caption, HashSet<RegisterStorage> regs, TextWriter sb)
		{
			sb.Write(caption);
			EmitRegistersCore(arch, regs, sb);
		}

        public static void EmitRegisterValues<TValue>(string caption, Dictionary<Storage, TValue> symbols, TextWriter sb)
        {
            sb.Write(caption);
            foreach (var de in symbols.OrderBy(de => de.Key.ToString()))
            {
                sb.Write(" {0}:{1}", de.Key, de.Value);
            }
        }
        
		private static void EmitRegistersCore(IProcessorArchitecture arch, HashSet<RegisterStorage> regs, TextWriter sb)
		{
            foreach (var reg in regs.Where(r => r!= null).OrderBy(r => r.Name))
            {
                sb.Write(" ");
                sb.Write(reg.Name);
            }
		}

        public void EmitFlagGroup(IProcessorArchitecture arch, string caption, uint grfFlags, TextWriter sb)
        {
            sb.Write(caption);
            sb.Write(" {0}", arch.GrfToString(grfFlags));
        }

		public string EmitRegisters(IProcessorArchitecture arch, string caption, HashSet<RegisterStorage> regs)
		{
			StringWriter sw = new StringWriter();
			EmitRegisters(arch, caption, regs, sw);
			return sw.ToString();
		}

		public string EmitFlagGroup(IProcessorArchitecture arch, string caption, uint grfFlags)
		{
			StringWriter sw = new StringWriter();
			EmitFlagGroup(arch, caption, grfFlags, sw);
			return sw.ToString();
		}
	}
}
