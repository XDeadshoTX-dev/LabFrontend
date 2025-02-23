﻿using LabBackend.Utils.Abstract;
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

            pattern = @$"(^\s*if\s+__name__\s*==\s*['""]__main__['""]\s*:\s*\r?\n)(.*)$";
        }

        public override string InsertCode(Match match, string fileContent, string code)
        {
            string mainDeclaration = match.Groups[1].Value;
            string existingContent = match.Groups[2].Value;

            string indent = block.GetIndent(this.deepSchema);
            string newContent = this.block.Name != "end"
                ? $"{mainDeclaration}{existingContent}{indent}{code}\n"
                : $"{mainDeclaration}{existingContent}{indent}{code}";

            return newContent;
        }
    }
}
