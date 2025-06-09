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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
	/// <summary>
	/// Maintains the dirty state of an arbitrary number of input controls by extending them
	/// with a "TrackChanges" property. If any of the textboxes change their text, the 
	/// IsDirty property of DirtyManager is set to true.
	/// </summary>
	[ProvideProperty("TrackChanges", typeof (IComponent))]
	public class DirtyManager : Component, IExtenderProvider
	{
		public event EventHandler IsDirtyChanged;

		private HashSet<object> trackedControls; 
		private bool isDirty;

		public DirtyManager()
		{
			trackedControls = new HashSet<object>();
		}

		[DefaultValue(false)]
		public bool IsDirty
		{
			get { return isDirty; }
			set 
			{
				isDirty = value; 
				if (IsDirtyChanged is not null)
					IsDirtyChanged(this, EventArgs.Empty);
			}
		}

		#region IExtenderProvider Members

		public bool CanExtend(object extendee)
		{
			return extendee is TextBox;
		}

		public bool GetTrackChanges(object extendee)
		{
			return trackedControls.Contains(extendee);
		}

		public void SetTrackChanges(object extendee, bool value)
		{
			if (value)
			{
				if (!trackedControls.Contains(extendee))
				{
					TextBox t = ((TextBox)extendee);
					t.TextChanged += new EventHandler(textBox_TextChanged);
                    trackedControls.Add(extendee);
				}
			}
			else 
			{
				if (trackedControls.Contains(extendee))
				{
					((TextBox)extendee).TextChanged -= new EventHandler(textBox_TextChanged);
					trackedControls.Remove(extendee);
				}
			}
		}

		#endregion

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}
	}
}
