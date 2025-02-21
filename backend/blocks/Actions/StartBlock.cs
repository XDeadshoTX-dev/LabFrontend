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
        public override void Execute(int deep)
        {
            switch (this.language.ToLower())
            {
                case "c":
                    this.code = @"#include <stdio.h>

int main()
{
}
";
                    break;
                case "c++":
                    this.code = @"#include <iostream>
using namespace std;

int main()
{
}
";
                    break;
                case "c#":
                    this.code = @"using System;

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
                    this.code = @"public class Main {
    public static void main(String[] args) {
    }
}
";
                    break;
                case "python":
                    this.code = @"if __name__ == '__main__':
    pass
";
                    break;
                default:
                    this.code = "";
                    break;
            }
            File.WriteAllText(this.fileName, code);
        }
    }
}
