using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
    [TestFixture]
    public class ProcedureDialogInteractorTests
    {
        private ProcedureDialogInteractor interactor;
        private SerializedProcedure proc;
 
        [SetUp]
        public void Setup()
        {
            proc = new SerializedProcedure();
            interactor = new ProcedureDialogInteractor(proc);
        }

        [Test]
        public void OnProcedure()
        {
            proc.Name = " foo ";
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual("foo", dlg.ProcedureName.Text); 
            }
        }

        [Test]
        public void ReturnValue()
        {
            proc.Name = "x";
            proc.Signature = new SerializedSignature();
            CreateReturnValue(proc.Signature);
            //proc.Signature.Arguments = new SerializedArgument[1];
            //proc.Signature.Arguments[0] = new SerializedArgument();
            //proc.Signature.Arguments[0].Name = "arg0";
            //proc.Signature.Arguments[0].Kind = new SerializedStackVariable(4);
            //proc.Signature.Arguments[0].Type = "int32";

            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                Assert.AreEqual(1, dlg.ArgumentList.Items.Count);
                Assert.AreEqual("<Return value>", dlg.ArgumentList.Items[0].Text);
            }
        }

        private static void CreateReturnValue(SerializedSignature sig)
        {
            sig.ReturnValue = new SerializedArgument();
            sig.ReturnValue.Kind = new SerializedRegister("eax");
            sig.ReturnValue.Type = "int32";
        }

        [Test]
        public void SelectReturnValue()
        {
            proc.Name = "x";
            proc.Signature = new SerializedSignature();
            CreateReturnValue(proc.Signature);
            using (ProcedureDialog dlg = interactor.CreateDialog())
            {
                dlg.Show();
                dlg.ArgumentList.SelectedIndices.Add(0);
                Assert.AreSame(dlg.ArgumentList.Items[0].Tag, dlg.ArgumentProperties.SelectedObject);
            }
        }
    }
}
