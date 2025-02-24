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
            this.Name = "start";
        }
        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            switch (this.Language.ToLower())
            {
                case "c":
                    this.Code = @"#include <stdio.h>

int main()
{
}
";
                    break;
                case "c++":
                    this.Code = @"#include <iostream>
using namespace std;

int main()
{
}
";
                    break;
                case "c#":
                    this.Code = @"using System;

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
                    this.Code = @"public class Main {
    public static void main(String[] args) {
    }
}
";
                    break;
                case "python":
                    this.Code = @"if __name__ == '__main__':
";
                    break;
                default:
                    this.Code = "";
                    break;
            }

            File.WriteAllText(this.FileName, Code);

            return $"{this.Name} success";
        }
    }
}
