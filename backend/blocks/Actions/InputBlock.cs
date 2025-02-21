using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    // Клас для команди INPUT V
    public class InputBlock : AbstractBlock
    {
        public InputBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.name = "InputBlock";
        }

        private bool IsValidVariableName(string variableName)
        {
            return Regex.IsMatch(variableName, @"^[a-zA-Z_]\w*$");
        }

        public override void Execute(int amountTabs)
        {
            if (!IsValidVariableName(this.content))
            {
                Console.WriteLine("Invalid variable name format");
                return;
            }

            Console.WriteLine($"Executing {this.id} \"{this.name}\": {this.content}");
            switch (this.language)
            {
                case "c":
                    Console.WriteLine($"{new string('\t', amountTabs)}scanf(\"%d\", &{this.content});");
                    break;
                case "c++":
                    Console.WriteLine($"{new string('\t', amountTabs)}cin >> {this.content};");
                    break;
                case "c#":
                    Console.WriteLine($"{new string('\t', amountTabs)}{this.content} = int.Parse(Console.ReadLine());");
                    break;
                case "python":
                    Console.WriteLine($"{new string('\t', amountTabs)}{this.content} = int(input())");
                    break;
                case "java":
                    Console.WriteLine($"{new string('\t', amountTabs)}{this.content} = Integer.parseInt(scan.nextLine());");
                    break;
            }
        }
    }
}
