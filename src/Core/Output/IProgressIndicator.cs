#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Output
{
    /// <summary>
    /// Interface to use when displaying progress during a 
    /// long operation.
    /// </summary>
    public interface IProgressIndicator
    {
        void SetCaption(string newCaption);

        void ShowStatus(string caption);
        void ShowProgress(string caption, int numerator, int denominator);
        void Advance(int count);
        void Finish();
    }

    public class NullProgressIndicator : IProgressIndicator
    {
        public static NullProgressIndicator Instance { get; } = new NullProgressIndicator();

        private NullProgressIndicator()
        {
        }

        public void SetCaption(string newCaption)
        {
        }

        public void ShowStatus(string caption)
        {
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
        }

        public void Advance(int count)
        {
        }

        public void Finish()
        {
        }
    }
}
