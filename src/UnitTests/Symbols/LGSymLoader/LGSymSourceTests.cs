#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core;
using Reko.Symbols.LGSymLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Symbols.LGSymLoader
{
    [TestFixture]
    public class LGSymSourceTests
    {
        private MemoryStream stream;
        private LGSymSource symSource;

        [Test]
        public void LGSym_Load()
        {
            MakeLeFile(
                LGSymSource.SYM_MAGIC,
                0u,
                (1u * 12) + 4u + 13u,
                1u,
                13u + 4u,

                0x00123400u,
                0x00123458u,
                0u,

                0u,

                "My_procedure\0");
            When_CreateLGSymSource();
            var worked = symSource.CanLoad("foo.sym");
            Assert.IsTrue(worked);

            var syms = symSource.GetAllSymbols();
            Assert.AreEqual(1, syms.Count);
            var sym = syms[0];
            Assert.AreEqual("My_procedure", sym.Name);
            Assert.AreEqual(Address.Ptr32(0x00123400), sym.Address);
            Assert.AreEqual(0x58, sym.Size);
            Assert.AreEqual(SymbolType.Unknown, sym.Type);
        }

        private void When_CreateLGSymSource()
        {
            this.symSource = new LGSymSource(this.stream);
        }

        private void MakeLeFile(params object[] args)
        {
            var writer = new LeImageWriter();
            foreach (var arg in args)
            {
                if (arg is uint)
                {
                    writer.WriteLeUInt32((uint)arg);
                }
                else if (arg is string)
                {
                    writer.WriteString((string)arg, Encoding.UTF8);
                }
            }
            var image = writer.ToArray();
            this.stream = new MemoryStream(image);
        }
    }
}
