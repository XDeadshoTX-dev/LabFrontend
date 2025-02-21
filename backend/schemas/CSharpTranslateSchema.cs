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

namespace WpfApp2.backend.schemas
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
                ? $"\n{block.getIndent(2)}{code}\n"
                : $"\n{block.getIndent(3)}{content}\n{block.getIndent(2)}{code}\n";

            return $"{mainSignature}\n{block.getIndent(2)}" +
                $"{openingBrace}" +
                $"{newContent}" +
                $"{block.getIndent(2)}{closingBrace}";

            //string before = match.Groups[1].Value.Trim();
            //string inside = match.Groups[2].Value.Trim();
            //string after = match.Groups[3].Value;


            //return $"{before}{newInside}{after}";
            //return null;
        }
    }
}
