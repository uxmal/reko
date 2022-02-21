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
using Reko.Gui.Forms;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public partial class MainWindow : Window, IMainForm
    {
        public MainWindow()
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

        System.Drawing.Size IMainForm.Size
        {
            get { return new System.Drawing.Size((int) this.Bounds.Width, (int) this.Bounds.Height); }
            set { this.Height = value.Height; this.Width = value.Width; }
    }

        FormWindowState IMainForm.WindowState {
            get => this.WindowState switch
            {
                WindowState.Normal => FormWindowState.Normal,
                WindowState.Minimized => FormWindowState.Minimized,
                WindowState.Maximized => FormWindowState.Maximized,
                _ => throw new NotImplementedException()
            };
            set
            {
                this.WindowState = value switch
                {
                    FormWindowState.Normal => WindowState.Normal,
                    FormWindowState.Minimized => WindowState.Minimized,
                    FormWindowState.Maximized => WindowState.Maximized,
                    _ => throw new NotImplementedException()
                };
            }
        }

        event EventHandler IMainForm.Closed
        {
            add { closedEvent += value; }
            remove { closedEvent -= value; }
        }
        private EventHandler? closedEvent = null;

        event EventHandler IMainForm.Load
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void IMainForm.Close()
        {
            this.Close();
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }



        object IMainForm.Invoke(Delegate action, params object[] args)
        {
            throw new NotImplementedException();
        }

        void IMainForm.LayoutMdi(DocumentWindowLayout layout)
        {
            throw new NotImplementedException();
        }

        void IMainForm.Show()
        {
            throw new NotImplementedException();
        }

        void IMainForm.UpdateToolbarState()
        {
            //$TODO:
        }
    }
}
