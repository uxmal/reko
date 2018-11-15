using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class LinearScanner
    {
        private Program program;
        private ISet<Address> symbols;

        public LinearScanner(Program program, ISet<Address> symbols)
        {
            this.program = program;
            this.symbols = symbols;
        }

        //$TODO: islands.
        public void Scan()
        {
            var items = (program.ImageMap.Items.Values.Where(i => i.DataType != null && i.DataType is UnknownType));
            {

            }
        }
    }
}
