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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Reko.UnitTests.Mocks
{
    public class FakeDecompiledFileService : IDecompiledFileService
    {
        private StringWriter disassembly = new StringWriter();
        private StringWriter intermediate = new StringWriter();
        private StringWriter decompiled = new StringWriter();
        private StringWriter typesWriter = new StringWriter();
        private StringWriter globalsWriter = new StringWriter();
        private IConfigurationService config = new FakeDecompilerConfiguration();

        public FakeDecompiledFileService()
        {
        }

        public TextWriter CreateDecompiledCodeWriter(string file)
        {
            return decompiled;
        }

        public void WriteDisassembly(Program program, Action<string, Formatter> writer)
        {
            writer("test.asm", new TextFormatter(disassembly));
        }

        public void WriteIntermediateCode(Program program, Action<string, TextWriter> writer)
        {
            writer("test.dis", intermediate);
        }

        public void WriteTypes(Program program, Action<string, TextWriter> writer)
        {
            writer("test.h", typesWriter);
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<Procedure>, TextWriter> writer)
        {
            writer("test.c", new Procedure[0], decompiled);
        }

        public void WriteGlobals(Program program, Action<string, TextWriter> writer)
        {
            writer("test.globals.c", globalsWriter);
        }

        // probing methods.

        public StringWriter DisassemblyWriter
        {
            get { return disassembly; }
        }

        public StringWriter TypesWriter
        {
            get { return typesWriter; }
        }

        public StringWriter GlobalsWriter
        {
            get { return globalsWriter; }
        }

    }
}
