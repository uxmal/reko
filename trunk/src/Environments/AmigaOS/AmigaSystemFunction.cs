using Decompiler.Core;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.AmigaOS
{
    public class AmigaSystemFunction
    {
        public int Offset;
        public string Name;
        public SerializedSignature Signature;
    }
}
