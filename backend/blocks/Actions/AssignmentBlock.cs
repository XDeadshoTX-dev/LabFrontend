using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    // Клас для присвоєння значення V1=V2
    public class AssignmentBlock : AbstractBlock
    {
        public AssignmentBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.name = "AssignmentBlock";
        }
        private bool IsValidAssignment(string data)
        {
            return Regex.IsMatch(data, @"^([a-zA-Z_]\w*)=([a-zA-Z_]\w*)$");
        }
        public override void Execute(int amountTabs)
        {
            if (!IsValidAssignment(this.content))
            {
                Console.WriteLine("Invalid assignment format");
                return;
            }

            Console.WriteLine($"Executing {this.id} \"{this.name}\": {this.content}");
            string indent = new string('\t', amountTabs);
            string[] variables = this.content.Split('=');
            if (variables.Length != 2)
            {
                throw new ArgumentException("Data should be in format 'V1=V2'");
            }
            string v1 = variables[0].Trim();
            string v2 = variables[1].Trim();

            StringBuilder code = new StringBuilder();
            switch (this.language)
            {
                case "c":
                    code.AppendLine($"{indent}strcpy({v1}, {v2});");
                    break;
                case "c++":
                case "c#":
                case "java":
                    code.AppendLine($"{indent}{v1} = {v2};");
                    break;
                case "python":
                    code.AppendLine($"{indent}{v1} = {v2}");
                    break;
                default:
                    throw new NotSupportedException($"Programming language '{this.language}' is not supported.");
            }
            Console.WriteLine(code.ToString());
        }
    }
}
