using Reko.Core.Loading;
using Reko.Gui.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Documents
{
    public class SegmentListItemViewModel : ChangeNotifyingObject
    {
        public static SegmentListItemViewModel FromImageSegment(ImageSegment seg)
        {
            return new SegmentListItemViewModel
            {
                Segment = seg,
                Name = seg.Name,
                Address = seg.Address.ToString(),
                End = seg.EndAddress.ToString(),
                //$TODO: PDP-10 and PDP-11 prefer octal
                Length = $"0x{seg.Size:X}",
                Read = seg.Access.HasFlag(AccessMode.Read) ? "R" : "-",
                Write = seg.Access.HasFlag(AccessMode.Write) ? "W" : "-",
                Execute = seg.Access.HasFlag(AccessMode.Execute) ? "X" : "-",
            };
        }

        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? End { get; set; }
        public string? Length { get; set; }
        public string? Read { get; set; }
        public string? Write { get; set; }
        public string? Execute { get; set; }

        public bool IsSelected { get; set; }

        public string Access { get => $"{Read}{Write}{Execute}"; }
        public ImageSegment? Segment { get; set; }
    }
}
