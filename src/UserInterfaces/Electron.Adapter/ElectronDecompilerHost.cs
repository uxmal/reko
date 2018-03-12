using Reko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Output;
using System.IO;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerHost : DecompilerHost
    {
        public IConfigurationService Configuration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void WriteDecompiledCode(Program program, Action<TextWriter> writer)
        {
            //$TODO: write output file
        }

        public void WriteDisassembly(Program program, Action<Formatter> writer)
        {
            //$TODO: write output file
        }

        public void WriteGlobals(Program program, Action<TextWriter> writer)
        {
            //$TODO: write output file
        }

        public void WriteIntermediateCode(Program program, Action<TextWriter> writer)
        {
            //$TODO: write output file
        }

        public void WriteTypes(Program program, Action<TextWriter> writer)
        {
            //$TODO: write output file
        }
    }
}
