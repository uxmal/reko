using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public struct Relocation
    {
        /// <summary>
        /// The location at which to relocate.
        /// </summary>
        public Address  Address { get; }

        /// <summary>
        /// The symbol used for the relocation.
        /// </summary>
        public string SymbolName { get; }

        /// <summary>
        /// The type of the relocation.
        /// </summary>
        public int RelocationType { get; }

        public Relocation(Address addr, string symbolName, int relocationType)
        {
            Address = addr;
            SymbolName = symbolName;
            RelocationType = relocationType;
        }

        public override string ToString()
        {
            return $"{Address}: {RelocationType,2} {SymbolName}";
        }
    }
}
