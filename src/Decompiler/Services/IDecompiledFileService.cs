#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Loading;

namespace Reko.Services
{
    /// <summary>
    /// Interface used by the decompiler's components to write outputs.
    /// </summary>
    public interface IDecompiledFileService
    {
        /// <summary>
        /// Creates a <see cref="TextWriter"/> given an absolute filename, creating
        /// any intermediary directories if necessary. If the file already exists, 
        /// it is overwritten.
        /// </summary>
        /// <param name="filename">Absolute path to the file being created.</param>
        /// <returns>A <see cref="TextWriter"/> to the created file.
        /// </returns>
        TextWriter CreateTextWriter(string filename);
        void WriteDisassembly(Program program, Action<string, Dictionary<ImageSegment, List<ImageMapItem>>, Formatter> writer);
        void WriteIntermediateCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer);
        void WriteTypes(Program program, Action<string, TextWriter> writer);
        void WriteDecompiledCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer);
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

        public TextWriter CreateTextWriter(string path)
        {
            return TextWriter.Null;
        }

        public void WriteDisassembly(Program program, Action<string, Dictionary<ImageSegment, List<ImageMapItem>>, Formatter> writer)
        {
            writer("", new Dictionary<ImageSegment, List<ImageMapItem>>(), new NullFormatter());
        }

        public void WriteIntermediateCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
        {
            writer("", program.Procedures.Values, TextWriter.Null);
        }

        public void WriteTypes(Program program, Action<string,TextWriter> writer)
        {
            writer("", TextWriter.Null);
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
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
        private readonly IServiceProvider services;
        private readonly IFileSystemService fsSvc;
        private readonly IDecompilerEventListener listener;

        public DecompiledFileService(IServiceProvider services, IFileSystemService fsSvc, IDecompilerEventListener listener)
        {
            this.services = services;
            this.fsSvc = fsSvc;
            this.listener = listener;
        }

        public TextWriter CreateTextWriter(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
            var dir = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(dir))
                fsSvc.CreateDirectory(dir);
            return new StreamWriter(fsSvc.CreateFileStream(filename, FileMode.Create, FileAccess.Write), new UTF8Encoding(false));
        }

        public void WriteDisassembly(Program program, Action<string, Dictionary<ImageSegment, List<ImageMapItem>>, Formatter> writer)
        {
            var outputPolicy = OutputFilePolicy.CreateOutputPolicy(this.services, program, program.User.OutputFilePolicy);
            foreach (var placement in outputPolicy.GetItemPlacements(".asm"))
            {
                var dasmFilename = Path.GetFileName(placement.Key);
                var dasmPath = Path.Combine(program.DisassemblyDirectory, dasmFilename);
                using (TextWriter output = CreateTextWriter(dasmPath))
                {
                    writer(dasmFilename, placement.Value, new TextFormatter(output));
                }
            }
        }

        public void WriteIntermediateCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
        {
            var outputPolicy = OutputFilePolicy.CreateOutputPolicy(this.services, program, program.User.OutputFilePolicy);
            foreach (var placement in outputPolicy.GetObjectPlacements(".dis", listener))
            {
                var irFilename = Path.GetFileName(placement.Key);
                var irPath = Path.Combine(program.SourceDirectory, irFilename);
                var procs = placement.Value.Values.OfType<Procedure>().ToArray();
                if (procs.Length > 0)
                {
                    using TextWriter output = CreateTextWriter(irPath);
                    writer(irFilename, procs, output);
                }
            }
        }

        public void WriteTypes(Program program, Action<string, TextWriter> writer)
        {
            var incFilename = GenerateDerivedFilename(program, ".h");
            var incPath = Path.Combine(program.IncludeDirectory, incFilename);
            using (TextWriter output = CreateTextWriter(incPath))
            {
                writer(incFilename, output);
            }
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
        {
            var outputPolicy = OutputFilePolicy.CreateOutputPolicy(this.services, program, program.User.OutputFilePolicy);
            foreach (var placement in outputPolicy.GetObjectPlacements(".c", listener))
            {
                var filename = placement.Key;
                var filePath = Path.Combine(program.SourceDirectory, filename);
                using TextWriter output = CreateTextWriter(filePath);
                writer(filename, placement.Value.Values, output);
            }
        }

        public void WriteGlobals(Program program, Action<string, TextWriter> writer)
        {
            var globalsFilename = GenerateDerivedFilename(program, "globals.c");
            var globalsPath = Path.Combine(program.SourceDirectory, globalsFilename);
            using (TextWriter output = CreateTextWriter(globalsPath))
            {
                writer(globalsFilename, output);
            }
        }

        public static string GenerateDerivedFilename(Program program, string newExtension)
        {
            return Path.ChangeExtension(program.Location.GetFilename(), newExtension);
        }
    }
}
