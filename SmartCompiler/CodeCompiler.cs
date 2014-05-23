using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace SmartCompiler
{
    public class CodeCompiler : IDisposable
    {
        public CodeCompiler(string FilePath, string BotDirectory)
        {
            this.FilePath = FilePath;
            this.BotDirectory = BotDirectory;
        }

        public bool Compile()
        {
            if (Directory.Exists(this.FilePath))
            {

                string[] sourcesPath = Directory.GetFiles(this.FilePath);
                List<string> sources = new List<string>();

                foreach (string s in sourcesPath)
                {
                    if (!s.Contains(".cs"))
                        continue;

                    sources.Add(File.ReadAllText(s));
                }


                using (CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    CompilerParameters options = new CompilerParameters
                    {
                        GenerateInMemory = false,
                        OutputAssembly = string.Format(@"{0}\{1}", Path.GetDirectoryName(this.BotDirectory + "Bots\\SmartCC\\Profile.dll"), Path.GetFileName(this.BotDirectory + "Bots\\SmartCC\\Profile.dll"))
                    };
                    options.ReferencedAssemblies.Add(BotDirectory + "Bots\\SmartCC.dll");
                    options.ReferencedAssemblies.Add("System.Core.dll");
                    CompilerResults results = provider.CompileAssemblyFromSource(options, sources.ToArray());
                    StringEnumerator enumerator = results.Output.GetEnumerator();



                    while (enumerator.MoveNext())
                    {
                    }

                    return (results.Errors.Count == 0);
                }
            }
            return false;
        }

        public void Dispose()
        {
        }

        public string FilePath { get; set; }

        public string BotDirectory { get; set; }

    }
}
