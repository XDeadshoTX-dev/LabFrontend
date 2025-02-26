using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp2.backend.blocks.Conditions
{
    public class ElseBlock : AbstractBlock
    {
        public ElseBlock(string languageCode, string data) : base(languageCode, data)
        {
            Name = "ElseBlock";
            PatternValidation = @"^[a-zA-Z_]\w*$";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData, Stack<string> bufferVariables)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, PatternValidation))
            {
                var match = Regex.Match(sanitizedData, PatternValidation);

                string content = match.Groups[0].Value;

                if (!bufferVariables.Contains(content))
                {
                    throw new Exception($"[Type: {Name}; Content: \"{content}\"] Variable \"{content}\" doesn't exists, set accessible variable");
                }

                sanitizedData = content;
                return true;
            }

            return false;
        }
        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            switch (this.Language)
            {
                case "c":
                case "c++":
                case "c#":
                    this.Code = @"else
{{
}}";
                    break;
                case "python":
                    this.Code = @"else:";
                    break;
                case "java":
                    this.Code = @"else {{
}}";
                    break;
            }

            string fileContent = ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            WriteAllText(updatedContent);

            return $"{Name} success";
        }
    }
}
