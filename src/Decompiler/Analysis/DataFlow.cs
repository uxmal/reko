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

using Reko.Core;
using Reko.Core.Lib;
using System.Collections.Generic;
using System.Linq;
using IProcessorArchitecture = Reko.Core.IProcessorArchitecture;
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

		public static void EmitRegisters(IProcessorArchitecture arch, string caption, Dictionary<RegisterStorage, uint> grfFlags, IEnumerable<Storage> regs, TextWriter sb)
		{
			sb.Write(caption);
            var sGrf = string.Join(" ", grfFlags
                .Where(f => f.Value != 0)
                .Select(f => arch.GetFlagGroup(f.Key, f.Value)!)
                .OrderBy(f => f.Name));
            if (sGrf.Length > 0)
			{
                sb.Write(" {0}", sGrf);
			}
			EmitRegistersCore(regs, sb);
		}

        public static void EmitRegisters(IProcessorArchitecture arch, string caption, Dictionary<RegisterStorage, uint> grfFlags, IDictionary<Storage, BitRange> regRanges, TextWriter sb)
        {
            sb.Write(caption);
            var sGrf = string.Join(" ", grfFlags
                .Where(f => f.Value != 0)
                .Select(f => arch.GetFlagGroup(f.Key, f.Value)!)
                .OrderBy(f => f.Name));
            if (sGrf.Length > 0)
            {
                sb.Write(" {0}", sGrf);
            }
            foreach (var de in regRanges.OrderBy(de => de.Key.Name))
            {
                sb.Write(" {0}:{1}", de.Key, de.Value);
            }
        }

        public static void EmitRegisters(
            IProcessorArchitecture arch,
            string caption,
            Dictionary<RegisterStorage, LiveOutFlagsUse> grfFlags,
            IDictionary<Storage, LiveOutUse> regRanges,
            TextWriter writer)
        {
            writer.Write(caption);
            var sGrf = string.Join(" ", grfFlags
                .Where(f => f.Value.Flags != 0)
                .Select(f => $"{arch.GetFlagGroup(f.Key, f.Value.Flags)}-{f.Value.Procedure.Name}")
                .OrderBy(f => f));
            if (sGrf.Length > 0)
            {
                writer.Write(" {0}", sGrf);
            }
            foreach (var de in regRanges.OrderBy(de => de.Key.Name))
            {
                writer.Write(" {0}:{1}-{2}", de.Key, de.Value.Range, de.Value.Procedure.Name);
            }
        }

        public static void EmitRegisters(string caption, HashSet<Storage> regs, TextWriter sb)
		{
			sb.Write(caption);
			EmitRegistersCore(regs, sb);
		}

        public static void EmitRegisterValues<TValue>(string caption, Dictionary<Storage, TValue> symbols, TextWriter sb)
        {
            sb.Write(caption);
            foreach (var de in symbols.OrderBy(de => de.Key.ToString()))
            {
                sb.Write(" {0}:{1}", de.Key, de.Value);
            }
        }

        private static void EmitRegistersCore(IEnumerable<Storage> regs, TextWriter sb)
		{
            foreach (var reg in regs.Where(r => r is not null).OrderBy(r => r.Name))
            {
                sb.Write(" ");
                sb.Write(reg.Name);
            }
		}

        public void EmitFlagGroup(IProcessorArchitecture arch, string caption, Dictionary<RegisterStorage,uint> flagRegs, TextWriter sb)
        {
            sb.Write(caption);
            foreach (var freg in flagRegs
                .Select(f => arch.GetFlagGroup(f.Key, f.Value)!)
                .OrderBy(f => f.Name))
            {
                sb.Write(" {0}", freg.Name);
            }
        }

		public string EmitRegisters(string caption, HashSet<Storage> regs)
		{
			var sw = new StringWriter();
			EmitRegisters(caption, regs, sw);
			return sw.ToString();
		}

		public string EmitFlagGroup(IProcessorArchitecture arch, string caption, Dictionary<RegisterStorage, uint> grfFlags)
		{
			var sw = new StringWriter();
			EmitFlagGroup(arch, caption, grfFlags, sw);
			return sw.ToString();
		}

        /// <summary>
        /// This structure is used to track, for a particular storage 
        /// that is live-out from a procedure, which bit range of that
        /// storage is used by an calling procedure, and the first 
        /// external caller found by Reko. The latter property is 
        /// used for diagnostic purposes.
        /// </summary>
        public readonly struct LiveOutUse
        {
            public LiveOutUse(in BitRange range, Procedure proc)
            {
                this.Range = range;
                this.Procedure = proc;
            }

            public BitRange Range { get; }
            public Procedure Procedure { get; }
        }

        public readonly struct LiveOutFlagsUse
        {
            public LiveOutFlagsUse(uint flags, Procedure proc)
            {
                this.Procedure = proc;
                this.Flags = flags;
            }

            public Procedure Procedure { get; }
            public uint Flags { get; }
        }
    }
}
