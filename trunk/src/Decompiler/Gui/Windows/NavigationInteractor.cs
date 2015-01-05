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

using Decompiler.Core;
using Decompiler.Gui.Components;
using Decompiler.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Performs back/forward navigation.
    /// </summary>
    /// <remarks>
    /// This interactor needs a pair of buttons -- for "Back" and "Forward" -- a timer, and 
    /// a way to show a menu to the user if the timer times out.
    /// </remarks>
    public class NavigationInteractor
    {
        public event EventHandler LocationChanged;

        private IButton btnBack;
        private IButton btnForward;
        private ITimer timer;
        private List<Address> navStack = new List<Address>();
        private int stackPosition = -1;

        public void Attach(IButton btnBack, IButton btnForward, ITimer timer)
        {
            this.btnBack = btnBack;
            this.btnForward = btnForward;
            this.timer = timer;

            btnBack.Click += btnBack_Click;
            btnForward.Click += btnForward_Click;
        }

        public Address Location
        {
            get {
                if (stackPosition >= navStack.Count)
                    return null;
                return navStack[stackPosition];
            }
        }

        private void EnableControls()
        {
            btnBack.Enabled = stackPosition > 0;
            btnForward.Enabled = stackPosition < navStack.Count - 1;
        }

        /// <summary>
        /// Call this when a user navigation action has occurred and you need to add an address
        /// to the "stack".
        /// </summary>
        /// <param name="address"></param>
        public void UserNavigateTo(Address address)
        {
            int itemsAhead = navStack.Count - (stackPosition + 1);
            if (stackPosition >= 0 && itemsAhead > 0)
            {
                Debug.Print("Removing {0}:{1}", stackPosition + 1, itemsAhead);
                navStack.RemoveRange(stackPosition + 1, itemsAhead);
            }
            navStack.Add(address);
            ++stackPosition;
            EnableControls();
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            if (stackPosition <= 0)
                return;
            --stackPosition;
            EnableControls();
            LocationChanged.Fire(this);
        }

        void btnForward_Click(object sender, EventArgs e)
        {
            if (stackPosition >= navStack.Count)
                return;
            ++stackPosition;
            EnableControls();
            LocationChanged.Fire(this);
        }
    }
}
