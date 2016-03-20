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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Reko.Gui.Windows.Controls
{
    /// <summary>
    /// Provides a text model that mixes code and data.
    /// </summary>
    public partial class MixedCodeDataModel : TextViewModel
    {
        const int BytesPerLine = 16;

        private Program program;
        private Address currentPosition;
        private Address endAddress;
        private Dictionary<ImageMapBlock, MachineInstruction[]> instructions;

        public MixedCodeDataModel(Program program)
        {
            this.program = program;
            var firstSeg = program.ImageMap.Segments.Values.FirstOrDefault();
            if (firstSeg == null)
            {
                this.currentPosition = program.ImageMap.BaseAddress;
                this.StartPosition = program.ImageMap.BaseAddress;
                this.endAddress = program.ImageMap.BaseAddress;
                this.LineCount = 0;
            }
            else
            {
                var lastSeg = program.ImageMap.Segments.Values.Last();
                this.currentPosition = firstSeg.Address;
                this.StartPosition = firstSeg.Address;
                this.endAddress = lastSeg.EndAddress;
                this.CollectInstructions();
                this.LineCount = CountLines();
            }
        }

        public object CurrentPosition {  get { return currentPosition; } }
        public object StartPosition { get; private set;  }
        public object EndPosition { get { return endAddress; } }

        public int LineCount { get; private set; }

        private int CountLines()
        {
            int sum = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                sum += CountBlockLines(item);
            }
            return sum;
        }

        private int CountBlockLines(ImageMapItem item)
        {
            var bi = item as ImageMapBlock;
            if (bi != null)
            {
                return CountDisassembledLines(bi);
            }
            else
            {
                return CountMemoryLines(item);
            }
        }

        /// <summary>
        /// Count the number of lines a memory area subtends.
        /// </summary>
        /// <remarks>
        /// We align mempry spans on 16-byte boundaries (//$REVIEW for now,
        /// this should be user-adjustable) so if we have a memory span 
        /// straddling such a boundary, we have to account for it. E.g. the
        /// span [01FC-0201] should be rendered:
        /// <code>
        /// 01FC                                    0C 0D 0F             ....
        /// 0200 00 01                                       ..
        /// </code>
        /// and therefore requires 2 lines even though the number of bytes is
        /// less than 16.
        /// </remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        private int CountMemoryLines(ImageMapItem item)
        {
            if (item.Size == 0)
                return 0;       //$TODO: this shouldn't ever happen!
            var linStart = item.Address.ToLinear();
            var linEnd = linStart + item.Size;
            linStart = Align(linStart, BytesPerLine);
            linEnd = Align(linEnd + (BytesPerLine - 1), BytesPerLine);
            return (int)(linEnd - linStart) / BytesPerLine;
        }

        private int CountDisassembledLines(ImageMapBlock bi)
        {
            return instructions[bi].Length;
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

        /// <summary>
        /// Preemptively collect the machine code instructions
        /// in all image map blocks.
        /// </summary>
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

        public int ComparePositions(object a, object b)
        {
            var diff = (Address)a - (Address)b;
            return diff.CompareTo(0);
        }

        /// <summary>
        /// Returns the (approximate) position. This doesn't have to be
        /// 100% precise, but it shouldn't be insanely wrong either.
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> GetPositionAsFraction()
        {
            var linPos = currentPosition;
            long numer = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                if (item.Address <= currentPosition)
                {
                    if (item.IsInRange(currentPosition))
                    {
                        numer += GetLineOffset(item, currentPosition);
                        break;
                    }
                    numer += CountBlockLines(item);
                }
            }
            long denom = LineCount;
            while (denom >= 0x80000000)
            {
                numer >>= 1;
                denom >>= 1;
            }
            return Tuple.Create((int)numer, (int)denom);
        }

        private int GetLineOffset(ImageMapItem item, Address addr)
        {
            var bi = item as ImageMapBlock;
            if (bi != null)
            {
                int i = 0;
                while (i < instructions[bi].Length)
                {
                    if (instructions[bi][i].Address >= addr)
                    {
                        break;
                    }
                    ++i;
                }
                return i;
            }
            else
            {
                return (int)((Align(addr.ToLinear(), BytesPerLine) -
                              Align(item.Address.ToLinear(), BytesPerLine)) /
                              BytesPerLine);
            }
        }

        public int MoveToLine(object position, int offset)
        {
            if (position == null)
                throw new ArgumentNullException("position");
            currentPosition = SanitizeAddress((Address) position);
            if (offset == 0)
                return 0;
            int moved = 0;
            if (offset > 0)
            {
                ImageMapItem item;

                if (!program.ImageMap.TryFindItem(currentPosition, out item))
                    return moved;
                int iItem = program.ImageMap.Items.IndexOfKey(item.Address);
                for (;;)
                {
                    Debug.Assert(item != null);
                    var bi = item as ImageMapBlock;
                    if (bi != null)
                    {
                        var instrs = instructions[bi];
                        int i = FindIndexOfInstructionAddress(instrs, currentPosition);
                        Debug.Assert(i >= 0, "TryFindItem said this item contains the address.");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < instrs.Length)
                        {
                            moved += offset;
                            currentPosition = instrs[iNew].Address;
                            return moved;
                        }
                        // Fell off the end.

                        if (offset > 0)
                        {
                            moved += instrs.Length - i;
                            offset -= instrs.Length - i;
                        }
                        else
                        {
                            moved -= i;
                            offset += i;
                        }
                    }
                    else
                    {
                        // Determine current line # within memory block

                        int i = FindIndexOfMemoryAddress(item, currentPosition);
                        int iEnd = CountBlockLines(item);
                        Debug.Assert(i >= 0, "Should have been inside item");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < iEnd)
                        {
                            moved += offset;
                            currentPosition = GetAddressOfLine(item, iNew);
                            return moved;
                        }
                        // Fall of the end
                        if (offset > 0)
                        {
                            moved += iEnd - i;
                            offset -= iEnd - i;
                        }
                        else
                        {
                            moved -= i;
                            offset += i;
                        }
                    }

                    // Since we fall off the current map item,
                    // move to next image map item.
                    ++iItem;
                    if (iItem >= program.ImageMap.Items.Count)
                    {
                        currentPosition = (Address)this.EndPosition;
                        return moved;
                    }
                    else
                    {
                        item = program.ImageMap.Items.Values[iItem];
                        currentPosition = item.Address;
                    }
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException("Moving backwards not implemented yet.");
        }

        /// <summary>
        /// Given an address, attempts to make sure that it points to a valid
        /// position in the address space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Address SanitizeAddress(Address position)
        {
            ImageMapItem item;
            if (program.ImageMap.TryFindItem(position, out item))
            {
                if (position < item.EndAddress)
                {
                    return position;
                }
                int iItem = program.ImageMap.Items.Keys.IndexOf(item.Address) + 1;
                if (iItem >= program.ImageMap.Items.Count)
                    return this.endAddress;
                return program.ImageMap.Items.Keys[iItem];
            }
            if (position < program.ImageMap.BaseAddress)
            {
                return program.ImageMap.BaseAddress;
            }
            return endAddress;
        }

        private Address GetAddressOfLine(ImageMapItem item, int i)
        {
            if (i == 0)
                return item.Address;
            else
                return Align(item.Address + i * BytesPerLine, BytesPerLine);
        }

        /// <summary>
        /// Find the index of the address within the item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        private int FindIndexOfMemoryAddress(ImageMapItem item, Address addr)
        {
            var addrStart = Align(item.Address, BytesPerLine);
            long idx = (addr - addrStart) / BytesPerLine;
            return (int) idx;
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
            if (denom <= 0)
                throw new ArgumentOutOfRangeException("denom", "Denominator must be larger than 0.");
#if SIMPLE
            long total = LineCount;
            long iPos = (numer * total) / denom;

            MoveToLine(StartPosition, (int)iPos);
#else
            if (numer <= 0)
            {
                currentPosition = (Address)StartPosition;
                return;
            }
            else if (numer >= denom)
            {
                currentPosition = (Address)EndPosition;
                return;
            }

            var targetLine = (int)(((long)numer * LineCount) / denom);
            int curLine = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                int size;
                var bi = item as ImageMapBlock;
                if (bi != null)
                {
                    size = CountDisassembledLines(bi);
                    if (curLine + size > targetLine)
                    {
                        this.currentPosition = instructions[bi][targetLine - curLine].Address;
                        return;
                    }
                }
                else
                {
                    size = CountMemoryLines(item);
                    if (curLine + size > targetLine)
                    {
                        this.currentPosition = GetAddressOfLine(item, targetLine - curLine);
                        return;
                    }
                }
                curLine += size;
            }
            currentPosition = (Address)EndPosition;
#endif
        }

        public int CountLines(object startPos, object endPos)
        {
            var oldPos = CurrentPosition;

            MoveToLine(startPos, 0);

            int numLines = 0;
            while (ComparePositions(CurrentPosition, endPos) < 0)
                MoveToLine(startPos, ++numLines);

            MoveToLine(oldPos, 0);

            return numLines;
        }

        public class DataItemNode
        {
            public Address StartAddress { get; internal set; }
            public Address EndAddress { get; internal set; }
            public Procedure Proc { get; private set; }
            public int NumLines { get; internal set; }
            public TextModelNode ModelNode { get; internal set; }
            public DataItemNode(Procedure proc, int numLines) { this.Proc = proc;  this.NumLines = numLines; }
        }

        public Collection<DataItemNode> GetDataItemNodes()
        {
            var nodes = new Collection<DataItemNode>();
            Procedure curProc = null;
            DataItemNode curNode = null;

            foreach (var item in program.ImageMap.Items.Values)
            {
                int numLines;
                var startAddr = item.Address;
                var endAddr = item.Address + item.Size;
                var bi = item as ImageMapBlock;
                if (bi != null)
                {
                    numLines = CountDisassembledLines(bi);
                    curProc = bi.Block.Procedure;
                }
                else
                {
                    numLines = CountMemoryLines(item);
                    curProc = null;
                }

                if (curNode == null || curNode.Proc != curProc || curProc == null)
                {
                    curNode = new DataItemNode(curProc, numLines);
                    curNode.StartAddress = startAddr;
                    curNode.EndAddress = endAddr;
                    nodes.Add(curNode);
                }
                else
                {
                    curNode.NumLines += numLines;
                    curNode.EndAddress = endAddr;
                }
            }

            return nodes;
        }
    }
}
