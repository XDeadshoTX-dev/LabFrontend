﻿using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabBackend.Blocks.Actions
{
    public class PrintBlock : AbstractBlock
    {
        public PrintBlock(string languageCode, string data) : base(languageCode, data)
        {
            this.Name = "PrintBlock";
            this.PatternValidation = @"^[a-zA-Z_]\w*$";
        }
        private bool IsValidAssignment(string data, ref string sanitizedData, Stack<string> bufferVariables)
        {
            string sanitizeData(string data)
            {
                return data.Replace(" ", "");
            }

            sanitizedData = sanitizeData(data);

            if (Regex.IsMatch(sanitizedData, this.PatternValidation))
            {
                var match = Regex.Match(sanitizedData, this.PatternValidation);

                string content = match.Groups[0].Value;

                if (!bufferVariables.Contains(content))
                {
                    throw new Exception($"[Type: {this.Name}; Content: \"{content}\"] Variable \"{content}\" doesn't exists, set accessible variable");
                }

                sanitizedData = content;
                return true;
            }

            return false;
        }
        public override string Execute(int deep, Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Wrong pattern");
            }

            switch (this.Language)
            {
                case "c":
                    this.Code = $"printf(\"%d\\n\", {sanitizedData});";
                    break;
                case "c++":
                    this.Code = $"std::cout << {sanitizedData} << std::endl;";
                    break;
                case "c#":
                    this.Code = $"Console.WriteLine({sanitizedData});";
                    break;
                case "python":
                    this.Code = $"print({sanitizedData})";
                    break;
                case "java":
                    this.Code = $"System.out.println({sanitizedData});";
                    break;
            }

            string fileContent = this.ReadAllText();
            string updatedContent = InsertCodeIntoMain(deep, fileContent);
            this.WriteAllText(updatedContent);

            return $"{this.Name} success";
        }
        public override string ExecuteMultithread(int deep, Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Wrong pattern");
            }

            switch (this.Language)
            {
                case "c":
                    this.Code = $"printf(\"%d\\n\", {sanitizedData});";
                    break;
                case "c++":
                    this.Code = $"std::cout << {sanitizedData} << std::endl;";
                    break;
                case "c#":
                    this.Code = $"Console.WriteLine({sanitizedData});";
                    break;
                case "python":
                    this.Code = $"print({sanitizedData})";
                    break;
                case "java":
                    this.Code = $"System.out.println({sanitizedData});";
                    break;
            }

            return this.Code;
        }
        public override string ExecuteValidation(Stack<string> bufferVariables)
        {
            string sanitizedData = string.Empty;
            if (!IsValidAssignment(this.Content, ref sanitizedData, bufferVariables))
            {
                throw new Exception($"[Type: {this.Name}; Content: \"{sanitizedData}\"] Wrong pattern");
            }

            return $"{this.Name} success";
        }
    }
}
