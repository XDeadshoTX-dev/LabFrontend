using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp2.backend.schemas
{
    class PythonTranslateSchema : AbstractTranslateSchema
    {
        public PythonTranslateSchema()
        {
            pattern = @"(static\s+void\s+Main\s*\(.*?\)\s*\{)(.*?)(\})";
        }
        public override string InsertCode(Match match, string code, AbstractBlock block)
        {
            return string.Empty;
        }
    }
}
