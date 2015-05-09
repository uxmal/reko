#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using Decompiler.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class AddressSearchResultTests
    {
        private MockRepository mr;
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            var image = new LoadedImage(Address.SegPtr(0xC00, 0), Enumerable.Range(0x0, 0x100).Select(b => (byte)b).ToArray());
            var imageMap = image.CreateImageMap();
            var arch = new Mocks.FakeArchitecture();
            this.program = new Program(image, imageMap, arch, new DefaultPlatform(sc, arch));
        }

        [Test]
        public void Asr_Create()
        {
            var results = new AddressSearchResult(sc, new List<AddressSearchHit>());
        }
    }

    /// SearchService - provides generic search services.
    ///  StartSearch(newsearcher, searchResult)
    ///  {
    ///      if searchWorker.Active()
    ///          stoplistening(searcher)
    ///          searchWorker.Deactivate()
    ///      clear.searchResults()
    ///      startlistenning(newsearcher);
    ///      searcher = searcher.Observe()
    ///      searchWorker.Start(searcher)
    ///  }
    ///  
    ///  public searcher
    ///  {
    ///     Notify() {
    ///         foreach listener() {
    ///             listener.OnNext();
    ///         }
    ///     }
    ///  }
    ///    

}