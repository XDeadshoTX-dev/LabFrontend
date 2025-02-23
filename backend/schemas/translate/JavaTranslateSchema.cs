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
    class JavaTranslateSchema : AbstractTranslateSchema
    {
        public JavaTranslateSchema(int deep, AbstractBlock block)
        {
            this.deepSchema = deep + 1;
            this.block = block;

            int currentBrace = this.deepSchema + 1;
            pattern = $@"^(?<line>(?:[^\{{]*\{{){{{currentBrace}}}.*)$";

        }
        public override string InsertCode(Match match, string fileContent, string code)
        {
            // Извлекаем строку (или блок), в котором найдено нужное вхождение фигурных скобок.
            string targetLine = match.Groups["line"].Value;

            // Разбиваем targetLine на отдельные строки. Добавляем разделитель "\r" для совместимости.
            string[] lines = targetLine.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            // Извлекаем последнюю строку.
            string lastLine = lines.Last();

            // Обработка: добавляем в конец последней строки дополнительный код с нужным отступом.
            string modifiedLastLine = lastLine + $"{this.block.GetIndent(this.deepSchema + 1)}" + code;

            // Заменяем последнюю строку в массиве.
            lines[lines.Length - 1] = modifiedLastLine;

            // Собираем строки обратно с использованием текущего разделителя строк.
            string modifiedTargetLine = string.Join(Environment.NewLine, lines);

            return modifiedTargetLine;
        }

    }
}
