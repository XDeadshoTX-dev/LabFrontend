using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp2.backend.schemas
{
    abstract class AbstractTranslateSchema
    {
        public string pattern = string.Empty;
        public abstract string InsertCode(Match match, string code, AbstractBlock block);
        public static AbstractTranslateSchema GetSchema(string language)
        {
            if (language == "c")
            {
                return new CTranslateSchema();
            }
            else if (language == "c++")
            {
                return new CPlusPlusTranslateSchema();
            }
            else if (language == "c#")
            {
                return new CSharpTranslateSchema();
            }
            else if (language == "java")
            {
                return new JavaTranslateSchema();
            }
            else if (language == "python")
            {
                return new PythonTranslateSchema();
            }
            return null;
        }
    }
}
