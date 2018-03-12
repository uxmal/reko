using System;
using Reko.Core;
using Reko.Core.Services;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronEventListener : DecompilerEventListener
    {
        private IDiagnosticsService diagnosticSvc;

        public ElectronEventListener(IDiagnosticsService diagnosticSvc)
        {
            this.diagnosticSvc = diagnosticSvc;
        }

        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            return JsCodeLocation.ForAddress(program, address);
        }

        public ICodeLocation CreateBlockNavigator(Program program, Block block)
        {
            return JsCodeLocation.ForBlock(program, block);
        }

        public ICodeLocation CreateJumpTableNavigator(Program program, Address addrIndirectJump, Address addrVector, int stride)
        {
            //$TODO: make a JumpTableNavigator
            return JsCodeLocation.ForAddress(program, addrIndirectJump);
        }

        public ICodeLocation CreateProcedureNavigator(Program program, Procedure proc)
        {
            return JsCodeLocation.ForProcedure(program, proc);
        }

        public ICodeLocation CreateStatementNavigator(Program program, Statement stm)
        {
            return JsCodeLocation.ForStatement(program, stm);
        }

        public void Error(ICodeLocation location, string message)
        {
            diagnosticSvc.Error(location, message);
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            diagnosticSvc.Error(location, ex, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Error(location, message, args);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            diagnosticSvc.Error(location, ex, message, args);
        }

        public void Info(ICodeLocation location, string message)
        {
            diagnosticSvc.Inform(location, message);
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Inform(location, message, args);
        }

        public bool IsCanceled()
        {
            return false;   //$TODO
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            //$TODO
        }

        public void ShowStatus(string caption)
        {
            //$TODO
        }

        public void Warn(ICodeLocation location, string message)
        {
            diagnosticSvc.Warn(location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Warn(location, message, args);
        }
    }
}