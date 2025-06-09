#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class SearchDialogInteractor
    {
        private ISearchDialog dlg;
        private ISettingsService settingsSvc;

        public void Attach(ISearchDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.Closed += dlg_Closed;
            dlg.Patterns.TextChanged += delegate { EnableControls(); };
            dlg.ScannedMemory.CheckedChanged += delegate { EnableControls(); };
            dlg.UnscannedMemory.CheckedChanged += delegate { EnableControls(); };
            dlg.SearchButton.Click += SearchButton_Click;
        }

        private void EnableControls()
        {
            dlg.StartAddress.Enabled = dlg.EndAddress.Enabled =
                dlg.Scopes.SelectedIndex == 2;
            dlg.SearchButton.Enabled =
                dlg.Patterns.Text.Length > 0 &&
                (dlg.ScannedMemory.Checked || dlg.UnscannedMemory.Checked);
        }

        void SearchButton_Click(object sender, EventArgs e)
        {
            if (!dlg.Patterns.Items.Cast<string>().Any(s => s == dlg.Patterns.Text))
            {
                // A new pattern, stash it.
                settingsSvc.SetList(
                    "SearchDialog/Patterns",
                    new[] { dlg.Patterns.Text }.Concat(dlg.Patterns.Items.Cast<string>()));
            }

            dlg.Patterns.Text = ConvertPatternToHexString(dlg.Encodings.SelectedIndex, dlg.Patterns.Text);

            var pattern = ConvertHexStringToByteArray(dlg.Patterns.Text);

            dlg.ImageSearcher = new KmpStringSearch<byte>(
                pattern,
                dlg.ScannedMemory.Checked,
                dlg.UnscannedMemory.Checked);
        }

        private const int EncodingHex = 0;
        private const int EncodingOct = 1;
        private const int EncodingAscii = 2;
        private const int EncodingUtf8 = 3;

        private static string ConvertPatternToHexString(int encoding, string pattern)
        {
            byte[] patternAsBytes;

            Debug.Print("Encoding pattern {0}", pattern);
            switch (encoding)
            {
            case EncodingHex:
                return pattern;
            case EncodingOct:
                patternAsBytes = Octize(pattern).ToArray();
                break;
            case EncodingAscii:
                patternAsBytes = Encoding.Convert(Encoding.Default, Encoding.ASCII, Encoding.Default.GetBytes(pattern));
                break;
            case EncodingUtf8:
                patternAsBytes =  Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(pattern));
                break;
            default: throw new NotImplementedException();
            }

            return ConvertNumbersToHexString(patternAsBytes);
        }

        private static string ConvertNumbersToHexString(byte[] digits)
        {
            var stringBuilder = new StringBuilder();
            foreach (var digit in digits)
            {
                if (stringBuilder.Length > 0)
                   stringBuilder.Append(" ");
                stringBuilder.Append(digit.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        private byte[] ConvertHexStringToByteArray(string pattern)
        {
            // Ignore any non-hex digits.
            return Hexize(pattern).ToArray();
        }

        private static int HexDigit(char c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            if ('A' <= c && c <= 'F')
                return c - 'A' + 10;
            if ('a' <= c && c <= 'f')
                return c - 'a' + 10;
            return -1;
        }

        private static int OctalDigit(char c)
        {
            var n = c - '0';
            if (0 <= n && n < 8)
                return n;
            else
                return -1;
        }

        public static IEnumerable<byte> Hexize(string pattern)
        {
            int digits = 0;
            int outByte = 0;
            foreach (char c in pattern)
            {
                int h = HexDigit(c);
                if (h >= 0)
                {
                    ++digits;
                    outByte = (outByte << 4) | h;
                    if (digits == 2)
                    {
                        yield return (byte) outByte;
                        digits = 0;
                        outByte = 0;
                    }
                }
            }
        }

        public static IEnumerable<byte> Octize(string pattern)
        {
            int digits = 0;
            int outByte = 0;
            foreach (char c in pattern)
            {
                int h = OctalDigit(c);
                if (h >= 0)
                {
                    ++digits;
                    outByte = outByte * 8 | h;
                    if (digits == 3)
                    {
                        yield return (byte) outByte;
                        digits = 0;
                        outByte = 0;
                    }
                }
            }
        }

        void dlg_Load(object sender, EventArgs e)
        {
            this.settingsSvc = dlg.Services.RequireService<ISettingsService>();
            dlg.Patterns.DataSource = settingsSvc.GetList("SearchDialog/Patterns");
            if (dlg.InitialPattern is not null)
                dlg.Patterns.Text = dlg.InitialPattern;
            dlg.RegexCheckbox.Checked = (int)(settingsSvc.Get("SearchDialog/Regexp", 0) ?? 0)!= 0;
            dlg.Encodings.SelectedIndex = (int)(settingsSvc.Get("SearchDialog/Encoding", 0) ?? 0);
            dlg.Scopes.SelectedIndex = (int)(settingsSvc.Get("SearchDialog/Scope", 0) ?? 0);
            dlg.ScannedMemory.Checked = (int)(settingsSvc.Get("SearchDialog/Scanned", 1) ?? 1) != 0;
            dlg.UnscannedMemory.Checked = (int)(settingsSvc.Get("SearchDialog/Unscanned", 1) ?? 1) != 0;
            EnableControls();
        }

        void dlg_Closed(object sender, EventArgs e)
        {
            settingsSvc.Set("SearchDialog/Scanned", dlg.ScannedMemory.Checked ? 1 : 0);
            settingsSvc.Set("SearchDialog/Unscanned", dlg.UnscannedMemory.Checked ? 1 : 0);
            EnableControls();
        }
    }
}
