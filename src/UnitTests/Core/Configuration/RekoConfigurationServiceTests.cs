#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Configuration
{
    [TestFixture]
    public class RekoConfigurationServiceTests
    {
        [Test]
        public void Rcfg_LoadOperatingEnvironment()
        {
            var cfgSvc = new RekoConfigurationService(new RekoConfiguration_v1
            {
                Environments = new []
                {
                    new Environment_v1
                    {
                        Name = "testOS",
                        Description = "Test OS",
                        Heuristics = new PlatformHeuristics_v1
                        {
                            ProcedurePrologs = new []
                            {
                                new BytePattern_v1
                                {
                                    Bytes = "55 8B EC",
                                    Mask = "FF FF FF",
                                },
                                new BytePattern_v1
                                {
                                    Bytes= "55 ?? 30"
                                }
                            }
                        },
                        Architectures = new[]
                        {
                            new PlatformArchitecture_v1
                            {
                                Name="testCPU",
                                 TypeLibraries = new[]
                                 {
                                     new TypeLibraryReference_v1
                                     {
                                         Name = "lp32.xml",
                                     }
                                 }
                            }
                        }
                    }
                }
            });

            var env = cfgSvc.GetEnvironment("testOS");
            Assert.AreEqual(2, env.Heuristics.ProcedurePrologs.Length);
            var pattern0 = env.Heuristics.ProcedurePrologs[0];
            Assert.AreEqual("55 8B EC", pattern0.Bytes);
            Assert.AreEqual("FF FF FF", pattern0.Mask);

            var pattern1 = env.Heuristics.ProcedurePrologs[1];
            Assert.AreEqual("55 ?? 30", pattern1.Bytes);
            Assert.IsNull(pattern1.Mask);

            var archs = env.Architectures;
            Assert.AreEqual(1, archs.Count);
            Assert.AreEqual("lp32.xml", archs[0].TypeLibraries[0].Name);
        }
    }
}
