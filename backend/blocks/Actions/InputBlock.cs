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

        public override void Execute(int deep)
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
                    this.code = $"{getIndent(deep)}scanf(\"%d\", &{this.content});";
                    break;
                case "c++":
                    this.code = $"{getIndent(deep)}cin >> {this.content};";
                    break;
                case "c#":
                    this.code = $"{getIndent(deep)}{this.content} = int.Parse(Console.ReadLine());";
                    break;
                case "python":
                    this.code = $"{getIndent(deep)}{this.content} = int(input())";
                    break;
                case "java":
                    this.code = $"{getIndent(deep)}{this.content} = Integer.parseInt(scan.nextLine());";
                    break;
            }
        }
    }
}
