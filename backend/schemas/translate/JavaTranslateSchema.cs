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

            int currentBrace = this.deepSchema + 1;
            //pattern = $@"^(?<line>(?:[^\{{]*\{{){{{currentBrace}}})(?<content>[\s\S]*?)(\}})";
            int amountSpaces = 4;
            pattern = $@"^\s{{{amountSpaces}}}.*\{{\s*\n(?<rawContent>[\s\S]*?)^\s{{{amountSpaces}}}\}}";
        }
        public override string InsertCode(Match match, string fileContent, string code)
        {
            // Извлекаем контент между скобками
            string rawContent = match.Groups["rawContent"].Value;

            // Добавляем новый код перед закрывающей скобкой
            string modifiedContent = rawContent + "\n" +
                                     $"{this.block.GetIndentCode(this.deepSchema + 1)}";

            // Вставляем обратно в файл
            return modifiedContent;
        }
    }
}
