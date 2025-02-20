using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    // Клас для присвоєння значення V=C
    public class ConstantAssignmentBlock : AbstractBlock
    {
        public ConstantAssignmentBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.name = "ConstantAssignmentBlock";
        }
        private bool IsValidAssignment(string data)
        {
            return Regex.IsMatch(data, @"^([a-zA-Z_]\w*)=(\d+)$");
        }

        public override void Execute(int amountTabs)
        {
            if (!IsValidAssignment(this.content))
            {
                Console.WriteLine("Invalid assignment format");
                return;
            }

            string[] parts = this.content.Split('=');
            string variableName = parts[0].Trim();
            string value = parts[1].Trim();

            Console.WriteLine($"Executing {this.id} \"{this.name}\": {this.content}");
            switch (this.language)
            {
                case "c":
                case "c++":
                case "c#":
                case "java":
                    Console.WriteLine($"{new string('\t', amountTabs)}int {variableName} = {value};");
                    break;
                case "python":
                    Console.WriteLine($"{new string('\t', amountTabs)}{variableName} = {value}");
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return;
            }
        }
    }
}
