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

using Reko.Core.Configuration;
using Reko.Core;
using System;
using System.IO;
using System.Threading;
using Reko.Core.Output;
using Reko.Core.Services;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Reko
{
    /// <summary>
    /// Interface used by the decompiler's components to write outputs.
    /// </summary>
    public interface IDecompiledFileService
    {
        void WriteDisassembly(Program program, Action<string, Formatter> writer);
        void WriteIntermediateCode(Program program, Action<string, TextWriter> writer);
        void WriteTypes(Program program, Action<string, TextWriter> writer);
        void WriteDecompiledCode(Program program, Action<string, IEnumerable<Procedure>, TextWriter> writer);
        void WriteGlobals(Program program, Action<string, TextWriter> writer);
    }

	/// <summary>
	/// Used when no actual output is required.
	/// </summary>
	public class NullDecompiledFileService : IDecompiledFileService
	{
        public static readonly IDecompiledFileService Instance = new NullDecompiledFileService();

        public NullDecompiledFileService()
        {
        }

		#region DecompilerHost Members

        public void WriteDisassembly(Program program, Action<string, Formatter> writer)
        {
            writer("",new NullFormatter());
        }

        public void WriteIntermediateCode(Program program, Action<string, TextWriter> writer)
        {
            writer("", TextWriter.Null);
        }

        public void WriteTypes(Program program, Action<string,TextWriter> writer)
        {
            writer("", TextWriter.Null);
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<Procedure>, TextWriter> writer)
        {
            writer("", program.Procedures.Values, TextWriter.Null);
        }

        public void WriteGlobals(Program program, Action<string, TextWriter> writer)
        {
            writer("", TextWriter.Null);
        }

        #endregion
    }

    public class DecompiledFileService : IDecompiledFileService
    {
        private readonly IFileSystemService fsSvc;

        public DecompiledFileService(IFileSystemService fsSvc)
        {
            this.fsSvc = fsSvc;
        }

        private TextWriter CreateTextWriter(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
            var dir = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(dir))
                fsSvc.CreateDirectory(dir);
            return new StreamWriter(fsSvc.CreateFileStream(filename, FileMode.Create, FileAccess.Write), new UTF8Encoding(false));
        }

        public void WriteDisassembly(Program program, Action<string, Formatter> writer)
        {
            var dasmFilename = Path.ChangeExtension(Path.GetFileName(program.Filename), ".asm");
            var dasmPath = Path.Combine(program.DisassemblyDirectory, dasmFilename);
            using (TextWriter output = CreateTextWriter(dasmPath))
            {
                writer(dasmFilename, new TextFormatter(output));
            }
        }

        public void WriteIntermediateCode(Program program, Action<string, TextWriter> writer)
        {
            var irFilename = Path.ChangeExtension(Path.GetFileName(program.Filename), ".dis");
            var irPath = Path.Combine(program.SourceDirectory, irFilename);
            using (TextWriter output = CreateTextWriter(irPath))
            {
                writer(irFilename, output);
            }
        }

        public void WriteTypes(Program program, Action<string, TextWriter> writer)
        {
            var incFilename = Path.ChangeExtension(Path.GetFileName(program.Filename), ".h");
            var incPath = Path.Combine(program.IncludeDirectory, incFilename);
            using (TextWriter output = CreateTextWriter(incPath))
            {
                writer(incFilename, output);
            }
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<Procedure>, TextWriter> writer)
        {
            var defaultFileName = Path.ChangeExtension(Path.GetFileName(program.Filename), ".c");
            var defaultPath = Path.Combine(program.SourceDirectory, defaultFileName);
            var procFiles =
               (from proc in program.Procedures.Values
                join file in program.User.ProcedureSourceFiles on proc.EntryAddress equals file.Key into files
                from file in files.DefaultIfEmpty()
                select new { file = file.Value ?? defaultPath, proc } into files
                group files by files.file into g
                select new {
                    file = g.Key,
                    procs = g
                        .Select(gg => gg.proc)
                        .OrderBy(gg => gg.EntryAddress)
                }).ToList();

            foreach (var item in procFiles)
            {
                using (TextWriter output = CreateTextWriter(item.file))
                {
                    writer(defaultFileName, item.procs, output);
                }
            }
        }

        public void WriteGlobals(Program program, Action<string, TextWriter> writer)
        {
            var globalsFilename = Path.ChangeExtension(Path.GetFileName(program.Filename), "globals.c");
            var globalsPath = Path.Combine(program.SourceDirectory, globalsFilename);
            using (TextWriter output = CreateTextWriter(globalsPath))
            {
                writer(globalsFilename, output);
            }
        }
    }
}
