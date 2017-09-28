using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerDriver
    {
        public void Decompile(string filename)
        {
            var sc = new ServiceContainer();
            var ldr = new Loader(sc);
            var reko = new DecompilerDriver(ldr, sc);
            reko.Decompile(filename);
        }
    }
}
