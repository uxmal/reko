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
        private Func<object, Task<object>> jsNotify;

        public ElectronDiagnosticsService( Func<object, Task<object>> jsNotify)
        {
            this.jsNotify = jsNotify;
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic diagnostic)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            WriteToJs("error", null, message);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            WriteToJs("error", null, string.Format("{0} {1}", message, ex));
        }

        public void Error(ICodeLocation location, string message)
        {
            WriteToJs("error", location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Error(location, string.Format("{0} {1}", message, GetExceptionMessage(ex)));
            //$TODO: stack trace
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Error(location, ex, string.Format(message, args));
            //$TODO: stack trace
        }

        public void Warn(string message)
        {
            WriteToJs("warning", null, message);
        }

        public void Warn(string message, params object[] args)
        {
            WriteToJs("warning", null, string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            WriteToJs("warning", location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Inform(string message)
        {
            WriteToJs("info", null, message);
        }

        public void Inform(string message, params object[] args)
        {
            WriteToJs("info", null, string.Format(message, args));
        }

        public void Inform(ICodeLocation location, string message)
        {
            WriteToJs("info", location, message);
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            Inform(location, string.Format(message, args));
        }

        private void WriteToJs(string status, ICodeLocation location, string message)
        {
            var jsLoc = location as JsCodeLocation;
            this.jsNotify(
                new {
                    status = status,
                    location = jsLoc != null ? jsLoc.Text : null,
                    message = message
                });
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