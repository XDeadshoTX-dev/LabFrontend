using LabBackend.Utils.Interfaces;
using LabBackend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LabBackend.Utils.Abstract
{
    public abstract class AbstractBlock : ICloneable
    {
        public string name { get; set; }
        public int id { get; set; }
        public string content { get; set; }
        protected string language { get; set; }
        public AbstractBlock[] Next { get; set; }

        public AbstractBlock(string languageCode, string content)
        {
            this.id = id;
            this.content = content;
            this.language = languageCode.ToLower();
            this.Next = Array.Empty<AbstractBlock>();
        }
        //public AbstractBlock(List<AbstractBlock> UIArray)
        //{
        //    this.id = 0;
        //    this.Next = UIArray.ToArray();
        //}
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
        public virtual void Execute()
        {
            Console.WriteLine($"Executing AbstractBlock: {name}, Data: {content}\n");
        }
        public virtual void Execute(int amountTabs)
        {
            Console.WriteLine($"Executing AbstractBlock: {name}, Data: {content}\n");
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
    }
}
