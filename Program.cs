using System;
using System.IO;
using System.Text;

namespace GitHubWikiSidbarToDocFX
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
                throw new Exception("Invalide number of arguments!");

            if (!File.Exists(args[0]))
                throw new FileNotFoundException(args[0]);

            var sidelines = File.ReadAllText(args[0]).Split('\n');
            StringBuilder toc = new StringBuilder();

            for (int i = 0; i < sidelines.Length; i++)
            {
                //checks and extraction
                if (sidelines[i].Length < 3)
                    continue;
                
                bool href = false;
                bool items = false;
                int indent = GetUntilOrEmpty(sidelines[i], "-").Length;
                string name = "";

                if (sidelines[i].Contains("[[") && sidelines[i].Contains("]]"))
                    href = true;

                if (i < sidelines.Length-1 && indent < GetUntilOrEmpty(sidelines[i + 1], "-").Length)
                    items = true;

                if (href)
                {
                    var start = sidelines[i].IndexOf("[[") + 2;
                    name = sidelines[i].Substring(start, sidelines[i].IndexOf("]]") - start);
                }
                else
                {
                    var start = sidelines[i].IndexOf("- ") + 2;
                    name = sidelines[i].Substring(start, sidelines[i].Length - start);
                }

                //generate toc
                
                //name
                toc.Append(Repeat(' ', indent));
                toc.Append("- name: ");
                toc.Append(name);
                toc.Append("\n");

                //href
                if (href)
                {
                    toc.Append(Repeat(' ', indent + 2));
                    toc.Append("href: ");
                    toc.Append(name.Replace(" ", "-"));
                    toc.Append(".md");
                    toc.Append("\n");
                }

                //items
                if (items)
                {
                    toc.Append(Repeat(' ', indent + 2));
                    toc.Append("items: ");
                    toc.Append("\n");
                }
            }

            var outfilepath = args.Length > 1 ? Path.Combine(args[1], "toc.yml") : "toc.yml";

            File.WriteAllText(outfilepath, toc.ToString());
        }

        public static string GetUntilOrEmpty(string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        public static string Repeat(char c, int count)
        {
            return new String(c, count);
        }
    }
}
