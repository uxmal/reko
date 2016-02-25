#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Reko.Gui.Windows.Controls
{
    /// <summary>
    /// Provides a text model that mixes code and data.
    /// </summary>
    public partial class MixedCodeDataModel : TextViewModel
    {
        private Program program;
        private Address currentPosition;
        private Dictionary<ImageMapBlock, MachineInstruction[]> instructions;

        public MixedCodeDataModel(Program program)
        {
            this.program = program;
            var firstSeg = program.ImageMap.Segments.Values.FirstOrDefault();
            if (firstSeg == null)
            {
                this.currentPosition = program.ImageMap.BaseAddress;
                this.StartPosition = program.ImageMap.BaseAddress;
                this.EndPosition = program.ImageMap.BaseAddress;
                this.LineCount = 0;
            }
            else
            {
                var lastSeg = program.ImageMap.Segments.Values.Last();
                this.currentPosition = firstSeg.Address;
                this.StartPosition = firstSeg.Address;
                this.EndPosition = lastSeg.EndAddress;
                this.CollectInstructions();
                this.LineCount = CountLines();
            }
        }

        public object CurrentPosition {  get { return currentPosition; } }
        public object StartPosition { get; private set;  }
        public object EndPosition { get; private set;  }

        public int LineCount { get; private set; }

        private int CountLines()
        {
            int sum = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                var bi = item as ImageMapBlock;
                if (bi != null)
                {
                    sum += CountDisassembledLines(bi);
                }
                else
                {
                    sum += CountMemoryLines(item);
                }
            }
            return sum;
        }

        private int CountMemoryLines(ImageMapItem item)
        {
            var linStart = item.Address.ToLinear();
            var linEnd = linStart + item.Size;
            linStart = Align(linStart, 16);
            linEnd = Align(linEnd + (16 - 1), 16);
            return (int)(linEnd - linStart) / 16;
        }

        private static ulong Align(ulong ul, uint alignment)
        {
            return alignment * (ul / alignment);
        }

        private static Address Align(Address addr, uint alignment)
        {
            var lin = addr.ToLinear();
            var linAl = Align(lin, alignment);
            return addr - (int)(lin - linAl);
        }

        private void CollectInstructions()
        {
            this.instructions = new Dictionary<ImageMapBlock, MachineInstruction[]>();
            foreach (var bi in program.ImageMap.Items.Values.OfType<ImageMapBlock>())
            {
                var instrs = new List<MachineInstruction>();
                var addrStart = bi.Address;
                var addrEnd = bi.Address + bi.Size;
                var dasm = program.CreateDisassembler(addrStart).GetEnumerator();
                while (dasm.MoveNext() && dasm.Current.Address < addrEnd)
                {
                    instrs.Add(dasm.Current);
                }
                instructions.Add(bi, instrs.ToArray());
            }
        }

        private int CountDisassembledLines(ImageMapBlock bi)
        {
            return instructions[bi].Length;
        }

        public int ComparePositions(object a, object b)
        {
            var diff = (Address)a - (Address)b;
            return diff.CompareTo(0);
        }

        //$PERF: could benefit from a binary search, but basic blocks
        // are so small it may not make a difference.
        public static int FindIndexOfInstructionAddress(MachineInstruction[] instrs, Address addr)
        {
            var ul = addr.ToLinear();
            return Array.FindIndex(
                instrs,
                i => i.Contains(addr));
        }



        /// <summary>
        /// An segment of memory
        /// </summary>
        public class MemoryTextSpan : TextSpan
        {
            private string text;

            public MemoryTextSpan(string text, string style)
            {
                this.text = text;
                base.Style = style;
            }

            public override string GetText()
            {
                return text;
            }

            public override SizeF GetSize(string text, Font font, Graphics g)
            {
                SizeF sz = base.GetSize(text, font, g);
                return sz;
            }
        }

        /// <summary>
        /// An inert text span is not clickable nor has a context menu.
        /// </summary>
        public class InertTextSpan : TextSpan
        {
            private string text;

            public InertTextSpan(string text, string style)
            {
                this.text = text;
                base.Style = style;
            }

            public override string GetText()
            {
                return text;
            }

            public override SizeF GetSize(string text, Font font, Graphics g)
            {
                SizeF sz = base.GetSize(text, font, g);
                return sz;
            }
        }


        public Tuple<int, int> GetPositionAsFraction()
        {
            var linPos = currentPosition;
            long numer = 0;
            long denom = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                if (item.Address <= currentPosition)
                {
                    if (item.IsInRange(currentPosition))
                    {
                        numer += (currentPosition - item.Address);
                    }
                    else
                    {
                        numer += item.Size;
                    }
                }
                denom += item.Size;
            }
            while (denom >= 0x80000000)
            {
                numer >>= 1;
                denom >>= 1;
            }
            return Tuple.Create((int)numer, (int)denom);
        }

        public int MoveToLine(object position, int offset)
        {
            if (position == null)
                throw new ArgumentNullException("position");
            currentPosition = (Address)position;
            if (offset == 0)
                return 0;
            if (offset > 0)
            {

            }
            throw new NotImplementedException();
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
            throw new NotImplementedException();
        }
    }
}
