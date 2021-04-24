#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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

#nullable enable

using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui.Controls
{
    public class TextFileEditorInteractor : IWindowPane
    {
        private IServiceProvider? services;
        private ITextFileEditor? editor;
        private readonly string fileName;
        private bool textLoaded;

        public TextFileEditorInteractor(string fileName)
        {
            this.fileName = fileName;
            this.textLoaded = false;
        }

        public IWindowFrame? Frame { get; set; }

        public void Close()
        {
            if (editor != null)
            {
                this.editor.SaveButton.Click -= textEditor_Save;
                editor.Dispose();
            }
            editor = null;
        }

        public object CreateControl()
        {
            if (services == null)
                throw new InvalidOperationException("Services should be set");
            if (editor == null)
            {
                var svcFactory = services.RequireService<IServiceFactory>();
                editor = svcFactory.CreateTextFileEditor();
                this.editor.SaveButton.Click += textEditor_Save;
            }
            return editor;
        }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }

        public void DisplayFile()
        {
            if (services == null)
                return;
            if (editor == null)
                return;
            if (textLoaded)
                return;
            var fsSvc = services.RequireService<IFileSystemService>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            try
            {
                using var rdr = fsSvc.CreateStreamReader(
                    fileName, Encoding.UTF8);
                editor.TextBox.Text = rdr.ReadToEnd();
                this.textLoaded = true;
            }
            catch (Exception ex)
            {
                uiSvc.ShowError(ex, $"Couldn't open file '{fileName}'");
            }
        }

        private void textEditor_Save(object? sender, EventArgs e)
        {
            if (services == null)
                return;
            if (editor == null)
                return;
            var fsSvc = services.RequireService<IFileSystemService>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            try
            {
                using var writer = fsSvc.CreateStreamWriter(
                    fileName, false, Encoding.UTF8);
                writer.Write(editor.TextBox.Text);
            }
            catch(Exception ex)
            {
                uiSvc.ShowError(ex, $"Couldn't save file '{fileName}'");
            }
        }
    }
}
