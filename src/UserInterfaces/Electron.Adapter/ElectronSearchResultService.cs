using System;
using System.Collections.Generic;
using Reko.Core;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronSearchResultService : ISearchResultService
    {
        private dynamic searchResultChannel;

        public ElectronSearchResultService(object searchResultChannel)
        {
            this.searchResultChannel = searchResultChannel;
        }

        public void ShowAddressSearchResults(IEnumerable<ProgramAddress> hits, AddressSearchDetails code)
        {
            throw new NotImplementedException();
        }

        public void ShowSearchResults(ISearchResult result)
        {
            throw new NotImplementedException();
        }
    }
}