#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Services
{
    [TestFixture]
    public class FileSystemSettingsServiceTests
    {
        private Mock<IFileSystemService> fsSvc = default;
        private FileSystemSettingsService fsssSvc = default!;

        [SetUp]
        public void Setup()
        {
            this.fsSvc = new Mock<IFileSystemService>();
        }

        private void Given_Settings(string jsonSettings)
        {
            fsSvc.Setup(f => f.FileExists(
                It.IsNotNull<string>())).Returns(true);
            fsSvc.Setup(f => f.CreateFileStream(
                It.IsNotNull<string>(),
                FileMode.Open)).Returns(new MemoryStream(
                    Encoding.UTF8.GetBytes(jsonSettings.Replace('\'', '\"'))));
        }


        private void Given_FileSystemSettingsService()
        {
            this.fsssSvc = new FileSystemSettingsService(fsSvc.Object, OsPath.Absolute("home", "bob", ".config", "reko"));
        }


        [Test]
        public void Fsssvc_Load()
        {
            Given_Settings(@"
{
    'test': 'setting'
}");
            Given_FileSystemSettingsService();

            fsssSvc.Load();
            Assert.AreEqual("setting", (string) fsssSvc.Get("test", "failure"));
        }

        [Test]
        public void Fsssvc_Save_List()
        {
            Given_Settings(@"
{
    'stringlist': []
}");
            Given_FileSystemSettingsService();

            fsssSvc.SetList("stringlist", new[] {"A", "B","C"});

            var stm = new MemoryStream();
            fsssSvc.Save(stm);
            Assert.AreEqual(@"{
  ""stringlist"": [
    ""A"",
    ""B"",
    ""C""
  ]
}", Encoding.UTF8.GetString(stm.ToArray()));
        }
    }
}
