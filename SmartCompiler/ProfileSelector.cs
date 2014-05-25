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
            if (Directory.Exists(BotDirectory + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + ""))
            {

                string[] profiles = Directory.GetDirectories(BotDirectory + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + "");

                foreach (string s in profiles)
                {
                    comboBoxProfiles.Items.Add(s.Substring(s.LastIndexOf("\\")));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxProfiles.Text != "")
            {
                using (CodeCompiler compiler = new CodeCompiler(BotDirectory + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + "" + comboBoxProfiles.SelectedItem.ToString() +  Path.DirectorySeparatorChar , BotDirectory))
                {
                    if (compiler.Compile())
                    {
                    }
                    else
                    {
                    }
                }
                Close();
                StreamWriter writer = new StreamWriter(BotDirectory + "" + Path.DirectorySeparatorChar + "Bots" + Path.DirectorySeparatorChar + "SmartCC" + Path.DirectorySeparatorChar + "Profile.current");
                writer.WriteLine(comboBoxProfiles.SelectedItem.ToString().Substring(1));
                writer.Close();
            }
            else
            {
                MessageBox.Show("Error: you didn't select a profile from the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

    }
}
