using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfApp2.backend.schemas.translate;

namespace WpfApp2.backend.schemas.@abstract
{
    abstract class AbstractTranslateSchema
    {
        public string pattern = string.Empty;
        public RegexOptions regOptions;

        protected int deepSchema = 0;
        protected AbstractBlock block;

        public abstract string InsertCode(Match match, string fileContent, string code);
        public static AbstractTranslateSchema GetSchema(int deep, string language, AbstractBlock block)
        {
            if (language == "c")
            {
                return new CTranslateSchema(deep, block);
            }
            else if (language == "c++")
            {
                return new CPlusPlusTranslateSchema(deep, block);
            }
            else if (language == "c#")
            {
                return new CSharpTranslateSchema(deep, block);
            }
            else if (language == "java")
            {
                return new JavaTranslateSchema(deep, block);
            }
            else if (language == "python")
            {
                return new PythonTranslateSchema(deep, block);
            }
            return null;
        }
    }
}
