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
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch
{
    public class UnitTestGenerationService : ITestGenerationService
    {
        private readonly IServiceProvider services;

        public UnitTestGenerationService(IServiceProvider services)
        {
            this.services = services;
        }

        public string OutputDirectory { get; set; }

        public void RemoveFiles(string filePrefix)
        {
        }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message, Func<byte[], string> hexizer)
        {
            var test = TestGenerationService.GenerateDecoderUnitTest(testPrefix, addrStart, rdr, message, hexizer ?? Hexizer);
            Console.WriteLine(test);
        }

        public void ReportMissingDecoder(string testPrefix, Address addrStart, string message, string opcodeAsText)
        {
            var test = TestGenerationService.GenerateDecoderUnitTest(testPrefix, addrStart, message, opcodeAsText);
            Console.WriteLine(test);
        }

        public void ReportMissingRewriter(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, Func<byte[], string> hexizer)
        {
            var test = TestGenerationService.GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, hexizer ?? Hexizer);
            Console.WriteLine(test);
        }

        public void ReportMissingRewriter(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, string opcodeAsText)
        {
            var test = TestGenerationService.GenerateRewriterUnitTest(testPrefix, instr, mnemonic, rdr, message, opcodeAsText);
            Console.WriteLine(test);
        }


        private static string Hexizer(byte[] bytes)
        {
            return string.Join("",bytes.Select(b => $"{b:X2}"));
        }

        public void ReportProcedure(string fileName, string testCaption, Procedure proc)
        {
            Console.WriteLine("== {0} ===============", fileName);
            TestGenerationService.ReportProcedure(testCaption, proc, Console.Out);
        }

        public void GenerateUnitTestFromProcedure(string fileName, string testCaption, Procedure proc)
        {
            Console.WriteLine("== {0} ===============", fileName);
            TestGenerationService.GenerateUnitTestFromProcedure(testCaption, proc, Console.Out);
        }
    }
}