#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
	public class DisassemblyControl : Control
	{
        private VScrollBar vscroll;

        public DisassemblyControl()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            ImageChanged += DisassemblyControl_StateChange;
            ArchitectureChanged += DisassemblyControl_StateChange;
            StartAddressChanged += DisassemblyControl_StateChange;
     
            vscroll = new VScrollBar();
            vscroll.Margin = new Padding(10);

            vscroll.Enabled = false;
            vscroll.Visible = true;
            vscroll.Value = 0;
            vscroll.Minimum = 0;
            vscroll.Maximum = 10000;
            vscroll.MinimumSize = new Size(0, 20);
            vscroll.Dock = DockStyle.Right;
            Controls.Add(vscroll);
            vscroll.ValueChanged += delegate { Invalidate(); } ;
        }

        [Browsable(false)]
        public LoadedImage Image { get { return image; } set { image = value; ImageChanged.Fire(this); } }
        public event EventHandler ImageChanged;
        private LoadedImage image;

        [Browsable(false)]
        public IProcessorArchitecture Architecture { get { return arch; } set { arch = value; ArchitectureChanged.Fire(this); } }
        public event EventHandler ArchitectureChanged;
        private IProcessorArchitecture arch;

        [Browsable(false)]
        public Address StartAddress { get { return startAddress; } set { startAddress = value; StartAddressChanged.Fire(this); } }
        public event EventHandler StartAddressChanged;
        private Address startAddress;

        [Browsable(false)]
        public Address TopAddress { get { return topAddress; } set { topAddress = value; TopAddressChanged.Fire(this); } }
        public event EventHandler TopAddressChanged;
        private Address topAddress;

        [Browsable(false)]
        public Address SelectedAddress
        {
            get { return selectedAddress; }
            set
            {
                selectedAddress = value;
                TopAddress = value;
                Invalidate();
                SelectedAddressChanged.Fire(this);
            }
        }
        public event EventHandler SelectedAddressChanged;
        private Address selectedAddress;

        void DisassemblyControl_StateChange(object sender, EventArgs e)
        {
            if (arch == null || image == null || !image.IsValidAddress(startAddress))
            {
                vscroll.Enabled = false;
            }
            else
            {
                vscroll.Enabled = true;
                vscroll.Minimum = 0;
                vscroll.Maximum = image.Bytes.Length - 1;

                using (var g = CreateGraphics())
                {
                    var cyRow = g.MeasureString("M", Font).Height;
                    int EstimateRows = (int) Math.Ceiling(Height / cyRow);
                    int estimateByteSize = EstimateRows * 3;
                }
            }
            Invalidate();
        }

        public void DumpAssembler(Graphics g)
        {
            var rcClient = ClientRectangle;
            g.Clear(BackColor);
            if (arch == null || image == null || !image.IsValidAddress(topAddress))
            {
                return;
            }

#if SCROLLABLE
            g.TranslateTransform(this.AutoScrollPosition.X,
                                          this.AutoScrollPosition.Y);
#endif

            var dumper = new Dumper(arch);
            dumper.ShowAddresses = true;
            dumper.ShowCodeBytes = true;
            var addrStart = TopAddress;
            var dasm = arch.CreateDisassembler(image.CreateReader(addrStart));
            var cyRow = (int) g.MeasureString("M", Font).Height;
            var rc = new RectangleF(g.ClipBounds.Left, g.ClipBounds.Top, rcClient.Width, cyRow);
            var addrFinal = StartAddress;
            for (; rc.Top < rcClient.Bottom; rc.Offset(0, cyRow))
            {
                try
                {
                    if (!dasm.MoveNext())
                    {
                        addrFinal = Image.BaseAddress + Image.Bytes.Length;
                        break;
                    }
                }
                catch
                {
                    break;
                }
                addrFinal = dasm.Current.Address;
                var writer = new StringWriter();
                var instr = dasm.Current;
                dumper.DumpAssemblerLine(image, instr, writer);
                var s = writer.ToString();
                g.DrawString(s, Font, SystemBrushes.ControlText, rc);
            }
            //UpdateScrollbar(addrStart, addrFinal);
            //Debug.Print("Client rect: {0}", rcClient);
            //Debug.Print("scrool rect: {0}", vscroll.Bounds);
        }


        private void UpdateScrollbar(Address addrBegin, Address addrEnd)
        {
            //vscroll.Enabled = true;
            //vscroll.Minimum = 0;
            //vscroll.Maximum = Image.Bytes.Length - 0x10;
            //vscroll.LargeChange = 100;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Debug.Print("Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue);
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            Debug.Print("Press: ch:{0}", e.KeyChar);
            base.OnKeyPress(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DumpAssembler(e.Graphics);
        }

        //public Control CreateControl()
        //{
        //    txtDisassembly = new TextBox
        //    {
        //        Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0))),
        //        Multiline = true,
        //        Name = "txtDisassembly",
        //        ReadOnly = true,
        //        WordWrap = false,
        //    };
        //    txtDisassembly.Resize += txtDisassembly_Resize;
        //    txtDisassembly.KeyDown += txtDisassembly_KeyDown;
        //    txtDisassembly.KeyPress += txtDisassembly_KeyPress;
        //    return txtDisassembly;
        //}

	}
}
