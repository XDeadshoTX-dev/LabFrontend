using LabBackend.Utils.Abstract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LabBackend.Blocks.Conditions
{
    public class ConditionBlock : AbstractBlock
    {
        public ConditionBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "Condition";
            this.PatternValidation = @"^([a-zA-Z_]\w*)(==|<)(\d+)$";
        }

        private bool IsValidCondition(string data, ref string sanitizedData, Stack<string> bufferVariables)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, this.PatternValidation))
            {
                var match = Regex.Match(sanitizedData, this.PatternValidation);

                string variableName = match.Groups[1].Value;
                string operatorText = match.Groups[2].Value;
                int number = int.Parse(match.Groups[3].Value);

                if (!bufferVariables.Contains(variableName))
                {
                    throw new Exception($"[Type: {this.Name}; Content: \"{variableName} {operatorText} {number}\"] Variable \"{variableName}\" doesn't exists, set accessible variable");
                }

                if (number > 0)
                {
                    sanitizedData = $"{variableName} {operatorText} {number}";
                    return true;
                }

                throw new Exception($"[Type: {this.Name}; Content: \"{variableName} {operatorText} {number}\"] The number must be greater than and equal to 0");
            }

            return false;
        }

        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidCondition(this.Content, ref sanitizedData, bufferVariables))
            {
                string[] messageContent = sanitizedData.Split(new string[] { "<", "=="}, StringSplitOptions.None);
                throw new Exception($"[Type: {this.Name}; Content: \"{messageContent[0]} (< | ==) {messageContent[1]}\"] Wrong pattern");
            }

            string[] delimiters = { "<", "==" };

            string usedDelimiter = null;
            foreach (var delimiter in delimiters)
            {
                if (this.Content.Contains(delimiter))
                {
                    usedDelimiter = delimiter;
                    break;
                }
            }

            string[] parts = this.Content.Split(delimiters, StringSplitOptions.None);
            string variableName = parts[0].Trim();
            string value = parts[1].Trim();

            switch (this.Language)
            {
                case "python":
                    this.Code = @$"if {variableName} {usedDelimiter} {value}:";
                    break;
                case "c":
                case "c++":
                case "c#":
                    this.Code = @$"if ({variableName} {usedDelimiter} {value})
{{
}}";
                    break;
                case "java":
                    this.Code = @$"if ({variableName} {usedDelimiter} {value}) {{
}}";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return "error";
            }

            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);

            return $"{this.Name} success";
        }
    }
}
