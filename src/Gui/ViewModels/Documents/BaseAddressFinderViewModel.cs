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

using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Documents
{
    public class BaseAddressFinderViewModel : Reko.Gui.Reactive.ChangeNotifyingObject
    {
        private IServiceProvider services;
        private Program program;
        private readonly string startText;
        private readonly string stopText;
        private Task? finderTask;

        public BaseAddressFinderViewModel(
            IServiceProvider services,
            Program program,
            string startText,
            string stopText)
        {
            this.services = services;
            this.program = program;
            this.startText = startText;
            this.stopText = stopText;
            this.startStopButtonText = startText;
            this.Results = new ObservableCollection<BaseAddressResult>();
            this.baseAddress = "";
        }

        public bool ByString
        {
            get => byString;
            set => this.RaiseAndSetIfChanged(ref byString, value);
        }
        private bool byString;

        public bool ByProlog
        {
            get => byProlog;
            set => this.RaiseAndSetIfChanged(ref byProlog, value);
        }
        private bool byProlog;

        public int ByStringProgress
        {
            get => byStringProgress;
            set => this.RaiseAndSetIfChanged(ref byStringProgress, value);
        }
        private int byStringProgress;

        public int ByPrologProgress
        {
            get => byPrologProgress;
            set => this.RaiseAndSetIfChanged(ref byPrologProgress, value);
        }
        private int byPrologProgress;

        public string StartStopButtonText
        {
            get => startStopButtonText;
            set => this.RaiseAndSetIfChanged(ref startStopButtonText, value);
        }
        public string startStopButtonText;

        public ObservableCollection<BaseAddressResult> Results { get; }

        public string BaseAddress
        {
            get => baseAddress;
            set => this.RaiseAndSetIfChanged(ref baseAddress, value);
        }
        public string baseAddress;

        public async Task StartStopFinder()
        {
            if (finderTask is null)
            {
                this.StartStopButtonText = stopText;
                await Task.Run(StartFinder_work);
                this.StartStopButtonText = startText;
                this.finderTask = null;
            }
            else
            {
                // A task is running. Cancel it.
                this.finderTask = null;
            }
        }

        private void StartFinder_work()
        {
            if (program.SegmentMap.Segments.Values.FirstOrDefault()?.MemoryArea
                is not ByteMemoryArea mem)
            {
                return;
            }

            IBaseAddressFinder s = new FindBaseString(
                program.Architecture.Endianness,
                mem,
                NullProgressIndicator.Instance);
            var results = s.Run();
            this.Results.Clear();
            var arch = program.Architecture;
            foreach (var result in results)
            {
                this.Results.Add(new BaseAddressResult
                {
                    Address = RenderAddress(result.Address, arch),
                    Confidence = result.Confidence
                });
            }
        }

        private static string RenderAddress(ulong address, IProcessorArchitecture arch)
        {
            var bitSize = arch.PointerType.BitSize;
            int digits = arch.DefaultBase switch
            {
                16 => (bitSize + 3) / 4,
                8 => (bitSize + 2) / 3,
                _ => throw new NotImplementedException($"Unimplemented bit size {arch.DefaultBase}.")
            };
            var sAddr = Convert.ToString(
                (long) address,
                arch.DefaultBase)
                .ToUpperInvariant();
            return Convert.ToString(sAddr).PadLeft(digits, '0');
        }
    }

    public class BaseAddressResult
    {
        public string? Address { get; set; }

        public int Confidence { get; set; }
    }
}
