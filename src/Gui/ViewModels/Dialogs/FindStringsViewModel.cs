#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Gui.Reactive;
using Reko.Gui.Services;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class FindStringsViewModel : ChangeNotifyingObject
    {
        private Program program;
        private IDecompilerShellUiService uiSvc;
        private ISettingsService settingsSvc;
        private IDialogFactory dialogFactory;

        public FindStringsViewModel(
            Program program,
            IDecompilerShellUiService uiSvc,
            ISettingsService settingsSvc,
            IDialogFactory dialogFactory)
        {
            this.program = program;
            this.uiSvc = uiSvc;
            this.settingsSvc = settingsSvc;
            this.dialogFactory = dialogFactory;
            this.minLength = 5;
            this.program = program;
            this.SearchAreasMru = LoadMruFromSettings();
        }

        public int CharacterType
        {
            get => characterType;
            set => base.RaiseAndSetIfChanged(ref characterType, value);
        }
        private int characterType;

        public int StringKind
        {
            get => stringKind;
            set => base.RaiseAndSetIfChanged(ref stringKind, value);
        }
        private int stringKind;

        public int MinLength
        {
            get => minLength;
            set => base.RaiseAndSetIfChanged(ref minLength, value);
        }
        private int minLength;

        public ObservableCollection<ListOption> SearchAreasMru { get; }

        public int SelectedMruIndex
        {
            get => selectedMruIndex;
            set => base.RaiseAndSetIfChanged(ref selectedMruIndex, value);
        }
        private int selectedMruIndex;

        public StringFinderCriteria GetCriteria()
        {
            return GetCriteria(this.CharacterType, this.StringKind, this.MinLength);
        }

        public StringFinderCriteria GetCriteria(int characterType, int stringKind, int minLength)
        {
            Encoding encoding;
            PrimitiveType charType;
            Func<ByteMemoryArea, Address, long, EndianImageReader> rdrCreator;
            switch (characterType)
            {
            default:
                encoding = Encoding.ASCII;
                charType = PrimitiveType.Char;
                rdrCreator = (m, a, b) => new LeImageReader(m, a, b);
                break;
            case 1:
                encoding = Encoding.GetEncoding("utf-16LE");
                charType = PrimitiveType.WChar;
                rdrCreator = (m, a, b) => new LeImageReader(m, a, b);
                break;
            case 2:
                encoding = Encoding.GetEncoding("utf-16BE");
                charType = PrimitiveType.WChar;
                rdrCreator = (m, a, b) => new BeImageReader(m, a, b);
                break;
            }

            StringType strType;
            switch (stringKind)
            {
            default: strType = StringType.NullTerminated(charType); break;
            case 1: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.Byte); break;
            case 2: case 3: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.UInt16); break;
            }

            var searchAreasListOption = this.SearchAreasMru[this.SelectedMruIndex];
            var searchAreas = (searchAreasListOption.Value is List<SearchArea> sa && sa.Count != 0)
                ? sa
                : null;
            return new StringFinderCriteria(
                StringType: strType,
                Encoding: encoding,
                Areas: searchAreas,  
                MinimumLength: minLength,
                CreateReader: rdrCreator);
        }

        public async ValueTask SelectSearchArea()
        {
            var dlg = this.dialogFactory.CreateSearchAreaDialog(program, new List<SearchArea>());
            var searchAreas = await uiSvc.ShowModalDialog(dlg);
            if (searchAreas is null || searchAreas.Count == 0)
                return;
            UpdateSearchAreaMru(searchAreas);
        }

        public void UpdateSearchAreaMru(List<SearchArea>? searchAreas)
        {
            var iExisting = IndexOf(this.SearchAreasMru, e => EqualSearchAreas(e.Value, searchAreas));
            ListOption item;
            if (iExisting != 0)
            {
                if (iExisting >= 1)
                {
                    item = this.SearchAreasMru[iExisting];
                    this.SearchAreasMru.RemoveAt(iExisting);
                }
                else
                {
                    item = new ListOption(SearchArea.Format(searchAreas) ?? "", searchAreas);
                }
                this.SearchAreasMru.Insert(0, item);
            }
            this.selectedMruIndex = -1;  // Hack to force an event to be raised.
            this.SelectedMruIndex = 0;
        }

        private static bool EqualSearchAreas(object? value, List<SearchArea>? right)
        {
            if (value is not List<SearchArea> left)
                return false;
            if (right is null)
                return false;
            if (left.Count != right.Count)
                return false;
            for (int i = 0; i < left.Count; ++i)
            {
                if (!left[i].Equals(right[i]))
                    return false;
            }
            return true;
        }

        private static int IndexOf<T>(ObservableCollection<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }


        public ObservableCollection<ListOption> LoadMruFromSettings()
        {
            var items = settingsSvc.GetList("FindStringsDialog/AreasMru");
            if (items is null || items.Length == 0)
            {
                return new ObservableCollection<ListOption>
                {
                    new ListOption("Entire program", null)
                };
            }
            else
            {
                var list = new ObservableCollection<ListOption>();
                foreach (string s in items)
                {
                    ListOption item;
                    if (string.IsNullOrEmpty(s))
                    {
                        item = new ListOption("Entire program", null);
                        list.Add(item);
                    }
                    else
                    {
                        if (SearchArea.TryParse(program, s, out var sa))
                        {
                            item = new ListOption(s, sa);
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        public void SaveMruToSettings()
        {
            settingsSvc.SetList("FindStringsDialog/AreasMru", SearchAreasMru
                .Select(a => a.Value is null ? "" : a.Text));
        }
    }
}
