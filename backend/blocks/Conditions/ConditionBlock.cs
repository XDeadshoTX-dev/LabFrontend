using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Conditions
{
    // Клас для умовного блоку V==C або V<C
    public class ConditionBlock : AbstractBlock
    {
        public ConditionBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "Condition";
        }

        private bool IsValidCondition(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z_]\w*(==|<)\d+$");
        }

        public override void Execute(int amountTabs)
        {
            if (!IsValidCondition(this.Content))
            {
                Console.WriteLine("Invalid condition format");
                return;
            }

            Console.WriteLine($"Executing {this.Id} \"{this.Name}\": {this.Content}");

            string condition = "";

            switch (this.Language)
            {
                case "python":
                    condition = $"{new string('\t', amountTabs)}if {this.Content}:";
                    break;
                case "c":
                case "c++":
                case "c#":
                case "java":
                    condition = $"{new string('\t', amountTabs)}if ({this.Content}) {{";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return;
            }

            Console.WriteLine(condition);
        }
    }
}
