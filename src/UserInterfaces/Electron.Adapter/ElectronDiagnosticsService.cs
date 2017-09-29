using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using System.IO;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronDiagnosticsService : IDiagnosticsService
    {
        private TextWriter writer;

        public ElectronDiagnosticsService(TextWriter textWriter)
        {
            this.writer = textWriter;
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic diagnostic)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            writer.WriteLine("error: {0}", message);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            writer.WriteLine("error: {0} {1}", message, ex);
        }

        public void Error(ICodeLocation location, string message)
        {
            writer.WriteLine("{0}: error: {1}", location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            writer.WriteLine("{0}: error: {1} {2}", location, message, GetExceptionMessage(ex));
            writer.WriteLine(ex.StackTrace);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            writer.WriteLine("{0}: error: {1} {2}", location, string.Format(message, args), GetExceptionMessage(ex));
            writer.WriteLine(ex.StackTrace);
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, params object[] args)
        {
            writer.WriteLine("warning: {0}", string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            writer.WriteLine("{0}: warning: {1}", location, string.Format(message, args));
        }

        public void Inform(string message)
        {
            writer.WriteLine(message);
        }

        public void Inform(string message, params object[] args)
        {
            writer.WriteLine(message, args);
        }

        public void Inform(ICodeLocation location, string message)
        {
            writer.WriteLine("{0}: {1}", location, message);
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            Inform(location, string.Format(message, args));
        }

        public void ClearDiagnostics()
        {
            throw new NotImplementedException();
        }

        private StringBuilder GetExceptionMessage(Exception ex)
        {
            var sb = new StringBuilder();
            var fmt = "{0}";
            while (ex != null)
            {
                sb.AppendFormat(fmt, ex.Message);
                fmt = " {0}";
                ex = ex.InnerException;
            }
            return sb;
        }
    }
}