using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public record VarargsResult(
            FunctionType Signature,
            Address FormatStringAddress,
            StringConstant FormatString);
}
