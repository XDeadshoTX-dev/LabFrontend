using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class StartBlock : AbstractBlock
    {
        public StartBlock(string languageCode) : base(languageCode, "")
        {
            this.name = "start";
        }
        public override void Execute()
        {
            string code = "";

            switch (language.ToLower())
            {
                case "c++":
                    code = "#include <iostream>\n" +
                           "using namespace std;\n" +
                           "\n" +
                           "int main()\n" +
                           "{\n" +
                           "    cout << \"Hello World!\\n\";\n" +
                           "    return 0;\n" +
                           "}";
                    break;

                case "c#":
                    code = "using System;\n" +
                           "\n" +
                           "class Program\n" +
                           "{\n" +
                           "    static void Main(string[] args)\n" +
                           "    {\n" +
                           "        Console.WriteLine(\"Hello World!\");\n" +
                           "    }\n" +
                           "}";
                    break;

                case "python":
                    code = "if __name__ == '__main__':\n" +
                           "    print(\"Hello World!\")";
                    break;

                default:
                    Console.WriteLine("Unsupported language.");
                    return;
            }

            string fileName = $"GeneratedCode.{GetFileExtension()}";
            File.WriteAllText(fileName, code);
        }
        private string GetFileExtension()
        {
            switch (language.ToLower())
            {
                case "c++":
                    return "cpp";
                case "c#":
                    return "cs";
                case "python":
                    return "py";
                default:
                    return "txt";
            }
        }
    }
}
