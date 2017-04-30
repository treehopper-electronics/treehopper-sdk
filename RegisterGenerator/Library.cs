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
        public string Namespace { get; set; }
        public bool MultiRegisterAccess { get; set; } = true;
        public List<Register> Registers { get; set; }
        public int FirstReadAddress { get; set; }
        public int FirstWriteAddress { get; set; }
        public int TotalReadBytes { get; set; }
        public int TotalWriteBytes { get; set; }

        public void Preprocess()
        {
            Registers = Registers.OrderBy(i => i.AddressNumber).ToList(); // sort by address asc
            foreach (var reg in Registers)
            {
                reg.Preprocess();

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
