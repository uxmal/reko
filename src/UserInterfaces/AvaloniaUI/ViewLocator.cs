using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model.Core;
using Reko.Gui;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using System;
using System.Diagnostics;

namespace Reko.UserInterfaces.AvaloniaUI
{
    /// <summary>
    /// This class is used as a Data template to instantiate views from their
    /// corresponding view models. It assumes all view model classes names end
    /// with "...ViewModel" and the corresponding view models have the name
    /// ending with "...View".
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;


        /// <summary>
        /// Given a view model object <see cref="data"/>, creates an instance
        /// of the corresponding View class. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type is { })
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase || data is IDockable || data is IWindowPane;
        }
    }
}
