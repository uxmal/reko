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

using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class MenuStatusSetter
    {
        private CommandStatus cmdStatus;
        private CommandText cmdText;
        private EventHandler popupHandler;
        private CommandMenuEventHandler clickHandler;

        public MenuStatusSetter(EventHandler popupHandler, CommandMenuEventHandler clickHandler)
        {
            this.cmdStatus = new CommandStatus();
            this.cmdText = new CommandText("&");
            this.popupHandler = popupHandler;
            this.clickHandler = clickHandler;
        }

        public int SetStatus(IMenuAdapter menu, ICommandTarget ct)
        {
            int visibleItemsInGroup = 0;
            int iLastSeparator = -1;
            int totalVisible = 0;
            for (int i = 0; i < menu.Count; ++i)
            {
                if (menu.IsSeparator(i))
                {
                    cmdStatus.Status = visibleItemsInGroup > 0 ? MenuStatus.Visible | MenuStatus.Enabled : 0;
                    visibleItemsInGroup = 0;
                    iLastSeparator = i;
                    menu.SetStatus(i, cmdStatus.Status);
                }
                else if (menu.IsTemporary(i))
                {
                    menu.RemoveAt(i);
                    --i;
                }
                else
                {
                    var cmdId = menu.GetCommandID(i);
                    cmdStatus.Status = 0;
                    if (cmdId is null)
                    {
                        cmdStatus.Status = MenuStatus.Enabled | MenuStatus.Visible;
                        ++visibleItemsInGroup;
                        ++totalVisible;
                    }
                    else if (ct.QueryStatus(cmdId, cmdStatus, cmdText))
                    {
                        if (menu.IsDynamic(i))
                        {
                            menu.SetText(i, cmdText.Text);
                            cmdId = new CommandID(cmdId.Guid, cmdId.ID + 1);
                            while (ct.QueryStatus(cmdId, cmdStatus, cmdText))
                            {
                                CommandMenuItem itemNew = new CommandMenuItem(cmdText.Text, cmdId.Guid, cmdId.ID);
                                itemNew.IsTemporary = true;
                                itemNew.DropDownOpening += popupHandler;
                                itemNew.Click += clickHandler;
                                menu.InsertAt(++i, itemNew);
                                if ((cmdStatus.Status & MenuStatus.Visible) != 0)
                                {
                                    ++visibleItemsInGroup;
                                    ++totalVisible;
                                }
                                cmdId = new CommandID(cmdId.Guid, cmdId.ID + 1);
                            }
                        }
                        else
                        {
                            if ((cmdStatus.Status & MenuStatus.Visible) != 0)
                            {
                                ++visibleItemsInGroup;
                                ++totalVisible;
                            }
                        }
                    }
                    menu.SetStatus(i, cmdStatus.Status);
                }
            }
            if (iLastSeparator >= 0 && visibleItemsInGroup == 0)
            {
                menu.SetStatus(iLastSeparator, 0);
            }
            return totalVisible;
        }
    }
}
