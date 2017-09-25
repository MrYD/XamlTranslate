using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace CreateEnum
{
    class Program
    {
        private static Dictionary<string, string> _dictionary;
        private const string XamlFile =
            @"C:\Users\yidao\gitrepos\liquid-app-regi\liquid-app-regi\Liquid.Regi.Resources\Resources\japanese.xaml";
        static void Main(string[] args)
        {
            Load();
            var d = _dictionary.OrderBy(x => x.Value);
            var text = "using System;\n";
            text += "namespace Liquid.Utility.Enum\n";
            text += "{\n";
            text += "    public enum ResourceKeys\n";
            text += "    {\n";
            foreach (var item in d)
            {
                text += "        /// <summary>\n        /// ";
                foreach (var chr in item.Key)
                {
                    if (chr == '\n')
                    {
                        text += "\n        /// ";
                    }
                    else
                    {
                        text += chr;
                    }

                }
                text += $"\n        /// </summary>\n        {item.Value},\n";
            }
            text += "    }\n";
            text += "}\n";
            Console.WriteLine(text);
            var sw = new StreamWriter(
                @"C:\Users\yidao\gitrepos\liquid-app-regi\liquid-app-regi\Liquid.Utility\Enum\ResourceKeys.cs",
                false,
                Encoding.GetEncoding("utf-8"));
            sw.Write(text);
            sw.Close();
        }

        static void Load()
        {
            var reader = new XamlReader();
            var sr = new StreamReader(XamlFile,
                Encoding.Default);

            var d = reader.LoadAsync(new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd()))) as ResourceDictionary;

            _dictionary = new Dictionary<string, string>();
            foreach (DictionaryEntry v in d)
            {
                _dictionary.Add(v.Value.ToString(), v.Key.ToString());
            }
        }
    }
}
