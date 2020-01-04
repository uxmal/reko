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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Services
{
    /// <summary>
    /// Service used to display warnings in the diagnostics window
    /// of the Reko GUI.
    /// </summary>
    public interface IDiagnosticsService
    {
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception ex, string message);
        void Error(ICodeLocation location, string message);
        void Error(ICodeLocation location, string message, params object[] args);
        void Error(ICodeLocation location, Exception ex, string message);
        void Error(ICodeLocation location, Exception ex, string message, params object [] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
        void Warn(ICodeLocation location, string message);
        void Warn(ICodeLocation location, string message, params object[] args);
        void Inform(string message);
        void Inform(string message, params object[] args);
        void Inform(ICodeLocation location, string message);
        void Inform(ICodeLocation location, string message, params object[] args);
        void ClearDiagnostics();
    }
}
