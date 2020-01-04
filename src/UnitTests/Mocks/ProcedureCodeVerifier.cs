#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using System;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Mocks
{
    public static class ProcedureCodeVerifier
    {
        public static void AssertCode(Procedure proc, string expected)
        {
            var writer = new StringWriter();
            var procWriter = new ProcedureStatementsWriter();
            procWriter.WriteProcedure(proc, writer);
            var actual = writer.ToString();
            if (expected != actual)
            {
                Console.WriteLine(actual);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
