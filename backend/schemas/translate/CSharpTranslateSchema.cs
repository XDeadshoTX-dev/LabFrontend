using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using WpfApp2.backend.schemas.@abstract;

namespace WpfApp2.backend.schemas.translate
{
    class CSharpTranslateSchema : AbstractTranslateSchema
    {
        public CSharpTranslateSchema()
        {
            pattern = @"(static\s+void\s+Main\s*\(.*?\))\s*(\{)(.*?)(\})";
        }
        public override string InsertCode(Match match, string code, AbstractBlock block)
        {
            string mainSignature = match.Groups[1].Value; // static void Main(string[] args)
            string openingBrace = match.Groups[2].Value;  // {
            string content = match.Groups[3].Value.Trim(); // rawContent
            string closingBrace = match.Groups[4].Value;  // }

            string newContent = string.IsNullOrEmpty(content)
                ? $"\n{block.GetIndent(2)}{code}\n"
                : $"\n{block.GetIndent(3)}{content}\n{block.GetIndent(2)}{code}\n";

            return $"{mainSignature}\n{block.GetIndent(2)}" +
                $"{openingBrace}" +
                $"{newContent}" +
                $"{block.GetIndent(2)}{closingBrace}";
        }
    }
}
