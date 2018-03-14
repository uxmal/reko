using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Reko.UnitTests.Arch.Microchip
{
    /// <summary>
    /// Samples PIC XML definitions
    /// </summary>
    public static class PICSamples
    {

        private static Dictionary<InstructionSetID, PIC> _samples = new Dictionary<InstructionSetID, PIC>();
        private static Dictionary<InstructionSetID, string> _xmlsamples =
            new Dictionary<InstructionSetID, string>()
            {
                { InstructionSetID.PIC16, "PIC16Sample.xml" },
                { InstructionSetID.PIC16_ENHANCED, "PIC16EnhSample.xml" },
                { InstructionSetID.PIC16_FULLFEATURED, "PIC16EnhV1Sample.xml" },
                { InstructionSetID.PIC18, "PIC18Sample.xml" },
                { InstructionSetID.PIC18_EXTENDED, "PIC18ExtdSample.xml" },
                { InstructionSetID.PIC18_ENHANCED, "PIC18EnhSample.xml" }
            };


        public static PIC GetSample(InstructionSetID instrID)
        {

            if (!_samples.TryGetValue(instrID, out PIC _pic))
            {
                if (!_xmlsamples.TryGetValue(instrID, out string xmlfname))
                    throw new NotImplementedException($"Unsupported instruction set: {instrID}");
                string xmlfpath = Path.Combine(@"Arch/Microchip/Samples", xmlfname);
                if (!File.Exists(xmlfpath))
                    throw new FileNotFoundException($"Missing PIC XML sample file for instruction set: {instrID}", xmlfname);
                try
                {
                    _pic = XDocument.Load(xmlfpath).Root.ToObject<PIC>();
                    _samples.Add(instrID, _pic);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Unable to retrieve sample PIC definition for instruction set: {instrID}", ex);
                }
            }
            return _pic;
        }

    }

}
