using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.OdbgScript
{
    public class Librarian
    {
        public class fLibraryBreakPointCallback
        {

        }

        public class LIBRARY_ITEM_DATA
        {
            public string? szLibraryPath;

        }

        internal static LIBRARY_ITEM_DATA GetLibraryInfoEx(object p)
        {
            throw new NotImplementedException();
        }
    }
}
