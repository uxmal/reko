using Reko.Core;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.Arch.Arm
{
    public class Arm32PointerScanner : PointerScanner<uint>
    {
        public Arm32PointerScanner(EndianImageReader rdr, HashSet<uint> knownLinearAddress, PointerScannerFlags flags)
            : base(rdr, knownLinearAddress, flags)
        {
        }

        public override int PointerAlignment => 4;

        public override uint GetLinearAddress(Address address)
        {
            return address.ToUInt32();
        }

        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0x0F000000) == 0x0B000000)         // BL
            {
                int offset = ((int) opcode << 8) >> 6;
                target = (uint) ((long) rdr.Address.ToLinear() + 8 + offset);
                return true;
            }
            else
            {
                target = 0;
                return false;
            }
        }

        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target)
        {
            if ((opcode & 0x0F000000) == 0x0B000000)         // BL
            {
                int offset = ((int) opcode << 8) >> 6;
                target = (uint) ((long) rdr.Address.ToLinear() + 8 + offset);
                return true;
            }
            else
            {
                target = 0;
                return false;
            }
        }

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode)
        {
            return rdr.TryPeekUInt32(0, out opcode);
        }

        public override bool TryPeekPointer(EndianImageReader rdr, out uint target)
        {
            return rdr.TryPeekUInt32(0, out target);
        }
    }
}