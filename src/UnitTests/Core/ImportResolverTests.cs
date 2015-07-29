#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class ImportResolverTests
    {
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IServiceProvider services;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.services = mr.Stub<IServiceProvider>();
        }

        [Test]
        public void Impres_ProcedureByName()
        {
            var proj = new Project
            {
                MetadataFiles = 
                {
                    new MetadataFile
                    {
                         ModuleName = "foo",
                         TypeLibrary = new TypeLibrary 
                         {
                              ServicesByName =
                              {
                                  {
                                      "bar@4", 
                                      new SystemService 
                                      { 
                                          Name = "bar", Signature= new ProcedureSignature()
                                      }
                                  }
                              }
                         }
                    }
                }
            };

            var impres = new ImportResolver(proj);
            var ep = impres.ResolveProcedure("foo", "bar@4", new DefaultPlatform(services, arch));
            Assert.AreEqual("bar", ep.Name);
        }

        [Test]
        public void Impres_ProcedureByOrdinal()
        {
            var proj = new Project
            {
                MetadataFiles = 
                {
                    new MetadataFile
                    {
                         ModuleName = "foo",
                         TypeLibrary = new TypeLibrary 
                         {
                              ServicesByVector =
                              {
                                  {
                                      9, 
                                      new SystemService 
                                      { 
                                          Name = "bar", Signature= new ProcedureSignature()
                                      }
                                  }
                              }
                         }
                    }
                }
            };

            var impres = new ImportResolver(proj);
            var ep = impres.ResolveProcedure("foo", 9, new DefaultPlatform(services, arch));
            Assert.AreEqual("bar", ep.Name);
        }
    }
}
