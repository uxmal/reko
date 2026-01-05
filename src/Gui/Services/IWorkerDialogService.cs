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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    public interface IWorkerDialogService
    {
        /// <summary>
        /// Starts working in a background thread.
        /// </summary>
        /// <param name="caption">Caption to display in the worker dialog.</param>
        /// <param name="backgroundWork"></param>
        /// <returns>True it task completed, false if it threw an exception.</returns>
        ValueTask<bool> StartBackgroundWork(string caption, Action backgroundWork);

        /// <summary>
        /// Allows the background thread to update the caption.
        /// </summary>
        void SetCaption(string newCaption);

        void FinishBackgroundWork();

        void ShowError(string p, Exception ex);

        void Error(ICodeLocation location, Exception ex, string message);
    }
}
