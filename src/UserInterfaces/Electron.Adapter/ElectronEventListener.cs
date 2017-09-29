using System;
using Reko.Core;
using Reko.Core.Services;

namespace Reko.Gui.Electron.Adapter
{
    public class ElectronEventListener : DecompilerEventListener
    {
        private IDiagnosticsService diagnosticSvc;
        private ElectronDecompilerDriver driver;

        public ElectronEventListener(IDiagnosticsService diagnosticSvc)
        {
            this.diagnosticSvc = diagnosticSvc;
        }

        public ElectronEventListener(ElectronDecompilerDriver driver)
        {
            this.driver = driver;
        }

        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        public ICodeLocation CreateBlockNavigator(Program program, Block block)
        {
            return new NullCodeLocation(string.Format("{0}:{1}", block.Procedure.Name, block.Name));
        }

        public ICodeLocation CreateJumpTableNavigator(Program program, Address addrIndirectJump, Address addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
        }

        public ICodeLocation CreateProcedureNavigator(Program program, Procedure proc)
        {
            return new NullCodeLocation(string.Format("{0}", proc));
        }

        public ICodeLocation CreateStatementNavigator(Program program, Statement stm)
        {
            return new NullCodeLocation(string.Format("{0}:{1}:{2}:{3} {4}",
                stm.Block.Procedure.Name,
                stm.Block.Name,
                stm.Block.Statements.IndexOf(stm),
                stm.LinearAddress,
                stm.Instruction));

        }

        public void Error(ICodeLocation location, string message)
        {
            Console.WriteLine("ERR:  {0} {1}", location, message);
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Console.WriteLine("ERR:  {0} {1} {2}", location, message, ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Console.WriteLine("ERR:  {0} {1}", location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Console.WriteLine("ERR:  {0} {1} {2}", location, string.Format(message, args), ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        public void Info(ICodeLocation location, string message)
        {
            Console.WriteLine("Info: {0} {1}", location, message);
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Console.WriteLine("Info: {0} {1}", location, string.Format(message, args));
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
            Console.WriteLine("Warn: {0} {1}", location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Console.WriteLine("Warn: {0} {1}", location, string.Format(message, args));
        }
    }
}