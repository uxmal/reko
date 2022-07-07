#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Reko.Gui.Services;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public partial class MessageBox : Window
    {
        public MessageBox()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public enum MessageBoxButtons
        {
            OK,
            OKCancel,
            YesNo,
            YesNoCancel
        }

        public static Task<DialogResult> Show(
            Window parent,
            string text,
            string title,
            MessageBoxButtons buttons)
        {
            var msgbox = new MessageBox()
            {
                Title = title,
            };
            msgbox.FindControl<Label>("Text").Content = text;
            var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

            var res = DialogResult.OK;

            void AddButton(string caption, DialogResult r, bool isDefault = false)
            {
                var btn = new Button { Content = caption };
                btn.Click += delegate
                {
                    res = r;
                    msgbox.Close();
                };
                buttonPanel.Children.Add(btn);
                if (isDefault)
                    res = r;
            }

            switch (buttons)
            {
            case MessageBoxButtons.OK:
            case MessageBoxButtons.OKCancel:
                AddButton("Ok", DialogResult.OK, true);
                break;
            case MessageBoxButtons.YesNo:
            case MessageBoxButtons.YesNoCancel:
                AddButton("Yes", DialogResult.Yes);
                AddButton("No", DialogResult.No, true);
                break;
            }
            if (buttons == MessageBoxButtons.OKCancel || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Cancel", DialogResult.Cancel, true);
            }

            var tcs = new TaskCompletionSource<DialogResult>();
            msgbox.Closed += delegate { tcs.TrySetResult(res); };
            if (parent is null)
                msgbox.Show();
            else
                msgbox.ShowDialog(parent);
            return tcs.Task;
        }
    }
}
