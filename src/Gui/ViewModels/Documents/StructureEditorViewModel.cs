#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Lib;
using Reko.Gui.Reactive;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Gui.ViewModels.Documents
{
    public class StructureEditorViewModel : ChangeNotifyingObject
    {
        private readonly Program program;

        public StructureEditorViewModel(IServiceProvider services, Program program)
        {
            this.program = program;
            this.DataTypes = new ObservableCollection<ListOption>();
            this.StructFields = new ObservableCollection<StructField>
            {
                new StructField(this)
                {
                    Name = "<Name>",
                    Offset = "0x0",
                    DataType = "int32",
                }
            };
            this.StructFields.CollectionChanged += StructFields_CollectionChanged;
            RenderStructure();
        }


        public ObservableCollection<ListOption> DataTypes { get; set; }

        public bool IsDeletable { get => false; }

        public string? RenderedType
        {
            get { return renderedType; }
            set { this.RaiseAndSetIfChanged(ref renderedType, value); }
        }
        private string? renderedType;

        public ObservableCollection<StructField> StructFields { get; set; }

        public ListOption? SelectedStructure
        {
            get { return selectedStructure; }
            set { this.RaiseAndSetIfChanged(ref selectedStructure, value); }
        }
        private ListOption? selectedStructure;

        public bool ShowPadding
        {
            get { return showPadding; }
            set { this.RaiseAndSetIfChanged(ref showPadding, value); }
        }
        private bool showPadding;

        [CIdentifier]
        public string? StructureName
        {
            get { return structureName; }
            set {
                this.RaiseAndSetIfChanged(ref structureName, value);
                RenderStructure();
            }
        }
        private string? structureName;

        public void AddField()
        {
            this.StructFields.Add(new StructField(this)
            {
                Offset = "0x0", //$TODO: append to last field.
            });
        }

        public void RenderStructure()
        {
            if (this.StructFields is null)
                return;
            var sb = new StringBuilder();
            sb.Append("struct ");
            sb.Append(string.IsNullOrEmpty(StructureName) ? "<no-name>" : StructureName.Trim());
            sb.AppendLine(" {");

            RenderStructureFields(sb);

            sb.AppendLine("}");

            this.RenderedType = sb.ToString();
        }

        private void RenderStructureFields(StringBuilder sb)
        {
            var sbLine = new StringBuilder();
            foreach (var (f, offset, valid) in this.StructFields.Select(f => StringToOffset(f))
                .OrderBy(ff => ff.offset))
            {
                sbLine.Append("    ");
                sbLine.Append(f.DataType);
                sbLine.Append(" ");
                sbLine.Append(f.Name);
                sbLine.Append(";");
                var padChars = 39 - sbLine.Length;
                if (padChars > 0)
                {
                    sbLine.Append(' ', padChars);
                }
                var sOffset = valid ? $"{offset:X}" : "???";
                sbLine.AppendFormat(" // 0x{0:X}: {1}", offset, f.Description);
                sb.AppendLine(sbLine.ToString());
                sbLine.Clear();
            }
        }

        private (StructField f, int offset, bool valid) StringToOffset(StructField f)
        {
            if (string.IsNullOrWhiteSpace(f.Offset))
                return (f, 0, false);
            var sOffset = f.Offset.Trim();
            int sign = 1;
            int i = 0;
            if (sOffset[i] == '-')
            {
                sign = -1;
                ++i;
            }
            int offset;
            if (sOffset.IndexOf("0x", i, StringComparison.InvariantCultureIgnoreCase) == i)
            {
                if (!int.TryParse(sOffset.AsSpan(i + 2), NumberStyles.AllowHexSpecifier, null, out offset))
                    return (f, 0, false);
            }
            else
            {
                if (!int.TryParse(sOffset.AsSpan(i + 2), out offset))
                    return (f, 0, false);
            }
            return (f, offset * sign, true);
        }

        private void StructFields_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RenderStructure();
        }


    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CIdentifierAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is string s && IsValidIdentifierName(s);
        }

        public static bool IsValidIdentifierName(string identifierName)
        {
            if (string.IsNullOrWhiteSpace(identifierName))
                return false;
            var s = identifierName.Trim();
            return CIdentifierAttribute.identifierName.IsMatch(s);
        }
        private static Regex identifierName = new Regex("[a-zA-Z_][a-zA-Z_0-9]*");
    }


    public class StructField : ChangeNotifyingObject
    {
        private readonly StructureEditorViewModel viewmodel;

        public StructField(StructureEditorViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
        }

        public string? Name
        {
            get { return name; }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
                this.viewmodel.RenderStructure();
            }
        }
        private string? name;

        public string? DataType {
            get { return dataType; }
            set { this.RaiseAndSetIfChanged(ref dataType, value);
                this.viewmodel.RenderStructure();
            }
        }
        private string? dataType;

        public string? Offset
        {
            get { return offset; }
            set
            {
                this.RaiseAndSetIfChanged(ref offset, value);
                this.viewmodel.RenderStructure();
            }
        }
        private string? offset;

        public string? Description
        {
            get { return description; }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
                this.viewmodel.RenderStructure();
            }
        }
        private string? description;
    }
}