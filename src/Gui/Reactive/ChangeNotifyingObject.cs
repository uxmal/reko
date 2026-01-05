#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Gui.Reactive
{
    /// <summary>
    /// Base class for objects that raise events when properties
    /// are mutated.
    /// </summary>
    /// <remarks>
    /// This is the same idea as ReactiveUI.ReactiveObject, but 
    /// we don't want to take the dependency on an external 
    /// assembly just for one method.
    /// </remarks>
    public class ChangeNotifyingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
