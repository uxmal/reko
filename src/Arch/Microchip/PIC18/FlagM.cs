using System;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// Values that represent PIC18 condition code flags in STATUS register.
    /// </summary>
    [Flags]
    public enum FlagM
    {
        /// <summary>Carry/Borrow flag.</summary>
        C = 1,
        /// <summary>Digit Carry/Borrow flag.</summary>
        DC = 2,
        /// <summary>Zero flag.</summary>
        Z = 4,
        /// <summary>Overflow flag.</summary>
        OV = 8,
        /// <summary>Negative flag.</summary>
        N = 16,
        /// <summary>Power-down flag.</summary>
        PD = 32,
        /// <summary>Time-out flag.</summary>
        TO = 64
    }

}
