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
using WpfApp2.backend.schemas.@abstract;

namespace LabBackend.Utils.Abstract
{
    public abstract class AbstractBlock : ICloneable
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }
        public AbstractBlock[] Next { get; set; }

        public string Language { get; set; }
        protected int LanguageIndent { get; set; }

        protected string FileName { get; set; }
        protected string Code { get; set; }
        protected string PatternValidation { get; set; }

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
            this.FileName = $"GeneratedCode.{GetFileExtension()}";
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
        public string GetIndentCode(int deep)
        {
            string indent = new string(' ', this.LanguageIndent * deep);
            string[] lines = this.Code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = indent + lines[i];
            }

            return string.Join(Environment.NewLine, lines);
        }
        protected string ReadAllText()
        {
            return File.ReadAllText(this.FileName);
        }
        protected void WriteAllText(string content)
        {
            File.WriteAllText(this.FileName, content);
        }
        protected string InsertCodeIntoMain(int deep, string fileContent)
        {
            AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(
                deep, 
                this);

            string updatedContent = Regex.Replace(
                fileContent,
                translateSchema.pattern,
                match =>
                {
                    AbstractTranslateSchema translateSchema = AbstractTranslateSchema.GetSchema(
                        deep, 
                        this);

                    string result = translateSchema.InsertCode( 
                        match,
                        fileContent);

                    return result;
                },
                translateSchema.regOptions
            );

            return updatedContent;
        }
        public virtual void Execute()
        {
        }
        public virtual string Execute(int deep, Stack<string> bufferVariables)
        {
            return string.Empty;
        }
        public virtual string ExecuteMultithread(int deep, Stack<string> bufferVariables)
        {
            return string.Empty;
        }
        public virtual string ExecuteValidation(Stack<string> bufferVariables)
        {
            return string.Empty;
        }
    }
}
