using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfApp2.backend.schemas.@abstract;

namespace WpfApp2.backend.schemas.translate
{
    class CPlusPlusTranslateSchema : AbstractTranslateSchema
    {
        public CPlusPlusTranslateSchema(int deep, AbstractBlock block)
        {
            this.deepSchema = deep;
            this.block = block;

            this.regOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            string amountSpaces = this.block.GetIndent(this.deepSchema);
            pattern = $@"({amountSpaces}\{{)(.*?)(\n{amountSpaces}\}})";
        }
        public override string InsertCode(Match match, string fileContent, string code)
        {
            string openingBrace = match.Groups[1].Value.Trim();  // {
            string content = match.Groups[2].Value.Trim();       // raw content
            string closingBrace = match.Groups[3].Value.Trim();  // }

            string newContent = string.IsNullOrEmpty(content)
                ? $"\n{block.GetIndentCode(this.deepSchema + 1)}\n"
                : $"\n{block.GetIndent(this.deepSchema + 1)}{content}\n{block.GetIndentCode(this.deepSchema + 1)}\n";

            return $"{block.GetIndent(this.deepSchema)}" +
                   $"{openingBrace}" +
                   $"{newContent}" +
                   $"{block.GetIndent(this.deepSchema)}{closingBrace}";
        }
    }
}
