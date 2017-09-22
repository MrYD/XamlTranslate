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
            Console.WriteLine("using System;");
            Console.WriteLine("namespace Liquid.Utility.Enum");
            Console.WriteLine("{");
            Console.WriteLine("    public enum ResourceKeys");
            Console.WriteLine("    {");
            foreach (var item in d)
            {     
                Console.Write("        /// <summary>\n        /// ");
                foreach (var chr in item.Key)
                {
                    if (chr=='\n')
                    {
                       Console.Write("\n        /// ");
                    }
                    else
                    {
                        Console.Write(chr);
                    }
                    
                }
                Console.WriteLine($"\n        /// </summary>\n        {item.Value},");
            }
            Console.WriteLine("    }");
            Console.WriteLine("}");
            Console.Read();
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
