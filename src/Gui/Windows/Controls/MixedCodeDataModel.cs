﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        private Address addrCur;
        private int commentOffset;
        private Address addrEnd;
        private Dictionary<ImageMapBlock, MachineInstruction[]> instructions;
        private IDictionary<Address, string[]> comments;

        public MixedCodeDataModel(Program program)
        {
            this.program = program;
            var firstSeg = program.SegmentMap.Segments.Values.FirstOrDefault();
            if (firstSeg == null)
            {
                this.addrCur = program.ImageMap.BaseAddress;
                this.StartPosition = program.ImageMap.BaseAddress;
                this.addrEnd = program.ImageMap.BaseAddress;
                this.LineCount = 0;
            }
            else
            {
                var lastSeg = program.SegmentMap.Segments.Values.Last();
                this.addrCur = firstSeg.Address;
                this.StartPosition = firstSeg.Address;
                this.addrEnd = lastSeg.EndAddress;
                this.CollectInstructions();
                this.LineCount = CountLines();
            }
            this.comments = program.User.Annotations.ToSortedList(
                a => a.Address,
                a => a.Text.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.None));
            this.commentOffset = 0;
        }

        public MixedCodeDataModel(MixedCodeDataModel that)
        {
            this.program = that.program;
            this.addrCur = that.addrCur;
            this.addrEnd = that.addrEnd;
            this.instructions = that.instructions;
            this.LineCount = that.LineCount;
        }

        public object CurrentPosition { get { return addrCur; } }
        public object StartPosition { get; private set; }
        public object EndPosition { get { return addrEnd; } }

        public int LineCount { get; private set; }

        public MixedCodeDataModel Clone()
        {
            return new MixedCodeDataModel(this);
        }

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
            foreach (var bi in program.ImageMap.Items.Values.OfType<ImageMapBlock>().ToList())
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
            long numer = 0;
            foreach (var item in program.ImageMap.Items.Values)
            {
                if (item.Address <= addrCur)
                {
                    if (item.IsInRange(addrCur))
                    {
                        numer += GetLineOffset(item, addrCur);
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
            addrCur = SanitizeAddress((Address) position);
            this.commentOffset = 0;
            if (offset == 0)
                return 0;
            int moved = 0;
            if (offset > 0)
            {
                ImageMapItem item;

                if (!program.ImageMap.TryFindItem(addrCur, out item))
                    return moved;
                int iItem = program.ImageMap.Items.IndexOfKey(item.Address);
                for (;;)
                {
                    Debug.Assert(item != null);
                    var bi = item as ImageMapBlock;
                    if (bi != null)
                    {
                        var instrs = instructions[bi];
                        int i = FindIndexOfInstructionAddress(instrs, addrCur);
                        Debug.Assert(i >= 0, "TryFindItem said this item contains the address.");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < instrs.Length)
                        {
                            moved += offset;
                            addrCur = instrs[iNew].Address;
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

                        int i = FindIndexOfMemoryAddress(item, addrCur);
                        int iEnd = CountBlockLines(item);
                        Debug.Assert(i >= 0, "Should have been inside item");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < iEnd)
                        {
                            moved += offset;
                            addrCur = GetAddressOfLine(item, iNew);
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
                        // At the end of image, no need for SanitizeAddress
                        addrCur = (Address)this.EndPosition;
                        return moved;
                    }
                    else
                    {
                        // At the start of an item, no need for SanitizeAddress
                        item = program.ImageMap.Items.Values[iItem];
                        addrCur = item.Address;
                    }
                }
            }
            throw new NotImplementedException("Moving backwards not implemented yet.");
        }

        /// <summary>
        /// Given an address, attempts to make sure that it points to a valid
        /// position in the address space or to the EOF
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Address SanitizeAddress(Address position)
        {
            ImageMapItem item;
            if (program.ImageMap.TryFindItem(position, out item))
            {
                if (item.IsInRange(position))
                {
                    // Safely inside an item.
                    return position;
                }
                // If we're positioned at the end of the item
                // advance to the start of the next item if possible.
                int iItem = program.ImageMap.Items.Keys.IndexOf(item.Address) + 1;
                if (iItem >= program.ImageMap.Items.Count)
                {
                    return this.addrEnd;
                }
                return program.ImageMap.Items.Keys[iItem];
            }
            // We're outside the range of all items, so peg the position
            // at either the beginning or the end.
            if (position < program.ImageMap.BaseAddress)
            {
                return program.ImageMap.BaseAddress;
            }
            return addrEnd;
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
            // This is PTomin's simpler implementation of SetPositionAsFraction
            // Notice that just like the original implementation, it is O(n) where
            // n is the number of items in the image map. Consider measuring 
            // performance on really large image maps (1,000,000 items or more)
            // to see if the brittle code in the #else branch makes any difference
            // and if not, keep the SIMPLE implementation.

            long total = LineCount;
            long iPos = (numer * total) / denom;

            MoveToLine(StartPosition, (int)iPos);
#else
            if (numer <= 0)
            {
                addrCur = (Address)StartPosition;
                return;
            }
            else if (numer >= denom)
            {
                addrCur = (Address)EndPosition;
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
                        this.addrCur = instructions[bi][targetLine - curLine].Address;
                        return;
                    }
                }
                else
                {
                    size = CountMemoryLines(item);
                    if (curLine + size > targetLine)
                    {
                        this.addrCur = GetAddressOfLine(item, targetLine - curLine);
                        return;
                    }
                }
                curLine += size;
            }
            addrCur = (Address)EndPosition;
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
