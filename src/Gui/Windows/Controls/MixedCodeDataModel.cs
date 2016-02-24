using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Windows.Controls
{
    public class MixedCodeDataModel : TextViewModel
    {
        private Program program;
        private Address currentPosition;

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
            return (int)(linEnd - linStart);
        }

        private ulong Align(ulong ul, uint alignment)
        {
            return alignment * (ul / alignment);
        }

        private int CountDisassembledLines(ImageMapBlock bi)
        {
            var addrStart = bi.Address;
            var addrEnd = bi.Address + bi.Size;
            var dasm = program.CreateDisassembler(addrStart).GetEnumerator();
            int sum = 1;        // 1 for the label name.
            while (dasm.MoveNext() && dasm.Current.Address < addrEnd)
            {
                ++sum;
            }
            return sum;
        }

        public int ComparePositions(object a, object b)
        {
            var diff = (Address)a - (Address)b;
            return diff.CompareTo(0);
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var spans = new List<LineSpan>();
            ImageSegment seg;
            ImageMapItem item;
            program.ImageMap.TryFindSegment(currentPosition, out seg);
            program.ImageMap.TryFindItem(currentPosition, out item);
            IEnumerator<MachineInstruction> dasm = null;
            while (count != 0 && seg!= null && item != null)
            {
                var bi = item as ImageMapBlock;
                if (bi != null)
                {
                    if (dasm == null)
                    {
                        dasm = program.CreateDisassembler(currentPosition).GetEnumerator();
                    }
                    if (dasm.MoveNext())
                    {
                        var instr = dasm.Current;
                        var line = DisassemblyTextModel.RenderAsmLine(program, instr);
                        currentPosition = instr.Address + instr.Length;
                        spans.Add(line);
                        --count;
                    }
                }
                else
                {
                    var tuple = RenderMemoryLine(currentPosition, item);
                    currentPosition = tuple.Item1;
                    spans.Add(tuple.Item2);
                    --count;
                }
                if (!item.IsInRange(currentPosition))
                {
                    break;
                }
            }
            return spans.ToArray();
        }

        private Tuple<Address, LineSpan> RenderMemoryLine(Address addr, ImageMapItem item)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
            throw new NotImplementedException();
        }
    }
}
