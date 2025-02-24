using LabBackend.Utils.Abstract;
using System;
using System.CodeDom;
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
    public class ConstantAssignmentBlock : AbstractBlock
    {
        public ConstantAssignmentBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "ConstantAssignmentBlock";
            this.PatternValidation = @"^([a-zA-Z_]\w*)=(\d+)$";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData, List<string> bufferVariables)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            string sanitized = sanitizeData(data);
            sanitizedData = sanitized;

            if (Regex.IsMatch(sanitizedData, this.PatternValidation))
            {
                var match = Regex.Match(sanitizedData, this.PatternValidation);
                
                if (bufferVariables.Exists(item => item == match.Groups[1].Value))
                {
                    throw new Exception($"[Type: {this.Name}; \"Content: {match.Groups[1].Value} = {match.Groups[2].Value}\"] Variable \"{match.Groups[1].Value}\" exists, change variable name");
                }

                int number = int.Parse(match.Groups[2].Value);

                if (number >= 0)
                {
                    return true;
                }

                throw new Exception($"[Type: {this.Name}; \"Content: {this.Content}\"] The number must be greater than and equal to 0");
            }

            return false;
        }

        public override string Execute(int deep, List<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; \"Content: {this.Content}\"] Wrong pattern");
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
                    this.Code = $"int {variableName} = {value};";
                    break;
                case "python":
                    this.Code = $"{variableName} = {value}";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return "error";
            }

            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);

            return variableName;
        }
    }
}
