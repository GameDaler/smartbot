using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Bot
{
    public class GDCallbackData
    {
        public string type = "";
        public string[] args = Array.Empty<string>();

        public GDCallbackData() { }
        public GDCallbackData(string? type, params object[]? args)
        {
            this.type = type ?? "";
            this.args = args == null || args.Length == 0
                ? Array.Empty<string>()
                : args.Select(a => a?.ToString() ?? "").ToArray();
            if (args == null || args.Length == 0 || type == null) return;

            List<string> stringArgs = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                stringArgs.Add(args[i].ToString());
            }

            this.type = type.ToString();

            this.args = stringArgs.ToArray();
        }

        // Метод: десериализация из callbackData
        public GDCallbackData(string callbackData)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            for (int i = 0; i < callbackData.Length; i++)
            {
                if (callbackData[i] == ':')
                {
                    if (i + 1 < callbackData.Length && callbackData[i + 1] == ':')
                    {
                        current.Append(':'); // экранированный символ
                        i++; // пропускаем второй ':'
                    }
                    else
                    {
                        parts.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(callbackData[i]);
                }
            }
            parts.Add(current.ToString()); // добавляем последнюю часть

            if (parts.Count == 0)
                type = "";
            if (parts.Count <= 1)
                args = Array.Empty<string>();
            else
            {
                type = parts[0];
                args = parts.Skip(1).ToArray();
            }
        }

        public override string ToString()
        {
            string result = "Type: \"" + type + "\"";

            for (int i = 0; i < args.Length; i++)
            {
                result += ", Arg" + i + ": \"" + args[i].ToString() + "\"";
            }
            return result;
        }

        // Метод: сериализация в callbackData
        public string? ToCallbackData()
        {
            // Экранируем каждое значение
            string Escape(string s) => s.Replace(":", "::");

            // Я не знаю как это работает, это жпт
            return string.Join(":", new[] { Escape(type) }.Concat(args.Select(Escape)));
        }
        public static string ToCallbackData(string type, params object[] args)
        {
            string Escape(string s) => s.Replace(":", "::");

            var safeArgs = args ?? Array.Empty<string>(); // ✅ безопасная замена null
            var allParts = new[] { type }.Concat(safeArgs.Select(a => Escape(a.ToString() ?? "")));
            return string.Join(":", allParts);
        }
    }
}
