#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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
using System.Runtime.Serialization;

namespace Reko.ImageLoaders.IHex32
{
    public class IHEX32Exception : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the Intel HEX32 stream line number at which the exception occurred.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNum { get; } = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lineNo">(Optional) The line number in the HEX32 datum.</param>
        public IHEX32Exception(int lineNo = 0)
            : base()
        {
            LineNum = lineNo;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="lineNo">(Optional) The line number in the HEX32 datum.</param>
        public IHEX32Exception(string message, int lineNo = 0)
            : base(message)
        {
            LineNum = lineNo;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="lineNo">(Optional) The line number in the HEX32 datum.</param>
        public IHEX32Exception(string message, Exception innerException, int lineNo = 0)
            : base(message, innerException)
        {
            LineNum = lineNo;
        }

        /// <summary>
        /// Constructor for serialization.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that
        ///                    holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that
        ///                       contains contextual information about the source or destination.</param>
        public IHEX32Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            LineNum = (int)info.GetValue(nameof(LineNum), typeof(int));
        }

        /// <summary>
        /// When overridden in a derived class, sets the
        /// <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the
        /// exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that
        ///                    holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that
        ///                       contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(LineNum), LineNum, typeof(int));
        }

        #endregion
    }

}
