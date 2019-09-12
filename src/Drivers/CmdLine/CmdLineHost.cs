#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.IO;
using System.Text;

namespace Reko.CmdLine
{
    public class CmdLineHost : DecompilerHost
    {
        public CmdLineHost()
        {
        }

        public virtual TextWriter CreateTextWriter(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
            var dir = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(dir);
            return new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), new UTF8Encoding(false));
        }

        public void WriteDisassembly(Program program, Action<Formatter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.DisassemblyFilename))
            {
                writer(new TextFormatter(output));
            }
        }

        public void WriteIntermediateCode(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.IntermediateFilename))
            {
                writer(output);
            }
        }

        public void WriteTypes(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.TypesFilename))
            {
                writer(output);
            }
        }

        public void WriteDecompiledCode(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.OutputFilename))
            {
                writer(output);
            }
        }

        public void WriteGlobals(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.GlobalsFilename))
            {
                writer(output);
            }
        }

        public IConfigurationService Configuration
        {
            get { throw new NotImplementedException(); }
        }
    }
}
