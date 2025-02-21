using LabBackend.Utils.Interfaces;
using LabBackend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using WpfApp2.backend.schemas.translate;

namespace LabBackend.Utils.Abstract
{
    public abstract class AbstractBlock : ICloneable
    {
        public string name { get; set; }
        public int id { get; set; }
        public string content { get; set; }
        public AbstractBlock[] Next { get; set; }
        protected string language { get; set; }
        protected int languageIndent { get; set; }
        protected string code { get; set; }
        protected string fileName { get; set; }

        public AbstractBlock(string languageCode, string content)
        {
            this.id = id;
            this.content = content;
            this.Next = Array.Empty<AbstractBlock>();

            this.language = languageCode.ToLower();
            switch(this.language.ToLower())
            {
                case "c":
                case "c++":
                case "c#":
                case "java":
                case "python":
                    this.languageIndent = 4;
                    break;
            }
            fileName = $"GeneratedCode.{GetFileExtension()}";
        }
        public virtual object Clone()
        {
            return (AbstractBlock)MemberwiseClone();
        }
        public virtual string getNameBlock()
        {
            return name;
        }
        public virtual int getId()
        {
            return id;
        }
        public virtual void setData(string data)
        {
            this.content = data;
        }
        public virtual string getData()
        {
            return content;
        }
        protected string GetFileExtension()
        {
            switch (this.language)
            {
                case "c":
                    return "c";
                case "c++":
                    return "cpp";
                case "c#":
                    return "cs";
                case "java":
                    return "java";
                case "python":
                    return "py";
                default:
                    return "";
            }
        }
        public string getIndent(int deep)
        {
            return new string(' ', this.languageIndent * deep);
        }
        protected string readAllText()
        {
            return File.ReadAllText(this.fileName);
        }
        protected void writeAllText(string content)
        {
            File.WriteAllText(this.fileName, content);
        }
        protected string InsertCodeIntoMain(string fileContent, string codeToInsert)
        {
            AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(this.language);

            string updatedContent = Regex.Replace(
                fileContent,
                translateSchema.pattern,
                match =>
                {
                    AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(this.language);
                    string result = translateSchema.InsertCode(match, codeToInsert, this);

                    return result;
                },
                RegexOptions.Singleline | RegexOptions.IgnoreCase
            );

            return updatedContent;
        }
        public virtual void Execute()
        {
        }
        public virtual void Execute(int amountTabs)
        {
        }
    }
}
