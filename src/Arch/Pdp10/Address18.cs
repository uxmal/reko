using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10
{
    public class Address18 : Address
    {
        private const uint Mask = (1 << 18) - 1;
        
        public Address18(uint uAddr) : base(Pdp10Architecture.Ptr18)
        {
            this.Offset = uAddr;
        }

        public override bool IsNull => Offset == 0;

        public override bool IsZero => Offset == 0;

        public override sealed ulong Offset { get; }

        public override ushort? Selector => null;

        public override Address Add(long offset)
        {
            return new Address18((uint) ((long) Offset + offset) & Mask);
        }

        public override Address Align(int alignment)
        {
            return new Address18((uint)(alignment * (((long)Offset + alignment - 1) / alignment)));
        }

        public override Expression CloneExpression()
        {
            throw new NotImplementedException();
        }

        public override string GenerateName(string prefix, string suffix)
        {
            var octal = Convert.ToString((uint)Offset, 8);
            return $"{prefix}{octal}{suffix}";
        }

        public override Address NewOffset(ulong offset)
        {
            return new Address18((uint)offset & Mask);
        }

        public override Constant ToConstant()
        {
            return Constant.Create(this.DataType, Offset);
        }

        public override ulong ToLinear()
        {
            return Offset;
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return (uint)Offset;
        }

        public override string ToString()
        {
            return Convert.ToString((uint) Offset, 8);
        }
    }
}
