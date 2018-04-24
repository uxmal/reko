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

using Reko.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.UserInterfaces.WindowsForms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class WindowsFormsSettingsServiceTests
    {
        private MockRepository mr;
        private IRegistryService regSvc;
        private ServiceContainer sc;
        private WindowsFormsSettingsService settingsSvc;
        private IRegistryKey hkcu;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            regSvc = mr.StrictMock<IRegistryService>();
            hkcu = mr.StrictMock<IRegistryKey>();

            sc = new ServiceContainer();
            sc.AddService(typeof(IRegistryService), regSvc);
            regSvc.Stub(r => r.CurrentUser).Return(hkcu);

            settingsSvc = new WindowsFormsSettingsService(sc);
        }

        [Test]
        public void WFSS_SetString()
        {
            var hk = mr.StrictMock<IRegistryKey>();
            hkcu.Expect(h => h.OpenSubKey(@"Software\jklSoft\Reko", true)).Return(hk);
            hk.Expect(h => h.SetValue("foo", "bar"));
            mr.ReplayAll();

            settingsSvc.Set("foo", "bar");

            mr.VerifyAll();
        }

        [Test]
        public void WFSS_SetStringWithDeepPath()
        {
            var hk = mr.StrictMock<IRegistryKey>();
            hkcu.Expect(h => h.OpenSubKey(@"Software\jklSoft\Reko\Deep\Scope", true)).Return(hk);
            hk.Expect(h => h.SetValue("foo", "bar"));
            mr.ReplayAll();

            settingsSvc.Set("Deep/Scope/foo", "bar");

            mr.VerifyAll();
        }

        [Test]
        public void WFSS_SetIntegerWithPath()
        {
            var hk = mr.StrictMock<IRegistryKey>();
            hkcu.Expect(h => h.OpenSubKey(@"Software\jklSoft\Reko", true)).Return(hk);
            hk.Expect(h => h.SetValue("foo", 3));
            mr.ReplayAll();

            settingsSvc.Set("foo", 3);

            mr.VerifyAll();
        }

        [Test]
        public void WFSS_GetExistingInt()
        {
            var hk = mr.StrictMock<IRegistryKey>();
            hkcu.Expect(h => h.OpenSubKey(@"Software\jklSoft\Reko", false)).Return(hk);
            hk.Expect(h => h.GetValue("foo", 3)).Return(4);
            mr.ReplayAll();

            int v = (int) settingsSvc.Get("foo", 3);
            Assert.AreEqual(4, v);

            mr.VerifyAll();
        }
    }
}
