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
        // These properties come in from the JSON deserialization:

        /// <summary>
        /// The name of the register value
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The offset of the register value (optional)
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// The width of the register value (optional, default 1)
        /// </summary>
        public int Width { get; set; } = 1;

        /// <summary>
        /// Whether the register value is signed or unsigned (optional, default false)
        /// </summary>
        public bool IsSigned { get; set; }

        /// <summary>
        /// The enum associated with this value (optional)
        /// </summary>
        public Enum Enum { get; set; }

        // These properties are calculated during processing

        /// <summary>
        /// Whether this is the last register value
        /// </summary>
        public bool Last { get; set; }
        public string CapitalizedName => Name.ToPascalCase();
        public string Bitmask => $"0x{((1 << Width) - 1):X}";

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
