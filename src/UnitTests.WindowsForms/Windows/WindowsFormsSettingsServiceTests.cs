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

using Moq;
using NUnit.Framework;
using Reko.UserInterfaces.WindowsForms;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class WindowsFormsSettingsServiceTests
    {
        private ServiceContainer sc;
        private WindowsFormsSettingsService settingsSvc;
        private Mock<IRegistryService> regSvc;
        private Mock<IRegistryKey> hkcu;

        [SetUp]
        public void Setup()
        {
            regSvc = new Mock<IRegistryService>();
            hkcu = new Mock<IRegistryKey>();

            sc = new ServiceContainer();
            sc.AddService(typeof(IRegistryService), regSvc.Object);
            regSvc.Setup(r => r.CurrentUser).Returns(hkcu.Object);

            settingsSvc = new WindowsFormsSettingsService(sc);
        }

        [Test]
        public void WFSS_SetString()
        {
            var hk = new Mock<IRegistryKey>();
            hkcu.Setup(h => h.OpenSubKey(@"Software\jklSoft\Reko", true))
                .Returns(hk.Object)
                .Verifiable();
            hk.Setup(h => h.SetValue("foo", "bar"))
                .Verifiable();
            settingsSvc.Set("foo", "bar");

            hkcu.VerifyAll();
        }

        [Test]
        public void WFSS_SetStringWithDeepPath()
        {
            var hk = new Mock<IRegistryKey>();
            hkcu.Setup(h => h.OpenSubKey(@"Software\jklSoft\Reko\Deep\Scope", true))
                .Returns(hk.Object)
                .Verifiable();
            hk.Setup(h => h.SetValue("foo", "bar"))
                .Verifiable();

            settingsSvc.Set("Deep/Scope/foo", "bar");

            hk.VerifyAll();
            hkcu.VerifyAll();
        }

        [Test]
        public void WFSS_SetIntegerWithPath()
        {
            var hk = new Mock<IRegistryKey>();
            hkcu.Setup(h => h.OpenSubKey(@"Software\jklSoft\Reko", true))
                .Returns(hk.Object)
                .Verifiable();
            hk.Setup(h => h.SetValue("foo", 3)).Verifiable();

            settingsSvc.Set("foo", 3);

            hk.VerifyAll();
            hkcu.VerifyAll();
        }

        [Test]
        public void WFSS_GetExistingInt()
        {
            var hk = new Mock<IRegistryKey>();
            hkcu.Setup(h => h.OpenSubKey(@"Software\jklSoft\Reko", false))
                .Returns(hk.Object)
                .Verifiable();
            hk.Setup(h => h.GetValue("foo", 3))
                .Returns(4)
                .Verifiable();

            int v = (int) settingsSvc.Get("foo", 3);
            Assert.AreEqual(4, v);

            hk.VerifyAll();
            hkcu.VerifyAll();
        }
    }
}
