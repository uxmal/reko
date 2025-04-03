using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Pdp.Memory
{
    public class PdpTypes
    {
        public static PrimitiveType Word18 { get; } = PrimitiveType.CreateWord(18);
        public static PrimitiveType Word36 { get; } = PrimitiveType.CreateWord(36);
        public static PrimitiveType Int36 { get; } = PrimitiveType.Create(Domain.SignedInt, 36);
        public static PrimitiveType Word72 { get; } = PrimitiveType.CreateWord(72);

        public static PrimitiveType Ptr18 { get; } = PrimitiveType.Create(Domain.Pointer, 18);
        public static PrimitiveType Real36 { get; } = PrimitiveType.Create(Domain.Real, 36);
        public static PrimitiveType Real72 { get; } = PrimitiveType.Create(Domain.Real, 72);

        public static bool TryParseAddress(string? txtAddr, out Address addr)
        {
            if (txtAddr is null)
            {
                addr = default!;
                return false;
            }
            uint uAddr = 0;
            foreach (var c in txtAddr)
            {
                var u = (uint) (c - '0');
                if (u > 8)
                {
                    addr = default!;
                    return false;
                }
                uAddr = (uAddr << 3) | u;
            }
            addr = Pdp10Architecture.Ptr18(uAddr);
            return true;
        }
    }
}
