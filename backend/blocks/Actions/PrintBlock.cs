using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    // Клас для команди PRINT V
    public class PrintBlock : AbstractBlock
    {
        public PrintBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "PrintBlock";
        }
        private bool IsValidVariableName(string variableName)
        {
            return Regex.IsMatch(variableName, @"^[a-zA-Z_]\w*$");
        }

        public override void Execute(int amountTabs)
        {
            if (!IsValidVariableName(this.Content))
            {
                Console.WriteLine("Invalid variable name format");
                return;
            }

            Console.WriteLine($"Printing {this.Id} \"{this.Name}\": {this.Content}");
            switch (this.Language)
            {
                case "c":
                    Console.WriteLine($"{new string('\t', amountTabs)}printf(\"%s\", {this.Content});");
                    break;
                case "c++":
                    Console.WriteLine($"{new string('\t', amountTabs)}std::cout << {this.Content} << std::endl;");
                    break;
                case "c#":
                    Console.WriteLine($"{new string('\t', amountTabs)}Console.WriteLine({this.Content});");
                    break;
                case "python":
                    Console.WriteLine($"{new string('\t', amountTabs)}print({this.Content});");
                    break;
                case "java":
                    Console.WriteLine($"{new string('\t', amountTabs)}System.out.println({this.Content});");
                    break;
            }
        }
    }
}
