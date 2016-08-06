using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Builds a function application from a call site and a callee, with the assumption that the last
    /// parameter, whose name is '...', is the start of the varargs part. The preceding parameter must therefore
    /// be the format string. If this string is fed with a constant integer, we walk the memory at that address
    /// to see if we can use the format string.
    /// </summary>
    public class PrintfApplicationBuilder : ApplicationBuilder
    {
        public PrintfApplicationBuilder(
            IProcessorArchitecture arch, Frame frame, CallSite site, Expression callee, FunctionType sigCallee, bool ensureVariables) :
                    base(arch, frame, site, callee, sigCallee, ensureVariables)
        {

        }

        public override List<Expression> BindArguments(Frame frame, FunctionType sigCallee)
        {
            var actuals = new List<Expression>();
            int i;
            for (i = 0; i < sigCallee.Parameters.Length-1; ++i)
            {
                var formalArg = sigCallee.Parameters[i];
                var actualArg = formalArg.Storage.Accept(this);
                if (formalArg.Storage is OutArgumentStorage)
                {
                    actuals.Add(new OutArgument(frame.FramePointer.DataType, actualArg));
                }
                else
                {
                    actuals.Add(actualArg);
                }
            }
            return actuals;
        }
    }
}
