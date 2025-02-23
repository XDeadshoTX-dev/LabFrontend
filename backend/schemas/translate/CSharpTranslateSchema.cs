﻿using LabBackend.Utils.Abstract;
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
        public CSharpTranslateSchema(int deep, AbstractBlock block)
        {
            this.deepSchema = deep + 3;
            this.block = block;
            this.regOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;

            string amountSpaces = this.block.GetIndent(this.deepSchema - 1);
            pattern = $@"({amountSpaces}\{{)(.*?)(\n{amountSpaces}\}})";
        }
        public override string InsertCode(Match match, string fileContent)
        {
            string openingBrace = match.Groups[1].Value.Trim();  // {
            string content = match.Groups[2].Value.Trim();       // raw content
            string closingBrace = match.Groups[3].Value.Trim();  // }

            string newContent = string.IsNullOrEmpty(content)
                ? $"\n{this.block.GetIndentCode(this.deepSchema)}\n"
                : $"\n{this.block.GetIndent(this.deepSchema)}{content}\n{block.GetIndentCode(this.deepSchema)}\n";

            return $"{this.block.GetIndent(this.deepSchema - 1)}" +
                   $"{openingBrace}" +
                   $"{newContent}" +
                   $"{this.block.GetIndent(this.deepSchema - 1)}{closingBrace}";
        }
    }
}
