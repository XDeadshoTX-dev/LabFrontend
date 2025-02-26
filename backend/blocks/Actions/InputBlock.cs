using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class InputBlock : AbstractBlock
    {
        public InputBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "InputBlock";
            this.PatternValidation = @"^[a-zA-Z_]\w*$";
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

                if (bufferVariables.Contains(match.Groups[0].Value))
                {
                    throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Variable \"{sanitizedData}\" exists, change variable name");
                }

                return true;
            }

            return false;
        }

        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Wrong pattern!");
            }

            switch (this.Language)
            {
                case "c":
                    this.Code = @$"int {sanitizedData};
fgets(input, sizeof(input), stdin);
{sanitizedData} = atoi(input);";
                    break;
                case "c++":
                    this.Code = @$"int {sanitizedData};
std::cin >> {sanitizedData};";
                    break;
                case "c#":
                    this.Code = $"int {sanitizedData} = int.Parse(Console.ReadLine());";
                    break;
                case "python":
                    this.Code = $"{sanitizedData} = int(input())";
                    break;
                case "java":
                    this.Code = $"{sanitizedData} = Integer.parseInt(scan.nextLine());";
                    break;
            }

            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);

            return sanitizedData;
        }
    }
}
