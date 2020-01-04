#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using NUnit.Framework;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Item = Reko.Core.Lib.LinqAlgorithms.Item;
using Link = Reko.Core.Lib.LinqAlgorithms.Link;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class LinqAlgorithmTests
    {
        [Test]
        public void LinqWcc()
        {
            var items = 
                Enumerable.Range(1, 16)
                .ToDictionary(k => (long)k, v => new Item { id = v, component_id = v });
            var links =
                Enumerable.Range(1, 7)
                .Select(n => new Link { first = n, second = n + 1 })
                .Concat(
                    Enumerable.Range(9, 7)
                    .Select(n => new Link { first = n, second = n + 1 }));

            LinqAlgorithms.scc(items, links);
        }

        [Test]
        public void LinqWcc2()
        {
            var items =
                Enumerable.Range(1, 8)
                .ToDictionary(k => (long)k, v => new Item { id = v, component_id = v });

            var links = new List<Link>
            {
                new Link { first = 4, second = 5 },
                new Link { first = 5, second = 6 },
                new Link { first = 1, second = 2 },
                new Link { first = 2, second = 4 },
                new Link { first = 3, second = 4 },
            };


            LinqAlgorithms.scc(items, links);
        }
    }
    /*
    */
}
