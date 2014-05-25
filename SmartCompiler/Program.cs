using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using System.Windows.Forms;
using System.IO;
namespace SmartCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[1] == "true")
            {
                Application.EnableVisualStyles();
                Application.Run(new ProfileSelector(args[0]));
            }
            else
            {
                if (args[0] == "null")
                    args[0] = "";
                using (CodeCompiler compiler = new CodeCompiler(args[0] + "\\Bots\\SmartCC\\Profiles\\Defaut\\", args[0]))
                {
                    if (compiler.Compile())
                    {
                    }
                }
                    String path = args[0] + "\\Bots\\SmartCC\\Profile.current";
                    using (var stream = new FileStream(path, FileMode.Truncate))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("Defaut");
                            writer.Close();

                        }
                    }
            }
        }
    }
}
