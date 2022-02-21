using Dock.Model.ReactiveUI.Controls;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class ProjectBrowserViewModel : Tool
    {
        public ProjectBrowserViewModel()
        {
            this.ProjectBrowser = new List<Node>
            {
                new Node("Arch"),
                new Node("Dependencies",
                    new Node("foo.dll"),
                    new Node("bar.dll")),
                new Node("EntryPoints",
                    new Node("_start"),
                    new Node("WinMain")),
            };
        }
        public Project? Project { get; set; }

        public List<Node> ProjectBrowser { get; set; }
    }

    public class Node
    {
        public Node(string name, params Node[] nodes)
        {
            this.Text = name;
            this.Nodes = new(nodes);
        }
            
        public string? Text { get; set; }
        public List<Node> Nodes { get; set; }
    }

}
