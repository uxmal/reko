using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui.Reactive;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Documents
{
    public class LowLevelViewModel : ChangeNotifyingObject
    {
        private readonly IServiceProvider services;

        public LowLevelViewModel(IServiceProvider services)
        {
            this.services = services;
            this.Architectures = BuildArchitectureViewModel();
        }


        public ListOption[] Architectures { get; set; }

        private ListOption[] BuildArchitectureViewModel()
        {
            var result = new List<ListOption>();
            result.Add(new ListOption("(Default)", null));
            var cfgSvc = services?.GetService<IConfigurationService>();
            if (cfgSvc is not null)
            {
                foreach (var arch in cfgSvc.GetArchitectures().OrderBy(a => a.Description))
                {
                    var choice = new ListOption(arch.Description ?? arch.Name!, arch.Name);
                    result.Add(choice);
                }
            }
            return result.ToArray();
        }
    }
}
