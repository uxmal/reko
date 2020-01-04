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

using Reko.Core;
using Reko.Gui.Components;
using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Performs back/forward navigation.
    /// </summary>
    /// <remarks>
    /// This interactor is connected to a navigable control, which will have a pair of buttons -- for "Back" and "Forward" -- a timer, and 
    /// a way to show a menu to the user if the timer times out.
    /// </remarks>
    public class NavigationInteractor<T>
    {
        private INavigableControl<T> navControl;
        private List<T> navStack = new List<T>();
        private int stackPosition = 0;

        public void Attach(INavigableControl<T> navControl)
        {
            this.navControl = navControl;

            EnableControls();

            navControl.BackButton.Click += btnBack_Click;
            navControl.ForwardButton.Click += btnForward_Click;
        }

        private T Location
        {
            get {
                if (stackPosition >= navStack.Count)
                    return default(T);
                return navStack[stackPosition];
            }
        }

        private void EnableControls()
        {
            navControl.BackButton.Enabled = stackPosition > 0;
            navControl.ForwardButton.Enabled = stackPosition < (navStack.Count - 1);
        }

        /// <summary>
        /// Call this when a user navigation action has occurred and you need to add an address
        /// to the "stack".
        /// </summary>
        /// <param name="address"></param>
        public void RememberAddress(T address)
        {
            int itemsAhead = navStack.Count - stackPosition;
            if (stackPosition >= 0 && itemsAhead > 0)
            {
                var stackAddress = navStack[stackPosition];
                if (stackAddress != null && 
                    !stackAddress.Equals(navControl.CurrentAddress))
                {
                    stackPosition++;
                    itemsAhead--;
                }
                navStack.RemoveRange(stackPosition, itemsAhead);
            }
            navStack.Add(navControl.CurrentAddress);    // Remember where we were...
            ++stackPosition;
            navStack.Add(address);    // Remember where we will be...
            EnableControls();
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            if (stackPosition <= 0)
                return;
            --stackPosition;
            EnableControls();
            navControl.CurrentAddress = Location;        // ...and move to the new position.
        }

        void btnForward_Click(object sender, EventArgs e)
        {
            if (stackPosition >= navStack.Count)
                return;
            ++stackPosition;
            EnableControls();
            navControl.CurrentAddress = Location;        // ...and move to the new position.
        }
    }
}
