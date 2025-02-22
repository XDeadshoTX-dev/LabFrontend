using LabBackend.Utils.Abstract;
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
        }

        private bool IsValidCondition(string data, ref string sanitizedData)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, @"^[a-zA-Z_]\w*(==|<)\d+$"))
            {
                var match = Regex.Match(sanitizedData, @"^([a-zA-Z_]\w*)(==|<)(\d+)$");

                string comparisonOperator = match.Groups[2].Value;

                int number = int.Parse(match.Groups[3].Value);

                if (number > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Execute(int deep)
        {
            string sanitizedData = string.Empty;
            if (!IsValidCondition(this.Content, ref sanitizedData))
            {
                Console.WriteLine("Invalid condition format");
                return;
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
                    this.Code = @$"if {variableName} {usedDelimiter} {value}";
                    break;
                case "c":
                case "c++":
                case "c#":
                case "java":
                    this.Code = @$"if ({variableName} {usedDelimiter} {value})
{{
}}";
                    break;
                default:
                    Console.WriteLine("Unknown programming language");
                    return;
            }
            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);
        }
    }
}
