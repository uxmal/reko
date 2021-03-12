using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    public class UserGlobal
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public SerializedType DataType { get; set; }
        public string? Comment { get; set; }

        public UserGlobal(Address address, string name, SerializedType dataType)
        {
            Address = address;
            Name = name;
            DataType = dataType;
        }

        public static string GenerateDefaultName(Address address) => string.Format("g_{0:X}", address.ToLinear());
    }
}
