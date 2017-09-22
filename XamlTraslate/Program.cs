using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace XamlTraslate
{
    class Program
    {
        private static Dictionary<string, string> _dictionary;
        private const string KeyString = "ResourceString";
        //private const string Dir = @"C:\Users\yidao\gitrepos\liquid-app-regi\liquid-app-regi\Liquid";
        private const string Dir = @"C:\Users\yidao\gitrepos\liquid-app-regi\liquid-app-regi\Liquid.Regi.Dialog";
        private const string XamlFile =
            @"C:\Users\yidao\gitrepos\liquid-app-regi\liquid-app-regi\Liquid.Regi.Resources\Resources\japanese.xaml";
        static void Main(string[] args)
        {
            Load();

            string[] files = Directory.GetFiles(
                Dir, "*.xaml", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var text = File.ReadAllText(file);

                var regex = new Regex("\\s*<LineBreak(></LineBreak>|/>)\\s*");
                text = regex.Replace(text, "&#xa;");

                var mc = Regex.Matches(text, "\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                int i = 0;
                foreach (Match m in mc)
                {
                    var jpn = m.Value.Replace("\"", "");

                    if (Japanese.IsConteinsJamanese(jpn))
                    {
                        if (!_dictionary.ContainsKey(jpn))
                        {
                            _dictionary.Add(jpn, KeyString + _dictionary.Count.ToString("D5"));
                        }

                        var insertstr = $"\"{{StaticResource {_dictionary[jpn]}}}\"";
                        text = text.Remove(i + m.Index, m.Length).Insert(i + m.Index, insertstr);
                        i += insertstr.Length - m.Length;
                    }
                }

                mc = Regex.Matches(text, ">(.*?)<", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                i = 0;
                foreach (Match m in mc)
                {
                    var jpn = m.Value.Remove(m.Value.Length - 1, 1).Remove(0, 1);

                    if (Japanese.IsConteinsJamanese(jpn))
                    {
                        var t = jpn[0];
                        while (t == ' ' || t == '\n' || t == '\r')
                        {
                            jpn = jpn.Remove(0, 1);
                            t = jpn[0];
                        }
                        t = jpn[jpn.Length - 1];
                        while (t == ' ' || t == '\n' || t == '\r')
                        {
                            jpn = jpn.Remove(jpn.Length - 1, 1);
                            t = jpn[jpn.Length - 1];
                        }
                        if (!_dictionary.ContainsKey(jpn))
                        {
                            _dictionary.Add(jpn, KeyString + _dictionary.Count.ToString("D5"));
                        }

                        var insertstr = $" Text=\"{{StaticResource {_dictionary[jpn]}}}\"><";
                        text = text.Remove(i + m.Index, m.Length).Insert(i + m.Index, insertstr);
                        i += insertstr.Length - m.Length;
                    }
                }

                mc = Regex.Matches(text, "</TextBlock Text=\"{StaticResource " + KeyString + "(\\d+)}\">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                i = 0;
                foreach (Match m in mc)
                {
                    regex = new Regex("\\d+");
                    string number = regex.Match(m.Value).Value;
                    var insertstr = $"</TextBlock><TextBlock Text=\"{{StaticResource {KeyString}{number}}}\"/>";
                    text = text.Remove(i + m.Index, m.Length).Insert(i + m.Index, insertstr);
                    i += insertstr.Length - m.Length;
                }


                var sw = new StreamWriter(
                    file,
                    false,
                    Encoding.GetEncoding("utf-8"));
                sw.Write(text);
                sw.Close();
                Console.WriteLine(file);
            }

            Save();
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

        static void Save()
        {
            var d = _dictionary.OrderBy((x) => x.Value);
            var xaml = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n"
                     + "                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n"
                     + "                    xmlns:system=\"clr-namespace:System;assembly=mscorlib\"\n"
                     + "                    xml:space=\"preserve\">\n";
            foreach (var item in d)
            {
                xaml +=
                    $"    <system:String x:Key=\"{item.Value}\">{item.Key.Replace("\r\n", "&#xa;").Replace("\n", "&#xa;")}</system:String>\n";
            }
            xaml += "</ResourceDictionary>";

            var sw = new System.IO.StreamWriter(XamlFile,
                false,
                System.Text.Encoding.GetEncoding("utf-8"));
            sw.Write(xaml);
            sw.Close();
        }
    }
}
