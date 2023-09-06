using System;
using System.Globalization;

namespace Reko.Arch.OpenRISC.Aeon.Assembler
{
    public class AssemblerDiagnostics
    {
        public AssemblerDiagnostics(string filename)
        {
            this.Filename = filename;
        }

        public string Filename { get; }
        public int LineNumber { get; set; }
        public bool Failed { get; private set; }

        public void Error(string  message)
        {
            this.Failed = true;
            WriteDiagnostic("error", message);
        }

        private void WriteDiagnostic(string type, string message)
        {
            Console.WriteLine("{0}({1}): {2}: {3}", Filename, LineNumber, type, message);
        }
    }
}