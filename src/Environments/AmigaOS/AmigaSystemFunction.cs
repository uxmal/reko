using Reko.Core;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.AmigaOS
{
    public class AmigaSystemFunction
    {
        public int Offset;
        public string Name;
        public SerializedSignature Signature;
    }
}
