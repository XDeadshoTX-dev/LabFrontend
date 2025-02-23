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
            //pattern = $@"^(?<line>(?:[^\{{]*\{{){{{currentBrace}}})(?<content>[\s\S]*)$";
            pattern = $@"^(?<line>(?:[^\{{]*\{{){{{currentBrace}}})(?<content>[\s\S]*?)(\}})";
        }
        public override string InsertCode(Match match, string fileContent, string code)
        {
            // Извлекаем строку (или блок), в котором найдено нужное вхождение фигурных скобок.
            string targetLine = match.Groups["line"].Value;
            string contentAfterBrace = match.Groups["content"].Value.Trim();

            // Добавляем новый код после открывающей скобки с правильным отступом
            string modifiedContent = string.IsNullOrEmpty(contentAfterBrace) 
                                    ?  targetLine + "\n" +
                                       contentAfterBrace +
                                       $"{this.block.GetIndentCode(this.deepSchema + 1)}" + "\n" +
                                       $"{this.block.GetIndent(this.deepSchema)}}}"
                                    : targetLine + "\n" +
                                       $"{this.block.GetIndent(this.deepSchema + 1)}{contentAfterBrace}" +
                                       $"\n{this.block.GetIndentCode(this.deepSchema + 1)}" + 
                                       $"\n{this.block.GetIndent(this.deepSchema)}}}";

            return modifiedContent;
        }
    }
}
