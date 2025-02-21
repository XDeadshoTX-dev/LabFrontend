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
            this.Name = "InputBlock";
        }

        private bool IsValidVariableName(string variableName)
        {
            return Regex.IsMatch(variableName, @"^[a-zA-Z_]\w*$");
        }

        public override void Execute(int deep)
        {
            if (!IsValidVariableName(this.Content))
            {
                Console.WriteLine("Invalid variable name format");
                return;
            }

            Console.WriteLine($"Executing {this.Id} \"{this.Name}\": {this.Content}");
            switch (this.Language)
            {
                case "c":
                    this.Code = $"{GetIndent(deep)}scanf(\"%d\", &{this.Content});";
                    break;
                case "c++":
                    this.Code = $"{GetIndent(deep)}cin >> {this.Content};";
                    break;
                case "c#":
                    this.Code = $"{GetIndent(deep)}{this.Content} = int.Parse(Console.ReadLine());";
                    break;
                case "python":
                    this.Code = $"{GetIndent(deep)}{this.Content} = int(input())";
                    break;
                case "java":
                    this.Code = $"{GetIndent(deep)}{this.Content} = Integer.parseInt(scan.nextLine());";
                    break;
            }
        }
    }
}
