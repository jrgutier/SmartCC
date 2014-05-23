using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.IO;

namespace SmartCompiler
{
    public partial class ProfileSelector : Form
    {
        private string BotDirectory;
        public ProfileSelector(string directory)
        {
            InitializeComponent();
            this.BotDirectory = directory;
            if (Directory.Exists(BotDirectory + "\\Bots\\SmartCC\\Profiles\\"))
            {

                string[] profiles = Directory.GetDirectories(BotDirectory + "\\Bots\\SmartCC\\Profiles\\");

                foreach (string s in profiles)
                {
                    comboBox1.Items.Add(s.Substring(s.LastIndexOf("\\")));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.OutputEncoding = Encoding.UTF8;
            using (CodeCompiler compiler = new CodeCompiler(BotDirectory + "Bots\\SmartCC\\Profiles\\"+comboBox1.SelectedItem.ToString()+"\\", BotDirectory))
            {
                if (compiler.Compile())
                {
                }
                else
                {
                }
            }
            Close();
            StreamWriter writer = new StreamWriter(BotDirectory + "Bots\\SmartCC\\Profile.current");
            writer.WriteLine(comboBox1.SelectedItem.ToString().Substring(1));
            writer.Close();
        }

    }
}
