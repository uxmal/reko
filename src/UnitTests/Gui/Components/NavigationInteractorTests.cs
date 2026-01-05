#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Gui.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Components
{
    public class NavigationInteractorTests
    {
        [Test]
        public void NavInt_Start()
        {
            var navInt = new NavigationInteractor<int>();
            Assert.IsFalse(navInt.BackEnabled);
            Assert.IsFalse(navInt.ForwardEnabled);
        }

        [Test]
        public void NavInt_Remember()
        {
            var navInt = new NavigationInteractor<int>();
            navInt.RememberLocation(0x42);
            Assert.IsTrue(navInt.BackEnabled);
            Assert.IsFalse(navInt.ForwardEnabled);
        }

        [Test]
        public void NavInt_Remember_Back_one_step()
        {
            var navInt = new NavigationInteractor<int>();
            navInt.RememberLocation(0x42);
            var prev = navInt.NavigateBack();
            Assert.IsFalse(navInt.BackEnabled);
            Assert.IsTrue(navInt.ForwardEnabled);
            Assert.AreEqual(0x42, prev);
        }

        [Test]
        public void NavInt_Remember_two_back_two_fwd()
        {
            var navInt = new NavigationInteractor<int>();
            navInt.RememberLocation(0x10);
            navInt.RememberLocation(0x20);

            // Go back 2 steps
            _ = navInt.NavigateBack();
            var prev = navInt.NavigateBack();
            Assert.AreEqual(0x10, prev);
            Assert.IsFalse(navInt.BackEnabled);
            Assert.IsTrue(navInt.ForwardEnabled);

            // Now forward 2 steps.
            navInt.NavigateForward();
            prev = navInt.NavigateForward();
            Assert.AreEqual(0x20, prev);
            Assert.IsTrue(navInt.BackEnabled);
            Assert.IsFalse(navInt.ForwardEnabled);
        }

        [Test]
        public void NavInt_Remember_one_back_nav_back()
        {
            var navInt = new NavigationInteractor<int>();
            navInt.RememberLocation(0x10);
            navInt.RememberLocation(0x20);

            // Go back 1 step, then go somewhere new
            _ = navInt.NavigateBack();
            navInt.RememberLocation(0x30);

            //Now back 2 steps.
            var prev = navInt.NavigateBack();
            Assert.AreEqual(0x30, prev);
            prev = navInt.NavigateBack();
            Assert.AreEqual(0x10, prev);
        }
    }
}
