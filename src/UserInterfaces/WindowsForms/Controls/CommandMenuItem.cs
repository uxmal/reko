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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
	/// <summary>
	/// Extends MenuItems to contain a MenuCommand.
	/// </summary>
	public class CommandMenuItem : MenuItem
	{
		public new event CommandMenuEventHandler Click;

		private MenuCommand cmd;
		private bool isDynamic;
		private bool isTemp;
        private int imgIndex;
        private string imgKey;

		public CommandMenuItem()
		{
            this.imgIndex = -1;
            this.imgKey = null;
		}

		public CommandMenuItem(string text)
		{
			Text = text.Replace('_', '&');
			cmd = null;
		}

		public CommandMenuItem(string text, Guid cmdSet, int cmdId)
		{
            if (text == null)
            {
                Debug.Print("************** {0} **************", cmdId);
                return;
            }
            else
            {
                Text = text.Replace('_', '&');
            }
			cmd = new MenuCommand(null, new CommandID(cmdSet, cmdId));
		}

        public int ImageIndex
        {
            get { return imgIndex; }
            set { imgIndex = value; }
        }

        public string ImageKey
        {
            get { return imgKey; }
            set { imgKey = value; }
        }

		public bool IsDynamic
		{
			get { return isDynamic; }
			set { isDynamic = value; } 
		}

		public bool IsTemporary
		{
			get { return isTemp; }
			set { isTemp = value; }
		}

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Text, IsDynamic ? "Dyn" : "", IsTemporary ? "Tmp" : "   ");
        }

		public MenuCommand MenuCommand
		{
			get { return cmd; }
		}

		protected override void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, new CommandMenuEventArgs(this));
		}
	}

	public delegate void CommandMenuEventHandler(object sender, CommandMenuEventArgs e);

	public class CommandMenuEventArgs : EventArgs
	{
		private CommandMenuItem item;

		public CommandMenuEventArgs(CommandMenuItem item)
		{
			this.item = item;
		}

		public CommandMenuItem Item
		{
			get { return item; }
		}
	}
}
