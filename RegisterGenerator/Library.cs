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
        public List<Register> Registers { get; set; }

        public void Preprocess()
        {
            for(int i=0;i<Registers.Count;i++)
                Registers[i].Preprocess();
        }
    }
}
