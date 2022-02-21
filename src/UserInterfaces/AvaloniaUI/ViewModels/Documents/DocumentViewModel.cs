using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using Dock.Model.ReactiveUI.Controls;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class DocumentViewModel : Document
    {
        public DocumentViewModel()
        {
            this.Procedures = new VirtualList<ProcedureViewModel>(
                i => new ProcedureViewModel(i),
                1000);
        }

        public string? Content { get; set; }

        public IList<ProcedureViewModel> Procedures { get; set; }

    }
}
