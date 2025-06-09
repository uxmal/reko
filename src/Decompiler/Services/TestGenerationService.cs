#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Services
{
    public class TestGenerationService : ITestGenerationService
    {
        private readonly IServiceProvider services;
        private readonly object decoderLock = new();
        private readonly object rewriterLock = new();
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
            if (outDir is null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureDecoderFile(fsSvc, filename);
            var instrBytes = ReadInstructionBytes(addrStart, rdr);
            lock (decoderLock)
            {
                if (this.emittedDecoderTests[filename].Contains(instrBytes))
                    return;
                this.emittedDecoderTests[filename].Add(instrBytes);
            }
            hexizer ??= Hexizer;
            var test = GenerateDecoderUnitTest(testPrefix, addrStart, hexizer(instrBytes), message);
            AttemptToAppendText(fsSvc, decoderLock, filename, test);
        }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, string message, string opcodeAsText)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir is null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureDecoderFile(fsSvc, filename);
            var instrBytes = Encoding.ASCII.GetBytes(opcodeAsText);
            lock (decoderLock)
            {
                if (this.emittedDecoderTests[filename].Contains(instrBytes))
                    return;
                this.emittedDecoderTests[filename].Add(instrBytes);
            }
            var test = GenerateDecoderUnitTest(testPrefix, addrStart, opcodeAsText, message);
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

        public void ReportMissingRewriter(
            string testPrefix,
            MachineInstruction instr,
            string mnemonic,
            EndianImageReader rdr,
            string message,
            Func<byte[], string>? hexizer = null)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir is null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureRewriterFile(fsSvc, filename);
            if (this.emittedRewriterTests[filename].Contains(mnemonic))
                return;
            emittedRewriterTests[filename].Add(mnemonic);
            hexizer ??= Hexizer;
            var test = GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, hexizer);
            AttemptToAppendText(fsSvc, rewriterLock, filename, test);
        }

        public void ReportMissingRewriter(
            string testPrefix,
            MachineInstruction instr,
            string mnemonic,
            EndianImageReader rdr,
            string message,
            string opcodeAsText)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir is null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureRewriterFile(fsSvc, filename);
            if (this.emittedRewriterTests[filename].Contains(mnemonic))
                return;
            emittedRewriterTests[filename].Add(mnemonic);
            var test = GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, opcodeAsText);
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
            return GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, hexbytes);
        }

        public static string GenerateRewriterUnitTest(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, string hexbytes)
        {
            var sb = new StringWriter();

            if (!string.IsNullOrEmpty(message))
            {
                sb.WriteLine("        // {0}", message);
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
            if (OutputDirectory is not null)
                return OutputDirectory;
            var dcSvc = this.services.GetService<IDecompilerService>();
            if (dcSvc is null)
                return null;
            if (dcSvc.Decompiler is null)
                return null;
            if (dcSvc.Decompiler.Project is null)
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
            var dir = GetOutputDirectory(fsSvc)!;
            foreach (var filename in fsSvc.GetFiles(dir, filePrefix + "*"))
            {
                fsSvc.DeleteFile(filename);
            }
        }

        public void ReportProcedure(string fileName, string testCaption, Procedure proc)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var dir = GetOutputDirectory(fsSvc)!;
            var absFileName = Path.Combine(dir, fileName);
            using var w = fsSvc.CreateStreamWriter(absFileName, true, Encoding.UTF8);
            ReportProcedure(testCaption, proc, w);
        }

        public static void ReportProcedure(string testCaption, Procedure proc, TextWriter writer)
        {
            writer.WriteLine(testCaption);
            proc.Write(false, writer);
            writer.WriteLine();
        }

        public void GenerateUnitTestFromProcedure(string fileName, string testCaption, Procedure proc)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var dir = GetOutputDirectory(fsSvc)!;
            var absFileName = Path.Combine(dir, fileName);
            using var w = fsSvc.CreateStreamWriter(absFileName, true, Encoding.UTF8);
            GenerateUnitTestFromProcedure(testCaption, proc, w);
        }

        public static void GenerateUnitTestFromProcedure(string testCaption, Procedure proc, TextWriter writer)
        {
            writer.WriteLine(testCaption);
            var mg = new MockGenerator(writer, "m.");
            mg.WriteMethod(proc);
            writer.WriteLine();
        }

        private class InstrBytesComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[]? x, byte[]? y)
            {
                if (x is null)
                    return y is null;
                if (y is null)
                    return false;
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
