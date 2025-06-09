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
using Reko.Core.Collections;
using Reko.Core.Machine;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// Provides a text model that mixes code and data.
    /// </summary>
    public partial class MixedCodeDataModel : ITextViewModel
    {
        const int BytesPerLine = 16;

        private readonly Program program;
        private readonly ImageMap imageMap;
        private readonly ISelectedAddressService selSvc;
        private ModelPosition curPos;
        private readonly ModelPosition endPos;
        private readonly Dictionary<ImageMapBlock, MachineInstruction[]> instructions;
        private readonly IDictionary<Address, string[]> comments;
        protected readonly TextSpanFactory factory;

        public MixedCodeDataModel(
            Program program,
            ImageMap imageMap,
            TextSpanFactory factory,
            ISelectedAddressService selSvc)
        {
            this.program = program;
            this.imageMap = imageMap;
            this.factory = factory;
            this.selSvc = selSvc;

            var firstSeg = program.SegmentMap.Segments.Values.FirstOrDefault();
            if (firstSeg is null)
            {
                this.curPos = Pos(imageMap.BaseAddress);
                this.StartPosition = Pos(imageMap.BaseAddress);
                this.endPos = Pos(imageMap.BaseAddress);
                this.instructions = default!;
                this.LineCount = 0;
            }
            else
            {
                var lastSeg = program.SegmentMap.Segments.Values.Last();
                this.curPos = Pos(firstSeg.Address);
                this.StartPosition = Pos(firstSeg.Address);
                this.endPos = Pos(lastSeg.EndAddress);
                this.instructions = this.CollectInstructions();
                this.LineCount = CountLines();
            }
            this.comments = program.User.Annotations.ToSortedList(
                a => a.Address,
                a => Lines(a.Text));
        }


        public MixedCodeDataModel(MixedCodeDataModel that)
        {
            this.program = that.program;
            this.imageMap = that.imageMap;
            this.factory = that.factory;
            this.curPos = that.curPos;
            this.StartPosition = that.StartPosition;
            this.endPos = that.endPos;
            this.instructions = that.instructions;
            this.LineCount = that.LineCount;
            this.comments = that.comments;
            this.selSvc = that.selSvc;
        }

        public object CurrentPosition { get { return curPos; } }
        public object StartPosition { get; private set; }
        public object EndPosition { get { return endPos; } }

        public int LineCount { get; private set; }

        public MixedCodeDataModel Clone() => new MixedCodeDataModel(this);

        private int CountLines()
        {
            int sum = 0;
            foreach (var item in imageMap.Items.Values)
            {
                sum += CountBlockLines(item);
            }
            return sum;
        }

        private int CountBlockLines(ImageMapItem item)
        {
            if (item is ImageMapBlock bi)
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
        /// We align memory spans on 16-byte boundaries (//$REVIEW for now,
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
            //if (!instructions.TryGetValue(bi, out var instrs))
            //    return 0;
            //return instrs.Length;
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
        private Dictionary<ImageMapBlock, MachineInstruction[]> CollectInstructions()
        {
            var instructions = new Dictionary<ImageMapBlock, MachineInstruction[]>();
            foreach (var bi in imageMap.Items.Values.OfType<ImageMapBlock>()
                .ToList())
            {
                var instrs = new List<MachineInstruction>();
                if (bi.Size > 0 && bi.Block!.Procedure is not null)
                {
                    var addrStart = bi.Address;
                    var addrEnd = bi.Address + bi.Size;
                    var arch = bi.Block.Procedure.Architecture;
                    var dasm = program.CreateDisassembler(arch, addrStart).GetEnumerator();
                    while (dasm.MoveNext() && dasm.Current.Address < addrEnd)
                    {
                        instrs.Add(dasm.Current);
                    }
                }
                instructions.Add(bi, instrs.ToArray());
            }

            return instructions;
        }

        public int ComparePositions(object a, object b)
        {
            return ((ModelPosition)a).CompareTo((ModelPosition)b);
        }

        /// <summary>
        /// Returns the (approximate) position. This doesn't have to be
        /// 100% precise, but it shouldn't be insanely wrong either.
        /// </summary>
        /// <returns></returns>
        public (int, int) GetPositionAsFraction()
        {
            long numer = 0;
            foreach (var item in imageMap.Items.Values)
            {
                if (item.Address <= curPos.Address)
                {
                    if (item.IsInRange(curPos.Address))
                    {
                        numer += GetLineOffset(item, curPos.Address);
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
            return ((int)numer, (int)denom);
        }

        private int GetLineOffset(ImageMapItem item, Address addr)
        {
            if (item is ImageMapBlock bi)
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
            if (position is null)
                throw new ArgumentNullException(nameof(position));
            curPos = SanitizePosition((ModelPosition)position);
            if (offset == 0)
                return 0;
            int moved = 0;
            if (offset > 0)
            {

                if (!imageMap.TryFindItem(curPos.Address, out var item))
                    return moved;
                int iItem = imageMap.Items.IndexOfKey(item.Address);
                for (;;)
                {
                    Debug.Assert(item is not null);
                    if (item is ImageMapBlock bi)
                    {
                        var instrs = instructions[bi];
                        int i = FindIndexOfInstructionAddress(
                            instrs, curPos.Address);
                        Debug.Assert(i >= 0, "TryFindItem said this item contains the address.");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < instrs.Length)
                        {
                            moved += offset;
                            curPos = Pos(instrs[iNew].Address);
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

                        int i = FindIndexOfMemoryAddress(item, curPos.Address);
                        int iEnd = CountBlockLines(item);
                        Debug.Assert(i >= 0, "Should have been inside item");
                        int iNew = i + offset;
                        if (0 <= iNew && iNew < iEnd)
                        {
                            moved += offset;
                            curPos = Pos(GetAddressOfLine(item, iNew));
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
                    if (iItem >= imageMap.Items.Count)
                    {
                        // At the end of image, no need for SanitizeAddress
                        curPos = (ModelPosition)this.EndPosition;
                        return moved;
                    }
                    else
                    {
                        // At the start of an item, no need for SanitizeAddress
                        item = imageMap.Items.Values[iItem];
                        curPos = Pos(item.Address);
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
        private ModelPosition SanitizePosition(ModelPosition position)
        {
            if (imageMap.TryFindItem(position.Address, out var item))
            {
                if (item.IsInRange(position.Address))
                {
                    // Safely inside an item.
                    return position;
                }
                // If we're positioned at the end of the item
                // advance to the start of the next item if possible.
                int iItem = imageMap.Items.Keys.IndexOf(item.Address) + 1;
                if (iItem >= imageMap.Items.Count)
                {
                    return this.endPos;
                }
                return Pos(imageMap.Items.Keys[iItem]);
            }
            // We're outside the range of all items, so peg the position
            // at either the beginning or the end.
            if (position.Address < imageMap.BaseAddress)
            {
                return Pos(imageMap.BaseAddress);
            }
            return endPos;
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
                curPos = (ModelPosition)StartPosition;
                return;
            }
            else if (numer >= denom)
            {
                curPos = (ModelPosition)EndPosition;
                return;
            }

            var targetLine = (int)(((long)numer * LineCount) / denom);
            int curLine = 0;
            foreach (var item in imageMap.Items.Values)
            {
                int size;
                if (item is ImageMapBlock bi)
                {
                    size = CountDisassembledLines(bi);
                    if (curLine + size > targetLine)
                    {
                        this.curPos = Pos(instructions[bi][targetLine - curLine].Address);
                        return;
                    }
                }
                else
                {
                    size = CountMemoryLines(item);
                    if (curLine + size > targetLine)
                    {
                        this.curPos = Pos(GetAddressOfLine(item, targetLine - curLine));
                        return;
                    }
                }
                curLine += size;
            }
            curPos = (ModelPosition)EndPosition;
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

        private static ModelPosition Pos(Address addr, int offset = 0)
        {
            return new ModelPosition(addr, offset);
        }

        public static object Position(Address addr, int offset)
        {
            return Pos(addr, offset);
        }

        public static Address PositionAddress(object position)
        {
            return ((ModelPosition)position).Address;
        }

        private static string[] Lines(string s)
        {
            return s.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None);
        }

        private class ModelPosition : IComparable<ModelPosition>
        {
            public readonly Address Address;
            public readonly int Offset;

            public ModelPosition(Address addr, int offset)
            {
                this.Address = addr;
                this.Offset = offset;
            }

            public int CompareTo(ModelPosition? that)
            {
                if (that is null)
                    return 1;
                var cmp = this.Address.CompareTo(that.Address);
                if (cmp != 0)
                    return cmp;
                return this.Offset.CompareTo(that.Offset);
            }

            public override string ToString()
            {
                return $"{Address}({Offset})";
            }
        }

        public class DataItemNode
        {
            public Address StartAddress { get; set; }
            public Address EndAddress { get; set; }
            public Procedure? Proc { get; private set; }
            public int NumLines { get; set; }
            public TextModelNode? ModelNode { get; set; }
            public DataItemNode(Procedure? proc, int numLines, Address addrStart, Address addrEnd)
            {
                this.Proc = proc;
                this.NumLines = numLines;
                this.StartAddress = addrStart;
                this.EndAddress = addrEnd;
            }
        }

        public Collection<DataItemNode> GetDataItemNodes()
        {
            var nodes = new Collection<DataItemNode>();
            Procedure? curProc;
            DataItemNode? curNode = null;

            foreach (var item in imageMap.Items.Values)
            {
                int numLines;
                var startAddr = item.Address;
                var endAddr = item.Address + item.Size;
                if (item is ImageMapBlock bi)
                {
                    numLines = CountDisassembledLines(bi);
                    curProc = bi.Block?.Procedure;
                }
                else
                {
                    numLines = CountMemoryLines(item);
                    curProc = null;
                }

                if (curNode is null || curNode.Proc != curProc || curProc is null)
                {
                    curNode = new DataItemNode(curProc, numLines, startAddr, endAddr);
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
