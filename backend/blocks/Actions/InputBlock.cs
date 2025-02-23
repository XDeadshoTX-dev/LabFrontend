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
                    this.Code = $"scanf(\"%d\", &{sanitizedData});";
                    break;
                case "c++":
                    this.Code = $"cin >> {sanitizedData};";
                    break;
                case "c#":
                    this.Code = $"{sanitizedData} = int.Parse(Console.ReadLine());";
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
        }
    }
}
