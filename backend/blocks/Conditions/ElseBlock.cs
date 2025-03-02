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
        public ElseBlock(string languageCode) : base(languageCode, "")
        {
            Name = "ElseBlock";
            PatternValidation = @"^[a-zA-Z_]\w*$";
        }
        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            switch (this.Language)
            {
                case "c":
                case "c++":
                case "c#":
                    this.Code = @"else
{
}";
                    break;
                case "python":
                    this.Code = @"else:";
                    break;
                case "java":
                    this.Code = @"else {
}";
                    break;
            }

            string fileContent = ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            WriteAllText(updatedContent);

            return $"{Name} success";
        }
        public override string ExecuteMultithread(int deep, Stack<string> bufferVariables)
        {
            switch (this.Language)
            {
                case "c":
                case "c++":
                case "c#":
                    this.Code = @"else
{
}";
                    break;
                case "python":
                    this.Code = @"else:";
                    break;
                case "java":
                    this.Code = @"else {
}";
                    break;
            }

            return this.Code;
        }
        public override string ExecuteValidation(Stack<string> bufferVariables)
        {
            return $"{this.Name} success";
        }
    }
}
