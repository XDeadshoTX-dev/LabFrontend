using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class PrintBlock : AbstractBlock
    {
        public PrintBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "PrintBlock";
            this.PatternValidation = @"^[a-zA-Z_]\w*$";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData)
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

        public override void Execute(int deep)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData))
            {
                Console.WriteLine("Invalid variable name format");
                return;
            }

            switch (this.Language)
            {
                case "c":
                    this.Code = $"printf(\"%s\", {sanitizedData});";
                    break;
                case "c++":
                    this.Code = $"std::cout << {sanitizedData} << std::endl;";
                    break;
                case "c#":
                    this.Code = $"Console.WriteLine({sanitizedData});";
                    break;
                case "python":
                    this.Code = $"print({sanitizedData});";
                    break;
                case "java":
                    this.Code = $"System.out.println({sanitizedData});";
                    break;
            }
            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);
        }
    }
}
