using Reko.Core;
using Reko.Core.Hll.C;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Environments.OS2
{
    /// <summary>
    /// <see cref="Platform"/> class representing 32-bit OS/2 versions 2.0 and up. 
    /// </summary>
    /// <remarks>
    /// Yes, the "Warp" may be a bit of a misnomer.
    /// </remarks>
    public class OS2Platform32 : Platform
    {
        public OS2Platform32(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "os2-32")
        {
        }

        public override string DefaultCallingConvention => "__cdecl";

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
