using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LabBackend.Blocks.Actions
{
    // Клас для присвоєння значення V=C
    public class ConstantAssignmentBlock : AbstractBlock
    {
        public ConstantAssignmentBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "ConstantAssignmentBlock";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, @"^([a-zA-Z_]\w*)=(\d+)$"))
            {
                var match = Regex.Match(sanitizedData, @"^([a-zA-Z_]\w*)=(\d+)$");
                int number = int.Parse(match.Groups[2].Value);

                return number >= 0;
            }

            return false;
        }

        public override void Execute(int deep)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData))
            {
                Console.WriteLine("Invalid assignment format");
                return;
            }

            string[] parts = this.Content.Split('=');
            string variableName = parts[0].Trim();
            string value = parts[1].Trim();

            switch (this.Language)
            {
                case "c":
                case "c++":
                case "c#":
                case "java":
                    this.Code = $"{GetIndent(deep + 1)}int {variableName} = {value};";
                    break;
                case "python":
                    this.Code = $"{GetIndent(deep)}{variableName} = {value}";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return;
            }
            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(fileContent, this.Code);
            this.WriteAllText(updatedContent);
        }
    }
}
