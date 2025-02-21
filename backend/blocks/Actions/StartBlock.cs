using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
                case "c":
                    code = @"#include <stdio.h>

int main()
{
    return 0;
}
";
                    break;
                case "c++":
                    code = @"#include <iostream>
using namespace std;

int main()
{
}
";
                    break;
                case "c#":
                    code = @"using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
";
                    break;
                case "java":
                    code = @"public class Main {
    public static void main(String[] args) {
    }
}
";
                    break;
                case "python":
                    code = @"if __name__ == '__main__':
    pass
";
                    break;
                default:
                    code = "";
                    break;
            }
            string fileName = $"GeneratedCode.{this.GetFileExtension()}";
            File.WriteAllText(fileName, code);
        }
    }
}
