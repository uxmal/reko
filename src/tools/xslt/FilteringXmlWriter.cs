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

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Reko.Tools.Xslt
{
	public class FilteringXmlWriter : System.Xml.XmlTextWriter
	{
		public FilteringXmlWriter(Stream stm)
			: base(new StreamWriter(stm, new UTF8Encoding(false)))
		{
		}

		public FilteringXmlWriter(TextWriter txt)
			: base(txt)
		{
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
		}

        public override void WriteCharEntity(char ch)
        {
            if (ch == '&')
                base.WriteRaw("&");
            else
                base.WriteCharEntity(ch);
        }
	}
}
