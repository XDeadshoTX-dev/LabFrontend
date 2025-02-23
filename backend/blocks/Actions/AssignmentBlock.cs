using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class AssignmentBlock : AbstractBlock
    {
        public AssignmentBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "AssignmentBlock";
            this.PatternValidation = @"^([a-zA-Z_]\w*)=([a-zA-Z_]\w*)$";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData, List<string> bufferVariables)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, this.PatternValidation))
            {
                return true;
            }

            return false;
        }
        public override string Execute(int deep, List<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                Console.WriteLine("Invalid assignment format");
                return "error";
            }

            string[] variables = this.Content.Split('=');
            if (variables.Length != 2)
            {
                throw new ArgumentException("Data should be in format 'V1=V2'");
            }
            string v1 = variables[0].Trim();
            string v2 = variables[1].Trim();

            switch (this.Language)
            {
                case "c":
                    this.Code = $"strcpy({v1}, {v2});";
                    break;
                case "c++":
                case "c#":
                case "java":
                    this.Code = $"{v1} = {v2};";
                    break;
                case "python":
                    this.Code = $"{v1} = {v2}";
                    break;
                default:
                    throw new NotSupportedException($"Programming language '{this.Language}' is not supported.");
            }

            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);

            return "success";
        }
    }
}
