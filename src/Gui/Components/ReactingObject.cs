#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Components
{
    /// <summary>
    /// This is a base class that implements <see cref="INotifyPropertyChanged" />
    /// similar to ReactiveUI</see>
    /// </summary>
    public class ReactingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on an object implementing <see cref="INotifyPropertyChanged"/>
        /// using CallerMemberName to raise the notification
        /// and the ref to the backing field to set the property.
        /// </summary>
        /// <remarks>
        /// This is almost identical to the ReactiveUI implementation, but we 
        /// have our own implementation here to avoid pulling in a dependency 
        /// on ReactiveUI</remarks>
        /// <typeparam name="TObj">The type of the object.</typeparam>
        /// <typeparam name="TRet">The type of the property value.</typeparam>
        /// <param name="sender">The object raising the notification.</param>
        /// <param name="backingField">A reference to the backing field for this
        /// property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property, usually
        /// automatically provided through the <see cref="CallerMemberName" /> attribute.</param>
        /// <returns>The newly set value, normally discarded.</returns>
        protected TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }
            backingField = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return newValue;
        }
    }
}
