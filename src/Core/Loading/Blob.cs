#region License
/* Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Loading
{
    /// <summary>
    /// A blob is simply a sequence of bytes, of which nothing is known.
    /// To further process this, information needs to be gathered from 
    /// the user.
    /// </summary>
    public class Blob : ILoadedImage
    {
        public Blob(ImageLocation location, byte[] image)
        {
            this.Image = image;
            this.Location = location;
        }

        public ImageLocation Location { get; }

        public byte[] Image { get; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitBlob(this, context);
        }
    }
}
