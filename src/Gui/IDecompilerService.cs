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
using System.IO;
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Provides access to the running instance of the decompiler.
    /// </summary>
    public interface IDecompilerService
    {
        /// <summary>
        /// Event is fired when the decompiler is changed for some reason.
        /// </summary>
        event EventHandler DecompilerChanged;

        IDecompiler Decompiler { get; set; }

        string ProjectName { get; }
    }

    public class DecompilerService : IDecompilerService
    {
        private IDecompiler decompiler;

        public event EventHandler DecompilerChanged;

        #region IDecompilerService Members

        public IDecompiler Decompiler
        {
            get { return decompiler; }
            set
            {
                decompiler = value;
                if (DecompilerChanged != null)
                {
                    DecompilerChanged(this, EventArgs.Empty);
                }
            }
        }

        //$REVIEW" huh?
        public string ProjectName
        {
            get
            { 
                if (decompiler == null)
                    return "";
                if (decompiler.Project== null)
                    return "";
                return Path.GetFileName(decompiler.Project.Programs[0].Filename);
            }
        }

        #endregion
    }
}
