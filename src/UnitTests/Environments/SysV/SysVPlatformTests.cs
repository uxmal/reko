#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Environments.SysV;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Reko.Core.Types;

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    public class SysVPlatformTests
    {
        [Test]
        public void SysV_TerminatingFunction()
        {
            var mr = new MockRepository();
            var sc = new ServiceContainer();
            var arch = mr.Stub<IProcessorArchitecture>();
            var tlSvc = mr.Stub<ITypeLibraryLoaderService>();
            var cfgSvc = mr.Stub<IConfigurationService>();
            sc.AddService<IConfigurationService>(cfgSvc);
            sc.AddService<ITypeLibraryLoaderService>(tlSvc);
            cfgSvc.Stub(c => c.GetInstallationRelativePath("libc.xml"))
                .Return("libc.xml");
            cfgSvc.Stub(c => c.GetEnvironment(null))
                .IgnoreArguments()
                .Return(new OperatingEnvironmentElement
                {
                     TypeLibraries = 
                     {
                         new TypeLibraryElement 
                         {
                              Name="libc.xml"
                         }
                     },
                     CharacteristicsLibraries =
                     {
                         new TypeLibraryElement
                         {
                             Name="libcharacteristics.xml",
                         }
                     }
                });
            tlSvc.Stub(t => t.LoadCharacteristics(null))
                .IgnoreArguments()
                .Return(new CharacteristicsLibrary
                {
                    Entries =
                    {
                        { 
                            "exit", 
                            new ProcedureCharacteristics {
                                Terminates = true
                            }
                        }
                    }
                });
            tlSvc.Stub(t => t.LoadMetadataIntoLibrary(null, null, null))
                .IgnoreArguments()
                .Return(new TypeLibrary
                {
                    Signatures =
                    {
                         {
                            "exit",
                            new FunctionType()
                         }
                     }
                });
            mr.ReplayAll();

            var sysv = new SysVPlatform(sc, arch);
            var proc = sysv.LookupProcedureByName(null, "exit");
            Assert.IsTrue(proc.Characteristics.Terminates, "exit should have been marked as terminating.");
        }
    }
}
