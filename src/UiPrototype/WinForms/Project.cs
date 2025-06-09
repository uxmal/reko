using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Reko.UiPrototype.WinForms
{
    public class Project
    {
        public List<ProjectBinary> Binaries;
        public DataType[] DataTypes;
    }

    public class ProjectBinary
    {
        public string FilePath { get; set; }
        public ProgramImage RawBytes { get; set; }

        public Program[] Programs;
    }

    public class ImageFormat
    {
        public ProgramImage ExtractedImage;
        public ImageHeader Header;
        public SortedList<uint, Section> Sections;
    }

    public class ImageHeader
    {
        public ProgramImage Image;
        public uint ImageOffset;
    }

    public class Section
    {
        public string Name;
        public uint ImageOffset;
        public uint Size;
        public ProgramImage Image;
    }

    public class ProgramImage
    {
        public uint Address;
        public byte [] Bytes;
    }

    public class Program
    {
        public ProgramImage RelocatedBytes { get; set; }      // relocated image.

        public ImageMap ImageMap;
        public Dictionary<uint, Procedure> EntryPoints { get; set; } 
        public Dictionary<uint, Procedure> Procedures { get; set; }
        public Dictionary<uint, Annotation> Annotations { get; set; }
    }

    public class Procedure
    {
        public uint LinearAddress;
        
        public string Name { get { return name; } set { name = value; NameChanged.Raise(this); } }
        public event EventHandler NameChanged;
        private string name;

        public bool UserSpecified;

        public FuncType Signature;
        public Frame Frame;
    }

    public class Block
    {
        public uint LinearAddress;
        public List<Block> Pred;
        public List<Statement> Instrs;
        public List<Block> Succ;
    }

    public class Statement { }

    public class ImageMap
    {
        public bool TryGetEnclosing(int offset, out Block b) { b = null; return false; }
        public bool TryGetExact(int offset, out Block b) { b = null; return false; }
    }

    public class Frame
    {
        public StructureType Bindings;
    }

    public class Annotation
    {
        public uint LinearAddress;
        public string Text;
    }

    public interface ProcessorArchitecture
    {
    }

    public static class EventEx
    {
        public static void Raise(this EventHandler eh, object sender)
        {
            if (eh is not null)
                eh(sender, EventArgs.Empty);
        }
    }
}
