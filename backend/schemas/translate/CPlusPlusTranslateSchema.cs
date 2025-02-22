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
            pattern = @"(static\s+void\s+Main\s*\(.*?\)\s*\{)(.*?)(\})";
        }
        public override string InsertCode(Match match, string code)
        {
            return string.Empty;
        }
    }
}
