using NUnit.Framework;
using Reko.Gui.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Moq;
using Reko.Gui.Services;
using Reko.Scanning;

namespace Reko.UnitTests.Gui.ViewModels.Dialogs
{
    [TestFixture]
    public class FindStringsViewModelTests
    {
        private Program program = default!;
        private Mock<IDialogFactory> dlgFactory = default!;
        private Mock<IDecompilerShellUiService> uiSvc = default!;
        private Mock<ISettingsService> settingsSvc = default!;

        [SetUp]
        public void Setup()
        {
            this.program = new Program();
            this.uiSvc = new Mock<IDecompilerShellUiService>();
            this.settingsSvc = new Mock<ISettingsService>();
            this.dlgFactory = new Mock<IDialogFactory>();
        }

        [Test]
        public void Fsvm_AddToMru()
        {
            var fsvm = new FindStringsViewModel(program, uiSvc.Object, settingsSvc.Object, dlgFactory.Object);
            Assert.AreEqual(1, fsvm.SearchAreasMru.Count);
            Assert.AreEqual("Entire program", fsvm.SearchAreasMru[0].Text);
            fsvm.UpdateSearchAreaMru(new List<SearchArea>
            {
                SearchArea.FromAddressRange(program, Address.Ptr16(0x1000), 0x100) 
            });
            Assert.AreEqual(2, fsvm.SearchAreasMru.Count);
            Assert.AreEqual("[1000-10FF]", fsvm.SearchAreasMru[0].Text);
            fsvm.UpdateSearchAreaMru(new List<SearchArea>
            {
                SearchArea.FromAddressRange(program, Address.Ptr16(0x2000), 0x100)
            });

            Assert.AreEqual(3, fsvm.SearchAreasMru.Count);
            Assert.AreEqual("[2000-20FF]", fsvm.SearchAreasMru[0].Text);
            Assert.AreEqual("[1000-10FF]", fsvm.SearchAreasMru[1].Text);
        }
    }
}
