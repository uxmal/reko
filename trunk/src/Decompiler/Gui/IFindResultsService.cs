using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// Implementors expose a service that allow clients to display results of a search.
    /// </summary>
    interface IFindResultsService
    {
        void ShowResults(IList<FindResult> results);
    }
}
