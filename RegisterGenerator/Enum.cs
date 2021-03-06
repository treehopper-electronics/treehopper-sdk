﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterGenerator
{
    public class Enum
    {
        // These properties come in from de-serializing the JSON
        public bool IsPublic { get; set; }
        public Dictionary<string, int?> Values { get; set; }
        public string PluralizedName { get; set; }


        public List<KeyValuePair<string, EnumVal>> ValuesList = new List<KeyValuePair<string, EnumVal>>();
        public void Preprocess()
        {
            if (Values == null) return;
            int offset = 0;
            foreach (var key in Values.Keys.ToList())
            {
                if (Values[key].HasValue)
                {
                    offset = Values[key].Value + 1;
                }
                else
                {
                    Values[key] = offset;
                    offset += 1;
                }

                ValuesList.Add(new KeyValuePair<string, EnumVal>(key, new EnumVal(Values[key].Value)));
            }

            // This helps the template know if it's the last element so it can, for example, omit a terminal comma
            ValuesList.Last().Value.Last = true;
        }
    }
}
