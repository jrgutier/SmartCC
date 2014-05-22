using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using System.Windows.Forms;

namespace SmartCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AAA");
            if (args[1] == "true")
            {
                Application.EnableVisualStyles();
                Application.Run(new ProfileSelector(args[0]));
            }
            else
            {
                if (args[0] == "null")
                    args[0] = "";
                    using (CodeCompiler compiler = new CodeCompiler(args[0] + "Bots\\SmartCC\\Profiles\\Defaut\\", args[0]))
                {
                    if (compiler.Compile())
                    {
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
