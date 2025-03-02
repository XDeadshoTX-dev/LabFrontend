using LabBackend.Utils.Abstract;
using System;
using System.Text.RegularExpressions;
using WpfApp2.backend.schemas.@abstract;

namespace WpfApp2.backend.schemas.translate
{
    class PythonTranslateSchema : AbstractTranslateSchema
    {
        public PythonTranslateSchema(int deep, AbstractBlock block)
        {
            this.deepSchema = deep + 1;
            this.block = block;
            this.regOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            pattern = @$"(^\s*if\s+__name__\s*==\s*['""]__main__['""]\s*:\s*\r?\n)(.*)(?![\s\S]*if\s+__name__\s*==\s*['""]__main__['""])";
        }

        public override string InsertCode(Match match)
        {
            string mainDeclaration = match.Groups[1].Value;
            string existingContent = match.Groups[2].Value;

            string newContent = this.block.Name != "end"
                ? $"{mainDeclaration}{existingContent}{this.block.GetIndentCode(this.deepSchema)}\n"
                : $"{mainDeclaration}{existingContent}{this.block.GetIndentCode(this.deepSchema)}";

            return newContent;
        }
    }
}
