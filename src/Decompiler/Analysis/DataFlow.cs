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

        /// <summary>
        /// Renders a textual representation of the given registers <paramref name="regs"/>.
        /// </summary>
        /// <param name="arch">Current <see cref="IProcessorArchitecture">processor architecture</see>.</param>
        /// <param name="caption">Caption to emit before the register themselves.</param>
        /// <param name="grfFlags">Flag group variables to render.</param>
        /// <param name="regs">Register variables to render.</param>
        /// <param name="sb"><see cref="TextWriter"/> to which the output is written.</param>
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

        /// <summary>
        /// Renders a textual representation of the given registers and their live bit 
        /// ranges, specified in <paramref name="regRanges"/>, to the output <paramref name="writer"/>.
        /// </summary>
        /// <param name="arch">Current <see cref="IProcessorArchitecture">processor architecture</see>.</param>
        /// <param name="caption">Caption to emit before the register themselves.</param>
        /// <param name="grfFlags">Flag group variables to render.</param>
        /// <param name="regRanges">Register variables to render.</param>
        /// <param name="writer"><see cref="StringWriter"/> to which the output is written.</param>
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

        /// <summary>
        /// Renders a textual representation of the given registers,
        /// specified in <paramref name="regs"/>, to the output <paramref name="writer"/>.
        /// </summary>
        /// <param name="caption">Caption to write before the registers.</param>
        /// <param name="regs">Registers to write.</param>
        /// <param name="writer"><see cref="TextWriter"/> to which the output is written.
        /// </param>
        public static void EmitRegisters(string caption, HashSet<Storage> regs, TextWriter writer)
		{
			writer.Write(caption);
			EmitRegistersCore(regs, writer);
		}

        /// <summary>
        /// Renders a textual representation of the given registers in a 
        /// dictionary,
        /// specified in <paramref name="symbols"/>, to the output <paramref name="writer"/>.
        /// </summary>
        public static void EmitRegisterValues<TValue>(string caption, Dictionary<Storage, TValue> symbols, TextWriter writer)
        {
            writer.Write(caption);
            foreach (var de in symbols.OrderBy(de => de.Key.ToString()))
            {
                writer.Write(" {0}:{1}", de.Key, de.Value);
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

        private void EmitFlagGroup(IProcessorArchitecture arch, string caption, Dictionary<RegisterStorage,uint> flagRegs, TextWriter sb)
        {
            sb.Write(caption);
            foreach (var freg in flagRegs
                .Select(f => arch.GetFlagGroup(f.Key, f.Value)!)
                .OrderBy(f => f.Name))
            {
                sb.Write(" {0}", freg.Name);
            }
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
            /// <summary>
            /// Constructs a <see cref="LiveOutUse"/> instance.
            /// </summary>
            /// <param name="range">Live bit range.</param>
            /// <param name="proc">First procedure to use this bit range. This 
            /// value is used solely for diagnostic and debugging 
            /// purposes.
            /// </param>
            public LiveOutUse(in BitRange range, Procedure proc)
            {
                this.Range = range;
                this.Procedure = proc;
            }

            /// <summary>
            /// The bits which are live.
            /// </summary>
            public BitRange Range { get; }

            /// <summary>
            /// The first procedure that made the bits in <see cref="Range"/> live.
            /// </summary>
            /// <remarks>
            /// This property is used for diagnostic purposes only.
            /// </remarks>
            public Procedure Procedure { get; }
        }

        /// <summary>
        /// This structure is used to track, for a particular flag group storage 
        /// that is live-out from a procedure, which bits of that
        /// storage are used by an calling procedure, and the first 
        /// external caller found by Reko. The latter property is 
        /// used for diagnostic purposes.
        /// </summary>

        public readonly struct LiveOutFlagsUse
        {
            /// <summary>
            /// Constructs a <see cref="LiveOutFlagsUse"/> instance.
            /// </summary>
            /// <param name="liveBits">A mask representing the live bits of a flag
            /// register.</param>
            /// <param name="proc">First procedure to use this bit range. This 
            /// value is used solely for diagnostic and debugging 
            /// purposes.
            /// </param>
            public LiveOutFlagsUse(uint liveBits, Procedure proc)
            {
                this.Procedure = proc;
                this.Flags = liveBits;
            }


            /// <summary>
            /// The first procedure that made the bits in <see cref="Flags"/> live.
            /// </summary>
            /// <remarks>
            /// This property is used for diagnostic purposes only.
            /// </remarks>
            public Procedure Procedure { get; }


            /// <summary>
            /// A mask representing the live bits of a flag register.
            /// </summary>
            public uint Flags { get; }
        }
    }
}
