#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 .
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
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Wraps access to the Win32 registry.
    /// </summary>
    public interface IRegistryService
    {
        IRegistryKey CurrentUser { get; }
    }

    public interface IRegistryKey : IDisposable
    {
        void DeleteValue(string name);
        object GetValue(string name, object defaultValue);
        IRegistryKey OpenSubKey(string keyName, bool writeable);
        void SetValue(string name, object value);
    }
}
