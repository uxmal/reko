using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{
    public struct Bitfield
    {
        public readonly int Position;
        public readonly int Length;
        public readonly uint Mask;

        public Bitfield(int position, int length)
        {
            this.Position = position;
            this.Length = length;
            this.Mask = (1U << length) - 1U;
        }

        public uint Read(uint u)
        {
            return (u >> Position) & Mask;
        }

        public uint Read(ulong u)
        {
            return (uint)((u >> Position) & Mask);
        }

        public int ReadSigned(uint u)
        {
            var v = (u >> Position) & Mask;
            var m = 1u << (Length - 1);
            var s = (v ^ m) - m;
            return (int)s;
        }

        public static uint ReadFields(Bitfield[] bitfields, uint u)
        {
            uint n = 0;
            foreach (var bitfield in bitfields)
            {
                n = n << bitfield.Length | ((u >> bitfield.Position) & bitfield.Mask);
            }
            return n;
        }

        public static int ReadSignedFields(Bitfield[] fields, uint wInstr)
        {
            int n = 0;
            int bitsTotal = 0;
            foreach (var bitfield in fields)
            {
                n = n << bitfield.Length | (int)((wInstr >> bitfield.Position) & bitfield.Mask);
                bitsTotal += bitfield.Length;
            }
            n = n << (32 - bitsTotal);
            n = n >> (32 - bitsTotal);
            return n;
        }

        public override string ToString()
        {
            return $"[{Position}..{Position + Length})";
        }
    }
}
