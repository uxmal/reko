using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class PhaseProgressInteractorTests
    {
        private PhaseProgressDialog dlg;
        private PhaseProgressInteractor interactor;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Test()
        {
            PhaseProgressInteractor interactor = new TestPhaseProgressInteractor();
        }

        private class TestPhaseProgressInteractor : PhaseProgressInteractor
        {
            protected override void DoWork()
            {
            }
        }
    }
}
