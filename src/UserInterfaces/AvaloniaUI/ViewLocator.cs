using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model.Core;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using System;

namespace Reko.UserInterfaces.AvaloniaUI
{
    /// <summary>
    /// This class is used to map back to a particular view base on the name
    /// of its ViewModel.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase || data is IDockable;
        }
    }
}
