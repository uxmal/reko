#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

using System;
using Reko.Libraries.Microchip;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// A PIC architecture options specifying PIC target model and potential instruction set execution mode.
    /// </summary>
    public class PICArchitectureOptions
    {
        public PICArchitectureOptions() {
            ProcessorModel = null!;
            LoaderType = null!;
        }

        public PICArchitectureOptions(IPICProcessorModel processorModel, PICExecMode execMode, string ldrType)
        {
            ProcessorModel = processorModel ?? throw new ArgumentNullException(nameof(processorModel));
            PICExecutionMode = execMode;
            LoaderType = ldrType;
        }

        public PICArchitectureOptions(string picName, PICExecMode execMode, string ldrType = "raw")
            : this(PICProcessorModel.GetModel(picName)!, execMode, ldrType)
        {
        }

        public PICArchitectureOptions(PICArchitectureOptionsPicker picker)
        {
            if (picker is null)
                throw new ArgumentNullException(nameof(picker));
            ProcessorModel = PICProcessorModel.GetModel(picker.PICName)!;
            PICExecutionMode = (picker.AllowExtended ? PICExecMode.Extended : PICExecMode.Traditional);
            LoaderType = picker.LoaderType;
        }

        /// <summary>
        /// Gets or sets the processor mode builders.
        /// </summary>
        public IPICProcessorModel ProcessorModel { get; set; }

        /// <summary>
        /// Gets or sets the PIC instruction set execution mode.
        /// </summary>
        public PICExecMode PICExecutionMode { get; set; }


        /// <summary>
        /// Gets or sets the binary file format used for loading the image.
        /// </summary>
        public string LoaderType { get; set; }

        public override string ToString() => $"{ProcessorModel.PICName},{PICExecutionMode}";

    }

    /// <summary>
    /// Class used to get the architecture options from the user.
    /// </summary>
    public class PICArchitectureOptionsPicker
    {
        public PICArchitectureOptionsPicker()
        {
            PICName = null!;
        }

        public PICArchitectureOptionsPicker(PICArchitectureOptions opts)
        {
            if (opts is null)
                throw new ArgumentNullException(nameof(opts));
            PICName = opts.ProcessorModel.PICName;
            AllowExtended = opts.PICExecutionMode == PICExecMode.Extended;
            LoaderType = opts.LoaderType;
        }

        /// <summary>
        /// Name of the PIC.
        /// </summary>
        public string PICName { get; set; }

        /// <summary>
        /// True to permit decoding of Extended Execution mode of PIC18, false otherwise.
        /// This is a hint in case the configuration fuses are not part of the binary image.
        /// </summary>
        public bool AllowExtended { get; set; }

        /// <summary>
        /// Gets or sets the binary file format.
        /// </summary>
        public string LoaderType { get; set; } = "raw";

        public override string ToString() => $"{PICName}{(AllowExtended ? ",Extended" : "")}";

    }

}
