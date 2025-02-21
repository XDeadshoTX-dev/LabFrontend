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
        public string Name { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }
        public AbstractBlock[] Next { get; set; }
        protected string Language { get; set; }
        protected int LanguageIndent { get; set; }
        protected string Code { get; set; }
        protected string FileName { get; set; }

        public AbstractBlock(string languageCode, string content)
        {
            this.Id = Id;
            this.Content = content;
            this.Next = Array.Empty<AbstractBlock>();

            this.Language = languageCode.ToLower();
            switch(this.Language.ToLower())
            {
                case "c":
                case "c++":
                case "c#":
                case "java":
                case "python":
                    this.LanguageIndent = 4;
                    break;
            }
            FileName = $"GeneratedCode.{GetFileExtension()}";
        }
        public virtual object Clone()
        {
            return (AbstractBlock)MemberwiseClone();
        }
        public virtual string GetNameBlock()
        {
            return Name;
        }
        public virtual int GetId()
        {
            return Id;
        }
        public virtual void SetData(string data)
        {
            this.Content = data;
        }
        public virtual string GetData()
        {
            return Content;
        }
        protected string GetFileExtension()
        {
            switch (this.Language)
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
        public string GetIndent(int deep)
        {
            return new string(' ', this.LanguageIndent * deep);
        }
        protected string ReadAllText()
        {
            return File.ReadAllText(this.FileName);
        }
        protected void WriteAllText(string content)
        {
            File.WriteAllText(this.FileName, content);
        }
        protected string InsertCodeIntoMain(string fileContent, string codeToInsert)
        {
            AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(this.Language);

            string updatedContent = Regex.Replace(
                fileContent,
                translateSchema.pattern,
                match =>
                {
                    AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(this.Language);
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
