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
            this.name = "ConstantAssignmentBlock";
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
            if (!IsValidAssignment(this.content, ref sanitizedData))
            {
                Console.WriteLine("Invalid assignment format");
                return;
            }

            string[] parts = this.content.Split('=');
            string variableName = parts[0].Trim();
            string value = parts[1].Trim();

            switch (this.language)
            {
                case "c":
                case "c++":
                case "c#":
                case "java":
                    this.code = $"{getIndent(deep + 1)}int {variableName} = {value};";
                    break;
                case "python":
                    this.code = $"{getIndent(deep)}{variableName} = {value}";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return;
            }
            string fileContent = this.readAllText();
            string updatedContent = InsertCodeIntoMain(fileContent, this.code);
            this.writeAllText(updatedContent);
        }
    }
}
