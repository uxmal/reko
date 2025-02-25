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
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class TextSelection
    {
        private TextView textView;

        internal TextSelection(TextView textView)
        {
            this.textView = textView;
        }

        public TextPointer Start { get { return textView.GetStartSelection(); } }

        public TextPointer End { get { return textView.GetEndSelection(); } }

        public bool IsEmpty { get { return textView.IsSelectionEmpty(); } }

        public void Save(Stream stream, string cfFormat)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (cfFormat is null)
                throw new ArgumentNullException(nameof(cfFormat));
            if (cfFormat != System.Windows.Forms.DataFormats.UnicodeText)
                throw new NotSupportedException();

            textView.SaveSelectionToStream(stream);
        }
    }
}
