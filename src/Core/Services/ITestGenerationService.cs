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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core.Services
{
    /// <summary>
    /// Reko calls methods on this interface when it encounters errors. The methods generate unit tests
    /// that can be incorporated into the UnitTest projct.
    /// </summary>
    public interface ITestGenerationService
    {
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
        void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message);

        /// <summary>
        /// This method is called when an incomplete rewriter fails to rewrite a valid machine 
        /// instruction.
        /// </summary>
        /// <param name="testPrefix">Prefix to use in the generated unit test.</param>
        /// <param name="instr">The <see cref="MachineInstruction"/> that didn't get rewritten.</param>
        /// <param name="rdr">Image reader positioned after the end of the machine instruction.</param>
        /// <param name="message">Optional message that will be emitted as a comment.</param>
        void ReportMissingRewriter(string testPrefix, MachineInstruction instr, EndianImageReader rdr, string message);
    }

    public class TestGenerationService : ITestGenerationService
    {
        private readonly IServiceProvider services;
        private readonly HashSet<string> emittedDecoderTests;
        private readonly Dictionary<string, HashSet<int>> emittedRewriterTests;

        public TestGenerationService(IServiceProvider services)
        {
            this.services = services;
            this.emittedRewriterTests = new Dictionary<string, HashSet<int>>();
            this.emittedDecoderTests = new HashSet<string>();
        }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir == null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureDecoderFile(fsSvc, filename);
            var test = GenerateDecoderUnitTest(testPrefix, addrStart, rdr, message);
            fsSvc.AppendAllText(filename, test);
        }

        public void ReportMissingRewriter(string testPrefix, MachineInstruction instr, EndianImageReader rdr, string message)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var outDir = GetOutputDirectory(fsSvc);
            if (outDir == null)
                return;
            var filename = Path.Combine(outDir, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureRewriterFile(fsSvc, filename);
            if (this.emittedRewriterTests[filename].Contains(instr.MnemonicAsInteger))
                return;
            emittedRewriterTests[filename].Add(instr.MnemonicAsInteger);
            var test = GenerateRewriterUnitTest(testPrefix, instr, rdr, message);
            fsSvc.AppendAllText(filename, test);
        }

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for a disassembler.
        /// </summary>
        public static string GenerateDecoderUnitTest(string testPrefix, Address addrInstr, EndianImageReader rdr, string message)
        {
            var writer = new StringWriter();
            var r2 = rdr.Clone();
            int len = (int) (r2.Address - addrInstr);
            r2.Offset -= len;
            var bytes = r2.ReadBytes(len);
            var instrHexBytes = string.Join("", bytes.Select(b => b.ToString("X2")));
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

        public static string GenerateRewriterUnitTest(string testPrefix, MachineInstruction instr, EndianImageReader rdr, string message)
        {
            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;
            var bytes = r2.ReadBytes(instr.Length);

            var sb = new StringWriter();

            if (!string.IsNullOrEmpty(message))
            {
                sb.WriteLine($"        // {0}", message);
            }
            sb.WriteLine("        [Test]");
            sb.WriteLine("        public void {0}_{1}()", testPrefix, instr.MnemonicAsString);
            sb.WriteLine("        {");
            sb.WriteLine("            Given_HexString(\"{0}\");", string.Join("", bytes.Select(b => b.ToString("X2"))));
            sb.WriteLine("            AssertCode(     // {0}", instr);
            sb.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", instr.Address, instr.Length);
            sb.WriteLine("                \"1|L--|@@@\");");
            sb.WriteLine("        }");
            sb.WriteLine();
            return sb.ToString();
        }

        private void EnsureDecoderFile(IFileSystemService fsSvc, string filename)
        {
            if (!emittedDecoderTests.Contains(filename))
            {
                emittedDecoderTests.Add(filename);
                var header = string.Join(Environment.NewLine,
                    "// This file contains unit tests automatically generated by Reko decompiler.",
                    "// Please copy the contents of this file and report it on GitHub, using the ",
                    "// following URL: https://github.com/uxmal/reko/issues",
                    "",
                    "");
                fsSvc.WriteAllText(filename, header);
            }
        }

        private void EnsureRewriterFile(IFileSystemService fsSvc, string filename)
        {
            if (!emittedRewriterTests.ContainsKey(filename))
            {
                emittedRewriterTests.Add(filename, new HashSet<int>());
                var header = string.Join(Environment.NewLine,
                    "// This file contains unit tests automatically generated by Reko decompiler.",
                    "// Please copy the contents of this file and report it on GitHub, using the ",
                    "// following URL: https://github.com/uxmal/reko/issues",
                    "",
                    "");
                fsSvc.WriteAllText(filename, header);
            }
        }

        private string GetOutputDirectory(IFileSystemService fsSvc)
        {
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
    }
}
