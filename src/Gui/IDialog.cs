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
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Abstracts the notion of a dialog for assistance when unit testing.
    /// </summary>
    public interface IDialog : IDisposable
    {
        string? Text { get; set; }
    }

    /// <summary>
    /// Implemented by dialogs that return a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDialog<T> : IDialog
    {
        /// <summary>
        /// The value returned by the dialog.
        /// </summary>
        T Value { get; }
    }
}
