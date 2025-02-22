using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class EndBlock : AbstractBlock
    {
        public EndBlock(string languageCode) : base(languageCode, "")
        {
            this.Name = "end";
        }

        public override void Execute(int deep)
        {
            switch (this.Language)
            {
                case "c":
                    this.Code = "exit(0);";
                    break;
                case "c++":
                    this.Code = "exit(0);";
                    break;
                case "c#":
                    this.Code = "Environment.Exit(0);";
                    break;
                case "python":
                    this.Code = "exit()";
                    break;
                case "java":
                    this.Code = "System.exit(0);";
                    break;
                default:
                    this.Code = "// Unsupported language";
                    break;
            }
            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);
        }
    }
}
