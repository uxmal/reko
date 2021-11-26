#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Scripts.Python;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Scripts.Python
{
    [TestFixture]
    public class PythonModuleTests
    {
        private ServiceContainer services;
        private StringWriter outputWriter;
        private Dictionary<string, string> filesContents;

        [SetUp]
        public void Setup()
        {
            this.services = new ServiceContainer();
            var eventListener = new FakeDecompilerEventListener();
            services.AddService<DecompilerEventListener>(eventListener);
            var cfgSvcMock = new Mock<IConfigurationService>();
            cfgSvcMock
                .Setup(
                    c => c.GetInstallationRelativePath(It.IsAny<string[]>()))
                .Returns<string[]>(path => string.Join('/', path));
            services.AddService<IConfigurationService>(cfgSvcMock.Object);
            var fsSvcMock = new Mock<IFileSystemService>();
            services.AddService<IFileSystemService>(fsSvcMock.Object);
            fsSvcMock
                .Setup(f => f.ReadAllBytes(It.IsAny<string>()))
                .Returns<string>(
                    fileName => Encoding.Default.GetBytes(
                        filesContents[fileName]));
            var outputSvcMock = new Mock<IOutputService>();
            this.outputWriter = new StringWriter();
            outputSvcMock
                .Setup(o => o.EnsureOutputSource("Scripting"))
                .Returns(outputWriter);
            services.AddService<IOutputService>(outputSvcMock.Object);
            filesContents = new Dictionary<string, string>();
        }

        private void Given_File(string fileName, string fileContents)
        {
            filesContents[fileName] = fileContents;
        }

        private PythonModule EvaluateScript(string fileName, string script)
        {
            var bytes = Encoding.Default.GetBytes(script);
            var imageUri = ImageLocation.FromUri(fileName);
            return new PythonModule(services, imageUri, bytes);
        }

        private string ReplacePythonVersion(string s)
        {
            var engine = IronPython.Hosting.Python.CreateEngine();
            return s.Replace(engine.Setup.DisplayName, "{Python version}");
        }

        private void AssertConsoleOutput(string expected)
        {
            var actual = ReplacePythonVersion(outputWriter.ToString());
            if (actual != expected)
            {
                Console.WriteLine(actual);
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PyMod_ConsoleOutput()
        {
            Given_File(
                "Python/_program.py",
                @"
class Program:
    pass
"
            );
            Given_File(
                "Python/_reko.py",
                @"
class Reko:
    def __init__(*args, **kwargs):
        print('Initializing Reko API')
"
            );

            EvaluateScript("script.py", "print('This is a script file')");

            var expected =
            #region Expected
@"{Python version}
Evaluating script.py
Initializing Reko API
This is a script file
";
            #endregion
            AssertConsoleOutput(expected);
        }
    }
}
