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
using System.Text;

namespace Reko.Core.Services
{
    public interface ITestGenerationService
    {
        /// <summary>
        /// The directory in which the output files are stored.
        /// </summary>
        string TestOutputDirectory { get; set; }

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
        private readonly Dictionary<string, HashSet<int>> emittedTests;

        public TestGenerationService(IServiceProvider services)
        {
            this.services = services;
            this.emittedTests = new Dictionary<string, HashSet<int>>();
        }

        public string TestOutputDirectory { get; set; }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var filename = Path.Combine(TestOutputDirectory, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureFile(fsSvc, filename);
            fsSvc.AppendAllText(filename, "");
        }

        public void ReportMissingRewriter(string testPrefix, MachineInstruction instr, EndianImageReader rdr, string message)
        {
            var fsSvc = services.RequireService<IFileSystemService>();
            var filename = Path.Combine(TestOutputDirectory, Path.ChangeExtension(testPrefix, ".tests"));
            EnsureFile(fsSvc, filename);
            if (this.emittedTests[filename].Contains(instr.MnemonicAsInteger))
                return;
            emittedTests[filename].Add(instr.MnemonicAsInteger);
        }

        public static string GenerateRewriterUnitTest(string testPrefix, MachineInstruction instr, EndianImageReader rdr, string message)
        {
            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendLine($"        // {message}");
            }
            sb.AppendLine($"        [Test]");
            sb.AppendLine($"        public void {testPrefix}Rw_{instr.MnemonicAsString}()");
            sb.AppendLine("        {");

            if (instr.Length > 2)
            {
                var wInstrHi = r2.ReadUInt16();
                var wInstrLo = r2.ReadUInt16();
                uint wInstr = (((uint) wInstrHi) << 16) | wInstrLo;
                sb.AppendLine($"            RewriteCode(\"{wInstr:X8}\"); // {instr}");
            }
            else
            {
                var wInstr = r2.ReadUInt16();
                sb.AppendLine($"            RewriteCode(\"{wInstr:X4}\"); // {instr}");
            }
            sb.AppendLine("            AssertCode(");
            sb.AppendLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            sb.AppendLine($"                \"1|L--|@@@\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            return sb.ToString();
        }

        private void EnsureFile(IFileSystemService fsSvc, string filename)
        {
            if (!emittedTests.ContainsKey(filename))
            {
                emittedTests.Add(filename, new HashSet<int>());
                var header = string.Join(Environment.NewLine,
                    "// This file contains unit tests automatically generated by Reko decompiler.",
                    "// Please copy the contents of this file and report it on GitHub, using the ",
                    "// following URL: https://github.com/uxmal/reko/issues",
                    "");
                fsSvc.WriteAllText(filename, header);
            }
        }

    }
}
