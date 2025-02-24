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
        private bool IsValidAssignment(string data, ref string sanitizedData, Stack<string> bufferVariables)
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
                string variableName = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (bufferVariables.Contains(variableName))
                {
                    throw new Exception($"[Type: {this.Name}; Content: \"{variableName} = {value}\"] Variable \"{variableName}\" exists, change variable name");
                }

                int number = int.Parse(value);

                if (number >= 0)
                {
                    sanitizedData = $"{variableName} = {number}";
                    return true;
                }

                throw new Exception($"[Type: {this.Name}; Content: \"{variableName} = {number}\"] The number must be greater than and equal to 0");
            }

            return false;
        }

        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Wrong pattern");
            }

            string[] parts = sanitizedData.Split('=');
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
