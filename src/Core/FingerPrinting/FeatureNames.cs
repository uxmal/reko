using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.FingerPrinting;

/// <summary>
/// Names of different features Reko is able to extract from procedures.
/// </summary>
public static class FeatureNames
{
    /// <summary>
    /// The accumulated total size of the procedure in bytes.
    /// </summary>
    public const string ProcedureSize = "procsize";

    /// <summary>
    /// A hash computed from the machine instructions of the procedure.
    /// </summary>
    public const string MachineInstructionHash = "mihash";

    /// <summary>
    /// A hash computed from the intermediate representation of the procedure.
    /// </summary>
    public const string IRHash = "irhash";
}
