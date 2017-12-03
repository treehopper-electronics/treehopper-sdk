using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class Library
    {
        public string Name { get; set; }
        public string NameLower => Name.ToLower();
        public string Namespace { get; set; }
        public string NamespaceLower => Namespace.ToLower();
        public string MultiRegisterAccess { get; set; }
        public string MultiRegisterAccessCapitalized => MultiRegisterAccess.ToPascalCase();
        /// <summary>
        /// Unordered dictionary of registers (what's serialized)
        /// </summary>
        public Dictionary<string, Register> Registers { get; set; }

        /// <summary>
        /// Ordered list of registers
        /// </summary>
        public List<Register> RegisterList { get; set; }
        public int FirstReadAddress { get; set; }
        public int FirstWriteAddress { get; set; }
        public int TotalReadBytes { get; set; }
        public int TotalWriteBytes { get; set; }
        public string[] NamespaceFragments { get; set; }

        public void Preprocess()
        {
            if (MultiRegisterAccess == null)
                MultiRegisterAccess = "true";

            NamespaceFragments = Namespace.Split('.');

            foreach (var reg in Registers)
            {
                reg.Value.Name = reg.Key;
            }

            RegisterList = Registers.Values.OrderBy(i => i.AddressNumber).ToList(); // sort by address asc
            foreach (var reg in RegisterList)
            {
                reg.Preprocess();
                reg.LibraryName = Name;

                if (reg.IsReadOnly)
                {
                    if (FirstReadAddress == 0)
                        FirstReadAddress = reg.AddressNumber;

                    TotalReadBytes = (reg.AddressNumber + reg.NumBytes) - FirstReadAddress;
                }

                if (reg.IsWriteOnly)
                {
                    if (FirstWriteAddress == 0)
                        FirstWriteAddress = reg.AddressNumber;

                    TotalWriteBytes = (reg.AddressNumber + reg.NumBytes) - FirstWriteAddress;
                }
            }
        }
    }
}
