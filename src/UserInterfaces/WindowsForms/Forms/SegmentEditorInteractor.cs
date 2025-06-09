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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Gui;
using System;
using System.Globalization;
using System.Linq;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class SegmentEditorInteractor
    {
        private SegmentEditorDialog dlg;

        public void Attach(SegmentEditorDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += Dlg_Load;
        }

        public UserSegment CreateUserSegment()
        {
            var arch = SelectedArchitecture();
            if (!TryParseHex(dlg.Length.Text, out ulong length))
                return null;

            Address addr;
            if (arch is not null)
            {
                if (!arch.TryParseAddress(dlg.Address.Text, out addr))
                    return null;
            }
            else
            {
                if (!TryParseHex(dlg.Address.Text, out var uAddr))
                    return null;
                addr = Address.Ptr32((uint)uAddr);
            }

            if (!TryParseHex(dlg.Offset.Text, out var offset))
                return null;

            return new UserSegment
            {
                Name = dlg.SegmentName.Text,
                Offset = offset,
                Length = (uint) length,
                Address = addr,
                Architecture = arch,
                AccessMode = GetAccessMode()
            };
        }

        private void EnableControls()
        {
            bool enableOk = true;
            var mode = GetAccessMode();
            enableOk &= (mode & AccessMode.Read) != 0;
            dlg.OkButton.Enabled = enableOk;
        }

        private AccessMode GetAccessMode()
        {
            AccessMode mode = 0;
            if (dlg.ReadMode.Checked)
                mode |= AccessMode.Read;
            if (dlg.WriteMode.Checked)
                mode |= AccessMode.Write;
            if (dlg.ExecuteMode.Checked)
                mode |= AccessMode.Execute;
            return mode;
        }

        private void LoadArchitectureNames()
        {
            dlg.Architectures.Items.Add(new ListOption("(None)", ""));
            dlg.Architectures.Items.AddRange(
                dlg.Services.RequireService<IConfigurationService>()
                    .GetArchitectures()
                    .Select(a => (object)new ListOption(a.Name, a.Name))
                    .ToArray());
        }

        public void SelectArchitecture(string archName)
        {
            foreach (ListOption item in dlg.Architectures.Items)
            {
                if (item.Text == archName)
                {
                    dlg.Architectures.SelectedItem = item;
                    return;
                }
            }
        }

        private IProcessorArchitecture SelectedArchitecture()
        {
            var oArch = (ListOption) dlg.Architectures.SelectedItem;
            if (oArch is null || string.IsNullOrEmpty((string)oArch.Value))
                return null;
            return (IProcessorArchitecture) oArch.Value;
        }

        private bool TryParseHex(string s, out ulong value)
        {
            return ulong.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private void Dlg_Load(object sender, EventArgs e)
        {
            LoadArchitectureNames();
            EnableControls();
        }
    }
}
