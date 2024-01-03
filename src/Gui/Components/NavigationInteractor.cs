#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

//using Reko.Gui.Controls;
using System.Collections.Generic;

namespace Reko.Gui.Components
{
    /// <summary>
    /// Performs back/forward navigation.
    /// </summary>
    /// <remarks>
    /// This interactor is connected to a navigable control, which will have a pair of buttons -- for "Back" and "Forward" -- a timer, and 
    /// a way to show a menu to the user if the timer times out.
    /// </remarks>
    public class NavigationInteractor<T> : ReactingObject
    {
        private readonly List<T> navStack = new List<T>();
        private int stackPosition = 0;

        public NavigationInteractor()
        {
            EnableControls();
        }

        public bool BackEnabled
        {
            get { return backEnabled; }
            set { this.RaiseAndSetIfChanged(ref backEnabled, value); }
        }
        private bool backEnabled;

        public bool ForwardEnabled
        {
            get { return forwardEnabled; }
            set { this.RaiseAndSetIfChanged(ref forwardEnabled, value); }
        }
        private bool forwardEnabled;

        private void EnableControls()
        {
            this.BackEnabled = stackPosition > 0;
            this.ForwardEnabled = stackPosition < navStack.Count;
        }

        /// <summary>
        /// Call this when a user navigation action has occurred and you need to add an address
        /// to the "stack".
        /// </summary>
        /// <param name="locationDest">Location being navigated to.</param>
        public void RememberLocation(T locationDest)
        {
            int itemsAhead = navStack.Count - stackPosition;
            if (stackPosition >= 0 && itemsAhead > 0)
            {
                navStack.RemoveRange(stackPosition, itemsAhead);
            }
            ++stackPosition;
            navStack.Add(locationDest);    // Remember where we will be...
            EnableControls();
        }

        public T NavigateBack()
        {
            if (stackPosition <= 0)
                return default!;
            var result = navStack[--stackPosition];
            EnableControls();
            return result;
        }

        public T NavigateForward()
        {
            if (stackPosition >= navStack.Count)
                return default!;
            var result = navStack[stackPosition];
            ++stackPosition;
            EnableControls();
            return result;
        }
    }
}
