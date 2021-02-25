#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Reko.Core.Services
{
    /// <summary>
    /// Reko calls methods on this interface when it encounters errors. The methods generate unit tests
    /// that can be incorporated into the UnitTest project.
    /// </summary>
    public interface ITestGenerationService
    {
        /// <summary>
        /// Users can optionally override the output directory for more control 
        /// of where the output goes.
        /// </summary>
        string? OutputDirectory { get; set; }

        /// <summary>
        /// This method is called when an incomplete disassembler can't decode a byte sequence.
        /// </summary>
        /// <remarks>
        /// This must only be called on byte sequences that are known to be valid machine code, but
        /// haven't had a decoder written for them yet. Byte sequences that are known to be invalid
        /// machine code should never result in this method being called.
        /// </remarks>
        /// <param name="testPrefix">Prefix to use in the generated unit test.</param>
        /// <param name="addrStart">Address at which the undecoded byte sequence started.</param>
        /// <param name="rdr">Image reader positioned at the end of the byte sequence.</param>
        /// <param name="message">Optional message that will be emitted as a comment.</param>
        /// <param name="hexize">Optional function to convert raw bytes into text. By default, a hexadecimal string is 
        /// generated.</param>
        void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message, Func<byte[], string>? hexize = null);

        /// <summary>
        /// This method is called when an incomplete rewriter fails to rewrite a valid machine 
        /// instruction.
        /// </summary>
        /// <param name="testPrefix">Prefix to use in the generated unit test.</param>
        /// <param name="instr">The <see cref="MachineInstruction"/> that didn't get rewritten.</param>
        /// <param name="mnemonic">The mnemonic of the <see cref="MachineInstruction"/> that didn't get rewritten.</param>
        /// <param name="rdr">Image reader positioned after the end of the machine instruction.</param>
        /// <param name="message">Optional message that will be emitted as a comment.</param>
        /// <param name="hexize">Optional function to convert raw bytes into text. By default, a hexadecimal string is 
        /// generated.</param>
        void ReportMissingRewriter(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, Func<byte[], string>? hexize = null);

        /// <summary>
        /// Remove files starting with the given <paramref name="filePrefix"/> from the output directory.
        /// </summary>
        /// <param name="filePrefix"></param>
        void RemoveFiles(string filePrefix);

        /// <summary>
        /// Report the state of a procedure to a file determine by the filename.
        /// </summary>
        /// <param name="filePrefix"></param>
        /// <param name="testCaption"></param>
        /// <param name="proc"></param>
        void ReportProcedure(string fileName, string testCaption, Procedure proc);
    }

    public class TestGenerationService : ITestGenerationService
    {
        private readonly IServiceProvider services;
        private readonly object decoderLock = new object();
        private readonly object rewriterLock = new object();
        private readonly Dictionary<string, HashSet<byte[]>> emittedDecoderTests;
        private readonly Dictionary<string, HashSet<string>> emittedRewriterTests;

        public TestGenerationService(IServiceProvider services)
        {
            this.services = services;
            this.emittedRewriterTests = new Dictionary<string, HashSet<string>>();
            this.emittedDecoderTests = new Dictionary<string, HashSet<byte[]>>();
        }

        public string? OutputDirectory { get; set; }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message, Func<byte[], string>? hexizer)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir == null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureDecoderFile(fsSvc, filename);
            var instrBytes = ReadInstructionBytes(addrStart, rdr);
            if (this.emittedDecoderTests[filename].Contains(instrBytes))
                return;
            this.emittedDecoderTests[filename].Add(instrBytes);
            hexizer = hexizer ?? Hexizer;
            var test = GenerateDecoderUnitTest(testPrefix, addrStart, hexizer(instrBytes), message);
            AttemptToAppendText(fsSvc, decoderLock, filename, test);
        }

        private void AttemptToAppendText(IFileSystemService fsSvc, object lockObject, string filename, string text)
        {
            //$PERF: a better approach would be to queue up these requests for a worker to dispatch,
            // but it's overkill for this debugging facility.
            lock (lockObject)
            {
                fsSvc.AppendAllText(filename, text);
            }
        }

        private static string Hexizer(byte[] bytes) => string.Join("", bytes.Select(b => b.ToString("X2")));

        public void ReportMissingRewriter(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, Func<byte[], string>? hexizer = null)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir == null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureRewriterFile(fsSvc, filename);
            if (this.emittedRewriterTests[filename].Contains(mnemonic))
                return;
            emittedRewriterTests[filename].Add(mnemonic);
            hexizer = hexizer ?? Hexizer;
            var test = GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, hexizer);
            AttemptToAppendText(fsSvc, rewriterLock, filename, test);
        }

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for a disassembler.
        /// </summary>
        public static string GenerateDecoderUnitTest(string testPrefix, Address addrInstr, EndianImageReader rdr, string message, Func<byte[], string> hexizer)
        {
            byte[] bytes = ReadInstructionBytes(addrInstr, rdr);
            return GenerateDecoderUnitTest(testPrefix, addrInstr, hexizer(bytes), message);
        }

        public static string GenerateDecoderUnitTest(string testPrefix, Address addrInstr, string instrHexBytes, string message)
        {
            var writer = new StringWriter();
            writer.Write("// Reko: a decoder for the instruction {0} at address {1} has not been implemented.", instrHexBytes, addrInstr);
            if (!string.IsNullOrEmpty(message))
            {
                writer.Write(" ({0})", message);
            }
            writer.WriteLine();
            writer.WriteLine("[Test]");
            writer.WriteLine("public void {0}_{1}()", testPrefix, instrHexBytes);
            writer.WriteLine("{");
            writer.WriteLine("    AssertCode(\"@@@\", \"{0}\");", instrHexBytes);
            writer.WriteLine("}");

            return writer.ToString();
        }

        private static byte[] ReadInstructionBytes(Address addrInstr, EndianImageReader rdr)
        {
            var r2 = rdr.Clone();
            int len = (int) (r2.Address - addrInstr);
            r2.Offset -= len;
            var bytes = r2.ReadBytes(len);
            return bytes;
        }

        public static string GenerateRewriterUnitTest(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, Func<byte[], string> hexizer)
        {
            hexizer ??= Hexizer;
            byte[] bytes = ReadInstructionBytes(instr.Address!, rdr);
            var hexbytes = hexizer(bytes);
            var sb = new StringWriter();

            if (!string.IsNullOrEmpty(message))
            {
                sb.WriteLine($"        // {0}", message);
            }
            sb.WriteLine("        [Test]");
            sb.WriteLine("        public void {0}_{1}()", testPrefix, mnemonic);
            sb.WriteLine("        {");
            sb.WriteLine("            Given_HexString(\"{0}\");", hexbytes);
            sb.WriteLine("            AssertCode(     // {0}", instr);
            sb.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", instr.Address, instr.Length);
            sb.WriteLine("                \"1|L--|@@@\");");
            sb.WriteLine("        }");
            sb.WriteLine();
            return sb.ToString();
        }

        private void EnsureDecoderFile(IFileSystemService fsSvc, string filename)
        {
            lock (this.decoderLock)
            {
                if (!emittedDecoderTests.ContainsKey(filename))
                {
                    emittedDecoderTests.Add(filename, new HashSet<byte[]>(new InstrBytesComparer()));
                    var header = string.Join(Environment.NewLine,
                        "// This file contains unit tests automatically generated by Reko decompiler.",
                        "// Please copy the contents of this file and report it on GitHub, using the ",
                        "// following URL: https://github.com/uxmal/reko/issues",
                        "",
                        "");
                    fsSvc.WriteAllText(filename, header);
                }
            }
        }

        private void EnsureRewriterFile(IFileSystemService fsSvc, string filename)
        {
            //$PERF: //$SHARED_STATE: this will make performance terrible,
            // but ideally there should be no errors in the rewriters and disassemblers
            lock (this.rewriterLock)
            {
                if (!emittedRewriterTests.ContainsKey(filename))
                {
                    emittedRewriterTests.Add(filename, new HashSet<string>());
                    var header = string.Join(Environment.NewLine,
                        "// This file contains unit tests automatically generated by Reko decompiler.",
                        "// Please copy the contents of this file and report it on GitHub, using the ",
                        "// following URL: https://github.com/uxmal/reko/issues",
                        "",
                        "");
                    fsSvc.WriteAllText(filename, header);
                }
            }
        }

        private string? GetOutputDirectory(IFileSystemService fsSvc)
        {
            if (OutputDirectory != null)
                return OutputDirectory;
            var dcSvc = this.services.GetService<IDecompilerService>();
            if (dcSvc == null)
                return null;
            if (dcSvc.Decompiler == null)
                return null;
            if (dcSvc.Decompiler.Project == null)
                return null;
            if (dcSvc.Decompiler.Project.Programs.Count == 0)
                return null;
            var outDir = dcSvc.Decompiler.Project.Programs[0].DisassemblyDirectory;
            if (string.IsNullOrEmpty(outDir))
                return null;
            try
            {
                fsSvc.CreateDirectory(outDir);
                return outDir;
            }
            catch
            {
                return null;
            }
        }

        public void RemoveFiles(string filePrefix)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var dir = GetOutputDirectory(fsSvc);
            foreach (var filename in fsSvc.GetFiles(dir, filePrefix + "*"))
            {
                fsSvc.DeleteFile(filename);
            }
        }

        public void ReportProcedure(string fileName, string testCaption, Procedure proc)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var dir = GetOutputDirectory(fsSvc);
            var absFileName = Path.Combine(dir, fileName);
            using var w = fsSvc.CreateStreamWriter(absFileName, true, Encoding.UTF8);
            w.WriteLine(testCaption);
            proc.Write(false, w);
            w.WriteLine();
        }

        private class InstrBytesComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] x, byte[] y)
            {
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; ++i)
                {
                    if (x[i] != y[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(byte[] obj)
            {
                int h = obj.Length;
                for (int i = 0; i < obj.Length; ++i)
                {
                    h = h * 23 ^ obj[i];
                }
                return h;
            }
        }
    }
}
