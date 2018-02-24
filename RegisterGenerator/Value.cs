using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class Value
    {
        public string Name { get; set; }
        public string CapitalizedName => Name.ToPascalCase();
        public int? Offset { get; set; }
        public int Width { get; set; } = 1;
        public string Bitmask => $"0x{((1 << Width) - 1):X}";
        public bool IsSigned { get; set; }
        public bool Last { get; set; }
        public Enum Enum { get; set; }

        public void Preprocess()
        {
            if (Enum == null) return;
            if (Enum.PluralizedName == null)
            {
                PluralizationService service = PluralizationService.CreateService(CultureInfo.CurrentCulture);
                Enum.PluralizedName = service.Pluralize(CapitalizedName);
            }

            Enum.Preprocess();
        }
    }
}
