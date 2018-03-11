using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;

namespace Reko.Arch.Microchip.PIC16
{
    public class PIC16PointerScanner : PointerScanner<uint>
    {
        public PIC16PointerScanner(EndianImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
            : base(rdr, knownLinAddresses, flags)
        {
        }

        public override uint GetLinearAddress(Address address) => throw new NotImplementedException();

        public override int PointerAlignment => throw new NotImplementedException();

        public override bool TryPeekOpcode(EndianImageReader rdr, out uint opcode) => throw new NotImplementedException();
        public override bool TryPeekPointer(EndianImageReader rdr, out uint target) => throw new NotImplementedException();
        public override bool MatchCall(EndianImageReader rdr, uint opcode, out uint target) => throw new NotImplementedException();
        public override bool MatchJump(EndianImageReader rdr, uint opcode, out uint target) => throw new NotImplementedException();
    }
}
