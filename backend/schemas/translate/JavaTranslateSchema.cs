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
    class JavaTranslateSchema : AbstractTranslateSchema
    {
        public JavaTranslateSchema(int deep, AbstractBlock block)
        {
            this.deepSchema = deep + 1;
            this.block = block;

            this.regOptions = RegexOptions.Multiline;
            int currentBrace = this.deepSchema + 1;
            int amountSpaces = 4 * this.deepSchema;
            pattern = $@"^\s{{{amountSpaces}}}(?<before>[^\{{]*)\{{\s*\n(?<rawContent>[\s\S]*?)^\s{{{amountSpaces}}}\}}";
        }
        public override string InsertCode(Match match, string fileContent, string code)
        {
            string beforeBrace = match.Groups["before"].Value;
            string openingBrace = "{";
            string rawContent = match.Groups["rawContent"].Value;
            string closingBrace = "}";

            string modifiedContent = string.IsNullOrEmpty(rawContent)
                                     ? $"{this.block.GetIndent(this.deepSchema)}" + beforeBrace + openingBrace + "\n" +
                                       $"{this.block.GetIndentCode(this.deepSchema + 1)}" + "\n" +
                                       this.block.GetIndent(this.deepSchema) + closingBrace
                                     : $"{this.block.GetIndent(this.deepSchema)}" + beforeBrace + openingBrace + "\n" +
                                       rawContent +
                                       $"{this.block.GetIndentCode(this.deepSchema + 1)}" + "\n" +
                                       this.block.GetIndent(this.deepSchema) + closingBrace;

            return modifiedContent;
        }
    }
}
