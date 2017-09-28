using Reko.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDecompilerDriver
    {
	    public void Hello(object foo) {
		    MessageBox.Show("Hello, i'm Reko");
	    }

        public void Decompile(string filename)
        {
		    var sc = new ServiceContainer();
		    var ldr = new Loader(sc);
		    var reko = new DecompilerDriver(ldr, sc);
		    reko.Decompile(filename);
        }
    }
}
